using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public ParameterDungeon config;
    private void Awake()
    {
        config.count = 0;
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
