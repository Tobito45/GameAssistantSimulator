using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonitorSelectGood : MonoBehaviour
{
    private const float cooldown = 3f;

    [Header("Json")]
    [SerializeField]
    private string jsonFilePath;
    [SerializeField]
    private string jsonName;

    [Header("Monitor")]
    [SerializeField]
    private Image _goodPrefab;
    [SerializeField]
    private Transform[] _goodsInScrollContent;
    [SerializeField]
    private MonitorGoodList[] _goodList;

    [SerializeField]
    private GameObject[] _monitorSelecter, _yPanel, _selecterCanvas;
    private float[] timer = new float[KeyboardAndJostickController.MAXPLAYERS];

    [Header("UI")]
    [SerializeField]
    private Button[] _buttonPay, _buttonAccept;

    [SerializeField]
    private MainController _mainController;

    private GameObject[] _selectedObject = new GameObject[KeyboardAndJostickController.MAXPLAYERS];

    private List<GameObject>[] _objectsWillBeIterrated = new List<GameObject>[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _indexForIterator = new int[KeyboardAndJostickController.MAXPLAYERS];

    public Action<int> AfterPay;

    private void Start()
    {
        TextAsset json = Resources.Load<TextAsset>(jsonName);
        string jsonFile = json.ToString();
        SelectedDataJson[] dataObject = JsonConvert.DeserializeObject<SelectedDataJson[]>(jsonFile);

        for (int i = 0; i < _goodsInScrollContent.Length; i++)
        {
            _indexForIterator[i] = -1;
            _objectsWillBeIterrated[i] = new List<GameObject>();

            foreach (var item in dataObject)
            {
                var obj = Instantiate(_goodPrefab, _goodsInScrollContent[i]);
                Sprite loadedSprite = Resources.Load<Sprite>(item.PathImage);

                obj.transform.GetChild(0).gameObject.SetActive(false);

                Transform button = obj.transform.GetChild(1);
                button.GetComponent<Image>().sprite = loadedSprite;
                var comp = button.AddComponent<GoodInfoSelect>();
                comp.SetSelectInfo(item.Price, item.Name);
                _objectsWillBeIterrated[i].Add(obj.gameObject);

                 button.GetComponent<Button>().onClick.AddListener(() => OnSelectObject(obj.gameObject, i));
            }
            NextObjectSelect(i);
            AfterPay += _goodList[i].ClearGoods;

        }

        MainController.ForeachAllObjects(_monitorSelecter, (g) => g.SetActive(false));//!KeyboardAndJostickController.IsJosticConnected));
        MainController.ForeachAllObjects(_yPanel, (g) => g.SetActive(true));//KeyboardAndJostickController.IsJosticConnected));

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

            _buttonPay[index].onClick.Invoke();
        }

        DetectUIChanges();

        // foreach (int index in KeyboardAndJostickController.SelectGoodOnMonitor())
        //   _buttonAccept[index].onClick.Invoke();
    }

    private void DetectUIChanges()
    {
        if (KeyboardAndJostickController.IsJosticConnected)
        {
            if(!_mainController.IsMenu)
                foreach (int index in KeyboardAndJostickController.SelectGoodOnMonitor())
                    ActiveOrDisableCanvasSelect(index);


            foreach (int index in KeyboardAndJostickController.GetAButton())
            {
                if (!GameController.Instance.IsOpenedPanelUI[index])
                    continue;

                _buttonAccept[index].onClick.Invoke();
            }

            //if (KeyboardAndJostickController.GetAButton())
            //  _buttonAccept[0].onClick.Invoke();

            foreach (int index in KeyboardAndJostickController.GetBButton())
            {
                if (!GameController.Instance.IsOpenedPanelUI[index])
                    continue;

                _buttonPay[index].onClick.Invoke();
            }

            //if (KeyboardAndJostickController.GetBButton())
            //    _buttonPay[0].onClick.Invoke();


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
                        Debug.Log("Rogh");
                    }
                    else if (movement.horizontal < -0.25f)
                    {
                        NextObjectSelect(i, 1);
                        Debug.Log("Left");
                    }
                    else if (movement.vertical > 0.25f)
                    {
                        NextObjectSelect(i, -_objectsWillBeIterrated[i].Count / 2);
                        Debug.Log("up");

                    }
                    else if (movement.vertical < -0.25f)
                    {
                        NextObjectSelect(i, _objectsWillBeIterrated[i].Count / 2);
                        Debug.Log("down");
                    }
                    timer[i] = 0.05f;
                }
                else
                {
                    timer[i] -= Time.deltaTime;
                }
            }
        }
    }



    private void ActiveOrDisableCanvasSelect(int index)
    {
        GameController.Instance.IsOpenedPanelUI[index] = !_selecterCanvas[index].activeInHierarchy;
        _yPanel[index].SetActive(_selecterCanvas[index].activeInHierarchy && KeyboardAndJostickController.IsJosticConnected);
        _selecterCanvas[index].SetActive(!_selecterCanvas[index].activeInHierarchy);

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

    public void OnApply(int index)
    {
        if (_selectedObject[index] == null)
            return;

        //_selectedObject.transform.GetChild(0).gameObject.SetActive(false);
        _goodList[index].AddGood(_selectedObject[index].GetComponentInChildren<GoodInfo>());
        // _selectedObject = null;
    }

    public void OnPay(int index)
    {
        StartCoroutine(OnPayCoolDown(index));
    }

    private IEnumerator OnPayCoolDown(int index)
    {
        AfterPay(index);
        _buttonPay[index].interactable = false;
        _selecterCanvas[index].SetActive(false);

        if (KeyboardAndJostickController.IsJosticConnected)
        {
            _yPanel[index].SetActive(true);
            GameController.Instance.IsOpenedPanelUI[index] = false;
        }
        
        yield return new WaitForSeconds(cooldown);
        _buttonPay[index].interactable = true;

    }
}


class SelectedDataJson
{
    public string PathImage { get; set; }
    public string Name { get; set; }

    public float Price { get; set; }

}
