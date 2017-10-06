using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GuiTextFaded : MonoBehaviour
{
    public float fade_speed = 1f;
    public bool loop = false;

    //private void Start()
    //{
    //    PlayFade();
    //}

    // can ignore the update, it's just to make the coroutines get called for example
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        StartCoroutine(FadeTextToFullAlpha(1f, GetComponent<Text>()));
    //    }
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        StartCoroutine(FadeTextToZeroAlpha(1f, GetComponent<Text>()));
    //    }
    //}

    public void PlayFade()
    {
        StartCoroutine(FadeTextToFullAlpha(fade_speed, GetComponent<Text>()));
    }

    public void CancelFade()
    {
        StopAllCoroutines();
    }


    public IEnumerator FadeTextToFullAlpha(float fade_speed, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / fade_speed));
            yield return null;
        }

        StartCoroutine(FadeTextToZeroAlpha(fade_speed, GetComponent<Text>()));
    }

    public IEnumerator FadeTextToZeroAlpha(float fade_speed, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / fade_speed));
            yield return null;
        }

        if(loop)
            StartCoroutine(FadeTextToFullAlpha(fade_speed, GetComponent<Text>()));
    }
}