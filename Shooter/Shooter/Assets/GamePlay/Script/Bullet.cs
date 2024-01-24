using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float timeLife = 1f;
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.Sleep();
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {        
        Invoke("Desactive", timeLife);
    }
    public Rigidbody GetRB()
    {   
        if(rb == null)
            rb = GetComponent<Rigidbody>();

        return rb;
    }
    public virtual void EfectBullet()
    {

    }
    private void Desactive()
    {
        rb.Sleep();
        gameObject.SetActive(false);
    }
}
