using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.AI.Navigation;

public class GameController : MonoBehaviour
{
    public ParameterDungeon config;
    private void Awake()
    {
        config.count = 0;
        config.ending = false;
        config.IsKeySpawned = false;
        Sapwn();
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    void Sapwn()
    {
        bool sigue = true;
        while(sigue)
        {
            sigue = false;
            SpawnDungeonV2[] spv2 = FindObjectsByType<SpawnDungeonV2>(FindObjectsSortMode.InstanceID);
            for(int i = 0; i < spv2.Length; i++)
            {
                if (spv2[i] != null && !spv2[i].spawned) 
                {
                    spv2[i].Spawn();
                }
                else
                    spv2[i] = null;
            }
            foreach(SpawnDungeonV2 v in spv2)
            {
                if(v != null)
                {
                    sigue = true;
                }
            }
        }
    }
    public void Reload()
    {
        SceneManager.LoadScene("Desa");
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }
}
