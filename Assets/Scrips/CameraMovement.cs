using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement Boundaries")]
    [SerializeField]
    private float minX = -10f; 
    [SerializeField]
    private float maxX = 10f; 
    [SerializeField]
    private float minZ = -10f; 
    [SerializeField]
    private float maxZ = 10f;
    [Header("Movement Settings")]
    [SerializeField] 
    private float _moveSpeed = 5f;
    [SerializeField] 
    private float _sensitivity = 2f;

    float _rotationY = 0f;
    bool _movingPlayer = true;

    Vector3 _movPosition = Vector3.zero;
    Quaternion _rotation = Quaternion.identity;

    private void Awake()
    {
        _movPosition = transform.position;
        _rotation = transform.rotation;
    }

    void Update()
    {
        if(_movingPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical) * _moveSpeed * Time.deltaTime;
            movement = Camera.main.transform.TransformDirection(movement);
            movement.y = 0;

            Vector3 newPosition = transform.position + movement;

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

            transform.position = newPosition;



            if (Input.GetMouseButton(2))
            {
                float mouseX = Input.GetAxis("Mouse X");

                _rotationY += mouseX * _sensitivity;

                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, _rotationY, transform.localEulerAngles.z);
            }
        }
    }

    public void SetAttackCamera(Vector3 pos, Vector3 lookAt, Transform parent)
    {
        _movPosition = transform.position;
        _rotation = transform.rotation;
        _movingPlayer = false;

        transform.position = pos;
        transform.LookAt(pos + lookAt);
        transform.SetParent(parent);
    }

    public void SetMovementCamera()
    {
        transform.SetParent(null);
        transform.position = _movPosition;
        transform.rotation = _rotation;
        _movingPlayer = true;
    }
}
