using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class KeyBoardForJostic : MonoBehaviour
{
    [SerializeField]
    private GameObject _keyboard, _prefabForCreate;

    private string _symbols = "1234567890qwertyuiopasdfghjklzxcvbnm_+=*&@#%D";

    public Action<char> OnSelected;
    public Action OnDisable;

    private float[] timer = new float[KeyboardAndJostickController.MAXPLAYERS];
    private List<GameObject>[] _objectsWillBeIterrated = new List<GameObject>[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _indexForIterator = new int[KeyboardAndJostickController.MAXPLAYERS];
    private GameObject[] _selectedObject = new GameObject[KeyboardAndJostickController.MAXPLAYERS];


    private GridLayoutGroup _layoutGroup;
    public GameObject Keyboard => _keyboard;  
    // Start is called before the first frame update
    void Start()
    {
        _layoutGroup = _keyboard.GetComponent<GridLayoutGroup>();
        _objectsWillBeIterrated[0] = new List<GameObject>();

        GameController.Instance.OnStartNewGame += OnStartOfGame;

        CreatingButtons();
    }

    private void OnStartOfGame()
    {
        switch(KeyboardAndJostickController.GetCountGamepads())
        {
            case 1:
                _layoutGroup.constraintCount = 3;
                _layoutGroup.cellSize = new Vector2(100,100);
                break;
            case 2:
                _layoutGroup.constraintCount = 5;
                _layoutGroup.cellSize = new Vector2(75, 75);
                break;

        }
    }

    public void Active(bool act, Action disable)
    {
        _indexForIterator[0] = -1; //TODO
        _keyboard.SetActive(act);
        OnDisable = disable;
    }

    private void CreatingButtons()
    {
        foreach(char c in _symbols)
        {
            var obj = Instantiate(_prefabForCreate, _keyboard.transform);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = c.ToString();
            obj.GetComponentInChildren<Button>().onClick.AddListener(() => { OnSelected(c); });
            _objectsWillBeIterrated[0].Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        KeyboardControllingMenu();
    }


    private void KeyboardControllingMenu(int i = 0) //TODO
    {
        //for (int i = 0; i < KeyboardAndJostickController.GetCountGamepads(); i++)
        {
            if (!_keyboard.activeInHierarchy)
                return;

            foreach (int index in KeyboardAndJostickController.GetAButton())
            {
                _selectedObject[index].GetComponentInChildren<Button>().onClick.Invoke();
            }

            foreach (int index in KeyboardAndJostickController.GetBButton())
            {
                OnDisable();
            }


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
                    NextObjectSelect(i, -_objectsWillBeIterrated[i].Count / 3);

                }
                else if (movement.vertical < -0.25f)
                {
                    NextObjectSelect(i, _objectsWillBeIterrated[i].Count / 3);
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
