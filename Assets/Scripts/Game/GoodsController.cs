using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoodsController : MonoBehaviour
{
    private const float distanceBetweenObjects = 0.05f;
    private const float moveObjects = -0.4f;
    private const float picesOfObject = 4f;

    private HashSet<GameObject> _goods = new HashSet<GameObject>();

    [SerializeField]
    private Transform _pointerCreate;

    private void Start()
    {
        GameController.Instance.OnStartNewGame += ClearList;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            foreach (GameObject good in _goods)
            {
                good.transform.position += new Vector3(moveObjects * Time.deltaTime, 0, 0);
            }
        }
    }


    public GameObject AddNewItem()
    {
        (float posX, float sizeX, int deteceted) aktualGoodX = (0.0f, 0.0f, 0);
        if (_goods.Count > 0)
        {
            var maxGoodPos = _goods.Max(n => n.gameObject.transform.position.x);
            var aktrualGood = _goods.Where(n => n.gameObject.transform.position.x == maxGoodPos).First();
            aktualGoodX = (aktrualGood.transform.position.x, aktrualGood.transform.lossyScale.x, 1);
        }

        var newObject = Instantiate(GeneratorGoods.Instance.GetRandomGood(), _pointerCreate.transform.position, Quaternion.identity);


        newObject.transform.position += new Vector3(((aktualGoodX.posX - _pointerCreate.position.x)
                                    + aktualGoodX.sizeX / picesOfObject + newObject.transform.lossyScale.x / picesOfObject 
                                    + distanceBetweenObjects) * aktualGoodX.deteceted, 0, 0);

        newObject.GetComponent<DragObject>().OnEnterContainer += AddGood;
        newObject.GetComponent<DragObject>().OnExitContainer += RemoveGood;
        _goods.Add(newObject);
        return newObject;
    }

    private void ClearList() => _goods.Clear();

    private void RemoveGood(GameObject good) => _goods.Remove(good);

    private void AddGood(GameObject good)
    {
        if (_goods.Contains(good))
            return;

        _goods.Add(good);
    }
}
