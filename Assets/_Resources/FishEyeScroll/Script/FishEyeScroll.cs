using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishEyeScroll : MonoBehaviour {

    //Scroll Settings--------------------------------------------------------------------------------------------
    public RectTransform ContentPanel;
    public Transform[] Items;
    public float[] SnapSpots;
    public float[] ChangeSpots;   
    public int snapSpeed = 20;
    //-----------------------------------------------------------------------------------------------------------

    //Item Grow Setting------------------------------------------------------------------------------------------
    public float growSize;
    public float growSpeed;
    //-----------------------------------------------------------------------------------------------------------

    private int scroll_spot_index;
    private float begin_drag_spot;

    void Start()
    {
        for (int i = 0; i < SnapSpots.Length; i++)
            if (ContentPanel.anchoredPosition.x == SnapSpots[i]) { 
                scroll_spot_index = i;                
                break;
            }
    }

    void Update()
    {
        float spot = ContentPanel.anchoredPosition.x;
        if (spot > ChangeSpots[0])
        {
            if (scroll_spot_index != 0)
            {
                scroll_spot_index = 0;
                StartCoroutine(ZoomInItem(Items[0]));
                StartCoroutine(ZoomOutItem(Items[1]));
                return;
            }
        }
        for (int i = 0; i < ChangeSpots.Length - 1; i++)
        {
            if (ChangeSpots[i] > spot && spot > ChangeSpots[i + 1])
            {
                if (scroll_spot_index != i + 1)
                {
                    scroll_spot_index = i + 1;
                    StartCoroutine(ZoomOutItem(Items[i]));
                    StartCoroutine(ZoomInItem(Items[i + 1]));
                    StartCoroutine(ZoomOutItem(Items[i + 2]));
                    return;
                }
            }
        }

        if (spot < ChangeSpots[ChangeSpots.Length - 1])
        {
            if (scroll_spot_index != ChangeSpots.Length)
            {
                scroll_spot_index = ChangeSpots.Length;
                StartCoroutine(ZoomInItem(Items[ChangeSpots.Length]));
                StartCoroutine(ZoomOutItem(Items[ChangeSpots.Length - 1]));
                return;
            }
        }
    }

    public void onBeginDrag()
    {
        begin_drag_spot = ContentPanel.anchoredPosition.x;
    }

    public void onEndDrag()
    {
        float end_drag_spot = ContentPanel.anchoredPosition.x;
        //Drag to the right
        if (begin_drag_spot > end_drag_spot)
        {
            onNextClick();
        }
        //Drag to the left
        else
            onPreviousClick();
    }

    IEnumerator ZoomInItem(Transform transform)
    {     
        float timer = 0;
       
        while (true) 
        {                  
            while (growSize > transform.localScale.x)
                {
                    timer += Time.deltaTime;
                    transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growSpeed;
                    yield return null;
                }

            Color c = transform.gameObject.GetComponent<Image>().color;
            c.a = 1;
            transform.gameObject.GetComponent<Image>().color = c;
            yield break;         
        }    
    }

    IEnumerator ZoomOutItem(Transform transform)
    {
        if (transform.localScale.magnitude > new Vector3(1, 1, 1).magnitude)
        {
            float timer = 0;

            while (true)
            {
                while (1 < transform.localScale.x)
                {
                    timer += Time.deltaTime;
                    transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * growSpeed;
                    yield return null;
                }

                transform.localScale = new Vector3(1, 1, 1);
                Color c = transform.gameObject.GetComponent<Image>().color;
                c.a = 0.4f;
                transform.gameObject.GetComponent<Image>().color = c;
                yield break;
            }
        }
        else
            yield break;
    }

    IEnumerator SnaptoSpot(int spot_index, float current_spot)
    {
        if (current_spot < SnapSpots[spot_index])
        {
            float spot = ContentPanel.anchoredPosition.x;
            while (spot < SnapSpots[spot_index])
            {
                spot += snapSpeed;
                ContentPanel.anchoredPosition = new Vector3(spot, ContentPanel.anchoredPosition.y);
                yield return new WaitForSeconds(0.01f);
            }

            ContentPanel.anchoredPosition = new Vector3(SnapSpots[spot_index], ContentPanel.anchoredPosition.y);
            yield break;
        }
        else 
        {
            float spot = ContentPanel.anchoredPosition.x;
            while (spot > SnapSpots[spot_index])
            {
                spot -= snapSpeed;
                ContentPanel.anchoredPosition = new Vector3(spot, ContentPanel.anchoredPosition.y);
                yield return new WaitForSeconds(0.01f);
            }

            ContentPanel.anchoredPosition = new Vector3(SnapSpots[spot_index], ContentPanel.anchoredPosition.y);
            yield break;
        }
    }

    public void onNextClick()
    {
        if(scroll_spot_index < SnapSpots.Length - 1)
            StartCoroutine(SnaptoSpot(scroll_spot_index+1, ContentPanel.anchoredPosition.x));
    }

    public void onPreviousClick()
    {
        if (scroll_spot_index > 0)
            StartCoroutine(SnaptoSpot(scroll_spot_index-1, ContentPanel.anchoredPosition.x));
    }
}
