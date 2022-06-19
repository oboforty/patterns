using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Transform FindDeepChild(this Transform tr, string name)
    {
        var result = tr.Find(name);
        if (result)
            return result;

        int count = tr.childCount;
        for (int i = 0; i < count; i++)
        {
            result = tr.GetChild(i).FindDeepChild(name);
            if (result)
                return result;
        }

        return null;
    }

}
