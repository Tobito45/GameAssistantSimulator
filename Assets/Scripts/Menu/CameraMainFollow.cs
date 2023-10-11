using UnityEngine;

public class CameraMainFollow : MonoBehaviour
{
    private const float rotationSpeed = 5f;
    private readonly Vector3 rotationAxis = Vector3.up;

    [SerializeField]
    private Transform _targetObject;

    private void Update()
    {
       transform.RotateAround(_targetObject.position, rotationAxis, rotationSpeed * Time.deltaTime);
       transform.LookAt(_targetObject);
    }
}
