using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RotateController : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 150f;

    private GameObject[] _objectsRotate = new GameObject[KeyboardAndJostickController.MAXPLAYERS];
    public IEnumerable<GameObject> ObjectRotate => _objectsRotate;

    public void SetObjectRotate(int index, GameObject obj) => _objectsRotate[index] = obj;

    public DragObject DragObject(int index) => ObjectRotate.ElementAt(index)?.GetComponent<DragObject>();

    public GoodInfo GoodInfo(int index) => ObjectRotate.ElementAt(index)?.GetComponent<GoodInfo>();


    private void Update()
    {
        for (int i = 0; i < KeyboardAndJostickController.MAXPLAYERS; i++)
        {
            if (DragObject(i) == null || GameController.Instance.IsOpenedPanelUI[i])
            {
                continue;
            }

            float horizontalInput = 0;
            float verticalInput = 0;
            horizontalInput = -KeyboardAndJostickController.GetRotate(DragObject(i).Index).horizontal;
            verticalInput = KeyboardAndJostickController.GetRotate(DragObject(i).Index).vertical;


            float rotationAmount = rotationSpeed * Time.deltaTime;

            if (_objectsRotate[i] != null)
            {
                _objectsRotate[i].transform.Rotate(0f, horizontalInput * rotationAmount, 0f, Space.World);
                _objectsRotate[i].transform.Rotate(verticalInput * rotationAmount, 0f, 0f, Space.World);
            }
        }
    }
}
