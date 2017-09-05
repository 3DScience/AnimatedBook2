using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class BookController_FT : MonoBehaviour {

    private bool isAnimationPlay = false;
    private bool isOpenOrCloseAnimate = false;
    private bool isBookOpen = false;
    private bool isStartViewScence = false;

    //Scence Controller
    private GameObject Scence;

    //BackGround Plane
    private GameObject BGPlane; 

    //Book Static Page
    private GameObject topPageLeft, topPageRight;

    //Book Flip Page
    private GameObject page1, page2, page3, page4, page5, page6, page7, page8, page9, page10;

    //Flip Page Bone - The Bone Will Holding Content
    private GameObject left_pageske2, left_pageske3, left_pageske4, left_pageske5, left_pageske6, left_pageske7, left_pageske8, left_pageske9, left_pageske10, left_pageske11;
    private GameObject right_pageske1, right_pageske2, right_pageske3, right_pageske4, right_pageske5, right_pageske6, right_pageske7, right_pageske8, right_pageske9, right_pageske10;

    //Maximum content page
    public int max_page = 10;
    public int current_page = 0;

    //Book Material
    public Material SkyboxMat;
    public Material PageBlankMat;
    public Material PageStartLeft;
    public Material PageStartRight;
    public Material[] PageLeftMat;
    public Material[] PageRightMat;
    public Material[] PageFlipNextMat;
    public Material[] PageFlipPreMat;
    public Material[] BGMat;   

    //Flowchart Dialog
    public Flowchart FungusFlowchart;

    public bool UseAnimatorObject = false;
    public float BookSpeed = 1.0f;
    public float AnimationSpeed = 1f;

    // Use this for initialization
    void Start () {
        Scence = GameObject.Find("Scence");
        BGPlane = GameObject.Find("BGPlane");

        topPageLeft = GameObject.Find("Open_Book/PageLeft");
        topPageRight = GameObject.Find("Open_Book/PageRight");

        page1 = transform.Find("Flip_Page/page1").gameObject;
        page2 = transform.Find("Flip_Page/page2").gameObject;
        page3 = transform.Find("Flip_Page/page3").gameObject;
        page4 = transform.Find("Flip_Page/page4").gameObject;
        page5 = transform.Find("Flip_Page/page5").gameObject;
        page6 = transform.Find("Flip_Page/page6").gameObject;
        page7 = transform.Find("Flip_Page/page7").gameObject;
        page8 = transform.Find("Flip_Page/page8").gameObject;
        page9 = transform.Find("Flip_Page/page9").gameObject;
        page10 = transform.Find("Flip_Page/page10").gameObject;

        left_pageske2 = GameObject.Find("Book_Ske/page_ske2/left_pageske2");
        left_pageske3 = GameObject.Find("Book_Ske/page_ske3/left_pageske3");
        left_pageske4 = GameObject.Find("Book_Ske/page_ske4/left_pageske4");
        left_pageske5 = GameObject.Find("Book_Ske/page_ske5/left_pageske5");
        left_pageske6 = GameObject.Find("Book_Ske/page_ske6/left_pageske6");
        left_pageske7 = GameObject.Find("Book_Ske/page_ske7/left_pageske7");
        left_pageske8 = GameObject.Find("Book_Ske/page_ske8/left_pageske8");
        left_pageske9 = GameObject.Find("Book_Ske/page_ske9/left_pageske9");
        left_pageske10 = GameObject.Find("Book_Ske/page_ske10/left_pageske10");
        left_pageske11 = GameObject.Find("Book_Ske/page_ske11/left_pageske11");

        right_pageske1 = GameObject.Find("Book_Ske/page_ske1/right_pageske1");
        right_pageske2 = GameObject.Find("Book_Ske/page_ske2/right_pageske2");
        right_pageske3 = GameObject.Find("Book_Ske/page_ske3/right_pageske3");
        right_pageske4 = GameObject.Find("Book_Ske/page_ske4/right_pageske4");
        right_pageske5 = GameObject.Find("Book_Ske/page_ske5/right_pageske5");
        right_pageske6 = GameObject.Find("Book_Ske/page_ske6/right_pageske6");
        right_pageske7 = GameObject.Find("Book_Ske/page_ske7/right_pageske7");
        right_pageske8 = GameObject.Find("Book_Ske/page_ske8/right_pageske8");
        right_pageske9 = GameObject.Find("Book_Ske/page_ske9/right_pageske9");
        right_pageske10 = GameObject.Find("Book_Ske/page_ske10/right_pageske10");

        for(int i = 1; i <= max_page; i++)
        {
            Transform leftContent = transform.Find("Content/Page" + i + "/left");
            Transform rightContent = transform.Find("Content/Page" + i + "/right");
            Transform staticContent = transform.Find("Content/Page" + i + "/static");
            int leftChildCount = leftContent.childCount;
            int rightChildCount = rightContent.childCount;
            int staticChildCount = staticContent.childCount;

            for(int j = 0; j < leftChildCount; j++)
            {
                Transform child = leftContent.GetChild(0);
                Vector3 localPosition = child.transform.localPosition;
                Vector3 localRotation = child.transform.localRotation.eulerAngles;
                Vector3 localScale = child.transform.localScale;
                switch (i)
                {
                    case 1:
                        child.parent = right_pageske1.transform;
                        break;
                    case 2:
                        child.parent = right_pageske2.transform;
                        break;
                    case 3:
                        child.parent = right_pageske3.transform;
                        break;
                    case 4:
                        child.parent = right_pageske4.transform;
                        break;
                    case 5:
                        child.parent = right_pageske5.transform;
                        break;
                    case 6:
                        child.parent = right_pageske6.transform;
                        break;
                    case 7:
                        child.parent = right_pageske7.transform;
                        break;
                    case 8:
                        child.parent = right_pageske8.transform;
                        break;
                    case 9:
                        child.parent = right_pageske9.transform;
                        break;
                    case 10:
                        child.parent = right_pageske10.transform;
                        break;
                }
                child.transform.localPosition = localPosition;
                child.transform.localRotation = Quaternion.Euler(localRotation);
                child.transform.localScale = localScale;
               
                if(!UseAnimatorObject) { 
                    Vector3 rot = child.localRotation.eulerAngles;
                    rot = new Vector3(rot.x, rot.y, 180);
                    child.localRotation = Quaternion.Euler(rot);
                }
                else
                {
                    child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.speed
                        = child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.speed * AnimationSpeed;
                    child.gameObject.GetComponent<AnimatorBookObject>().scaleTo.speed
                        = child.gameObject.GetComponent<AnimatorBookObject>().scaleTo.speed * AnimationSpeed;

                    if (child.localPosition.z >= 0) {
                        Vector3 rot = child.localRotation.eulerAngles;
                        rot = new Vector3(-90, rot.y, rot.z);
                        child.localRotation = Quaternion.Euler(rot);
                        child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.currentAngle = rot;                      
                    }
                    else
                    {
                        Vector3 rot = child.localRotation.eulerAngles;
                        rot = new Vector3(90, rot.y, rot.z);
                        child.localRotation = Quaternion.Euler(rot);
                        child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.currentAngle = rot;                       
                    }

                    child.localScale = new Vector3(0, 0, 0);
                }
            }

            for (int j = 0; j < rightChildCount; j++)
            {
                Transform child = rightContent.GetChild(0);
                Vector3 localPosition = child.transform.localPosition;
                Vector3 localRotation = child.transform.localRotation.eulerAngles;
                Vector3 localScale = child.transform.localScale;
                switch (i)
                {
                    case 1:
                        child.parent = left_pageske2.transform;
                        break;
                    case 2:
                        child.parent = left_pageske3.transform;
                        break;
                    case 3:
                        child.parent = left_pageske4.transform;
                        break;
                    case 4:
                        child.parent = left_pageske5.transform;
                        break;
                    case 5:
                        child.parent = left_pageske6.transform;
                        break;
                    case 6:
                        child.parent = left_pageske7.transform;
                        break;
                    case 7:
                        child.parent = left_pageske8.transform;
                        break;
                    case 8:
                        child.parent = left_pageske9.transform;
                        break;
                    case 9:
                        child.parent = left_pageske10.transform;
                        break;
                    case 10:
                        child.parent = left_pageske11.transform;
                        break;
                }
                child.transform.localPosition = localPosition;
                child.transform.localRotation = Quaternion.Euler(localRotation);
                child.transform.localScale = localScale;                

                if (UseAnimatorObject)
                {
                    child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.speed
                        = child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.speed * AnimationSpeed;
                    child.gameObject.GetComponent<AnimatorBookObject>().scaleTo.speed
                        = child.gameObject.GetComponent<AnimatorBookObject>().scaleTo.speed * AnimationSpeed;

                    if (child.localPosition.z >= 0)
                    {
                        Vector3 rot = child.localRotation.eulerAngles;
                        rot = new Vector3(-90, rot.y, rot.z);
                        child.localRotation = Quaternion.Euler(rot);
                        child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.currentAngle = rot;                       
                    }
                    else
                    {
                        Vector3 rot = child.localRotation.eulerAngles;
                        rot = new Vector3(90, rot.y, rot.z);
                        child.localRotation = Quaternion.Euler(rot);
                        child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.currentAngle = rot;
                       
                    }

                    child.localScale = new Vector3(0, 0, 0);
                }
            }

            for (int j = 0; j < staticChildCount; j++)
            {
                Transform child = staticContent.GetChild(0);
                Vector3 localPosition = child.transform.localPosition;
                Vector3 localRotation = child.transform.localRotation.eulerAngles;
                Vector3 localScale = child.transform.localScale;
                switch (i)
                {
                    case 1:
                        child.parent = transform.Find("StaticContent/Page1");
                        break;
                    case 2:
                        child.parent = transform.Find("StaticContent/Page2");
                        break;
                    case 3:
                        child.parent = transform.Find("StaticContent/Page3");
                        break;
                    case 4:
                        child.parent = transform.Find("StaticContent/Page4");
                        break;
                    case 5:
                        child.parent = transform.Find("StaticContent/Page5");
                        break;
                    case 6:
                        child.parent = transform.Find("StaticContent/Page6");
                        break;
                    case 7:
                        child.parent = transform.Find("StaticContent/Page7");
                        break;
                    case 8:
                        child.parent = transform.Find("StaticContent/Page8");
                        break;
                    case 9:
                        child.parent = transform.Find("StaticContent/Page9");
                        break;
                    case 10:
                        child.parent = transform.Find("StaticContent/Page10");
                        break;
                }
                child.transform.localPosition = localPosition;
                child.transform.localRotation = Quaternion.Euler(localRotation);
                child.transform.localScale = localScale;

                child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.speed 
                    = child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.speed * AnimationSpeed;
                child.gameObject.GetComponent<AnimatorBookObject>().scaleTo.speed 
                    = child.gameObject.GetComponent<AnimatorBookObject>().scaleTo.speed * AnimationSpeed;

                if (child.gameObject.GetComponent<AnimatorBookObject>().flipObject == true )
                { 
                    if (child.localPosition.z >= 0)
                    {
                        Vector3 rot = child.localRotation.eulerAngles;
                        rot = new Vector3(-90, rot.y, rot.z);
                        child.localRotation = Quaternion.Euler(rot);
                        child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.currentAngle = rot;                 
                    }
                    else
                    {
                        Vector3 rot = child.localRotation.eulerAngles;
                        rot = new Vector3(90, rot.y, rot.z);
                        child.localRotation = Quaternion.Euler(rot);
                        child.gameObject.GetComponent<AnimatorBookObject>().rotateTo.currentAngle = rot;                   
                    }

                    child.localScale = new Vector3(0, 0, 0);
                }
            }
        }      
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    #region Book Navigate Function
    public void open()
    {
        CancelInvoke("ClearBGTexture");
        CancelInvoke("DissolveBGTexture"); 

        if (!isBookOpen)
        {
            StartCoroutine(openBookCoroutine());
            topPageLeft.GetComponent<Renderer>().material = PageStartLeft;
            topPageRight.GetComponent<Renderer>().material = PageStartRight;
            if (GameObject.Find("UIPanel/TabToOpenText") != null)
            {
                GameObject.Find("UIPanel/TabToOpenText").GetComponent<GuiTextFaded>().CancelFade();
                GameObject.Find("UIPanel/TabToOpenText").SetActive(false);
            }
        }
    }

    public void close()
    {
        InvokeRepeating("ClearBGTexture", 0f, 0.03F);
        StartCoroutine(closeBookCoroutine());
    }

    private IEnumerator openBookCoroutine()
    {
        FungusFlowchart.ExecuteBlock("StartScence");
        Animation animation = GetComponent<Animation>();
        string ani = "openBook";
        animation[ani].speed = BookSpeed * 3 / 4;
        animation.Play(ani);
        isOpenOrCloseAnimate = true;
        isBookOpen = true;

        do
        {
            yield return null;
        } while (animation.isPlaying);

        if(Scence.GetComponent<ScenceController_FT>().settings.graphic == BookSettings.LOW_QUALITY)
        {
            Transform shadow = transform.Find("BookShadow");
            if (shadow != null)
                shadow.gameObject.SetActive(false);
        }

        isOpenOrCloseAnimate = false;

        onNextPageClick();
    }

    private IEnumerator closeBookCoroutine()
    {
        isOpenOrCloseAnimate = true;
        yield return new WaitForSeconds(1.5f);

        Scence.GetComponent<ScenceController_FT>().RestartScence();

        if (Scence.GetComponent<ScenceController_FT>().settings.graphic == BookSettings.LOW_QUALITY)
        {
            Transform shadow = transform.Find("BookShadow");
            if (shadow != null)
                shadow.gameObject.SetActive(true);
        }

        Animation animation = GetComponent<Animation>();
        string ani = "closeBook";
        animation[ani].speed = BookSpeed * 3 / 4;
        animation.Play(ani);

        do
        {
            yield return null;
        } while (animation.isPlaying);

        current_page = 0;
        isOpenOrCloseAnimate = false;
        isBookOpen = false;
        isStartViewScence = false;
    }

    public void onNextPageClick()
    {
        if (isBookOpen)
        {
            if (current_page < max_page && !isAnimationPlay && !isOpenOrCloseAnimate)
            {
                StartCoroutine(nextPage(current_page));

                //Start Story Tell Scence
                if (!isStartViewScence)
                {
                    Scence.GetComponent<ScenceController_FT>().StartViewScence();
                    isStartViewScence = true;
                }
            }
            else if (current_page == max_page && !isAnimationPlay && !isOpenOrCloseAnimate)
            {
                close();
            }
        }
        else
        {
            if (!isOpenOrCloseAnimate)
            {
                open();
            }
        }
    }

    public void onPrePageClick()
    {
        if (current_page > 1 && !isAnimationPlay && !isOpenOrCloseAnimate)
            StartCoroutine(prePage(current_page));
    }
    #endregion


    #region Get Book Materials
    GameObject getFlipPage(int current_page)
    {
        switch (current_page)
        {
            case 1:
                return page1;
            case 2:
                return page2;
            case 3:
                return page3;
            case 4:
                return page4;
            case 5:
                return page5;
            case 6:
                return page6;
            case 7:
                return page7;
            case 8:
                return page8;
            case 9:
                return page9;
            case 10:
                return page10;
            default:
                return null;
        }
    }

    GameObject[] getPageSke(int current_page)
    {
        GameObject[] pageSke = new GameObject[4];
        GameObject left_prePageSke = null, right_prePageSke = null, left_curPageSke = null, right_curPageSke = null;
        switch (current_page)
        {
            case 1:
                left_curPageSke = right_pageske1;
                right_curPageSke = left_pageske2;
                break;
            case 2:
                left_prePageSke = right_pageske1;
                right_prePageSke = left_pageske2;
                left_curPageSke = right_pageske2;
                right_curPageSke = left_pageske3;
                break;
            case 3:
                left_prePageSke = right_pageske2;
                right_prePageSke = left_pageske3;
                left_curPageSke = right_pageske3;
                right_curPageSke = left_pageske4;
                break;
            case 4:
                left_prePageSke = right_pageske3;
                right_prePageSke = left_pageske4;
                left_curPageSke = right_pageske4;
                right_curPageSke = left_pageske5;
                break;
            case 5:
                left_prePageSke = right_pageske4;
                right_prePageSke = left_pageske5;
                left_curPageSke = right_pageske5;
                right_curPageSke = left_pageske6;
                break;
            case 6:
                left_prePageSke = right_pageske5;
                right_prePageSke = left_pageske6;
                left_curPageSke = right_pageske6;
                right_curPageSke = left_pageske7;
                break;
            case 7:
                left_prePageSke = right_pageske6;
                right_prePageSke = left_pageske7;
                left_curPageSke = right_pageske7;
                right_curPageSke = left_pageske8;
                break;
            case 8:
                left_prePageSke = right_pageske7;
                right_prePageSke = left_pageske8;
                left_curPageSke = right_pageske8;
                right_curPageSke = left_pageske9;
                break;
            case 9:
                left_prePageSke = right_pageske8;
                right_prePageSke = left_pageske9;
                left_curPageSke = right_pageske9;
                right_curPageSke = left_pageske10;
                break;
            case 10:
                left_prePageSke = right_pageske9;
                right_prePageSke = left_pageske10;
                left_curPageSke = right_pageske10;
                right_curPageSke = left_pageske11;
                break;
            default:
                break;
        }
        pageSke[0] = left_prePageSke;
        pageSke[1] = right_prePageSke;
        pageSke[2] = left_curPageSke;
        pageSke[3] = right_curPageSke;
        return pageSke;
    }

    GameObject getStaticContent(int current_page)
    {
        GameObject staticContent = null;
        switch (current_page)
        {
            case 1:
                staticContent = transform.Find("StaticContent/Page1").gameObject;
                break;
            case 2:
                staticContent = transform.Find("StaticContent/Page2").gameObject;
                break;
            case 3:
                staticContent = transform.Find("StaticContent/Page3").gameObject;
                break;
            case 4:
                staticContent = transform.Find("StaticContent/Page4").gameObject;
                break;
            case 5:
                staticContent = transform.Find("StaticContent/Page5").gameObject;
                break;
            case 6:
                staticContent = transform.Find("StaticContent/Page6").gameObject;
                break;
            case 7:
                staticContent = transform.Find("StaticContent/Page7").gameObject;
                break;
            case 8:
                staticContent = transform.Find("StaticContent/Page8").gameObject;
                break;
            case 9:
                staticContent = transform.Find("StaticContent/Page9").gameObject;
                break;
            case 10:
                staticContent = transform.Find("StaticContent/Page10").gameObject;
                break;
            default:
                break;
        }

        return staticContent;
    }

    Material[] getMaterial(int current_page)
    {
        Material[] book_mat = new Material[5];
        Material bookLeftMat = PageBlankMat;
        Material bookRightMat = PageBlankMat;
        Material pageFlipNextMat = null;
        Material BGPlaneMat = null;
        Material pageFlipPreMat = PageBlankMat;

        try
        {
            switch (current_page)
            {
                case 1:
                    if (PageLeftMat[0] != null)
                        bookLeftMat = PageLeftMat[0];
                    if (PageRightMat[0] != null)
                        bookRightMat = PageRightMat[0];
                    if (PageFlipNextMat[0] != null)
                        pageFlipNextMat = PageFlipNextMat[0];
                    if (BGMat[0] != null)
                        BGPlaneMat = BGMat[0];
                    if (PageFlipPreMat[0] != null)
                        pageFlipPreMat = PageFlipPreMat[0];
                    break;
                case 2:
                    if (PageLeftMat[1] != null)
                        bookLeftMat = PageLeftMat[1];
                    if (PageRightMat[1] != null)
                        bookRightMat = PageRightMat[1];
                    if (PageFlipNextMat[1] != null)
                        pageFlipNextMat = PageFlipNextMat[1];
                    if (BGMat[1] != null)
                        BGPlaneMat = BGMat[1];
                    if (PageFlipPreMat[1] != null)
                        pageFlipPreMat = PageFlipPreMat[1];
                    break;
                case 3:
                    if (PageLeftMat[2] != null)
                        bookLeftMat = PageLeftMat[2];
                    if (PageRightMat[2] != null)
                        bookRightMat = PageRightMat[2];
                    if (PageFlipNextMat[2] != null)
                        pageFlipNextMat = PageFlipNextMat[2];
                    if (BGMat[2] != null)
                        BGPlaneMat = BGMat[2];
                    if (PageFlipPreMat[2] != null)
                        pageFlipPreMat = PageFlipPreMat[2];
                    break;
                case 4:
                    if (PageLeftMat[3] != null)
                        bookLeftMat = PageLeftMat[3];
                    if (PageRightMat[3] != null)
                        bookRightMat = PageRightMat[3];
                    if (PageFlipNextMat[3] != null)
                        pageFlipNextMat = PageFlipNextMat[3];
                    if (BGMat[3] != null)
                        BGPlaneMat = BGMat[3];
                    if (PageFlipPreMat[3] != null)
                        pageFlipPreMat = PageFlipPreMat[3];
                    break;
                case 5:
                    if (PageLeftMat[4] != null)
                        bookLeftMat = PageLeftMat[4];
                    if (PageRightMat[4] != null)
                        bookRightMat = PageRightMat[4];
                    if (PageFlipNextMat[4] != null)
                        pageFlipNextMat = PageFlipNextMat[4];
                    if (BGMat[4] != null)
                        BGPlaneMat = BGMat[4];
                    if (PageFlipPreMat[4] != null)
                        pageFlipPreMat = PageFlipPreMat[4];
                    break;
                case 6:
                    if (PageLeftMat[5] != null)
                        bookLeftMat = PageLeftMat[5];
                    if (PageRightMat[5] != null)
                        bookRightMat = PageRightMat[5];
                    if (PageFlipNextMat[5] != null)
                        pageFlipNextMat = PageFlipNextMat[5];
                    if (BGMat[5] != null)
                        BGPlaneMat = BGMat[5];
                    if (PageFlipPreMat[5] != null)
                        pageFlipPreMat = PageFlipPreMat[5];
                    break;
                case 7:
                    if (PageLeftMat[6] != null)
                        bookLeftMat = PageLeftMat[6];
                    if (PageRightMat[6] != null)
                        bookRightMat = PageRightMat[6];
                    if (PageFlipNextMat[6] != null)
                        pageFlipNextMat = PageFlipNextMat[6];
                    if (BGMat[6] != null)
                        BGPlaneMat = BGMat[6];
                    if (PageFlipPreMat[6] != null)
                        pageFlipPreMat = PageFlipPreMat[6];
                    break;
                case 8:
                    if (PageLeftMat[7] != null)
                        bookLeftMat = PageLeftMat[7];
                    if (PageRightMat[7] != null)
                        bookRightMat = PageRightMat[7];
                    if (PageFlipNextMat[7] != null)
                        pageFlipNextMat = PageFlipNextMat[7];
                    if (BGMat[7] != null)
                        BGPlaneMat = BGMat[7];
                    if (PageFlipPreMat[7] != null)
                        pageFlipPreMat = PageFlipPreMat[7];
                    break;
                case 9:
                    if (PageLeftMat[8] != null)
                        bookLeftMat = PageLeftMat[8];
                    if (PageRightMat[8] != null)
                        bookRightMat = PageRightMat[8];
                    if (PageFlipNextMat[8] != null)
                        pageFlipNextMat = PageFlipNextMat[8];
                    if (BGMat[8] != null)
                        BGPlaneMat = BGMat[8];
                    if (PageFlipPreMat[8] != null)
                        pageFlipPreMat = PageFlipPreMat[8];
                    break;
                case 10:
                    if (PageLeftMat[9] != null)
                        bookLeftMat = PageLeftMat[9];
                    if (PageRightMat[9] != null)
                        bookRightMat = PageRightMat[9];
                    if (PageFlipNextMat[9] != null)
                        pageFlipNextMat = PageFlipNextMat[9];
                    if (BGMat[9] != null)
                        BGPlaneMat = BGMat[9];
                    if (PageFlipPreMat[9] != null)
                        pageFlipPreMat = PageFlipPreMat[9];
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Some materials is missing!");
            return book_mat;
        }

        book_mat[0] = bookLeftMat;
        book_mat[1] = bookRightMat;
        book_mat[2] = pageFlipNextMat;
        book_mat[3] = BGPlaneMat;
        book_mat[4] = pageFlipPreMat;
        return book_mat;
    }
    #endregion


    #region Book Open Next Page Progress
    private IEnumerator nextPage(int current_page)
    {
        current_page++;
        this.current_page = current_page;
        Debug.Log("Page " + current_page + " playing!");

        //Play animation pre page
        Animation animation = GetComponent<Animation>();
        string ani = "openPage" + current_page;
        animation[ani].speed = BookSpeed;
        animation.Play(ani);
        isAnimationPlay = true;

        //Hide current Background texture
        InvokeRepeating("HideBGTexture", 0f, 0.03F);
        RenderSettings.skybox = null;

        //Stop excuted dialog
        if (FungusFlowchart.FindBlock("Scence" + (current_page - 1)) != null)
            FungusFlowchart.FindBlock("Scence" + (current_page - 1)).Stop();

        //Set active current page
        getFlipPage(current_page).SetActive(true);
        getFlipPage(current_page).GetComponent<Renderer>().material = getMaterial(current_page)[2];

        //Set blank_page material to Book Right
        topPageRight.GetComponent<Renderer>().material = PageBlankMat;

        if (!UseAnimatorObject)
        {
            do
            {
                ScaleDownObject(getPageSke(current_page)[0]);
                ScaleDownObject(getPageSke(current_page)[1]);
                ScaleUpObject(getPageSke(current_page)[2]);
                ScaleUpObject(getPageSke(current_page)[3]);
                yield return null;
            } while (animation.isPlaying);

            //Scale in form
            if (getPageSke(current_page)[0] != null)
                getPageSke(current_page)[0].transform.localScale = new Vector3(0, 0, 0);
            if (getPageSke(current_page)[1] != null)
                getPageSke(current_page)[1].transform.localScale = new Vector3(0, 0, 0);
            if (getPageSke(current_page)[2] != null)
                getPageSke(current_page)[2].transform.localScale = new Vector3(1, 1, 1);
            if (getPageSke(current_page)[3] != null)
                getPageSke(current_page)[3].transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            if (getPageSke(current_page)[0] != null)
            {
                getPageSke(current_page)[0].transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getPageSke(current_page)[0].GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].PlayReverse();
            }
            if (getPageSke(current_page)[1] != null)
            {
                getPageSke(current_page)[1].transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getPageSke(current_page)[1].GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].PlayReverse();
            }
            if (getPageSke(current_page)[2] != null)
            {
                getPageSke(current_page)[2].transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getPageSke(current_page)[2].GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].Play();
            }
            if (getPageSke(current_page)[3] != null)
            {
                getPageSke(current_page)[3].transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getPageSke(current_page)[3].GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].Play();
            }
            if (getStaticContent(current_page - 1) != null)
            {
                getStaticContent(current_page - 1).transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getStaticContent(current_page - 1).GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].PlayReverse();
            }
            if (getStaticContent(current_page) != null)
            {
                getStaticContent(current_page).transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getStaticContent(current_page).GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].Play();
            }
            do
            {
                yield return null;
            } while (animation.isPlaying);

            if (getPageSke(current_page)[0] != null)
                getPageSke(current_page)[0].transform.localScale = new Vector3(0, 0, 0);
            if (getPageSke(current_page)[1] != null)
                getPageSke(current_page)[1].transform.localScale = new Vector3(0, 0, 0);
            if (getStaticContent(current_page - 1) != null)
                getStaticContent(current_page - 1).transform.localScale = new Vector3(0, 0, 0);
        }

        //Add Book materials when open page animation done
        try
        {
            topPageLeft.GetComponent<Renderer>().material = getMaterial(current_page)[0];
            topPageRight.GetComponent<Renderer>().material = getMaterial(current_page)[1];
            topPageLeft.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
            topPageRight.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
        }
        catch (Exception e)
        {
            Debug.Log("BookPage material is missing or does not have dissolve level component!");
        }

        //Dissolve show book page texture
        InvokeRepeating("DissolveBookPageTexture", 0f, 0.03F);

        //Deactive current flip page
        getFlipPage(current_page).SetActive(false);

        if (FungusFlowchart.FindBlock("Scence" + current_page) != null)
            FungusFlowchart.ExecuteBlock("Scence" + current_page);

        if (current_page == max_page)
            close();
    }
    #endregion


    #region Book Open Previous Page Progress
    private IEnumerator prePage(int current_page)
    {
        current_page--;
        this.current_page = current_page;
        Debug.Log("Page " + current_page + " playing!");

        //Play animation pre page
        Animation animation = GetComponent<Animation>();
        string ani = "closePage" + (current_page + 1);
        animation[ani].speed = BookSpeed;
        animation.Play(ani);
        isAnimationPlay = true;

        //Hide current Background texture
        InvokeRepeating("HideBGPrePageTexture", 0f, 0.03F);
        RenderSettings.skybox = null;

        //Stop excuted dialog
        if (FungusFlowchart.FindBlock("Scence" + (current_page + 1)) != null)
            FungusFlowchart.FindBlock("Scence" + (current_page + 1)).Stop();

        //Set active current page
        getFlipPage(current_page + 1).SetActive(true);
        getFlipPage(current_page + 1).GetComponent<Renderer>().material = getMaterial(current_page + 1)[4];

        //Set blank_page material to Top Page Left
        topPageLeft.GetComponent<Renderer>().material = PageBlankMat;

        if (!UseAnimatorObject)
        {
            do
            {
                ScaleUpObject(getPageSke(current_page + 1)[0]);
                ScaleUpObject(getPageSke(current_page + 1)[1]);
                ScaleDownObject(getPageSke(current_page + 1)[2]);
                ScaleDownObject(getPageSke(current_page + 1)[3]);
                yield return null;
            } while (animation.isPlaying);

            //Scale in form
            if (getPageSke(current_page + 1)[0] != null)
                getPageSke(current_page + 1)[0].transform.localScale = new Vector3(1, 1, 1);
            if (getPageSke(current_page + 1)[1] != null)
                getPageSke(current_page + 1)[1].transform.localScale = new Vector3(1, 1, 1);
            if (getPageSke(current_page + 1)[2] != null)
                getPageSke(current_page + 1)[2].transform.localScale = new Vector3(0, 0, 0);
            if (getPageSke(current_page + 1)[3] != null)
                getPageSke(current_page + 1)[3].transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            if (getPageSke(current_page + 1)[0] != null)
            {
                getPageSke(current_page + 1)[0].transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getPageSke(current_page + 1)[0].GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].Play();
            }
            if (getPageSke(current_page + 1)[1] != null)
            {
                getPageSke(current_page + 1)[1].transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getPageSke(current_page + 1)[1].GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].Play();
            }
            if (getPageSke(current_page + 1)[2] != null)
            {
                getPageSke(current_page + 1)[2].transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getPageSke(current_page + 1)[2].GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].PlayReverse();
            }
            if (getPageSke(current_page + 1)[3] != null)
            {
                getPageSke(current_page + 1)[3].transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getPageSke(current_page + 1)[3].GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].PlayReverse();
            }
            if (getStaticContent(current_page + 1) != null)
            {
                getStaticContent(current_page + 1).transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getStaticContent(current_page + 1).GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].PlayReverse();
            }
            if (getStaticContent(current_page) != null)
            {
                getStaticContent(current_page).transform.localScale = new Vector3(1, 1, 1);
                AnimatorBookObject[] child = getStaticContent(current_page).GetComponentsInChildren<AnimatorBookObject>();
                for (int i = 0; i < child.Length; i++)
                    child[i].Play();
            }
            do
            {
                yield return null;
            } while (animation.isPlaying);

            if (getPageSke(current_page + 1)[2] != null)
                getPageSke(current_page + 1)[2].transform.localScale = new Vector3(0, 0, 0);
            if (getPageSke(current_page + 1)[3] != null)
                getPageSke(current_page + 1)[3].transform.localScale = new Vector3(0, 0, 0);
            if (getStaticContent(current_page + 1) != null)
                getStaticContent(current_page + 1).transform.localScale = new Vector3(0, 0, 0);
        }

        //Set blank_page material to Top Page Right
        topPageRight.GetComponent<Renderer>().material = PageBlankMat;

        //Add Book materials when open page animation done
        try
        {
            topPageLeft.GetComponent<Renderer>().material = getMaterial(current_page)[0];
            topPageRight.GetComponent<Renderer>().material = getMaterial(current_page)[1];
            topPageLeft.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
            topPageRight.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
        }
        catch (Exception e)
        {
            Debug.Log("BookPage material is missing or does not have dissolve level component!");
        }

        //Dissolve show book page texture
        InvokeRepeating("DissolveBookPageTexture", 0f, 0.03F);

        //Deactive current flip page
        getFlipPage(current_page + 1).SetActive(false);        

        if (FungusFlowchart.FindBlock("Scence" + (current_page)) != null)
            FungusFlowchart.ExecuteBlock("Scence" + current_page);
    }
    #endregion


    #region Change Book Texture
    private void ClearBGTexture()
    {
        try
        {
            float dissolveLevel = BGPlane.GetComponent<Renderer>().material.GetFloat("_Level");
            if (dissolveLevel < 1)
                BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel + 0.02F);
            else
            {
                BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
                CancelInvoke("ClearBGTexture");
            }
        }
        catch (Exception e)
        {
            Debug.Log("BG material is missing or does not have dissolve level component!");
            BGPlane.SetActive(false);
            CancelInvoke("ClearBGTexture");
        }
    }

    private void HideBGTexture()
    {
        if (getMaterial(current_page)[3] != null)
        {
            try
            {
                float dissolveLevel = BGPlane.GetComponent<Renderer>().material.GetFloat("_Level");
                if (dissolveLevel < 1)
                    BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel + 0.02F);
                else
                {
                    BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
                    CancelInvoke("HideBGTexture");
                    BGPlane.GetComponent<Renderer>().material = getMaterial(current_page)[3];
                    BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
                    InvokeRepeating("DissolveBGTexture", 0f, 0.03F);
                }
            }
            catch (Exception e)
            {
                Debug.Log("BG material is missing or does not have dissolve level component!");
                BGPlane.SetActive(false);
                CancelInvoke("HideBGTexture");
            }
        }
        else
            CancelInvoke("HideBGTexture");
    }

    private void HideBGPrePageTexture()
    {
        try
        {
            float dissolveLevel = BGPlane.GetComponent<Renderer>().material.GetFloat("_Level");
            if (dissolveLevel < 1)
                BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel + 0.02F);
            else
            {
                BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
                CancelInvoke("HideBGPrePageTexture");
                BGPlane.GetComponent<Renderer>().material = getMaterial(current_page)[3];
                BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", 1F);
                InvokeRepeating("DissolveBGTexture", 0f, 0.03F);
            }
        }
        catch (Exception e)
        {
            Debug.Log("BG material is missing or does not have dissolve level component!");
            BGPlane.SetActive(false);
            CancelInvoke("HideBGPrePageTexture");
        }
    }

    private void DissolveBGTexture()
    {        
        try
        {
            float dissolveLevel = BGPlane.GetComponent<Renderer>().material.GetFloat("_Level");
            
            if (dissolveLevel > 0)
            {
                BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel - 0.02F);
            }
            else
            {
                BGPlane.GetComponent<Renderer>().material.SetFloat("_Level", 0F);
                if(SkyboxMat != null)
                    RenderSettings.skybox = SkyboxMat;

                CancelInvoke("DissolveBGTexture");
            }
        }
        catch (Exception e)
        {
            Debug.Log("BG material is missing or does not have dissolve level component!");
            BGPlane.SetActive(true);
            CancelInvoke("DissolveBGTexture");
        }
    }

    private void DissolveBookPageTexture()
    {
        try
        {
            if (topPageLeft.GetComponent<Renderer>().material.HasProperty("_Level"))
            {
                float dissolveLevel = topPageLeft.GetComponent<Renderer>().material.GetFloat("_Level");
                if (dissolveLevel > 0)
                {
                    topPageLeft.GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel - 0.02F);
                    topPageRight.GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel - 0.02F);
                }
                else
                {
                    topPageLeft.GetComponent<Renderer>().material.SetFloat("_Level", 0F);
                    topPageRight.GetComponent<Renderer>().material.SetFloat("_Level", 0F);
                    CancelInvoke("DissolveBookPageTexture");
                    isAnimationPlay = false;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("BookPage material is missing or does not have dissolve level component!");
            CancelInvoke("DissolveBookPageTexture");
        }
    }
    #endregion


    #region Support Own Scale Function
    private void ScaleUpObject(GameObject obj)
    {
        if (obj != null)
        {
            float x = 0.0066f;

            if (obj.transform.localScale.y < 1)
                obj.transform.localScale = obj.transform.localScale + new Vector3(x, x, x);

            if (obj.transform.localScale.y > 1)
                obj.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void ScaleDownObject(GameObject obj)
    {
        if (obj != null)
        {
            float x = 0.0066f;

            if (obj.transform.localScale.y > 0)
                obj.transform.localScale = obj.transform.localScale - new Vector3(x, x, x);

            if (obj.transform.localScale.y < 0)
                obj.transform.localScale = new Vector3(0, 0, 0);
        }
    } 
    #endregion
}
