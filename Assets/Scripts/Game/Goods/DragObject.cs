using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    public static float BOARDERTOLETGOGOOD = -2.1f;
    public static float BOARDERCANOTMOVE = -1.95f;


    [SerializeField]
    private GameObject _qCodeObject;

    [SerializeField]
    private TextMeshPro _textQrCode;

    private const float koef_mouse_pos = 0.045f;
    private const float SPEED = 0.5f;
    private RotateController _rotateController;
    private Rigidbody _rigidbody;
    private Material _basicMaterial;
    private Outline _outline;

    [SerializeField]
    private Material _detectedMaterial, _selectedMaterial;
    private GoodInfo _good;
    private bool _isDragging = false;
    private Vector3 _offset;

    private bool _isActualGameIsEnd = false;

    public delegate void ContainerDelegete(GameObject good, int index);
    public event ContainerDelegete OnExitContainer;
    public event ContainerDelegete OnEnterContainer;
    public event Action<DragObject, int> OnLetGoGood;

    public GameObject GetQCodeObject => _qCodeObject;
    public string GetCode => _textQrCode.text;
    public int Index { get; set; }


    private void Start()
    {
        _rotateController = FindObjectOfType<RotateController>();
        _rigidbody = GetComponent<Rigidbody>();
        _good = GetComponent<GoodInfo>();
        _basicMaterial = GetComponent<MeshRenderer>().material;


        _outline = gameObject.AddComponent<Outline>();
        _outline.enabled = false;
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _outline.OutlineWidth = 7;

        GameController.Instance.OnEndGame += OnEndGame;

        if (_textQrCode != null)
        {
            _textQrCode.text = null;
            if(UnityEngine.Random.Range(0, 2) == 0)
            {
                _textQrCode.gameObject.SetActive(true);
                for(int i = 0; i < 5; i++)
                {
                    _textQrCode.text = _textQrCode.text + UnityEngine.Random.Range(1, 9).ToString();
                }
            } else
            {
                _textQrCode.gameObject.SetActive(false);
            }

        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            TakeItem();
        }
    }

    private void OnMouseUp()
    {
        LetGoItem();
    }

    public void TakeItem()
    {
        _isDragging = true;
        _offset = transform.position - GetMouseWorldPosition();
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        _rotateController.SetObjectRotate(Index, gameObject);
    }

    public void CanBeSelected(bool canBeSelected)
    {
        if (!KeyboardAndJostickController.IsJosticConnected)
            return;

        if (canBeSelected)
        {
            //GetComponent<MeshRenderer>().material = _selectedMaterial;
            _outline.OutlineColor = Color.green;
        }
        else
        {
            //GetComponent<MeshRenderer>().material = _detectedMaterial;
            _outline.OutlineColor = Color.yellow;
        }
        _outline.enabled = true;

    }
    public void LetGoItem()
    {
        _isDragging = false;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        _rotateController.SetObjectRotate(Index, null);
    }

    public bool HasQrCode => _textQrCode != null && _textQrCode.gameObject.activeSelf;

    private void Update()
    {
        if (_isActualGameIsEnd)
            return;

        if (transform.position.y < BOARDERTOLETGOGOOD && _outline.enabled)//GetComponent<MeshRenderer>().material != _basicMaterial)
        {
            OnLetGoGood(this, Index);
            // GetComponent<MeshRenderer>().material = _basicMaterial;
            _outline.enabled = false;
        }


        if (!GameController.Instance.IsOpenedPanelUI[Index])
        {
            foreach (int index in KeyboardAndJostickController.LetsGoGood())
            {
                if (index == Index && _isDragging)
                {
                    LetGoItem();

                }
            }
        }

        if (_isDragging && !GameController.Instance.IsOpenedPanelUI[Index])
        {
            MoveGood();
        }
    }

    private void MoveGood()
    {
        (float horizontal, float vertical) input = (0.0f, 0.0f);
        if (KeyboardAndJostickController.IsJosticConnected)
        {
            input = KeyboardAndJostickController.GetMovement(Index);

            if (transform.position.y + input.vertical * Time.deltaTime < BOARDERCANOTMOVE && input.vertical < 0)
                input.vertical = 0;

            transform.position += new Vector3(input.horizontal, input.vertical, 0) * SPEED * Time.deltaTime;
        }
        else
        {
            Vector3 mousePos = GetMouseWorldPosition();
            input = (mousePos.x,mousePos.y);

            if (input.vertical + _offset.y < BOARDERCANOTMOVE)
                transform.position = new Vector3(input.horizontal + _offset.x, transform.position.y, transform.position.z);
            else
                transform.position = new Vector3(input.horizontal + _offset.x, input.vertical + _offset.y, transform.position.z);
        }

        if (_qCodeObject != null && _good.IsScaned == false && !HasQrCode)
        {
            Vector3 rayOrigin = _qCodeObject.transform.position;
            Vector3 rayDirection = _qCodeObject.transform.up;

            Ray ray = new Ray(rayOrigin, rayDirection);
            RaycastHit hit;
            Debug.DrawRay(rayOrigin, rayDirection, Color.red);
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.name == "RedDetecter")
                    GameController.Instance.QCodeDetecter.DetectGood(_good, Index);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Container")
        {
            //GetComponent<MeshRenderer>().material = _basicMaterial;
            _outline.enabled = false;
            Match match = Regex.Match(collision.gameObject.name, @"\d+");
            OnEnterContainer(this.gameObject, int.Parse(match.Value) - 1);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Container")
        {
            //GetComponent<MeshRenderer>().material = _detectedMaterial;
            _outline.enabled = true && KeyboardAndJostickController.IsJosticConnected;
            _outline.OutlineColor = Color.yellow;
            Match match = Regex.Match(collision.gameObject.name, @"\d+");
            OnExitContainer(this.gameObject, int.Parse(match.Value) - 1);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z * koef_mouse_pos;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void OnEndGame(int index)
    {
        if (index != Index)
            return;

        _outline.enabled = false;
        if (_isDragging)
            LetGoItem();

        _isActualGameIsEnd = true;

        GameController.Instance.OnEndGame -= OnEndGame;
    }
}
