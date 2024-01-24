using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public KeyCode key;
    public Weapon weapon;
    void Start()
    {
        
    }
        
    void Update()
    {
        if(Input.GetKeyDown(key))
        {
            weapon.Shoot();
        }
    }
}
