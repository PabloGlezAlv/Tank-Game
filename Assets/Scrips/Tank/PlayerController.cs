using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



public class PlayerController : Movement
{
    [SerializeField]
    private float _strengthChange = 50;
    [Header("UX")]
    [SerializeField]
    private LayerMask _clickable;
    [SerializeField]
    private GameObject _moveArea;
    [SerializeField]
    CameraMovement _movementCamera;
    [SerializeField]
    Transform _attackCamera;

    protected override void Awake()
    {
        base.Awake();

        _moveArea.transform.localScale = new Vector3(_inputDistance / transform.localScale.x * 2, _moveArea.transform.localScale.y, _inputDistance / transform.localScale.z * 2);
        _moveArea.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void MoveCanon()
    {
        //Rotation
        _yaw += _rotationSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        _pitch += _rotationSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, MIN_ANGLE, MAX_ANGLE);

        _topTank.transform.eulerAngles = new Vector3(_pitch, _yaw, _topTank.transform.eulerAngles.z);

        //Force
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            _shellTraj.AddForce(scrollInput * _strengthChange);
        }

        //Shoot
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _shellTraj.Shoot();
            _movementCamera.SetMovementCamera();
            _currentState = TankState.Wait;
        }
    }

    public override void MovePlayer()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _clickable))
            {
                if (Vector3.Distance(transform.position, hit.point) <= _inputDistance)
                {
                    _tankMovement.MovePlayer(hit.point);

                    if (--_currentSteps <= 0)
                    {
                        _moveArea.SetActive(false);
                        _currentState = TankState.Wait;
                    }
                }
                else // Fuera de rango
                {
                    _UIManager.ShowMessage("Press inside the circle");
                }

            }
        }
    }

    public override void StartMove()
    {
        base.StartMove();
        _moveArea.SetActive(true);
    }

    public override void StartShooting()
    {
        base.StartShooting();
        _movementCamera.SetAttackCamera(_attackCamera.position, _attackCamera.forward, _attackCamera);
    }
}
