using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefulCodeSnippets : MonoBehaviour
{
    void Metodo()
    {
        StartCoroutine(Corrutina());
    }
    IEnumerator Corrutina()
    {

        for (float scale = 1f; scale >= 0; scale -= 0.1f)
        {
            gameObject.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }
}
