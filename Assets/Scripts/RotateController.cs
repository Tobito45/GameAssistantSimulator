using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateController : MonoBehaviour
{
    private const float _rotationSpeed = 100f;

    public GameObject ObjectRotate { get; set; }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float rotationAmount = _rotationSpeed * Time.deltaTime;

        if (ObjectRotate != null)
        {
            // ��������� ������� ������ ������������ ��� (Y) �� ��������������� ����� (A/D)
            ObjectRotate.transform.Rotate(0f, horizontalInput * rotationAmount, 0f, Space.World);

            // ��������� ������� ������ �������������� ��� (X) �� ������������� ����� (W/S)
            ObjectRotate.transform.Rotate(verticalInput * rotationAmount, 0f, 0f, Space.World);
        }
    }
}
