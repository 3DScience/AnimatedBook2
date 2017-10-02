#if !UNITY_WEBGL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fungus;
using System.IO;

public class BookListController : MonoBehaviour {

    public static string catName;

    public Text txtCategotyName;
    public GameObject listUi;
    public GameObject bookListItemPref;
    public Flowchart flowchart;
    public RawImage bookDetailImg;
    public Button openBookBtn;
    public Button downloadBookBtn;
    public RectTransform bookDetailFrame;

    private RectTransform listUiRectTransform;
    private ScrollRect listUiScrollRect;
    private GridLayoutGroup contentGridLayout;

    private BookInfo selectedBook;

    List<GameObject> listGameObjectBookAdded = new List<GameObject>();
   
    void Start () {
        GetComponent<CurtainController>().HideCurtain();

        catName = Category.categoryName;
        if (catName == null)
        {
            catName = "science";
        }
        txtCategotyName.text = UppercaseFirst(catName);
        listUiRectTransform = listUi.GetComponent<RectTransform>();
        listUiScrollRect = listUi.GetComponent<ScrollRect>();
        contentGridLayout = listUi.GetComponentInChildren<GridLayoutGroup>();
        Debug.Log(listUiScrollRect);
        float cellWidth = (listUiRectTransform.rect.width - contentGridLayout.padding.left) / 3 - (contentGridLayout.spacing.x);
        //contentGridLayout.cellSize = new Vector2(cellWidth, cellWidth*1.3f);

        loadListBook();
        Debug.Log("test book by id 100001:" + BooksFireBaseDb.getInstance().getBookInfoById("100001").name);
        //for( int i=0; i<20; i ++)
        //{
        //    GameObject g=GameObject.Instantiate(bookListItemPref);
        //    g.transform.SetParent(contentGridLayout.transform);
        //    g.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //    BookListItemController bookListItemController = g.GetComponentInChildren<BookListItemController>();
        //    bookListItemController.idx = i;
        //    //Debug.Log("added " + bookListItemController.idx);
        //}

        if (Camera.main.aspect >= 1.6)
        {
            //Debug.Log("16:9");
            
        }
        else
        {
            //Debug.Log("4:3");
            bookDetailFrame.offsetMin = new Vector2(250, 250);
            bookDetailFrame.offsetMax = new Vector2(-250, -250);
        }
    }

    private void loadListBook()
    {

        foreach (var go in listGameObjectBookAdded)
        {
            Destroy(go);
            Debug.Log("remove gox"+ listGameObjectBookAdded.Count);
        }
        listGameObjectBookAdded.Clear();
        BooksFireBaseDb.getInstance().getBooksFromLocal(books => {
//			Debug.Log("books :: info :: "+ books);
            foreach (var book in books)
            {
                GameObject g = GameObject.Instantiate(bookListItemPref);
                g.SetActive(true);
                g.transform.SetParent(contentGridLayout.transform);
                g.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                listGameObjectBookAdded.Add(g);
                BookListItemController bookListItemController = g.GetComponentInChildren<BookListItemController>();
                bookListItemController.onclickCallBack = onBookClicked;
                bookListItemController.bookInfo = book;
               // Debug.Log("add gox"+ listGameObjectBookAdded.Count);
            }
            StartCoroutine(loadImages());
        }, catName);
        listUiScrollRect.normalizedPosition = new Vector2(0, 1);
      
    }

    private bool checkIsDownloadedAsset(string assetBundleName)
    {
        string platform = Application.platform.ToString();
        if (platform == RuntimePlatform.IPhonePlayer.ToString())
        {
            platform = "iOS";
        }
        string assetDataFolder = GlobalVar.DATA_PATH + "/" + platform + "/" + assetBundleName;
        if (File.Exists(assetDataFolder))
        {
            return true;
        }
        return false;
    }

    void onBookClicked (BookInfo bookInfo)
    {
        flowchart.ExecuteBlock("OpenBookDetail");
        selectedBook = bookInfo;

        //Load Book cover from local
        string thumbnailPath = GlobalVar.THUMBNAILS_PATH + "/" + Path.GetFileName(bookInfo.picture_url);
        byte[] fileData = File.ReadAllBytes(thumbnailPath);
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
        tex.LoadImage(fileData);
        bookDetailImg.texture = tex;

        if(checkIsDownloadedAsset(selectedBook.assetbundle))
        {
            openBookBtn.gameObject.SetActive(true);
            downloadBookBtn.gameObject.SetActive(false);
        }
        else 
        {
            openBookBtn.gameObject.SetActive(false);
            downloadBookBtn.gameObject.SetActive(true);
        }
    }

    IEnumerator loadImages()
    {
        foreach (var book in listGameObjectBookAdded)
        {
            BookListItemController bookListItemController = book.GetComponentInChildren<BookListItemController>();
            yield return bookListItemController.loadImg();
        }
    }

    public void refressButtonClicked()
    {
        BooksFireBaseDb.getInstance().reSaveBooksToLocal(()=> { loadListBook(); });
        
    }

    public void onHomeButtonClick()
    {       
        StartCoroutine(loadScene(GlobalVar.MAINSCENE));
    }

    IEnumerator loadScene(string senceName)
    {
        GetComponent<CurtainController>().CoverCurtain();
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(senceName);
    }

    public string UppercaseFirst(string s)
    {
        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }

    public void OpenBook()
    {
        if(selectedBook != null) {            
            string assetBundleName = "";
            assetBundleName = selectedBook.assetbundle;
            //Debug.Log("assetBundleName Book2dDetail: " + assetBundleName);

            SceneManager.LoadScene(GlobalVar.DOWNLOAD_ASSET_SCENE);
        }
    }

    public void ReDownloadBook()
    {
        //GlobalVar.shareContext.shareVar.Add ("bookInfo", bookInfo);
        SceneManager.LoadScene(GlobalVar.DOWNLOAD_ASSET_SCENE);
    }

    public void DeleteBook()
    {
        try
        {
            string platform = Application.platform.ToString();
            if (platform == RuntimePlatform.IPhonePlayer.ToString())
            {
                platform = "iOS";
            }
            string assetDataFolder = GlobalVar.DATA_PATH + "/" + platform + "/" + selectedBook.assetbundle;
            if (File.Exists(assetDataFolder))
            {
                File.Delete(assetDataFolder);
                File.Delete(assetDataFolder + ".mf");
                File.Delete(assetDataFolder + ".manifest");
                Caching.CleanCache();
            }

            openBookBtn.gameObject.SetActive(false);
            downloadBookBtn.gameObject.SetActive(true);
        }
        catch (System.Exception e)
        {
            //DebugOnScreen.Log(ex.ToString());
        }
    }

}
#endif