using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GuiImageFaded : MonoBehaviour
{
    public float fade_speed = 5f;
    //public bool loop = false;

    public System.Action<bool> onFadeDone;

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
        StartCoroutine(FadeImageToZeroAlpha(fade_speed, GetComponent<Image>()));
    }

    public void CancelFade()
    {
        StopAllCoroutines();
    }


    public IEnumerator FadeImageToFullAlpha(float fade_speed, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / fade_speed));
            yield return null;
        }

        //StartCoroutine(FadeImageToZeroAlpha(fade_speed, GetComponent<Image>()));
    }

    public IEnumerator FadeImageToZeroAlpha(float fade_speed, Image i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / fade_speed));
            yield return null;
        }

        if(onFadeDone != null)
            onFadeDone(true);

        //if (loop)
        //    StartCoroutine(FadeImageToFullAlpha(fade_speed, GetComponent<Image>()));
    }
}