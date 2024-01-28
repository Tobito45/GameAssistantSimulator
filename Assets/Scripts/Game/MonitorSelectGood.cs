using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("UI")]
    [SerializeField]
    private Button[] _buttonPay, _buttonAccept;


    private GameObject[] _selectedObject = new GameObject[KeyboardAndJostickController.MAXPLAYERS];

    private List<GameObject>[] _objectsWillBeIterrated = new List<GameObject>[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _indexForIterator = new int[KeyboardAndJostickController.MAXPLAYERS];

    public  Action<int> AfterPay;

    private void Start()
    {
        TextAsset json = Resources.Load<TextAsset>(jsonName);
        string jsonFile = json.ToString();
        SelectedDataJson[] dataObject = JsonConvert.DeserializeObject<SelectedDataJson[]>(jsonFile);

        for (int i = 0; i < _goodsInScrollContent.Length;i++)
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

                // button.GetComponent<Button>().onClick.AddListener(() => OnSelectObject(button.gameObject));
            }
            NextObjectSelect(i);
            AfterPay += _goodList[i].ClearGoods;

        }

    }

    private void Update()
    {
        foreach (int index in KeyboardAndJostickController.SelectNextGoodOnMonitor())
            NextObjectSelect(index);

        foreach (int index in KeyboardAndJostickController.ConfirmPayment())
            _buttonPay[index].onClick.Invoke();

        foreach (int index in KeyboardAndJostickController.SelectGoodOnMonitor())
            _buttonAccept[index].onClick.Invoke();
    }

    private void NextObjectSelect(int index)
    {
        _indexForIterator[index]++;
        if (_indexForIterator[index] >= _objectsWillBeIterrated[index].Count)
        {
            _indexForIterator[index] = 0;
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
        Debug.Log("w");
        StartCoroutine(OnPayCoolDown(index));
    }

    private IEnumerator OnPayCoolDown(int index)
    {
        Debug.Log("w2");
        AfterPay(index);
        _buttonPay[index].interactable = false;
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
