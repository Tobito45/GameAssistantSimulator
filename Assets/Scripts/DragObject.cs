using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    [SerializeField]
    private const float koef_mouse_pos = -0.3f;

    [SerializeField]
    private RotateController rotateController;

    [SerializeField]
    private GameObject QCodeObject;

    private Rigidbody _rigidbody;

   

    private bool isDragging = false;
    private Vector3 offset;
    //private GameObject parent;
    private Vector3 _position;
   
    public GameObject GetQCodeObject => QCodeObject;

    private void Start()
    {
       // parent = this.gameObject.transform.parent.gameObject;
       // _position = parent.transform.position - transform.position;

        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            isDragging = true;
            offset = transform.position - GetMouseWorldPosition();
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            rotateController.ObjectRotate = gameObject;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        rotateController.ObjectRotate = null;

    }

    private void Update()
    {
        //parent.transform.position = _position + transform.position;
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, transform.position.z);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z * koef_mouse_pos;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
