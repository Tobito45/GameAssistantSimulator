using UnityEngine;

public class CameraMainFollow : MonoBehaviour
{
    private readonly Vector3 ROTATION_AXIS = Vector3.up;

    [SerializeField]
    private float _rotationSpeed = 15f;

    [SerializeField]
    private Transform _targetLookObject, _targetRotateObject;

    private void Update()
    {
       transform.RotateAround(_targetRotateObject.position, ROTATION_AXIS, _rotationSpeed * Time.deltaTime);
       transform.LookAt(_targetLookObject);
    }
}
