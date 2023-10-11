using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonitorGoodList : MonoBehaviour
{
    private List<GoodInfo> _goodList = new List<GoodInfo>();


    [SerializeField]
    private Image _panelPrefab;

    [SerializeField]
    private TextMeshProUGUI _textName;
    [SerializeField]
    private TextMeshProUGUI _textPrice;
    [SerializeField]
    private TextMeshProUGUI _textSum;
    [SerializeField]
    private Transform _goodsInScrollContent;


    private float _sum;


    private void Start()
    {
        _sum = 0;
        _textSum.text = string.Empty;
        _textPrice.text = string.Empty;
        _textName.text = string.Empty;
    }

    public void AddGood(GoodInfo good)
    {
        var newItem = Instantiate(_panelPrefab, _goodsInScrollContent);

        newItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = good.GoodName;
        newItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = good.Price.ToString();
        _goodList.Add(good);

        _textName.text = good.GoodName;
        _textPrice.text = good.Price.ToString();
        _sum += good.Price;
        _textSum.text = _sum.ToString();

        GameController.Instance.OnAddGood(good);
    }

    public void ClearGoods()
    {
        _goodList.Clear();
        foreach(Transform item in _goodsInScrollContent)
        {
            Destroy(item.gameObject);
        }
        
        Start();


    }
}