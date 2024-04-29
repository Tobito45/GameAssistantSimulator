using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class GoodsController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private float distanceBetweenObjects = 0.05f;
    [SerializeField]
    private const float moveObjects = -0.4f;
    [SerializeField]
    private const float picesOfObject = 4f;

    [Header("References")]
    [SerializeField]
    private MonitorSelectGood _monitorSelectGood;
    [SerializeField]
    private MainController _mainController;
    [SerializeField]
    private RotateController _rotateController;

    private HashSet<GameObject>[] _goodsOnConveer = new HashSet<GameObject>[KeyboardAndJostickController.MAXPLAYERS];
    private List<DragObject>[] _goodsCanBeSelected = new List<DragObject>[KeyboardAndJostickController.MAXPLAYERS];
    private DragObject[] _goodSelected = new DragObject[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _indexSelected = new int[KeyboardAndJostickController.MAXPLAYERS];

    private bool[] _lockAutoPay = new bool[KeyboardAndJostickController.MAXPLAYERS];

    [SerializeField]
    private GoodsPlayerIterator[] _UIPlayerElements = new GoodsPlayerIterator[KeyboardAndJostickController.MAXPLAYERS];

    private void Start()
    {
        for (int i = 0; i < _goodsOnConveer.Count(); i++)
        {
            _goodsOnConveer[i] = new HashSet<GameObject>();
            _goodsCanBeSelected[i] = new List<DragObject>();
            _indexSelected[i] = -1;
            _lockAutoPay[i] = false;

            _monitorSelectGood.AfterPay += AfterPayLocker;
        }

        GameController.Instance.OnStartNewGame += ClearList;
    }

    private void Update()
    {
        foreach (int index in KeyboardAndJostickController.MoveGoodsConveyon())
        {
            if (GameController.Instance.IsOpenedPanelUI[index] || GameController.Instance.EndMenuController.IsEndPanelActive(index))
                continue;

            foreach (GameObject good in _goodsOnConveer[index])
                good.transform.position += new Vector3(moveObjects * Time.deltaTime, 0, 0);
        }


        foreach (int index in KeyboardAndJostickController.ChangeGoods())
        {
            if (GameController.Instance.IsOpenedPanelUI[index] || GameController.Instance.EndMenuController.IsEndPanelActive(index))
                continue;

            IndexSelectedItemPlus(index);
        }

        foreach (int index in KeyboardAndJostickController.TakeGood())
        {
            if (GameController.Instance.IsOpenedPanelUI[index] || GameController.Instance.EndMenuController.IsEndPanelActive(index))
                continue;

            if (_indexSelected[index] != -1)
            {
                _goodSelected[index] = _goodsCanBeSelected[index][_indexSelected[index]];
                _goodSelected[index].TakeItem();
            }
        }

        foreach (int index in KeyboardAndJostickController.LetsGoGood())
        {
            if (GameController.Instance.IsOpenedPanelUI[index] || GameController.Instance.EndMenuController.IsEndPanelActive(index))
                continue;

            _goodSelected[index] = null;
        }

        for (int i = 0; i < KeyboardAndJostickController.MAXPLAYERS; i++)
        {
            if (!KeyboardAndJostickController.IsJosticConnected)
            {
                _UIPlayerElements[i].TabPanelLetsGo.SetActive(false);
                _UIPlayerElements[i].TabPanelPick.SetActive(false);
            }

            if (GameController.Instance.IsOpenedPanelUI[i] || GameController.Instance.EndMenuController.IsEndPanelActive(i))
                _UIPlayerElements[i].GetFooterPanel.SetActive(false);
            else
                _UIPlayerElements[i].GetFooterPanel.SetActive(true);

            if (_rotateController.ObjectRotate.ElementAt(i) != null)
            {
                _UIPlayerElements[i].GetLetGoPanel.SetActive(true);
                _UIPlayerElements[i].GetPickPanel.SetActive(false);
            } else if (_goodsCanBeSelected[i].Count > 0) 
            {
                _UIPlayerElements[i].GetLetGoPanel.SetActive(false);
                _UIPlayerElements[i].GetPickPanel.SetActive(true);
            } else
            {
                _UIPlayerElements[i].GetLetGoPanel.SetActive(false);
                _UIPlayerElements[i].GetPickPanel.SetActive(false);
            }


            if (!_lockAutoPay[i] && _goodsCanBeSelected[i].Count == 0 && _goodSelected[i] == null && _goodsOnConveer[i].Count == 0
                    && !_mainController.IsMenu) 
            {
                _monitorSelectGood.OnPay(i);
            }
        }
    }


    public GameObject AddNewItem(int index)
    {
        (float posX, float sizeX, int deteceted) aktualGoodX = (0.0f, 0.0f, 0);
        if (_goodsOnConveer[index].Count > 0)
        {
            var maxGoodPos = _goodsOnConveer[index].Max(n => n.gameObject.transform.position.x);
            var aktrualGood = _goodsOnConveer[index].Where(n => n.gameObject.transform.position.x == maxGoodPos).First();
            aktualGoodX = (aktrualGood.transform.position.x, aktrualGood.transform.lossyScale.x, 1);
        }

        var newObject = Instantiate(GeneratorGoods.Instance.GetRandomGood(), _UIPlayerElements[index].GetPointCreate.transform.position, Quaternion.identity);

        newObject.transform.position += new Vector3(((aktualGoodX.posX - _UIPlayerElements[index].GetPointCreate.position.x)
                                    + aktualGoodX.sizeX / picesOfObject + newObject.transform.lossyScale.x / picesOfObject
                                    + distanceBetweenObjects) * aktualGoodX.deteceted, 0, 0);

        newObject.GetComponent<DragObject>().OnEnterContainer += AddGood;
        newObject.GetComponent<DragObject>().OnExitContainer += RemoveGood;
        newObject.GetComponent<DragObject>().OnLetGoGood += RemoveIntemFromSelected;
        newObject.GetComponent<DragObject>().Index = index;
        _goodsOnConveer[index].Add(newObject);

        _lockAutoPay[index] = false;
        return newObject;
    }

    private void ClearList()
    {
        for (int i = 0; i < _goodsOnConveer.Length; i++)
        {
            _goodsOnConveer[i].Clear();
            _goodsCanBeSelected[i].Clear();
            _goodSelected[i] = null;
            _indexSelected[i] = -1;
        }

        for (int i = 0; i < KeyboardAndJostickController.MAXPLAYERS; i++)
            _UIPlayerElements[i].GetFooterPanel.SetActive(true);
    }

    private void AfterPayLocker(int index) => _lockAutoPay[index] = true; 

    private void RemoveIntemFromSelected(DragObject good, int index)
    {

        if (_indexSelected[index] >= 0 && _indexSelected[index] < _goodsCanBeSelected[index].Count)
            if (_goodsCanBeSelected[index][_indexSelected[index]] == good)
                IndexSelectedItemPlus(index);

        _goodsCanBeSelected[index].Remove(good);
        IndexSelectedItemPlus(index);

    }

    private void RemoveGood(GameObject good, int index)
    {
        _goodsOnConveer[index].Remove(good);
        _goodsCanBeSelected[index].Add(good.GetComponent<DragObject>());
        if (_goodsCanBeSelected[index].Count == 1)
            IndexSelectedSet(0, index);

    }


    private void AddGood(GameObject good, int index)
    {

        if (_goodsOnConveer[index].Contains(good))
            return;

        if (_goodsCanBeSelected[index].Contains(good.GetComponent<DragObject>()))
        {
            _goodsCanBeSelected[index].Remove(good.GetComponent<DragObject>());
            IndexSelectedItemPlus(index);
        }

        _goodsOnConveer[index].Add(good);
    }

    private int IndexSelectedItemPlus(int index)
    {
        IndexSelectedSet(_indexSelected[index] + 1, index);
        if (_goodsCanBeSelected[index].Count == 0)
            IndexSelectedSet(-1, index);
        else if (_goodsCanBeSelected[index].Count > 0 && _indexSelected[index] >= _goodsCanBeSelected[index].Count)
            IndexSelectedSet(0, index);

        return _indexSelected[index];

    }

    private void IndexSelectedSet(int newIndex, int index)
    {
        if (_indexSelected[index] >= 0 && _indexSelected[index] < _goodsCanBeSelected[index].Count)
        {
            _goodsCanBeSelected[index][_indexSelected[index]].CanBeSelected(false);
            _goodsCanBeSelected[index][_indexSelected[index]].LetGoItem();
        }
        _indexSelected[index] = newIndex;
        if (_indexSelected[index] >= 0 && _indexSelected[index] < _goodsCanBeSelected[index].Count)
            _goodsCanBeSelected[index][_indexSelected[index]].CanBeSelected(true);

    }

    private void OnEndGame(int index) => _UIPlayerElements[index].GetFooterPanel.SetActive(false);
}


[System.Serializable]
class GoodsPlayerIterator
{
    [SerializeField]
    private Transform _pointerCreate;
    [SerializeField]
    private GameObject _footerPanel, _pickPanel, _letGoPanel, _tabPanelLetsGo, _tabPanelPick;

    public Transform GetPointCreate => _pointerCreate;
    public GameObject GetFooterPanel => _footerPanel;
    public GameObject GetPickPanel => _pickPanel;
    public GameObject GetLetGoPanel => _letGoPanel;
    public GameObject TabPanelLetsGo => _tabPanelLetsGo;
    public GameObject TabPanelPick => _tabPanelPick;
}