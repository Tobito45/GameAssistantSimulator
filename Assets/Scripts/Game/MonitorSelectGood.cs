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
    private const int MAXCOUNTMODES = 2;

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
   // [SerializeField]
   // private MonitorGoodList[] _goodList;

    //[SerializeField]
    //private GameObject[] _monitorSelecter, _yPanel, _selecterCanvas;
    private float[] timer = new float[KeyboardAndJostickController.MAXPLAYERS];

    [SerializeField]
    private MainController _mainController;

    private GameObject[] _selectedObject = new GameObject[KeyboardAndJostickController.MAXPLAYERS];

    private List<GameObject>[] _objectsWillBeIterrated = new List<GameObject>[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _indexForIterator = new int[KeyboardAndJostickController.MAXPLAYERS];

    [SerializeField]
    private MonitorSelectUIPlayerIterrator[] _buttonsModes = new MonitorSelectUIPlayerIterrator[KeyboardAndJostickController.MAXPLAYERS];
    [SerializeField]
    private MonitorSelectSceneObjectsPlayerIterrator[] _objectsScene = new MonitorSelectSceneObjectsPlayerIterrator[KeyboardAndJostickController.MAXPLAYERS];

    public Action<int> AfterPay;

    private void Start()
    {
        TextAsset json = Resources.Load<TextAsset>(jsonName);
        string jsonFile = json.ToString();
        SelectedDataJson[] dataObject = JsonConvert.DeserializeObject<SelectedDataJson[]>(jsonFile);

        for (int i = 0; i < 3; i++) //TODO
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
            AfterPay += _objectsScene[i].GetGoodList.ClearGoods;

            for (int j = 1; j <= 9; j++)
            {
                var obj = Instantiate(_numberPrefab, _buttonsModes[i].GetScrollNumberCode.transform);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = j.ToString();
                int index = i;
                obj.GetComponentInChildren<Button>().onClick.AddListener(() => OnSelectObject(obj.gameObject, index)); //_buttonsModes[index].GetInputFieldNumber.text += j.ToString()
            }
            ActivaeButtonModeWithIterator(i);

        }
        MainController.ForeachAllObjects(_objectsScene.Where(n => n.GetMonitroSelecter != null).Select(n => n.GetMonitroSelecter).ToArray(), (g) => g.SetActive(false));
        MainController.ForeachAllObjects(_objectsScene.Where(n => n.GetYPanel != null).Select(n => n.GetYPanel).ToArray(), (g) => g.SetActive(true));

    }

    private void Update()
    {
        foreach (int index in KeyboardAndJostickController.SelectNextGoodOnMonitor())
        {
            if (!GameController.Instance.IsOpenedPanelUI[index])
                continue;

            NextObjectSelect(index);
        }


        foreach (int index in KeyboardAndJostickController.ConfirmPayment())
        {
            if (!GameController.Instance.IsOpenedPanelUI[index])
                continue;

            //_buttonPay[index].onClick.Invoke();
        }

        DetectUIChanges();

    }

    private void DetectUIChanges()
    {
        if (KeyboardAndJostickController.IsJosticConnected)
        {
            foreach (int index in KeyboardAndJostickController.GetAButton())
            {
                if (!GameController.Instance.IsOpenedPanelUI[index])
                    continue;

                //_buttonAccept[index].onClick.Invoke();
                _buttonsModes[index].GetButtonRight.onClick.Invoke();
            }


            foreach (int index in KeyboardAndJostickController.GetYButton())
            {
                if (!GameController.Instance.IsOpenedPanelUI[index])
                    continue;

                // _buttonPay[index].onClick.Invoke();
                _buttonsModes[index].GetButtonLeft.onClick.Invoke();
            }

            if (!_mainController.IsMenu)
            {

                foreach (int index in KeyboardAndJostickController.GetBButton())
                {
                    if (_objectsScene[index].GetSelecterCanvas.activeInHierarchy)
                        ActiveOrDisableCanvasSelect(index);
                }

                foreach (int index in KeyboardAndJostickController.GetYButton())
                {
                    if (!_objectsScene[index].GetSelecterCanvas.activeInHierarchy)
                        ActiveOrDisableCanvasSelect(index);
                }
            }

            foreach (int index in KeyboardAndJostickController.GetButtonLT())
            {
                if (!GameController.Instance.IsOpenedPanelUI[index])
                    continue;

                ActivaeButtonModeWithIterator(index);
            }


            for (int i = 0; i < KeyboardAndJostickController.GetCountGamepads(); i++)
            {
                if (!GameController.Instance.IsOpenedPanelUI[i])
                    continue;

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
                        NextObjectSelect(i, -_objectsWillBeIterrated[i].Count / _buttonsModes[i].GetKoeficent);

                    }
                    else if (movement.vertical < -0.25f)
                    {
                        NextObjectSelect(i, _objectsWillBeIterrated[i].Count / _buttonsModes[i].GetKoeficent);
                    }
                    timer[i] = 0.07f;
                }
                else
                {
                    timer[i] -= Time.deltaTime;
                }
            }
        }
    }

    public void ActivaeButtonModeWithIterator(int index)
    {
        _buttonsModes[index].NextButton().onClick?.Invoke();
        _objectsWillBeIterrated[index] = _buttonsModes[index].GetObjectsToIterate();
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
        StartCoroutine(MainController.MakeActionAfterTime(() => { _buttonsModes[index].GetTextError.gameObject.SetActive(true); }, 
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
        //if(GameController.Instance.IsOpenedPanelUI[index])
        //    ActiveOrDisableCanvasSelect(index);
    }
}


class SelectedDataJson
{
    public string PathImage { get; set; }
    public string Name { get; set; }

    public float Price { get; set; }

}
[System.Serializable]
class MonitorSelectSceneObjectsPlayerIterrator
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
class MonitorSelectUIPlayerIterrator
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
