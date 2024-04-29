using UnityEngine;

public class CameraMainFollow : MonoBehaviour
{
    private const float rotationSpeed = 15f;
    private readonly Vector3 rotationAxis = Vector3.up;

    [SerializeField]
    private Transform _targetLookObject, _targetRotateObject;

    private void Update()
    {
       transform.RotateAround(_targetRotateObject.position, rotationAxis, rotationSpeed * Time.deltaTime);
       transform.LookAt(_targetLookObject);
    }
}
