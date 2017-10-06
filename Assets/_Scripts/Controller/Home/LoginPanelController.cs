using System.Collections;
using System.Collections.Generic;
#if !UNITY_WEBGL
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Facebook.Unity;
using Facebook.Unity.Editor;
using Firebase.Database;
using UnityEngine.SceneManagement;
using Fungus;

public class LoginPanelController : MonoBehaviour {

	/* do NOT use anymore
	public GameObject uiLogin;
	public GameObject usersLoginButton;
	public GameObject profilePanel;
	public GameObject loginButton;
	public GameObject profileButton;
	public GameObject loginPanel;
	*/
	public GameObject usersLoginPanel;
	public Text txtEmail;
	public InputField txtPassword;
	public Text txtError;
	public GameObject loadingPanel;
	public Flowchart flowchart;

	public const string GUEST_DEFAULT_EMAIL = "guest@gmail.com";
	public const string GUEST_DEFAULT_PASS  = "123456";

	private FirebaseUser userInfo;
	private const string USERSINFO = "userInfo";

	private const string PERMISSION_PRIVATE = "private";

	void OnEnable()
	{

		if (GlobalVar.DEBUG) {
			DebugOnScreen.Log("LoginPanelController-OnEnable");
		}

		txtError.gameObject.SetActive(false);
	}
	// Use this for initialization
	void Start () {
		if (GlobalVar.DEBUG) {
			DebugOnScreen.Log("LoginPanelController- Onstart ");
		}
	}

	void Awake ()
	{
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			//DebugOnScreen.Log("Initialize the Facebook SDK ");
			FB.Init(InitCallback, OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			DebugOnScreen.Log("Already initialized, signal an app activation App Event");
			FB.ActivateApp();
		}
	}
	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} else {
			//DebugOnScreen.Log("Failed to Initialize the Facebook SDK");
		}
	}
	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void OnUsersLoginButtonClick()
	{
		usersLoginPanel.SetActive (true);
	}

	public void DisableUsersLoginPanel()
	{
		usersLoginPanel.SetActive (false);
	}

	public void OnLoginButtonGuestClick()
	{
		//DebugOnScreen.Log("OnLoginButtonGuestClick ");
		ProfileFirebase.getInstance().Login (GUEST_DEFAULT_EMAIL, GUEST_DEFAULT_PASS, HandleSigninResult);
	}

	void HandleAction (UserInfo obj)
	{

	}

	public static bool islogin = false;
	public void OnLoginButtonClick()
	{
		loadingPanel.SetActive(true);
		if (GlobalVar.DEBUG) {
			DebugOnScreen.Log("LoginPanelController- OnLoginButtonClick, email=" + txtEmail.text + "/ pass=" + txtPassword.text);
		}

		//@nam's comment
		//need check blank here
		//need to show loading indicator
		ProfileFirebase.getInstance ().Login (txtEmail.text, txtPassword.text, HandleSigninResult);
	}

	public void OnLoginButtonFBClick()
	{
		//@nam's comment
		//need to show loading indicator
		if (islogin == false) {
			islogin = true;
			var perms = new List<string> (){ "public_profile", "email", "user_friends" };
			FB.LogInWithReadPermissions (perms, AuthFBCallback);
		}
	}

	public void OnLogOutButtonClick()
	{
		//if (GlobalVar.DEBUG)
		DebugOnScreen.Log("ProfilePanelController- OnLogOutButtonClick");
		try
		{
			DebugOnScreen.Log("ProfilePanelController- OnLogOutButtonClick  11111");
			GlobalVar.login = 2;

			islogin = false;

			ProfileFirebase.getInstance().auth.SignOut();
			DebugOnScreen.Log("ProfilePanelController- OnLogOutButtonClick  22222");

			//@nam's comment
			//how to run into this logic
			//check FB.IsLoggedIn (need to test)
			if (islogin == true) {
				FB.LogOut();
			}
			DebugOnScreen.Log("ProfilePanelController- OnLogOutButtonClick  333333");
			SceneManager.LoadScene(GlobalVar.MAINSCENE);
		}
		catch (System.Exception ex)
		{
			DebugOnScreen.Log("ProfilePanelController- OnLogOutButtonClick 444444");
			DebugOnScreen.Log(ex.Message);
			DebugOnScreen.Log(ex.ToString());
		}

	}

	private void AuthFBCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

			ProfileFirebase.getInstance ().LoginWithFaceBook (aToken.TokenString, HandleSigninResult);

		} else {
			islogin = false;
			//@nam's comment
			//need to show alert login failed here or cancelled or something wrong
			Debug.Log("User cancelled login");
		}
	}

	void HandleSigninResult(Task<Firebase.Auth.FirebaseUser> authTask)
	{

		loadingPanel.SetActive(false);
		if (authTask.IsCanceled)
		{
			//if (GlobalVar.DEBUG)
			DebugOnScreen.Log("LoginPanelController- SignIn canceled.");
		}
		else if (authTask.IsFaulted)
		{
			if (GlobalVar.DEBUG)
			{
				//DebugOnScreen.Log("LoginPanelController- Login encountered an error.");
				DebugOnScreen.Log(authTask.Exception.ToString());
			}
			txtError.text = "Your email or password is not correct!";
			txtError.gameObject.SetActive(true);
		}
		else if (authTask.IsCompleted)
		{
			//if (GlobalVar.DEBUG)
			DebugOnScreen.Log("LoginPanelController- Login completed.");

			userInfo = authTask.Result;

			DebugOnScreen.Log("LoginPanelController- Login completed 1111111");
			if (userInfo.Email != GUEST_DEFAULT_EMAIL) {
				DebugOnScreen.Log ("HandleSigninResult-CurrentUser: 111");
				checkInfoUsers(userInfo);
			}

			DebugOnScreen.Log("LoginPanelController- Login completed 3333333333333");
			deactiveLoginPanel();
		}

	}

	//@nam's comment
	//should move this function to ProfileFirebase
	public void checkInfoUsers(FirebaseUser userInfo_) {
		DebugOnScreen.Log("checkInfoUsers- 000000.");

		FirebaseDatabase.DefaultInstance.RootReference.Child(PERMISSION_PRIVATE)
			.Child(userInfo_.UserId).Child(USERSINFO).GetValueAsync().ContinueWith(task => {
				if (task.IsFaulted)
				{
					DebugOnScreen.Log("ERRR firebases connect.");
				}
				else if (task.IsCompleted)
				{
					DataSnapshot snapshot = task.Result;
					DebugOnScreen.Log("checkInfoUsers- 22222222.");

					if (snapshot.Exists) {
						DebugOnScreen.Log("checkInfoUsers- 33333333.");
						if (snapshot.Child("tester").Exists) {
							DebugOnScreen.Log("checkInfoUsers- 33333333 ---- 000000.");
							GlobalVar.tester = snapshot.Child("tester").Value.ToString();
						}
					}
					else {
						GlobalVar.tester = "0";
//						DebugOnScreen.Log("chua luu thong tin user:: " + GlobalVar.tester);
//						DebugOnScreen.Log("RefreshToken :: " + userInfo_.RefreshToken);
//						DebugOnScreen.Log("DisplayName :: " + userInfo_.DisplayName);
//						DebugOnScreen.Log("UserId 0:: " + userInfo_.UserId);
//						DebugOnScreen.Log("email:: " + userInfo_.Email);

						UserInfo _usersInfo = new UserInfo();
						_usersInfo.userID = userInfo_.UserId;
						_usersInfo.username = userInfo_.DisplayName;
						_usersInfo.firebase_token = userInfo_.RefreshToken;
						_usersInfo.email = userInfo_.Email;
						_usersInfo.tester = GlobalVar.tester;

						createNewUser(_usersInfo);
					}
					DebugOnScreen.Log("checkInfoUsers- 44444444.");
				}
				deactiveLoginPanel();
				DebugOnScreen.Log("checkInfoUsers- 555555555.");
			});
	}

	//@nam's comment
	//there is createNewUser function in ProfileFirebase already
	public void createNewUser (UserInfo userInfo) {
		DebugOnScreen.Log("createNewUser loading: " + USERSINFO);
		FirebaseDatabase.DefaultInstance
			.GetReference(PERMISSION_PRIVATE)
			.Child(userInfo.userID)
			.Child(USERSINFO)
			.SetRawJsonValueAsync(JsonUtility.ToJson(userInfo));
		DebugOnScreen.Log("createNewUser Done: " + userInfo.userID);
	}

	public void deactiveLoginPanel()
	{

		DebugOnScreen.Log("deactiveLoginPanel- 111111");
		//uiLogin.SetActive (false);
		GlobalVar.login = 1;

		flowchart.ExecuteBlock("LoginSuccess");
	}


}
#endif
