﻿using System.Collections;
using System.Collections.Generic;
#if !UNITY_WEBGL
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Facebook.Unity;
using UnityEngine.SceneManagement;

public class ProfilePanelController : MonoBehaviour
{
	//public GameObject dialogUi;
 //   public GameObject loginPanel;
 //   public GameObject profilePanel;
	//public GameObject loginButton;
	//public GameObject profileButton;
 //   public Text txtEmail;
 //   public Text txtCash;

    // Use this for initialization
    void Start()
    {
        if (GlobalVar.DEBUG)
        {
            Debug.Log("Onstart");
        }
        //loadUser();
        ProfileFirebase.getInstance().listenLoginStateChange(OnLoginStateChange);
    }

    public void OnLoginStateChange(bool logedin)
    {
        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("ProfilePanelController- OnLoginStateChange, logedin= "+ logedin);
        if (logedin)
        {
			//if (ProfileFirebase.getInstance().auth.CurrentUser.Email != "") {
   //         	txtEmail.text = ProfileFirebase.getInstance().auth.CurrentUser.Email;
			//} else {
			//	txtEmail.text = ProfileFirebase.getInstance ().auth.CurrentUser.UserId;
			//}

			//DebugOnScreen.Log("ProfilePanelController- OnLoginStateChange, txtEmail.text 111 = "+ txtEmail.text);
			GlobalVar.tester = "0";
			FirebaseDatabase.DefaultInstance.RootReference.Child ("private")
				.Child ("userInfo").Child (ProfileFirebase.getInstance ().auth.CurrentUser.UserId).GetValueAsync ().ContinueWith (taskValue => {
					if (taskValue.IsFaulted)
					{
						// nothing
					}
					else if (taskValue.IsCompleted)
					{
						DataSnapshot snapshot_ = taskValue.Result;
						GlobalVar.tester = snapshot_.Child("tester").Value.ToString();
					}
				});
        }

        else
        {
            //txtEmail.text = "";
        }
    }

//	public static void AuthStateChanged(object sender, System.EventArgs eventArgs) {
//		DebugOnScreen.Log ("AuthStateChanged");
//
//		if (FirebaseHelper.auth.CurrentUser != FirebaseHelper.firebaseUser) {
//			FirebaseHelper.signedIn = FirebaseHelper.firebaseUser != FirebaseHelper.auth.CurrentUser && FirebaseHelper.auth.CurrentUser != null;
//			if (!FirebaseHelper.signedIn && FirebaseHelper.firebaseUser != null) {
//				DebugOnScreen.Log("Signed out :: " + FirebaseHelper.firebaseUser.UserId);
//			}
//
//			FirebaseHelper.firebaseUser = FirebaseHelper.auth.CurrentUser;
//
//			if (FirebaseHelper.signedIn) {
//				Debug.LogFormat("AuthStateChanged :: User Changed: {0} ({1})",
//					FirebaseHelper.firebaseUser.DisplayName, FirebaseHelper.firebaseUser.UserId);
//			} else {
//				DebugOnScreen.Log("AuthStateChanged :: No user is signed in :: "  + FirebaseHelper.firebaseUser.UserId);
//			}
//		}
//	}


    void loadUser()
    {
       // txtEmail.text = ProfileFirebase.getInstance().user.Email;
    }


    public void OnLogOutButtonClick()
    {
        if (GlobalVar.DEBUG)
            DebugOnScreen.Log("ProfilePanelController- OnLogOutButtonClick");
        try
        {
			LoginPanelController.islogin = false;

            ProfileFirebase.getInstance().auth.SignOut();

			DebugOnScreen.Log("ProfilePanelController- OnLogOutButtonClick islogin: " + LoginPanelController.islogin);
			if (LoginPanelController.islogin == true) {
				FB.LogOut();
			}

            GlobalVar.login = 2;
            SceneManager.LoadScene(GlobalVar.MAINSCENE);
        }
        catch (System.Exception ex)
        {
            DebugOnScreen.Log(ex.Message);
            DebugOnScreen.Log(ex.ToString());
        }

    }


    public void deactiveProfilePanel()
    {		
		//GlobalVar.login = 2;
		//SceneManager.LoadScene(GlobalVar.MAINSCENE);
    }
}
#endif
