using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngineInternal;
using static UnityEngine.GraphicsBuffer;

public class AIMovement : Movement
{
    [Header("AI Parameters")]
    [SerializeField]
    float hitDistance = 0.1f;
    [SerializeField]
    float _actionsDelay = 1f;
    [SerializeField]
    float _shootAngle = 3;
    [SerializeField]
    Transform _target;
    [SerializeField]
    Transform _bulletSpawn;

    int _rotationDir = 1;

    bool _lookingTarget = false;
    private Vector3 _initialRotation;

    private float _previousDistance = 0;
    float _thinkingTime = 0;

    private HealthSystem _healthSystem;
    Queue<int> _damageReceived = new Queue<int>();
    int _hitChance = 0;
    int _failDistance = 1;

    protected override void Awake()
    {
        base.Awake();

        _healthSystem = GetComponent<HealthSystem>();
        _damageReceived.Enqueue(0);
        _damageReceived.Enqueue(1);
        _damageReceived.Enqueue(0);
        _damageReceived.Enqueue(1);

        _hitChance = 2; //To start hit chance is 50%
    }

    private void Start()
    {
        _UIManager.SetAILevel(_hitChance.ToString());
    }
    protected override void Update()
    {
        _thinkingTime -= Time.deltaTime;
        if (_thinkingTime > 0) return;

        base.Update();
    }

    public override void MoveCanon()
    {
        if (!_lookingTarget)
        {
            _yaw += _rotationSpeed * Time.deltaTime * _rotationDir;

            _topTank.transform.eulerAngles = new Vector3(_pitch, _yaw, _topTank.transform.eulerAngles.z);

            Vector3 directionToTarget = _target.transform.position - _topTank.transform.position;

            //Check if target reached
            float currentDistance = Vector3.Distance(_bulletSpawn.position, _target.position);
            if (_previousDistance < currentDistance)
            {
                SetupHeight();
            }

            _previousDistance = currentDistance;
        }
        else
        {
            _pitch += _rotationSpeed * Time.deltaTime * _rotationDir;
            //In case limit reached shoot
            if (_pitch >= MAX_ANGLE)
            {
                _pitch = MAX_ANGLE;
                _currentState = TankState.Wait;
                Invoke("Shoot", _actionsDelay);
                return;
            }
            else if (_pitch <= MIN_ANGLE)
            {
                _pitch = MIN_ANGLE;
                _currentState = TankState.Wait;
                Invoke("Shoot", _actionsDelay);
                return;
            }

            _topTank.transform.eulerAngles = new Vector3(_pitch, _yaw, _topTank.transform.eulerAngles.z);


            if (Vector3.Distance(_shellTraj.getCollisionPoint(), _target.position) < hitDistance * _failDistance)
            {
                _currentState = TankState.Wait;
                Invoke("Shoot", _actionsDelay);
            }
        }
    }
    
    //Parameters to start looking for a height
    private void SetupHeight()
    {
        _lookingTarget = true;

        //Chance of hiting
        if (Mathf.Clamp(_hitChance - 1, 0, 2) <= Random.Range(0, 4)) // Hit
            _failDistance = 1;
        else// No Hit
            _failDistance = 30;

        Vector3 currentPoint = _shellTraj.getCollisionPoint();

        //Check now vertical rotation direction
        if (Vector3.Distance(currentPoint, transform.position) < Vector3.Distance(_target.position, transform.position))
            _rotationDir = 1;
        else
            _rotationDir = -1;
    }

    private void Shoot()
    {
        _shellTraj.Shoot();
        Invoke("RotateStandard", _actionsDelay);
    }

    //Return to normal position
    void RotateStandard()
    {
        StartCoroutine(RotateTankSmoothly(_initialRotation));
    }

    private IEnumerator RotateTankSmoothly(Vector3 targetRotation)
    {
        Quaternion startRotation = _topTank.localRotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);

        float duration = Quaternion.Angle(startRotation, endRotation) / _rotationSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            _topTank.localRotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }

        _topTank.localRotation = endRotation;
    }

    public override void MovePlayer()
    {
        _tankMovement.MovePlayer(transform.position + transform.forward + transform.right);
        _currentState = TankState.Wait;
    }

    public override void StartMove()
    {
        base.StartMove();
        _thinkingTime = _actionsDelay;
    }

    public override void StartShooting()
    {
        base.StartShooting();

        ReCalculateAI();

        _thinkingTime = _actionsDelay;
        CalculateRotationDir();
        _previousDistance = Vector3.Distance(_bulletSpawn.position, _target.position);
        _initialRotation = _topTank.localEulerAngles;
    }
    
    //Calcualte AI intelligence
    private void ReCalculateAI()
    {
        int lastTurn = _healthSystem.LastBulletHitted();
        _hitChance += lastTurn;
        _hitChance -= _damageReceived.Dequeue();
        _damageReceived.Enqueue(lastTurn);

        _UIManager.SetAILevel(_hitChance.ToString());
    }

    //Look where to rotate the tank
    private void CalculateRotationDir()
    {
        Vector3 directionToTarget = new Vector3(
        _target.transform.position.x - _topTank.transform.position.x,
        0,
        _target.transform.position.z - _topTank.transform.position.z
    );

        float targetAngle = Mathf.Atan2(directionToTarget.z, directionToTarget.x) * Mathf.Rad2Deg;
        targetAngle = (targetAngle + 360) % 360;

        float currentAngle = _topTank.transform.eulerAngles.y;

        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        _lookingTarget = Mathf.Abs(angleDifference) < _shootAngle;

        if(Vector3.Distance(transform.position + transform.right, _target.transform.position) < Vector3.Distance(transform.position - transform.right, _target.transform.position))
            _rotationDir = 1;
        else
            _rotationDir = -1;
    }
}
