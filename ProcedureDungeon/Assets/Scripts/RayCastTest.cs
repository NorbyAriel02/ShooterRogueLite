using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!CanSpawn())
            Debug.Log("Hit");
    }
    bool CanSpawn()
    {
        RaycastHit hit;
        Physics.OverlapBox(transform.position + Vector3.right, Vector3.one, transform.rotation);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 3f))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    
}
