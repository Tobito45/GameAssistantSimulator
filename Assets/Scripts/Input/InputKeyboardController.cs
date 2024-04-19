using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputKeyboardController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _keyboard;
    [SerializeField]
    private GameObject _prefabForCreate;

    private string _symbols = "1234567890qwertyuiopasdfghjklzxcvbnm_+=*&@#%D";

    public Action<char>[] OnSelected = new Action<char>[KeyboardAndJostickController.MAXPLAYERS];
    public Action[] OnDisable = new Action[KeyboardAndJostickController.MAXPLAYERS];

    private float[] timer = new float[KeyboardAndJostickController.MAXPLAYERS];
    private List<GameObject>[] _objectsWillBeIterrated = new List<GameObject>[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _indexForIterator = new int[KeyboardAndJostickController.MAXPLAYERS];
    private GameObject[] _selectedObject = new GameObject[KeyboardAndJostickController.MAXPLAYERS];


    private GridLayoutGroup[] _layoutGroup = new GridLayoutGroup[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _movementConst = new int[KeyboardAndJostickController.MAXPLAYERS];

    void Start()
    {
        for (int i = 0; i < _keyboard.Length; i++)
        {
            _layoutGroup[i] = _keyboard[i].GetComponent<GridLayoutGroup>();
            _objectsWillBeIterrated[i] = new List<GameObject>();
        }
        GameController.Instance.OnStartNewGame += OnStartOfGame;

        CreatingButtons();
    }

    private void OnStartOfGame()
    {
        for (int i = 0; i < _keyboard.Length; i++)
        {
            switch (KeyboardAndJostickController.GetCountGamepads())
            {
                case 1:
                    _layoutGroup[i].constraintCount = 3;
                    _layoutGroup[i].cellSize = new Vector2(100, 100);
                    _movementConst[i] = 3;
                    break;
                case 2:
                    _layoutGroup[i].constraintCount = 5;
                    _layoutGroup[i].cellSize = new Vector2(75, 75);
                    _movementConst[i] = 5;
                    break;

            }
        }

    }

    public void Active(int index, bool act, Action disable)
    {
        _indexForIterator[index] = -1;
        if (_selectedObject[index] != null)
        {
            _selectedObject[index].transform.GetChild(0).gameObject.SetActive(false);
            _selectedObject[index] = null;
        }

        _keyboard[index].SetActive(act);
        OnDisable[index] = disable;
    }

    private void CreatingButtons()
    {
        for (int i = 0; i < _keyboard.Length; i++)
        {
            foreach (char c in _symbols)
            {
                int index = i;
                var obj = Instantiate(_prefabForCreate, _keyboard[i].transform);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = c.ToString();
                obj.GetComponentInChildren<Button>().onClick.AddListener(() => { OnSelected[index](c); });
                _objectsWillBeIterrated[index].Add(obj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (int index in KeyboardAndJostickController.GetAButton())
        {
            if (_selectedObject[index] != null)
                _selectedObject[index].GetComponentInChildren<Button>().onClick.Invoke();
        }

        foreach (int index in KeyboardAndJostickController.GetBButton())
        {
            if (OnDisable[index] != null)
            {
                OnDisable[index]();
            }
        }

        for (int i = 0; i < KeyboardAndJostickController.GetCountGamepads(); i++)
            KeyboardControllingMenu(i);
    }


    private void KeyboardControllingMenu(int i) //TODO
    {
        //for (int i = 0; i < KeyboardAndJostickController.GetCountGamepads(); i++)
        {
            if (!_keyboard[i].activeInHierarchy)
                return;

            if (timer[i] < 0)
            {
                var movement = KeyboardAndJostickController.GetMovement(i);

                if (movement.horizontal > 0.25f)
                {
                    NextObjectSelect(i, -1);
                }
                else if (movement.horizontal < -0.25f)
                {
                    NextObjectSelect(i, 1);
                }
                else if (movement.vertical > 0.25f)
                {
                    NextObjectSelect(i, -_objectsWillBeIterrated[i].Count / _movementConst[i]);

                }
                else if (movement.vertical < -0.25f)
                {
                    NextObjectSelect(i, _objectsWillBeIterrated[i].Count / _movementConst[i]);
                }
                timer[i] = 0.05f;
            }
            else
            {
                timer[i] -= Time.deltaTime;
            }
        }
    }

    private void NextObjectSelect(int index, int countAdd = 1)
    {
        _indexForIterator[index] += countAdd;
        /* if (_indexForIterator[index] >= _objectsWillBeIterrated[index].Count)
         {
             _indexForIterator[index] = 0;
         }*/
        if (_indexForIterator[index] >= _objectsWillBeIterrated[index].Count || _indexForIterator[index] < 0)
        {
            _indexForIterator[index] -= countAdd;
        }
        if (_indexForIterator[index] < 0)
            _indexForIterator[index] = 0;
        OnSelectObject(_objectsWillBeIterrated[index][_indexForIterator[index]], index);

    }

    private void OnSelectObject(GameObject button, int index)
    {
        if (_selectedObject[index] != null)
        {
            _selectedObject[index].transform.GetChild(0).gameObject.SetActive(false);
        }
        _selectedObject[index] = button.transform.gameObject;
        _selectedObject[index].transform.GetChild(0).gameObject.SetActive(true);
    }
}