using UnityEngine;

public class DragObject : MonoBehaviour
{

    [SerializeField]
    private GameObject _qCodeObject;

    private const float koef_mouse_pos = 0.045f;
    private RotateController _rotateController;
    private Rigidbody _rigidbody;

    private bool _isDragging = false;
    private Vector3 _offset;

    public delegate void ContainerDelegete(GameObject good);
    public event ContainerDelegete OnExitContainer;
    public event ContainerDelegete OnEnterContainer;
   
    public GameObject GetQCodeObject => _qCodeObject;

    private void Start()
    {
        _rotateController = FindObjectOfType<RotateController>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            _isDragging = true;
            _offset = transform.position - GetMouseWorldPosition();
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _rotateController.ObjectRotate = gameObject;
        }
    }

    private void OnMouseUp()
    {
        _isDragging = false;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rotateController.ObjectRotate = null;

    }

    private void Update()
    {
      if (_isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            transform.position = new Vector3(mousePos.x + _offset.x, mousePos.y + _offset.y, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Container")
            OnEnterContainer(this.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Container")
            OnExitContainer(this.gameObject);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z * koef_mouse_pos;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
