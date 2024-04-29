using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputKeyboardController : MonoBehaviour
{
    private const string SYMBOLS = "1234567890qwertyuiopasdfghjklzxcvbnm_+=*&@#%D";

    [SerializeField]
    private GameObject[] _keyboard;
    [SerializeField]
    private GameObject _prefabForCreate;

    public Action<char>[] OnSelected = new Action<char>[KeyboardAndJostickController.MAXPLAYERS];
    public Action[] OnDisable = new Action[KeyboardAndJostickController.MAXPLAYERS];

    private JosticScrollSelecter _scrollSelecter;


    private GridLayoutGroup[] _layoutGroup = new GridLayoutGroup[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _movementConst = new int[KeyboardAndJostickController.MAXPLAYERS];

    void Start()
    {
        _scrollSelecter = new JosticScrollSelecter();

        for (int i = 0; i < _keyboard.Length; i++)
        {
            _layoutGroup[i] = _keyboard[i].GetComponent<GridLayoutGroup>();
        }
        GameController.Instance.OnStartNewGame += OnStartOfGame;

        CreatingButtons();
    }

    private void OnStartOfGame()
    {
        for (int i = 0; i < _keyboard.Length; i++)
            switch (KeyboardAndJostickController.GetCountControllers())
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
                case 3:
                case 4:
                    _layoutGroup[i].constraintCount = 3;
                    _layoutGroup[i].cellSize = new Vector2(50, 50);
                    _movementConst[i] = 3;
                    break;
            }
    }

    public void Active(int index, bool act, Action disable)
    {
        _scrollSelecter.ResetIndexForIterator(index);
        if (_scrollSelecter.SelectedObject(index) != null)
        {
            _scrollSelecter.SelectedObject(index).transform.GetChild(0).gameObject.SetActive(false);
            _scrollSelecter.ResetSelectedObject(index);
        }

        _keyboard[index].SetActive(act);
        OnDisable[index] = disable;
    }

    private void CreatingButtons()
    {
        for (int i = 0; i < _keyboard.Length; i++)
            foreach (char c in SYMBOLS)
            {
                int index = i;
                var obj = Instantiate(_prefabForCreate, _keyboard[i].transform);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = c.ToString();
                obj.GetComponentInChildren<Button>().onClick.AddListener(() => { OnSelected[index](c); });
                _scrollSelecter.AddObjectThatWillBeIterated(index, obj);
            }
    }
    private void Update()
    {
        foreach (int index in KeyboardAndJostickController.GetAButton())
            if (_scrollSelecter.SelectedObject(index) != null)
                _scrollSelecter.SelectedObject(index).GetComponentInChildren<Button>().onClick.Invoke();

        foreach (int index in KeyboardAndJostickController.GetBButton())
            if (OnDisable[index] != null)
                OnDisable[index]();

        for (int i = 0; i < KeyboardAndJostickController.GetCountControllers(); i++)
            KeyboardControllingMenu(i);
    }


    private void KeyboardControllingMenu(int i)
    {
        if (!_keyboard[i].activeInHierarchy)
            return;

        _scrollSelecter.MoveIterator(i, _movementConst[i]);
    }
}
