using UnityEngine;

public class RotateController : MonoBehaviour
{
    private const float rotationSpeed = 150f;

    public GameObject ObjectRotate { get; set; }

    public DragObject DragObject => ObjectRotate?.GetComponent<DragObject>();

    public GoodInfo GoodInfo => ObjectRotate?.GetComponent<GoodInfo>();

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float rotationAmount = rotationSpeed * Time.deltaTime;

        if (ObjectRotate != null)
        {
            // ��������� ������� ������ ������������ ��� (Y) �� ��������������� ����� (A/D)
            ObjectRotate.transform.Rotate(0f, horizontalInput * rotationAmount, 0f, Space.World);

            // ��������� ������� ������ �������������� ��� (X) �� ������������� ����� (W/S)
            ObjectRotate.transform.Rotate(verticalInput * rotationAmount, 0f, 0f, Space.World);
        }
    }
}
