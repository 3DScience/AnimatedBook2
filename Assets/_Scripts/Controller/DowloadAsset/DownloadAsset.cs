#if !UNITY_WEBGL
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProgressBar;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class BookAndDependenciesInfo {
	public string version;
	public string[] dependencies;
}

[Serializable]
public class DependencyVersion {
	public string dependencyID;
	public string version;
}

[Serializable]
public class DependenciesArray {
	public DependencyVersion[] dependencies;
}

public class DownloadAsset : MonoBehaviour {
	public Text txtMsg;
    public Text progressBarText;
	public GameObject dialogMessagePref;
	int currentDownloadDependencyIdx = 0;
	string platform = Application.platform.ToString();
	private string assetDataFolder = "";
	private WWW www;
	private string url;
	private bool isSavingFile= false;
	private bool isDownloadingMainBook = true;	//main book
	private string jsonBookPath = "";
	private string jsonDependencyPath = "dependencies.json";
	string assetBundleName = "";
	ProgressBarBehaviour barBehaviour;
	private List<BookInfo> dependeciesBook = new List<BookInfo>();
	private DialogMessageController dialogMessageController;

	/*
	 json format
	  {
          "version":"1",
          "dependencies”:{[
            “0001”,
            “0002”]
          }
	  }	  
	 */

	// Use this for initialization
	void Start () {
        BookInfo bookInfo = (BookInfo)GlobalVar.shareContext.shareVar["bookInfo"];
        assetBundleName = bookInfo.assetbundle;

        if (!checkIsDownloadedAsset(assetBundleName))
        {
            GameObject obj = GameObject.Find("ProgressBarLabelInside");
            barBehaviour = obj.GetComponent<ProgressBarBehaviour>();
            barBehaviour.ProgressSpeed = 10000;
            
            //GlobalVar.shareContext.shareVar.Remove ("bookInfo");

            if (bookInfo == null)
            { // for testing
              //assetBundleName = "test_book";
                assetBundleName = "solar_system_book";
            }
            else
            {
                assetBundleName = bookInfo.assetbundle;
                if (bookInfo.dependencies != null)
                {
                    foreach (var bookId in bookInfo.dependencies)
                    {
                        BookInfo dependencyBook = BooksFireBaseDb.getInstance().getBookInfoById(bookId);
                        dependeciesBook.Add(dependencyBook);
                    }
                }

            }



            if (Application.platform == RuntimePlatform.WindowsEditor)// for testing
            {
                platform = "Android";
            }
            if (platform == RuntimePlatform.IPhonePlayer.ToString())
            {
                platform = "iOS";
            }
            try
            {
                assetDataFolder = GlobalVar.DATA_PATH + "/";
                string olderFile = assetDataFolder + assetBundleName;
                //DebugOnScreen.Log ("olderFile :: " +olderFile);
                if (File.Exists(olderFile))
                {
                    File.Delete(olderFile);
                    File.Delete(olderFile + ".mf");
                    File.Delete(olderFile + ".manifest");
                    Caching.CleanCache();
                }

                string jsonName = bookInfo.id;

                jsonName = jsonName + ".json";
                jsonBookPath = GlobalVar.JSONS_PATH + "/" + jsonName;
                jsonDependencyPath = GlobalVar.JSONS_PATH + "/" + jsonDependencyPath;

                //do not need to create this file here
                //			if (!File.Exists (jsonBookPath)) {
                //				var file = File.CreateText (jsonBookPath);
                //				file.Close ();
                //			}

                //			if (!File.Exists (jsonDependencyPath)) {
                //				var file = File.CreateText (jsonDependencyPath);
                //				file.Close ();
                //			}
            }
            catch (System.Exception ex)
            {
                //DebugOnScreen.Log(ex.ToString());
            }

            txtMsg.text = "Downloading contents";
            url = GlobalVar.BASE_ASSET_DOWNLOAD_URL + bookInfo.download_url + "/" + bookInfo.assetbundle + "_" + platform + ".zip";
            //if (GlobalVar.DEBUG)
            //DebugOnScreen.Log("url 1 =" + url);
            startDownload();
        }

        else
        {
            GameObject obj = GameObject.Find("ProgressBarLabelInside");
            barBehaviour = obj.GetComponent<ProgressBarBehaviour>();
            barBehaviour.ProgressSpeed = 10000;
            barBehaviour.Value = 100;          
            StartCoroutine(openBook());
        }
	}
	private void startDownload()
	{
		if (checkNetwork())
		{
			www = new WWW(url);

		}
		else
		{
			if (dialogMessageController == null)
			{
				initDialogMessage();
			}
			dialogMessageController.setActive(true);
			dialogMessageController.setMessage("No Internet connection!");
			dialogMessageController.setButtonText("Retry");
		}
	}

	private void initDialogMessage()
	{
		GameObject dialogMsg = GameObject.Instantiate(dialogMessagePref);
		dialogMessageController = dialogMsg.GetComponentInChildren<DialogMessageController>();
		dialogMessageController.onOkButtonClickCallback = dialogOkButtonClick;
		dialogMessageController.onBackButtonClickCallback = dialogBackButtonClick;
	}

	private void dialogOkButtonClick()
	{
		dialogMessageController.setActive(false);
		barBehaviour.Value = 0;
		progressBarText.text = txtMsg.text + " - 0%";
		startDownload();
	}
	private void dialogBackButtonClick()
	{
		SceneManager.UnloadSceneAsync(GlobalVar.DOWNLOAD_ASSET_SCENE);
		SceneManager.LoadScene(GlobalVar.BOOK2DDETAIL_SCENE);
	}
	private bool checkNetwork()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}
		return true;
	}
	private void handleDownloadError()
	{

		if (dialogMessageController == null)
		{
			initDialogMessage();
		}
		dialogMessageController.setActive(true);
		dialogMessageController.setMessage("Loading error ...");
		dialogMessageController.setButtonText("Retry");

#if DEVELOPMENT_BUILD
//DebugOnScreen.Log("download err:" + www.error);
#endif
		www.Dispose();
		www = null;
	}
	// Update is called once per frame
	void Update()
	{

		if (www == null)
			return;

		if (www.isDone)
		{
			if( www.error!=null)
			{
				handleDownloadError();
				return;
			}else  if (!isSavingFile)
			{
				barBehaviour.Value =100;
				isSavingFile = true;

				if (isDownloadingMainBook == true) {
					isDownloadingMainBook = false;
					StartCoroutine(createBookJsonFile());

				} else {
					//update version for dependencies
					if (currentDownloadDependencyIdx > 0 && currentDownloadDependencyIdx <= dependeciesBook.Count) {

						BookInfo downloadedDependency = dependeciesBook [currentDownloadDependencyIdx - 1];
						//DebugOnScreen.Log ("just downloaded dependency version info :: " + JsonUtility.ToJson (downloadedDependency));

						DependenciesArray depArr = new DependenciesArray();
						DependencyVersion[] alDepVerInfo = getAllDependenciesVersionInfo();//old info
						//DebugOnScreen.Log ("getAllDependenciesVersionInfo :: ok");

						if (alDepVerInfo != null && alDepVerInfo.Length > 0) {

							List<DependencyVersion> newverions = new List<DependencyVersion> (alDepVerInfo);

							foreach (DependencyVersion ver in alDepVerInfo) {
								//DebugOnScreen.Log ("old version :: " + ver.version);
								if (ver.dependencyID.Equals (downloadedDependency.id)) {
									//DebugOnScreen.Log ("getBookFullInfo :: ver :: " + ver.dependencyID); 
									ver.version = downloadedDependency.version;
									newverions.Remove (ver);

									break;
								}
							}

							DependencyVersion newver = new DependencyVersion ();
							newver.dependencyID = downloadedDependency.id;
							newver.version = downloadedDependency.version;

							newverions.Add (newver);
							depArr.dependencies = newverions.ToArray ();

						} else {
							//DebugOnScreen.Log ("else :: " + downloadedDependency.id + " :: " +downloadedDependency.version );
							DependencyVersion dependencyVersion = new DependencyVersion ();
							dependencyVersion.dependencyID = downloadedDependency.id;
							dependencyVersion.version = downloadedDependency.version;

							List<DependencyVersion> arr = new List<DependencyVersion> ();
							arr.Add (dependencyVersion);

							depArr.dependencies = arr.ToArray ();
						}

						//DebugOnScreen.Log ("New dependency version info");
						//DebugOnScreen.Log ("New dependency version info :: " + JsonUtility.ToJson (depArr));

						StartCoroutine (writeFileWithData (jsonDependencyPath, JsonUtility.ToJson (depArr)));
					}
				}

				StartCoroutine( saveFileToLocal());
			}

		}

		else
		{
			if(www.error != null)
			{
				handleDownloadError();
				return;
			}            
            barBehaviour.Value = www.progress*100;
            progressBarText.text = txtMsg.text + " - " + barBehaviour.Value + "%";
            if (Debug.isDebugBuild)
				Debug.Log("downloaded " + www.progress * 100 + " %");
		}

	}

	IEnumerator writeFileWithData(string jsPath, string data) {
		if (!File.Exists (jsPath)) {
			var file = File.CreateText (jsPath);
			file.Close ();
		}

		var openFile = File.Open(jsPath, FileMode.Truncate);

		Encoding encode = new UTF8Encoding();
		openFile.Write(encode.GetBytes(data), 0, encode.GetBytes(data).Length);
		openFile.Close();

		yield return null;
	}

	//if there is no dependency need to download again, www.isDone is still true;
	//in update() method, it will work wrong
	//=> need to reset it some where => after save data successfully
	private void downloadDependencies()
	{
		//DebugOnScreen.Log("currentDownloadDependencyIdx :: " + currentDownloadDependencyIdx);
		//DebugOnScreen.Log("dependeciesBook.Count :: " + dependeciesBook.Count);
		if(currentDownloadDependencyIdx >= dependeciesBook.Count )
		{
			//progressBarText.text = "Done";

			if (dependeciesBook.Count > 0)
			{
				string[] dependenciesAbName = new string[dependeciesBook.Count];
				int i = 0;
				foreach (BookInfo dep in dependeciesBook)
				{
					dependenciesAbName[i] = dep.name;
					i++;
				}
				BookLoader.dependenciesAbName = dependenciesAbName;
				//DebugOnScreen.Log ("TAI SAO DEO LOAD LEN :: " +dependenciesAbName);
			}

            //DebugOnScreen.Log ("TAI SAO DEO LOAD LEN");
            //BookLoader.assetBundleName = assetBundleName;
            //SceneManager.LoadScene(GlobalVar.BOOK_LOADER_SCENE);
            StartCoroutine(openBook());
		}else
		{
			isSavingFile = false;

			txtMsg.text = "Downloading dependencies ("+ (currentDownloadDependencyIdx+1)+"/"+dependeciesBook.Count+")";
			BookInfo dependencyBook = dependeciesBook[currentDownloadDependencyIdx];

			if( !checkDependencyToDownload(dependencyBook))
			{
				////DebugOnScreen.Log("Download dependency book: " + dependencyBook.name);

				url = GlobalVar.BASE_ASSET_DOWNLOAD_URL + dependencyBook.download_url + "/" + dependencyBook.assetbundle + "_" + platform + ".zip";
				//url = GlobalVar.BASE_ASSET_DOWNLOAD_URL + dependencyBook.download_url + "/" + platform + ".zip";
				//if (GlobalVar.DEBUG)
				//DebugOnScreen.Log("url 2=" + url);
				www = new WWW(url);
				currentDownloadDependencyIdx++;
			}
			else {
				////DebugOnScreen.Log("dependency book \"" + dependencyBook.name +"\" is existing.");
				currentDownloadDependencyIdx++;
				downloadDependencies();
			}

		}

	}

    IEnumerator openBook()
    {
        progressBarText.text = "Openning book";
        BookLoader.assetBundleName = assetBundleName;
        if (assetBundleName == null || assetBundleName == "")
        {
            //assetBundleName = "q10k_01"; 
            //assetBundleName = "littleredridinghood";
            assetBundleName = "solar_system_book";
            //assetBundleName = "nearest_stars";
        }
#if !UNITY_WEBGL
        // DebugOnScreen.Log("init mainfest 10");
        yield return AssetBundleHelper.getInstance().InitializeAssetBunder(assetBundleName);
        try
        {

            DebugOnScreen.Log("BookLoader Start assetBundleName=" + assetBundleName);
            BookSceneLoader sceneLoader = gameObject.AddComponent<BookSceneLoader>();
            sceneLoader.assetBundleName = assetBundleName;
            sceneLoader.sceneName = "page1";
        }
        catch (System.Exception ex)
        {

            DebugOnScreen.Log(ex.ToString());
        }
#else
#endif
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

    //if file is not exist or obsoleted => download
    //false: need to re-download
    //true: do not re-download
    private bool checkDependencyToDownload(BookInfo dependencyBook)
	{
		string abname = dependencyBook.assetbundle;
		string platform = Application.platform.ToString();
		if (platform == RuntimePlatform.IPhonePlayer.ToString())
		{
			platform = "iOS";
		}
		bool existed = File.Exists(assetDataFolder +platform+"/"+ abname);
		//DebugOnScreen.Log("checking exist ab name " + assetDataFolder + platform + "/" + abname+". Result=" +existed);
		//there is a case, a dependency had been downloaded by other books
		//its information is not being contained in this json
		//so, oldVersion will be blank
		if (existed) {
			string oldVersion = getDependencyVersion(dependencyBook.id);
			int intOldVersion = int.Parse(oldVersion);
			int intNewVersion = int.Parse(dependencyBook.version);

			//DebugOnScreen.Log("NewVersion :: " + intNewVersion);
			//DebugOnScreen.Log("OldVersion :: " + intOldVersion);

			if (intNewVersion > intOldVersion) {
				//DebugOnScreen.Log("Need to download new version of :: " + dependencyBook.assetbundle);
				return false;

			} else {
				//DebugOnScreen.Log("Do NOT download new version of :: " + dependencyBook.assetbundle);
				return true;
			}
		}

		//DebugOnScreen.Log ("checkDependencyToDownload :: not exist :: " + abname);
		return false;
	}

	IEnumerator saveFileToLocal()
	{
		if (Debug.isDebugBuild)
			Debug.Log("persistentDataPath=" + Application.persistentDataPath);
		string zipFile="";

		progressBarText.text = "Saving";
		yield return null;
		try
		{
			if (Debug.isDebugBuild)
				Debug.Log("dowload file from url " + url + " complete");
			byte[] data = www.bytes;

//			if (!Directory.Exists(assetDataFolder))
//			{
//				Directory.CreateDirectory(assetDataFolder);
//			}
			zipFile = assetDataFolder  + assetBundleName + ".zip";
			if (Debug.isDebugBuild)
				Debug.Log("dataFile=" + zipFile);
			System.IO.File.WriteAllBytes(zipFile, data);

			www.Dispose();
			www = null;

		}
		catch (System.Exception ex)
		{
			//DebugOnScreen.Log(ex.ToString());
			yield break;
		}

		StartCoroutine(unzipFile(zipFile, assetDataFolder));
		yield return null;

	}
	IEnumerator unzipFile(string zipFile,string path)
	{
		progressBarText.text = "Extracting";
		yield return null;
		try
		{
			ZipUtil.Unzip(zipFile, path);
			File.Delete(zipFile);
		}
		catch (System.Exception ex)
		{
			//DebugOnScreen.Log(ex.ToString());
			yield break;
		}

		if (Debug.isDebugBuild)
			Debug.Log("unzip done!");

		//DebugOnScreen.Log ("unzipFile completed :: download dep");
		downloadDependencies();
		//progressBarText.text = "DONE";
		//BookLoader.assetBundleName = assetBundleName;
		//SceneManager.LoadScene(GlobalVar.BOOK_LOADER_SCENE);
		yield return null;
	}

	IEnumerator createBookJsonFile() {
		BookInfo bookInfo = (BookInfo) GlobalVar.shareContext.shareVar["bookInfo"];
		//DebugOnScreen.Log ("createBookJsonFile :: " +jsonBookPath);

		//create new if file is not exist
		if (!File.Exists(jsonBookPath))
		{
			var file = File.CreateText(jsonBookPath);
			file.Close ();
			//DebugOnScreen.Log ("createBookJsonFile :: new");
			//create content
			BookAndDependenciesInfo content = new BookAndDependenciesInfo ();
			content.version = bookInfo.version;

			List<string> arr = new List<string> ();
			content.dependencies = arr.ToArray();

			//write content with no dependency info
			//DebugOnScreen.Log("content :: " +JsonUtility.ToJson(content));
			File.WriteAllText(jsonBookPath, JsonUtility.ToJson(content));

		} else {

			//DebugOnScreen.Log ("createBookJsonFile :: edit");
			//if existed, read file and update version
			string oldContent = readJsonFile (jsonBookPath);
			//DebugOnScreen.Log ("createBookJsonFile :: oldContent :: " + oldContent);

			BookAndDependenciesInfo oldContentObj = JsonUtility.FromJson<BookAndDependenciesInfo>(oldContent);
			oldContentObj.version = bookInfo.version;

			//DebugOnScreen.Log(JsonUtility.ToJson(oldContentObj));

			var openFile = File.Open(jsonBookPath, FileMode.Truncate);
			string newContent = JsonUtility.ToJson(oldContentObj);

			Encoding unicode = new UTF8Encoding();
			openFile.Write(unicode.GetBytes(newContent), 0, newContent.Length);
			openFile.Close();
		}

		yield return null;
	}

	private string readJsonFile(string jsonBookPath) {
		//DebugOnScreen.Log ("readJsonFile");
		if(File.Exists(jsonBookPath)){
			var sr = File.OpenText(jsonBookPath);
			string line = sr.ReadToEnd();
			sr.Close ();
			//			string line = File.ReadAllText (jsonBookPath);

			if (line != null){
				Debug.Log(line);

				return line;
			}

			//DebugOnScreen.Log ("tai sao lai null");
			return "";

		} else {
			//DebugOnScreen.Log("Could not Open the file: " + jsonBookPath);
			Debug.Log("Could not Open the file: " + jsonBookPath);
			return "";
		}
	}

	private BookAndDependenciesInfo getBookFullInfo(string jsonBookPath) {
		if(File.Exists(jsonBookPath)){
			string oldContent = readJsonFile (jsonBookPath);
			//DebugOnScreen.Log ("getBookFullInfo :: readJsonFile :: " +oldContent);
			BookAndDependenciesInfo oldContentObj = JsonUtility.FromJson<BookAndDependenciesInfo>(oldContent);
			//DebugOnScreen.Log ("getBookFullInfo :: parse ok");

			return oldContentObj;

		} else {
			//DebugOnScreen.Log ("Could not Open the file :: " +jsonBookPath);
			Debug.Log("Could not Open the file: " + jsonBookPath);
			return null;
		}
	}

	private DependencyVersion[] getAllDependenciesVersionInfo() {
		if(File.Exists(jsonDependencyPath)){
			string oldContent = readJsonFile (jsonDependencyPath);
			//DebugOnScreen.Log ("getAllDependenciesVersionInfo :: readJsonFile :: " +oldContent);
			DependenciesArray oldContentObj = JsonUtility.FromJson<DependenciesArray>(oldContent);
			//DebugOnScreen.Log ("getAllDependenciesVersionInfo :: parse ok");

			return oldContentObj.dependencies;

		} else {
			//DebugOnScreen.Log ("Could not Open the file :: " +jsonDependencyPath);
			Debug.Log("Could not Open the file: " + jsonDependencyPath);
			return null;
		}
	}

	private string getDependencyVersion (string bookID) {
		//DebugOnScreen.Log ("getDependencyVersion");

		DependencyVersion[] versions = getAllDependenciesVersionInfo();
		string res = "";
		//DebugOnScreen.Log ("bookID :: " +bookID);
		foreach(DependencyVersion ver in versions) {
			//DebugOnScreen.Log ("ver dep id :: " +ver.dependencyID);
			if (ver.dependencyID.Equals(bookID)) {
				res = ver.version;

				break;
			}
		}

		//DebugOnScreen.Log ("return :: " +res);
		return res;
	}

	void OnDestroy()
	{
		if(dialogMessageController != null)
		{
			dialogMessageController.DestroyGo();
		}
	}
}
#endif