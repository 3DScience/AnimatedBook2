﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using Fungus;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class HomeScript : MonoBehaviour {

	public GameObject uiLogin;
	public Flowchart flowchart;

	private bool selectedCategory=false;  
	ArrayList listBookInfo = new ArrayList();

	private LoginPanelController loginPanelController;

	private void Awake()
	{
		Application.targetFrameRate = 30;
		QualitySettings.vSyncCount = 0;
	}

	// Use this for initialization
	void Start () {
		//flowchart.ExecuteBlock("InitialScence");

		if(!GetComponent<CurtainController>().GetFirstLoad()) {
			GetComponent<CurtainController>().HideCurtain();	
		}

		ProfileFirebase.getInstance().getCurrentUser(getCurrentUserCallback);
	}

	void getCurrentUserCallback()
	{
		FirebaseUser user;
		user = ProfileFirebase.getInstance().auth.CurrentUser;
		if (user != null)
		{
			GlobalVar.login = 1;

			//DebugOnScreen.Log("getCurrentUserCallback-CurrentUser: Email=" + user.Email + ", DisplayName=" + user.DisplayName + ", userID=" + user.UserId);

			if (user.Email != LoginPanelController.GUEST_DEFAULT_EMAIL) {
				//DebugOnScreen.Log ("getCurrentUserCallback-CurrentUser: is not default user");

				loginPanelController = uiLogin.GetComponentInChildren<LoginPanelController> ();
				//DebugOnScreen.Log ("getCurrentUserCallback-CurrentUser: get LoginPanelController script successfully");
				loginPanelController.checkInfoUsers(user);
			}

			flowchart.ExecuteBlock("StartHomeScreen");

			//DebugOnScreen.Log ("getCurrentUserCallback :: Show Home screen");
		}
		else
		{
			//DebugOnScreen.Log ("getCurrentUserCallback :: no user logged in");
			GlobalVar.login = 2;
			flowchart.ExecuteBlock("InitialScence");
			//DebugOnScreen.Log("getCurrentUserCallback :: Show Login form);
		}
	}

	void loadAllCategory()
	{
		GameObject animal_book = GameObject.Find("animal_book");
		CategoryInfo cat1 = animal_book.AddComponent<CategoryInfo>();
		cat1.index = 0;
		cat1.categoryName = "nature";
		cat1.callback = OnSelectedBook;
		listBookInfo.Add(cat1);

		GameObject fairy_book = GameObject.Find("fairy_book");
		CategoryInfo cat2 = fairy_book.AddComponent<CategoryInfo>();
		cat2.index = 1;
		cat2.categoryName = "fairytale";
		cat2.callback = OnSelectedBook;
		listBookInfo.Add(cat2);

		GameObject science_book = GameObject.Find("science_book");
		CategoryInfo cat3 = science_book.AddComponent<CategoryInfo>();
		cat3.index = 2;
		cat3.categoryName = "science";
		cat3.callback = OnSelectedBook;
		listBookInfo.Add(cat3);

		GameObject fiction_book = GameObject.Find("fiction_book");
		CategoryInfo cat4 = fiction_book.AddComponent<CategoryInfo>();
		cat4.index = 3;
		cat4.categoryName = "fiction";
		cat4.callback = OnSelectedBook;
		listBookInfo.Add(cat4);

	}
	void OnSelectedBook(int index)
	{
		if (Debug.isDebugBuild) {
			Debug.Log("OnSelectedBook :: index = "+ index);
		}

		if (selectedCategory) {
			return;
		}

		selectedCategory = true;
		CategoryInfo categoryInfo = (CategoryInfo)listBookInfo[index];
		#if !UNITY_WEBGL
		BookListController.catName = categoryInfo.categoryName;
		LoadBookSelected(categoryInfo.categoryName);
		#endif
	}

	public void ButtonClick(string assetBundleName)
	{
		//Debug.Log("ButtonClick1 ");
		if (Debug.isDebugBuild)
		{

		}

	}

	public void LoadBookSelected(string categoryName)
	{
		Category.categoryName = categoryName;
		StartCoroutine(loadSceneWithAnimation(GlobalVar.CATEGORY_SCENE));
	}

	IEnumerator loadSceneWithAnimation(string senceName)
	{
		//slide a black curtain while transfering scene
		GetComponent<CurtainController>().CoverCurtain();
		yield return new WaitForSeconds(0.5f);

		SceneManager.LoadScene(senceName);      
	}


}
