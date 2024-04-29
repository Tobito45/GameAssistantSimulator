using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorGoods : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _sampleObjects;

    private static GeneratorGoods _instance;
    public static GeneratorGoods Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }
    
    public GameObject GetRandomGood()
    {
        return _sampleObjects[Random.Range(0, _sampleObjects.Length)];
    }

}
