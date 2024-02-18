using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPreloadingHelper : MonoBehaviour
{
    public static List<GameObject> Load(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, int amount)
    {
        List<GameObject> list = new List<GameObject>();
        for(int i = 0; i < amount; i++)
        {
            list.Add(Instantiate(prefab, position, rotation, parent));
        }
        return list;
    }
    public static GameObject GetObject(List<GameObject> list)
    {
        foreach(GameObject obj in list) { 
            if(!obj.activeSelf)
            {
                return obj;
            }
        }        
        return null;
    }
    public static int CalculateAmount(float timelife, float rateFire)
    {
        return Mathf.FloorToInt(timelife / rateFire) + 1;
    }
}
