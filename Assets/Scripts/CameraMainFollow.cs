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
        // Вычисляем новый угол поворота камеры на основе позиции целевого объекта
        //float desiredAngle = targetObject.eulerAngles.y;
        //Quaternion rotation = Quaternion.Euler(0f, desiredAngle, 0f);

        // Вычисляем новую позицию камеры на основе позиции целевого объекта и заданного расстояния
        //Vector3 desiredPosition = targetObject.position - (rotation * Vector3.forward * distance);

        // Применяем плавное перемещение камеры к новой позиции и повороту
        //transform.position = Vector3.Lerp(transform.position, desiredPosition, rotationSpeed * Time.deltaTime);
        //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        // transform.RotateAround(_targetObject, Vector3.left)
        transform.RotateAround(_targetObject.position, rotationAxis, rotationSpeed * Time.deltaTime);
        // Направляем камеру на целевой объект
        transform.LookAt(_targetObject);
    }
}
