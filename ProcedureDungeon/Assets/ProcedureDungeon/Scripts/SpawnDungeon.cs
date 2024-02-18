using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDungeon : MonoBehaviour
{
    public ParameterDungeon parameterDungeon;
    public int index;
    public List<GameObject> list;
    public Transform spawn;
    void Start()
    {
        int i = Random.Range(0, list.Count);
        if(index < parameterDungeon.index)
        {
            GameObject go = Instantiate(list[i], spawn.position, spawn.rotation);
            go.GetComponent<SpawnDungeon>().index = index + 1;
        }
    }
    
}
