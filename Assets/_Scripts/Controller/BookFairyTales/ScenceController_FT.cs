using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenceController_FT : MonoBehaviour {

    public Camera Camera;
    public Light Light;
    public GameObject Book;
    public GameObject GUI;
    public Flowchart Dialog;

    public GameObject Curtain;
    public GameObject SettingButton;
    public GameObject NavigateButton;
    public GameObject TabToOpenText;

    private bool isLightUp = false;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        QualitySettings.blendWeights = BlendWeights.FourBones;
    }

    // Use this for initialization
    void Start () {        
        GUI.SetActive(true);
        Curtain.GetComponent<GuiImageFaded>().onFadeDone = onCurtainDown;
        Curtain.GetComponent<GuiImageFaded>().PlayFade();

        //Start the light
        Light.intensity = 0f;
        InvokeRepeating("StartScenceLight", 0f, 0.015f);

        if (Camera.main.aspect >= 1.6)
        {
            //Debug.Log("16:9");
            Camera.fieldOfView = 56;
        }       
        else
        {
            //Debug.Log("4:3");
            Camera.fieldOfView = 66;
        }
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void onScreenTouch()
    {
        if(isLightUp) { 
            Book.GetComponent<BookController_FT>().open();
            TabToOpenText.SetActive(false);
            TabToOpenText.GetComponent<GuiTextFaded>().CancelFade();
        }
    }

    void onCurtainDown(bool b)
    {
        SettingButton.SetActive(true);
        NavigateButton.SetActive(true);
        TabToOpenText.SetActive(true);
        TabToOpenText.GetComponent<GuiTextFaded>().PlayFade();
        isLightUp = true;
    }

    void StartScenceLight()
    {
        Light.intensity = 5.5f;
        CancelInvoke();
        InvokeRepeating("FocusTheLight", 0f, 0.015f);

        //if (Light.intensity < 5.7)
        //    Light.intensity = Light.intensity + 0.035f;
        //else {
        //    CancelInvoke();           
        //    InvokeRepeating("FocusTheLight", 0f, 0.015f);                   
        //}
    }

    void FocusTheLight()
    {
        Light.spotAngle = 95;
        CancelInvoke();

        //if (Light.spotAngle > 90)
        //    Light.spotAngle = Light.spotAngle - 0.15f;
        //else
        //{
        //    Light.spotAngle = 90;         
        //    CancelInvoke();            
        //}
    }

    public void StartViewScence()
    {
        Light.intensity = 4.5f;
        Camera.GetComponent<MovingCam_FT>().GoToViewScence();
    }

    public void RestartScence()
    {
        Light.intensity = 5.5f;
        Camera.GetComponent<MovingCam_FT>().GoToStartScence();
        StartCoroutine(reactiveTabToOpenText());
    }

    IEnumerator reactiveTabToOpenText()
    {
        yield return new WaitForSeconds(1.5f);
        TabToOpenText.SetActive(true);
        TabToOpenText.GetComponent<GuiTextFaded>().PlayFade();
    }
}
