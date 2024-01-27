using UnityEngine;

public class RotateController : MonoBehaviour
{
    private const float rotationSpeed = 150f;

    public GameObject[] ObjectRotate { get; private set; }

    public void SetObjectRotate(int index, GameObject obj) => ObjectRotate[index] = obj;

    public DragObject DragObject(int index) => ObjectRotate[index]?.GetComponent<DragObject>();

    public GoodInfo GoodInfo(int index) => ObjectRotate[index]?.GetComponent<GoodInfo>();

    private void Start()
    {
        ObjectRotate = new GameObject[KeyboardAndJostickController.MAXPLAYERS];
    }

    private void Update()
    {
        for (int i = 0; i < KeyboardAndJostickController.MAXPLAYERS; i++)
        {
            if (DragObject(i) == null)
            {
                return;
            }

            float horizontalInput = 0;//Input.GetAxis("Horizontal");
            float verticalInput = 0;// Input.GetAxis("Vertical");
            horizontalInput = KeyboardAndJostickController.GetRotate(DragObject(i).Index).horizontal;
            verticalInput = KeyboardAndJostickController.GetRotate(DragObject(i).Index).vertical;


            float rotationAmount = rotationSpeed * Time.deltaTime;

            if (ObjectRotate[i] != null)
            {
                // ѕримен€ем поворот вокруг вертикальной оси (Y) по горизонтальному вводу (A/D)
                ObjectRotate[i].transform.Rotate(0f, horizontalInput * rotationAmount, 0f, Space.World);

                // ѕримен€ем поворот вокруг горизонтальной оси (X) по вертикальному вводу (W/S)
                ObjectRotate[i].transform.Rotate(verticalInput * rotationAmount, 0f, 0f, Space.World);
            }
        }
    }
}
