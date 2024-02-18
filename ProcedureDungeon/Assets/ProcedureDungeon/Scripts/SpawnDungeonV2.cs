using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class SpawnDungeonV2 : MonoBehaviour
{
    public ParameterDungeon parameterDungeon;
    public bool spawned = false;

    void Start()
    {
        
    }
    private void AssignParent()
    {
        if (parameterDungeon.parentRooms == null)
        {
            parameterDungeon.parentRooms = new GameObject("Rooms");
            parameterDungeon.parentDoors = new GameObject("Doors");
            parameterDungeon.parentDoors.transform.parent = parameterDungeon.parentRooms.transform;
        }
    }
    public void Spawn()
    {
        spawned = true;
        AssignParent();

        if (!CanSpawn()) return;

        SpawnDungeon();
                
        gameObject.SetActive(false);
    }
    void SpawnDungeon()
    {        
        int i = Random.Range(0, parameterDungeon.roomList.Count);
        Ending();
        
        if (parameterDungeon.count < parameterDungeon.index)
        {
            GameObject go = Instantiate(parameterDungeon.roomList[i], transform.position, transform.rotation, parameterDungeon.parentRooms.transform);
            go.name = parameterDungeon.count.ToString();
            parameterDungeon.count += 1;
        }
        else
            SpawnEndDoor();
    }
    void Ending()
    {
        if (parameterDungeon.count == parameterDungeon.index)
        {
            if (!parameterDungeon.ending)
            {                
                Instantiate(parameterDungeon.EndDungeon, transform.position - (transform.right *  parameterDungeon.offPos), transform.rotation);
                parameterDungeon.ending = true;
            }
            else if (!parameterDungeon.IsKeySpawned)
            {             
                Instantiate(parameterDungeon.KeyDungeon, transform.position - (transform.right * parameterDungeon.offPos), transform.rotation);
                parameterDungeon.IsKeySpawned = true;
            }
        }
    }
    void SpawnEndDoor()
    {
        GameObject go = Instantiate(parameterDungeon.doorList[0], transform.position - transform.right * 0.5f, transform.rotation, parameterDungeon.parentDoors.transform);
    }
    bool CanSpawn()
    {        
        if (Physics.BoxCast(transform.position + transform.right * parameterDungeon.deltaCenter, Vector3.one * parameterDungeon.sizeBoxCast, transform.right, out RaycastHit hit))
        {
            if (hit.collider.tag.Equals("Wall"))
            {
                SpawnEndDoor();
                print("Spawn Puerta al chocar muro");
            }

            return false;
        }
        else
        {         
            Debug.Log("No Hit " + gameObject.transform.parent.name);
            return true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireCube(transform.position + transform.right * parameterDungeon.deltaCenter, Vector3.one * parameterDungeon.sizeBoxCast);
        Gizmos.DrawCube(transform.position + transform.right * parameterDungeon.deltaCenter, Vector3.one * parameterDungeon.sizeBoxCast);        
    }
}
