using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BookListItemController : MonoBehaviour {
    public int idx{ get; set; }
    public BookInfo bookInfo;
    public Text txtName;
    public RawImage  img;

	private string thumbnailPath = "";
	private WWW imgLink;
	private bool loadLocal = false;
	private bool isSavingFile = false;

    // Use this for initialization
    void Start () {
		isSavingFile = false;

        string imageUrl = bookInfo.picture_url;
        //Debug.Log("ImageURL="+ imageUrl);
      //  if( imageUrl!=null && imageUrl!="")
        //StartCoroutine(loadImg(imageUrl));
        txtName.text = bookInfo.name;

		thumbnailPath = GlobalVar.THUMBNAILS_PATH + "/" + Path.GetFileName(bookInfo.picture_url);
		//DebugOnScreen.Log ("BookListItemController :: thumbnailPath :: " +thumbnailPath);
    }

	void Update()
	{

		if (imgLink == null) {
			return;
		}


		if (imgLink.isDone) {
			if (isSavingFile == false) {
				isSavingFile = true;
				StartCoroutine( saveFileToLocal());
			}	
		}
	}

    public void onClicked()
    {
        Debug.Log("onClicked idx="+idx);
        if (GlobalVar.shareContext.shareVar.ContainsKey("bookInfo"))
            GlobalVar.shareContext.shareVar.Remove("bookInfo");
        GlobalVar.shareContext.shareVar.Add("bookInfo",bookInfo);
        SceneManager.LoadScene(GlobalVar.BOOK2DDETAIL_SCENE);
    }


    public IEnumerator loadImg()
    {
		//DebugOnScreen.Log("BookListItemController :: loadImg");
        if (bookInfo.picture_url != null && bookInfo.picture_url != "")
        {
			if (thumbnailPath.Length == 0) {
				thumbnailPath = GlobalVar.THUMBNAILS_PATH + "/" + Path.GetFileName(bookInfo.picture_url);
			}

			if (loadLocal == false && !File.Exists(thumbnailPath)) {
				//DebugOnScreen.Log("BookListItemController :: loadImg :: load from link");
				imgLink = new WWW(bookInfo.picture_url);

				yield return imgLink;
				img.texture = imgLink.texture;

				imgLink.Dispose();
				imgLink = null;

			} else {
				//DebugOnScreen.Log("BookListItemController :: loadImg :: Exists");
				byte[] fileData = File.ReadAllBytes(thumbnailPath);
				Texture2D tex = new Texture2D(2, 2);
				tex.LoadImage(fileData);
				img.texture = tex;

				loadLocal = true;
			}
        }
    }

	IEnumerator saveFileToLocal() {
		//DebugOnScreen.Log("BookListItemController :: saveFileToLocal");
		try {
			
			byte[] data = imgLink.bytes;

			if (!Directory.Exists(GlobalVar.THUMBNAILS_PATH))
			{
				Directory.CreateDirectory(GlobalVar.THUMBNAILS_PATH);
			}

			File.WriteAllBytes(thumbnailPath, data);
			isSavingFile = false;
			//DebugOnScreen.Log("BookListItemController :: saveFileToLocal :: saved ok :: " + Path.GetFileName(thumbnailPath));

		} catch (System.Exception ex) {
			////DebugOnScreen.Log(ex.ToString());
			yield break;
		}

		yield return null;

	}
}
