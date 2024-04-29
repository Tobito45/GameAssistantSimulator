using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonitorSelectGood : MonoBehaviour
{
    private const float COOLDOWN = 3f;
    private readonly Vector2 NUMBER_SIZE_NUMBERS_UP_TWO = new Vector2(130, 130);
    private readonly Vector2 NUMBER_SIZE_NUMBERS_DOWN_TWO = new Vector2(250, 250);

    private readonly Vector2 SELECT_SIZE_UP_TWO = new Vector2(152, 141.5f);
    private readonly Vector2 SELECT_SIZE_DOWN_TWO = new Vector2(304, 283);

    [Header("Json")]
    [SerializeField]
    private string jsonFilePath;
    [SerializeField]
    private string jsonName;

    [Header("Monitor")]
    [SerializeField]
    private Image _goodPrefab;
    [SerializeField]
    private GameObject _numberPrefab;
  
    private float[] timer = new float[KeyboardAndJostickController.MAXPLAYERS];
    private GameObject[] _selectedObject = new GameObject[KeyboardAndJostickController.MAXPLAYERS];
    private List<GameObject>[] _objectsWillBeIterated = new List<GameObject>[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _indexForIterator = new int[KeyboardAndJostickController.MAXPLAYERS];

    [Header("References")]
    [SerializeField]
    private MainController _mainController;
    [SerializeField]
    private MonitorSelectUIPlayerIterator[] _buttonsModes = new MonitorSelectUIPlayerIterator[KeyboardAndJostickController.MAXPLAYERS];
    [SerializeField]
    private MonitorSelectSceneObjectsPlayerIterator[] _objectsScene = new MonitorSelectSceneObjectsPlayerIterator[KeyboardAndJostickController.MAXPLAYERS];

    public Action<int> AfterPay;

    private void Start()
    {
        GameController.Instance.OnStartNewGame += OnStartSettings;
        TextAsset json = Resources.Load<TextAsset>(jsonName);
        string jsonFile = json.ToString();
        SelectedDataJson[] dataObject = JsonConvert.DeserializeObject<SelectedDataJson[]>(jsonFile);

        for (int i = 0; i < KeyboardAndJostickController.MAXPLAYERS; i++)
        {
            GenerateSelectedItems(dataObject, i);

            AfterPay += _objectsScene[i].GetGoodList.ClearGoods;
            GenerateNumberItems(i);
            
            ActivaeButtonModeWithIterator(i);

        }
        FunctionsExtensions.ForeachAllObjects(_objectsScene.Where(n => n.GetMonitroSelecter != null).Select(n => n.GetMonitroSelecter).ToArray(), (g) => g.SetActive(false));
        FunctionsExtensions.ForeachAllObjects(_objectsScene.Where(n => n.GetYPanel != null).Select(n => n.GetYPanel).ToArray(), (g) => g.SetActive(true));
    }

    private void GenerateNumberItems(int i)
    {
        for (int j = 1; j <= 9; j++)
        {
            var obj = Instantiate(_numberPrefab, _buttonsModes[i].GetScrollNumberCode.transform);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = j.ToString();
            int index = i;
            obj.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                OnSelectObject(obj.gameObject, index);
                if (!KeyboardAndJostickController.IsJosticConnected)
                    OnApply(0);

            });
        }
    }

    private void GenerateSelectedItems(SelectedDataJson[] dataObject, int i)
    {
        foreach (var item in dataObject)
        {
            var obj = Instantiate(_goodPrefab, _buttonsModes[i].GetScrollSelectedGood.transform);
            Sprite loadedSprite = Resources.Load<Sprite>(item.PathImage);

            obj.transform.GetChild(0).gameObject.SetActive(false);

            Transform button = obj.transform.GetChild(1);
            button.GetComponent<Image>().sprite = loadedSprite;
            var comp = button.AddComponent<GoodInfoSelect>();
            comp.SetSelectInfo(item.Price, item.Name);

            int index = i;
            button.GetComponent<Button>().onClick.AddListener(() => OnSelectObject(obj.gameObject, index));
        }
    }

    private void Update()
    {
        DetectUIChanges();
    }

    private void OnStartSettings()
    {
        //setting setting for grid layout to numbers selecter scroll
        foreach (var obj in _buttonsModes.Select(n => n.GetScrollNumberCode))
        {
            if (KeyboardAndJostickController.GetCountControllers() > 2)
                obj.GetComponent<GridLayoutGroup>().cellSize = NUMBER_SIZE_NUMBERS_UP_TWO;
            else
                obj.GetComponent<GridLayoutGroup>().cellSize = NUMBER_SIZE_NUMBERS_DOWN_TWO;
        }

        //setting setting for grid layout to goods selecter scroll
        foreach (var obj in _buttonsModes.Select(n => n.GetScrollSelectedGood))
        {
            if (KeyboardAndJostickController.GetCountControllers() > 2)
                obj.GetComponent<GridLayoutGroup>().cellSize = SELECT_SIZE_UP_TWO;
            else
                obj.GetComponent<GridLayoutGroup>().cellSize = SELECT_SIZE_DOWN_TWO;
        }
    }

    private void DetectUIChanges()
    {
        if (KeyboardAndJostickController.IsJosticConnected)
        {
            ActivateRightButton();
            ActivateLeftButton();

            if (!_mainController.IsMenu)
            {
                DisablePanelSelect();
                EnablePanelSelect();
            }

            ChangePanelsInSelectPanel();
            MoveSelectedItemInScrolls();
        }
    }

    private void MoveSelectedItemInScrolls()
    {
        for (int i = 0; i < KeyboardAndJostickController.GetCountControllers(); i++)
        {
            if (!GameController.Instance.IsOpenedPanelUI[i])
                continue;

            if (timer[i] < 0)
            {
                var movement = KeyboardAndJostickController.GetMovement(i);

                if (movement.horizontal > 0.25f)
                    NextObjectSelect(i, -1);
                else if (movement.horizontal < -0.25f)
                    NextObjectSelect(i, 1);
                else if (movement.vertical > 0.25f)
                    NextObjectSelect(i, -_objectsWillBeIterated[i].Count / _buttonsModes[i].GetKoeficent);
                else if (movement.vertical < -0.25f)
                    NextObjectSelect(i, _objectsWillBeIterated[i].Count / _buttonsModes[i].GetKoeficent);

                timer[i] = 0.07f;
            }
            else
            {
                timer[i] -= Time.deltaTime;
            }
        }
    }

    private void ChangePanelsInSelectPanel()
    {
        //getting everyone who pressed RT
        foreach (int index in KeyboardAndJostickController.GetButtonRT())
        {
            //check if the product selection panel is activated
            if (!GameController.Instance.IsOpenedPanelUI[index] && !GameController.Instance.EndMenuController.IsEndPanelActive(index))
                continue;

            //activation of the product selection controlling
            ActivaeButtonModeWithIterator(index);
        }
    }

    private void EnablePanelSelect()
    {
        //getting everyone who pressed Y
        foreach (int index in KeyboardAndJostickController.GetYButton())
        {
            //check if the product selection panel can be activated
            if (!_objectsScene[index].GetSelecterCanvas.activeInHierarchy && !GameController.Instance.EndMenuController.IsEndPanelActive(index))
                //activation of the product selection panel
                ActiveOrDisableCanvasSelect(index);
        }
    }

    private void DisablePanelSelect()
    {
        //getting everyone who pressed B
        foreach (int index in KeyboardAndJostickController.GetBButton())
        {
            //check if the product selection panel can be activated
            if (_objectsScene[index].GetSelecterCanvas.activeInHierarchy && !GameController.Instance.EndMenuController.IsEndPanelActive(index))
                //disable of the product selection panel
                ActiveOrDisableCanvasSelect(index);
        }
    }

    private void ActivateLeftButton()
    {
        //getting everyone who pressed Y
        foreach (int index in KeyboardAndJostickController.GetYButton())
        {
            //check if the product selection panel is activated
            if (!GameController.Instance.IsOpenedPanelUI[index] && !GameController.Instance.EndMenuController.IsEndPanelActive(index))
                continue;


            //activating right button
            _buttonsModes[index].GetButtonLeft.onClick.Invoke();
        }
    }

    private void ActivateRightButton()
    {
        //getting everyone who pressed A
        foreach (int index in KeyboardAndJostickController.GetAButton())
        {
            //check if the product selection panel is activated
            if (!GameController.Instance.IsOpenedPanelUI[index] && !GameController.Instance.EndMenuController.IsEndPanelActive(index))
                continue;

            //activating right button
            _buttonsModes[index].GetButtonRight.onClick.Invoke();
        }
    }

    public void ActivaeButtonModeWithIterator(int index)
    {
        _buttonsModes[index].NextButton().onClick?.Invoke();
        _objectsWillBeIterated[index] = _buttonsModes[index].GetObjectsToIterate();
        _indexForIterator[index] = -1;
        NextObjectSelect(index);
    }

    public void NextButtonMode(int index)
    {
        _buttonsModes[index].NextButton();
    }

    public void ActiveOrDisableCanvasSelect(int index)
    {
        GameController.Instance.IsOpenedPanelUI[index] = !_objectsScene[index].GetSelecterCanvas.activeInHierarchy;
        _objectsScene[index].GetYPanel.SetActive(_objectsScene[index].GetSelecterCanvas.activeInHierarchy);
        _objectsScene[index].GetSelecterCanvas.SetActive(!_objectsScene[index].GetSelecterCanvas.activeInHierarchy);

    }

    private void NextObjectSelect(int index, int countAdd = 1)
    {
        _indexForIterator[index] += countAdd;
        if (_indexForIterator[index] >= _objectsWillBeIterated[index].Count || _indexForIterator[index] < 0)
        {
            _indexForIterator[index] -= countAdd;
        }
        OnSelectObject(_objectsWillBeIterated[index][_indexForIterator[index]], index);

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

    public void OnApply(int index)
    {
        if (_selectedObject[index] == null)
            return;

        var getInfo = _selectedObject[index].GetComponentInChildren<GoodInfo>();
        if (getInfo != null)
        {
            _objectsScene[index].GetGoodList.AddGood(getInfo);
        } else
        {
            if(_buttonsModes[index].GetInputFieldNumber.text.Length < _buttonsModes[index].GetInputFieldNumber.characterLimit)
            _buttonsModes[index].GetInputFieldNumber.text = _buttonsModes[index].GetInputFieldNumber.text + _selectedObject[index].GetComponentInChildren<TextMeshProUGUI>().text;
        }
    }

    public void OnCheckGood(int index)
    {
        var actualText = _buttonsModes[index].GetInputFieldNumber.text;
        GoodInfo good = null;

        if (GameController.Instance.GetByCode(actualText, index, out good))
        {
            _buttonsModes[index].GetTextError.text = "Good was added to list";
            _buttonsModes[index].GetTextError.color = Color.green;

            _objectsScene[index].GetGoodList.AddGood(good);
        } else
        {
            _buttonsModes[index].GetTextError.text = "Code does not exists";
            _buttonsModes[index].GetTextError.color = Color.red;
        }
        StartCoroutine(FunctionsExtensions.MakeActionAfterTime(() => { _buttonsModes[index].GetTextError.gameObject.SetActive(true); }, 
                                             () => { _buttonsModes[index].GetTextError.gameObject.SetActive(false); }, 2f));
        _buttonsModes[index].GetInputFieldNumber.text = null;
    }

    public void OnPay(int index)
    {
        StartCoroutine(OnPayCoolDown(index));
    }

    private IEnumerator OnPayCoolDown(int index)
    {
        AfterPay(index);
        _buttonsModes[index].GetButtonLeft.interactable = false;
        _objectsScene[index].GetSelecterCanvas.SetActive(false);
        GameController.Instance.IsOpenedPanelUI[index] = false;

        if (KeyboardAndJostickController.IsJosticConnected)
        {
            _objectsScene[index].GetYPanel.SetActive(true);
            GameController.Instance.IsOpenedPanelUI[index] = false;
        }

        yield return new WaitForSeconds(COOLDOWN);
        _buttonsModes[index].GetInputFieldNumber.text = null;
        _buttonsModes[index].GetButtonLeft.interactable = true;

        _objectsScene[index].GetYPanel.SetActive(true);
    }
}

class SelectedDataJson
{
    public string PathImage { get; set; }
    public string Name { get; set; }

    public float Price { get; set; }

}

[System.Serializable]
class MonitorSelectSceneObjectsPlayerIterator
{
    [SerializeField]
    private MonitorGoodList _goodList;

    [SerializeField]
    private GameObject _monitorSelecter, _yPanel, _selecterCanvas;

    public MonitorGoodList GetGoodList => _goodList;
    public GameObject GetMonitroSelecter => _monitorSelecter;
    public GameObject GetYPanel => _yPanel;
    public GameObject GetSelecterCanvas => _selecterCanvas;

}

[System.Serializable]
class MonitorSelectUIPlayerIterator
{
    [SerializeField]
    private List<GameObject> _buttons;
    [SerializeField]
    private List<Button> _buttonLeft, _buttonRight;
    [SerializeField]
    private List<GameObject> _contents;
    [SerializeField]
    private List<int> _koeficentScroll;
    [SerializeField]
    private TMP_InputField _inputNumber;
    [SerializeField]
    private TextMeshProUGUI _textError;
    private int _iterator = -1;

    public Button NextButton()
    {
        _iterator++;
        if (_iterator >= _buttons.Count)
            _iterator = 0;
        return _buttons[_iterator].GetComponent<Button>();
    }
    public TMP_InputField GetInputFieldNumber => _inputNumber;
    public TextMeshProUGUI GetTextError => _textError;
    public Button GetButtonLeft => _buttonLeft[_iterator];
    public Button GetButtonRight => _buttonRight[_iterator];
    public GameObject GetScrollSelectedGood => _contents[0]; //0 is for selected goods 
    public GameObject GetScrollNumberCode => _contents[1]; //1 is for number code 
    public int GetKoeficent => _koeficentScroll[_iterator]; //1 is for number code 
    public List<GameObject> GetObjectsToIterate()
    {
        List<GameObject> objectsToIterate = new List<GameObject>();
        Transform currentTransform = _contents[_iterator].transform;
        int childCount = currentTransform.childCount;
        for (int i = 0; i < childCount; i++)
            objectsToIterate.Add(currentTransform.GetChild(i).gameObject);
        return objectsToIterate;
    }
}
