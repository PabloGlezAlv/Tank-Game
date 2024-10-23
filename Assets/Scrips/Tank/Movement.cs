using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TankState { MoveTurn, AttackTurn, Wait }

[RequireComponent(typeof(TankMovement))]
public abstract class Movement : MonoBehaviour
{
    [SerializeField]
    protected UIManager _UIManager;
    [Header("Move Tank Parameters")]
    [SerializeField]
    protected int _numSteps = 1;
    [SerializeField]
    protected float _inputDistance = 5;
    
    protected const int MIN_ANGLE = 270;
    protected const int MAX_ANGLE = 300;
    [Header("Move Canon Parameters")]
    [SerializeField]
    protected Transform _topTank;
    [SerializeField]
    protected float _rotationSpeed = 5;
    protected float _pitch = -90;
    protected float _yaw = -180;

    protected TankMovement _tankMovement;
    protected ShellTraj _shellTraj;

    protected int _currentSteps = 0;

    protected TankState _currentState = TankState.Wait;
    public abstract void MoveCanon();
    public abstract void MovePlayer();

    protected virtual void Awake()
    {
        _tankMovement = GetComponent<TankMovement>();
        _shellTraj = GetComponent<ShellTraj>();
        _currentSteps = 0;
    }

    protected virtual void Update()
    {
        switch (_currentState)
        {
            case TankState.MoveTurn:
                MovePlayer();
                break;
            case TankState.AttackTurn:
                MoveCanon();
                break;
        }
    }

    public virtual void StartMove()
    {
        _currentState = TankState.MoveTurn;
        _shellTraj.enabled = false;
        _currentSteps = _numSteps;
    }

    public virtual void StartShooting()
    {
        _currentState = TankState.AttackTurn;
        _pitch = _topTank.transform.eulerAngles.x;
        _yaw = _topTank.transform.eulerAngles.y;
        _shellTraj.enabled = true;
    }
}
