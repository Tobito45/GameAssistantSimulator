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
            // ѕримен€ем поворот вокруг вертикальной оси (Y) по горизонтальному вводу (A/D)
            ObjectRotate.transform.Rotate(0f, horizontalInput * rotationAmount, 0f, Space.World);

            // ѕримен€ем поворот вокруг горизонтальной оси (X) по вертикальному вводу (W/S)
            ObjectRotate.transform.Rotate(verticalInput * rotationAmount, 0f, 0f, Space.World);
        }
    }
}
