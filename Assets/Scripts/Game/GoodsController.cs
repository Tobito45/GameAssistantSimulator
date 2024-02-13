using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoodsController : MonoBehaviour
{
    private const float distanceBetweenObjects = 0.05f;
    private const float moveObjects = -0.4f;
    private const float picesOfObject = 4f;

    private HashSet<GameObject>[] _goodsOnConveer = new HashSet<GameObject>[KeyboardAndJostickController.MAXPLAYERS];
    private List<DragObject>[] _goodsCanBeSelected = new List<DragObject>[KeyboardAndJostickController.MAXPLAYERS];
    private DragObject[] _goodSelected = new DragObject[KeyboardAndJostickController.MAXPLAYERS];
    private int[] _indexSelected = new int[KeyboardAndJostickController.MAXPLAYERS];

    [SerializeField]
    private Transform[] _pointerCreate;

    private bool[] _isPlayerEnded = new bool[KeyboardAndJostickController.MAXPLAYERS];

    private void Start()
    {
        for (int i = 0; i < _goodsOnConveer.Count(); i++)
        {
            _goodsOnConveer[i] = new HashSet<GameObject>();
            _goodsCanBeSelected[i] = new List<DragObject>();
            _indexSelected[i] = -1;
        }

        MainController.ForeachAllObjects(_isPlayerEnded, (x) => x = false);
        GameController.Instance.OnStartNewGame += ClearList;
    }

    private void Update()
    {


        foreach (int index in KeyboardAndJostickController.MoveGoodsConveyor())
        {
            if (GameController.Instance.IsOpenedPanelUI[index] || _isPlayerEnded[index])
                continue;

            foreach (GameObject good in _goodsOnConveer[index])//[0])
            {
                good.transform.position += new Vector3(moveObjects * Time.deltaTime, 0, 0);
            }
        }


        foreach (int index in KeyboardAndJostickController.ChangeGoods())
        {
            if (GameController.Instance.IsOpenedPanelUI[index] || _isPlayerEnded[index])
                continue;
            IndexSelectedItemPlus(index);
        }

        foreach (int index in KeyboardAndJostickController.TakeGood())
        {
            if (GameController.Instance.IsOpenedPanelUI[index] || _isPlayerEnded[index])
                continue;

            if (_indexSelected[index] != -1)
            {
                _goodSelected[index] = _goodsCanBeSelected[index][_indexSelected[index]];
                _goodSelected[index].TakeItem();
            }
        }

        foreach (int index in KeyboardAndJostickController.LetsGoGood())
        {
            if (GameController.Instance.IsOpenedPanelUI[index] || _isPlayerEnded[index])
                continue;
            
            _goodSelected[index] = null;
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

        var newObject = Instantiate(GeneratorGoods.Instance.GetRandomGood(), _pointerCreate[index].transform.position, Quaternion.identity);


        newObject.transform.position += new Vector3(((aktualGoodX.posX - _pointerCreate[index].position.x)
                                    + aktualGoodX.sizeX / picesOfObject + newObject.transform.lossyScale.x / picesOfObject 
                                    + distanceBetweenObjects) * aktualGoodX.deteceted, 0, 0);

        newObject.GetComponent<DragObject>().OnEnterContainer += AddGood;
        newObject.GetComponent<DragObject>().OnExitContainer += RemoveGood;
        newObject.GetComponent<DragObject>().OnLetGoGood += RemoveIntemFromSelected;
        newObject.GetComponent<DragObject>().Index = index;
        _goodsOnConveer[index].Add(newObject);
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

    }

    private void RemoveIntemFromSelected(DragObject good, int index)
    {

        if (_indexSelected[index] >= 0 && _indexSelected[index] < _goodsCanBeSelected[index].Count)
        {
            if (_goodsCanBeSelected[index][_indexSelected[index]] == good)
            {
                IndexSelectedItemPlus(index);
            }
        }
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
            _goodsCanBeSelected[index].Remove(good.GetComponent<DragObject>()); //slow???
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
        if(_indexSelected[index] >= 0 && _indexSelected[index] < _goodsCanBeSelected[index].Count)
        {
            _goodsCanBeSelected[index][_indexSelected[index]].CanBeSelected(false);
            _goodsCanBeSelected[index][_indexSelected[index]].LetGoItem();
        }
        _indexSelected[index] = newIndex;
        if (_indexSelected[index] >= 0 && _indexSelected[index] < _goodsCanBeSelected[index].Count)
        {
            _goodsCanBeSelected[index][_indexSelected[index]].CanBeSelected(true);
        }

    }

    private void OnEndGame(int index) => _isPlayerEnded[index] = true;
}
