using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDead : MonoBehaviour
{
    public float delay = 0.001f;
    void Start()
    {
        DamageableTarget target = GetComponent<DamageableTarget>();
        target.OnDie += Dead;
    }

    void Dead()
    {
        StartCoroutine(Scale());
    }
    IEnumerator Scale()
    {        
        for (float scale = 1f; scale >= 0; scale -= delay)
        {
            gameObject.transform.localScale = new Vector3 (scale, scale, scale);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
