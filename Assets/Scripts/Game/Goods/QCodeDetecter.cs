using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QCodeDetecter : MonoBehaviour
{
    [SerializeField]
    private MonitorGoodList _monitorGoodList;

   
    public void DetectGood(GoodInfo good)
    {
        _monitorGoodList.AddGood(good);
        good.IsScaned = true;
    }
}

