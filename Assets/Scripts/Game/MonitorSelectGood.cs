using Newtonsoft.Json;
using System;
using System.Collections;
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
    private Transform _goodsInScrollContent;
    [SerializeField]
    private MonitorGoodList _goodList;

    [Header("UI")]
    [SerializeField]
    private Button buttonPay;


    private GameObject SelectedObject;

    public event Action AfterPay;

    private void Start()
    {
        TextAsset json = Resources.Load<TextAsset>(jsonName);
        string jsonFile = json.ToString();
        SelectedDataJson[] dataObject = JsonConvert.DeserializeObject<SelectedDataJson[]>(jsonFile);

        foreach (var item in dataObject)
        {
            var obj = Instantiate(_goodPrefab, _goodsInScrollContent);
            Sprite loadedSprite = Resources.Load<Sprite>(item.PathImage);

            obj.transform.GetChild(0).gameObject.SetActive(false);

            Transform button = obj.transform.GetChild(1);
            button.GetComponent<Image>().sprite = loadedSprite;
            var comp = button.AddComponent<GoodInfoSelect>();
            comp.SetSelectInfo(item.Price, item.Name);

            button.GetComponent<Button>().onClick.AddListener(() => OnSelectObject(button.gameObject));
        }

        AfterPay += _goodList.ClearGoods;
    }

    private void OnSelectObject(GameObject button)
    {
        if(SelectedObject != null) 
            SelectedObject.transform.GetChild(0).gameObject.SetActive(false);

        SelectedObject = button.transform.parent.gameObject;
        SelectedObject.transform.GetChild(0).gameObject.SetActive(true);

    }

    public void OnApply()
    {
        if (SelectedObject == null)
            return;

        SelectedObject.transform.GetChild(0).gameObject.SetActive(false);
        _goodList.AddGood(SelectedObject.GetComponentInChildren<GoodInfo>());
        SelectedObject = null;
    }

    public void OnPay()
    {
       
        StartCoroutine(OnPayCoolDown());
    }

    private IEnumerator OnPayCoolDown()
    {
        AfterPay();
        buttonPay.interactable = false;
        yield return new WaitForSeconds(cooldown);
        buttonPay.interactable = true;

    }
}


class SelectedDataJson
{
    public string PathImage { get; set; }
    public string Name { get; set; }

    public float Price { get; set; }

}
