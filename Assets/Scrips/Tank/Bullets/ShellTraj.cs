using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ShellTraj : MonoBehaviour
{
    [Header("Shell parameters")]
    [SerializeField]
    private GameObject _bulletPrefab = null;
    [SerializeField]
    AudioClip _shootSound;
    [SerializeField]
    private float _force = 500;
    [SerializeField]
    private float _mass = 1;

    private float _vel;
    bool shoot = true;

    [Header("Trayectory parameters")]
    [SerializeField]
    private Transform _bulletSpawn;
    [SerializeField]
    private LayerMask trajLayers;
    [SerializeField]
    private float _collisionCheckRadius = 1f;

    LineRenderer _lineRenderer;
    private Vector3 _collisionPoint = Vector3.zero;
    private void Start()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    private void OnEnable()
    {
        shoot = true;
    }

    private void OnDisable()
    {
        _lineRenderer.positionCount = 0;
    }
    // Update is called once per frame
    void Update()
    {
        //Only one bullet per round
        if (!shoot) return;


        DrawTrajectory();
    }

    public void Shoot()
    {
        GameObject sphere = Instantiate(_bulletPrefab, _bulletSpawn.position, Quaternion.identity);
        Rigidbody sphereRB = sphere.GetComponent<Rigidbody>();
        sphere.GetComponent<Bullet>().SetOwner(TankType.Player);
        sphereRB.AddForce(_bulletSpawn.forward * _force);

        SoundManager.Instance.PlaySound(_shootSound);
        shoot = false;

        _lineRenderer.positionCount = 0;
    }

    void DrawTrajectory()
    {
        _lineRenderer.positionCount = SimulateArc().Count;
        for (int a = 0; a < _lineRenderer.positionCount; a++)
        {
            _lineRenderer.SetPosition(a, SimulateArc()[a]); //Add each Calculated Step to a LineRenderer to display a Trajectory. Look inside LineRenderer in Unity to see exact points and amount of them
        }
    }

    public void AddForce(float force)
    {
        _force += force;

        Mathf.Clamp(_force, 250, 1000);
    }
    private List<Vector3> SimulateArc()
    {
        List<Vector3> lineRendererPoints = new List<Vector3>();

        float maxDuration = 5f;
        float timeStepInterval = 0.1f;
        int maxSteps = (int)(maxDuration / timeStepInterval);

        Vector3 directionVector = _bulletSpawn.forward;
        Vector3 launchPosition = _bulletSpawn.position;

        _vel = _force / _mass * Time.fixedDeltaTime;

        for (int i = 0; i < maxSteps; ++i)
        {
            //f(t) = (x0 + x*t, y0 + y*t - 9.81t²/2)
            Vector3 calculatedPosition = launchPosition + directionVector * _vel * i * timeStepInterval;
            calculatedPosition.y += Physics.gravity.y / 2 * Mathf.Pow(i * timeStepInterval, 2);

            lineRendererPoints.Add(calculatedPosition);
            if (CheckForCollision(calculatedPosition))
            {
                break; //stop adding positions
            }
        }
        return lineRendererPoints;
    }

    public Vector3 getCollisionPoint()
    {
        return _collisionPoint;
    }


    private bool CheckForCollision(Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, _collisionCheckRadius, trajLayers);
        if (hitColliders.Length > 0)
        {
            _collisionPoint = hitColliders[0].gameObject.transform.position;
            return true;
        }

        return false;
    }
}
