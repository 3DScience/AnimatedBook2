using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoveredCurtain : MonoBehaviour {

    public Material curtain_mat;

    public bool isFirstLoad = true;

    public void Cover()
    {
        isFirstLoad = false;
        StartCoroutine(Cover_Coroutine());
    }

    public void Hide()
    {
        isFirstLoad = false;
        StartCoroutine(Hide_Coroutine());
    }

    IEnumerator Cover_Coroutine()
    {
        gameObject.SetActive(true);
        float dissolveLevel = 1F;
        curtain_mat.SetFloat("_Level", dissolveLevel);        
        while (dissolveLevel > 0) {
            dissolveLevel -= 0.1F;
            curtain_mat.SetFloat("_Level", dissolveLevel);
            yield return null;
        }
        curtain_mat.SetFloat("_Level", 0F);
    }

    IEnumerator Hide_Coroutine()
    {
        gameObject.SetActive(true);
        float dissolveLevel = 0F;
        curtain_mat.SetFloat("_Level", dissolveLevel);
        while (dissolveLevel < 1)
        {
            dissolveLevel += 0.1F;
            curtain_mat.SetFloat("_Level", dissolveLevel);
            yield return null;
        }
        curtain_mat.SetFloat("_Level", 1F);
        gameObject.SetActive(false);
    }
}
