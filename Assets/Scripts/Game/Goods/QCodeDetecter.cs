using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QCodeDetecter : MonoBehaviour
{
    [SerializeField]
    private MonitorGoodList[] _monitorGoodList;

   
    public void DetectGood(GoodInfo good, int index)
    {
        _monitorGoodList[index].AddGood(good);
        good.IsScaned = true;
    }
}

