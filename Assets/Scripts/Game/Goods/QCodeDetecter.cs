using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QCodeDetecter : MonoBehaviour
{
    [SerializeField]
    private RayDetect[] _rayBasic;

    [SerializeField]
    private MonitorGoodList _monitorGoodList;

    private Dictionary<int, RayDetecterInfo> _rayDetecters = new Dictionary<int, RayDetecterInfo>();


    public RayDetecterInfo this[int index]
    {
        get => _rayDetecters[index];
    }

    private void Start()
    {
        foreach (var ray in _rayBasic)
        {
            ray.OnRayDetectNesseryObject += OnDetect;
            _rayDetecters.Add(ray.Index, new RayDetecterInfo(ray.Index, ray));
        }
    }

    public void OnDetect(RayDetect ray, GoodInfo info)
    {
        StartCoroutine(OnDetectSetWhatDetect(ray, info));
    }

    private IEnumerator OnDetectSetWhatDetect(RayDetect ray, GoodInfo good)
    {
        RayDetecterInfo rayDetect = this[ray.Index];
        rayDetect.WhatIsDetect = good;
        rayDetect.IsDetect = true;

        if (IsEnoughToDetect() && good != null && !good.IsScaned)
        {
            _monitorGoodList.AddGood(good);
            good.IsScaned = true;
        }
        yield return new WaitForSeconds(1);

        if (rayDetect.WhatIsDetect == good)
        {
            rayDetect.WhatIsDetect = null;
            rayDetect.IsDetect = false;
        }

    }

    public bool IsEnoughToDetect() => _rayDetecters.Values.Where(n => n.IsDetect == true).Count() >= 2;
}


public record RayDetecterInfo(int Index, RayDetect RayDetect)
{
    public bool IsDetect { get; set; }
    public GoodInfo WhatIsDetect { get; set; }

}

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit
    {

    }
}
