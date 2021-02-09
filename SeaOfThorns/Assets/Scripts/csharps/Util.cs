using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static Vector2 V3ToV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }

    public static Vector3 KeepY(Vector3 v3, float y)
    {
        return new Vector3(v3.x, y,v3.z);
    }

    public static bool Between(float value, float left, float right)
    {
        if(value > left && value <= right)
            return true;
        else 
            return false;
    }
}
