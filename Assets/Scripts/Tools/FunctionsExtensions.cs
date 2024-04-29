using System;
using System.Collections;
using UnityEngine;

public class FunctionsExtensions
{
    public static void ForeachAllObjects<T>(T[] objects, Action<T> action)
    {
        foreach (T obj in objects)
            action(obj);
    }

    public static IEnumerator MakeActionAfterTime(Action actionBefore, Action actionAfter, float duraction)
    {
        actionBefore();
        yield return new WaitForSeconds(duraction);
        actionAfter();
    }
}
