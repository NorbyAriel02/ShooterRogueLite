using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Destroy(float delay)
    {
        Destroy(gameObject, delay);
    }
}
