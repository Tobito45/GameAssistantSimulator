using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMainFollow : MonoBehaviour
{
    private const float rotationSpeed = 5f;
    private const float distance = 5f;
    private readonly Vector3 rotationAxis = Vector3.up;

    [SerializeField]
    private Transform _targetObject;


    private void Start()
    {
       // Debug.Log(System.Enviroment.Version);
    }

    private void Update()
    {
        // ��������� ����� ���� �������� ������ �� ������ ������� �������� �������
        //float desiredAngle = targetObject.eulerAngles.y;
        //Quaternion rotation = Quaternion.Euler(0f, desiredAngle, 0f);

        // ��������� ����� ������� ������ �� ������ ������� �������� ������� � ��������� ����������
        //Vector3 desiredPosition = targetObject.position - (rotation * Vector3.forward * distance);

        // ��������� ������� ����������� ������ � ����� ������� � ��������
        //transform.position = Vector3.Lerp(transform.position, desiredPosition, rotationSpeed * Time.deltaTime);
        //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        // transform.RotateAround(_targetObject, Vector3.left)
        transform.RotateAround(_targetObject.position, rotationAxis, rotationSpeed * Time.deltaTime);
        // ���������� ������ �� ������� ������
        transform.LookAt(_targetObject);
    }
}
