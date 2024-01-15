using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class SpawnDungeonV2 : MonoBehaviour
{
    public ParameterDungeon parameterDungeon;    
    void Start()
    {
        Invoke("Spawn", 0.5f);
    }
    void Spawn()
    {
        if (!CanSpawn())
            return;

        //int r = Random.Range(0, 10);

        //if (r > 7)
        //    return;

        int i = Random.Range(0, parameterDungeon.roomList.Count);
        if (parameterDungeon.count < parameterDungeon.index)
        {
            GameObject go = Instantiate(parameterDungeon.roomList[i], transform.position, transform.rotation);
            parameterDungeon.count += 1;
        }
    }
    bool CanSpawn()
    {
        if (Physics.BoxCast(transform.position + transform.right * 5.5f, Vector3.one * 10, transform.right, transform.rotation))
        {            
            Debug.Log("Hit");
            return false;
        }
        else
        {         
            Debug.Log("No Hit " + gameObject.transform.parent.name);
            return true;
        }
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        Spawn();
    //    }
    //}
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + transform.right * 5.5f, Vector3.one * 10);
    }
}
