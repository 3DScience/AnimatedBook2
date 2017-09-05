using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenceController_FT : MonoBehaviour {

    [Header ("Game objects")]
    public Camera Camera;
    public Light Light;
    public GameObject Book;
    public GameObject Table;
    public GameObject UIPanel;
    public AudioSource BackgroundMusic;
    public Flowchart FungusFlowchart;

    [Header("Common ui")]
    public GameObject Curtain;
    public GameObject SettingButton;
    public GameObject NavigateButton;
    public GameObject TabToOpenText;

    [Header("Setting toogle")]
    public Toggle GraphicHighToggle;
    public Toggle GraphicLowToggle;
    public Toggle AutoPlayOnToggle;
    public Toggle AutoPlayOffToggle;
    public Toggle AudioOnToggle;
    public Toggle AudioOffToggle;
    public Toggle VoiceOnToggle;
    public Toggle VoiceOffToggle;

    private bool isLightUp = false;
    public BookSettings settings;

    public static readonly string LOW_GRAPHIC_SHADER = "Mobile/VertexLit";
    public static readonly string HIGH_GRAPHIC_SHADER = "Mobile/VertexLit";

    private void Awake()
    {
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        QualitySettings.blendWeights = BlendWeights.FourBones;
     
        loadBookSetting();

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

    // Use this for initialization
    void Start () {        
        UIPanel.SetActive(true);
        Curtain.GetComponent<GuiImageFaded>().onFadeDone = onCurtainDown;
        Curtain.GetComponent<GuiImageFaded>().PlayFade();

        //Start the light
        Light.intensity = 5.5f;
        Light.spotAngle = 95;              
    }
		
	void Update () {

    }

    public void onScreenTouch()
    {
        if(isLightUp) { 
            Book.GetComponent<BookController_FT>().open();          
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

    public void changeLight(float speed, float intensity)
    {
        if(Light.intensity != intensity)
            StartCoroutine(changeLight_Coroutine(speed, intensity));
    }

    IEnumerator changeLight_Coroutine(float speed, float intensity)
    {
        if (Light.intensity < intensity)
        {          
            while (Light.intensity < intensity)
            {
                Light.intensity += Time.deltaTime * speed;
                yield return null;
            }
            yield break;
        }
        else if (Light.intensity > intensity)
        {
            while (Light.intensity > intensity)
            {
                Light.intensity -= Time.deltaTime * speed;
                yield return null;
            }
            yield break;
        }
    }

    #region Setting Config
    void loadBookSetting()
    {
        settings = new BookSettings();
        settings.loadSetting();

        //Graphic setting
        if(settings.graphic == BookSettings.HIGH_QUALITY)
        {
            SetHighGraphic();
        }
        else if(settings.graphic == BookSettings.LOW_QUALITY)
        {
            SetLowGraphic();
        }
        else
        {
            int qualityLevel = QualitySettings.GetQualityLevel();
            if (qualityLevel <= 1)
                SetLowGraphic();
            else
                SetHighGraphic();
        }

        //Autoplay setting
        if (settings.autoplay == BookSettings.AUTOPLAY_ON)
        {
            SetAutoplayOn();
        }
        else
        {
            SetAutoplayOff();
        }

        //Audio setting
        if (settings.audio == BookSettings.AUDIO_ON)
        {
            TurnAudioOnOff(true);
        }
        else
        {
            TurnAudioOnOff(false);
        }

        //Voice setting
        if (settings.voice == BookSettings.VOICE_ON)
        {
            TurnVoiceOnOff(true);
        }
        else
        {
            TurnVoiceOnOff(false);
        }
    }

    public void SetHighGraphic()
    {
        GraphicLowToggle.isOn = false;
        GraphicHighToggle.isOn = true;       
        Table.GetComponent<Renderer>().material.shader = Shader.Find(HIGH_GRAPHIC_SHADER);
        Transform shadow = Book.transform.Find("BookShadow");
        if (shadow != null)
            shadow.gameObject.SetActive(true);
        settings.graphic = BookSettings.HIGH_QUALITY;
        settings.saveSetting();
    }

    public void SetLowGraphic()
    {
        GraphicHighToggle.isOn = false;
        GraphicLowToggle.isOn = true;
        Table.GetComponent<Renderer>().material.shader = Shader.Find(LOW_GRAPHIC_SHADER);
        Transform shadow =  Book.transform.Find("BookShadow");
        if (shadow != null)
            shadow.gameObject.SetActive(true);
        settings.graphic = BookSettings.LOW_QUALITY;
        settings.saveSetting();
    }

    public void SetAutoplayOn()
    {
        AutoPlayOffToggle.isOn = false;
        AutoPlayOnToggle.isOn = true;
        FungusFlowchart.SetIntegerVariable("autoplay", 1);
        settings.autoplay = BookSettings.AUTOPLAY_ON;
        settings.saveSetting();
    }

    public void SetAutoplayOff()
    {
        AutoPlayOffToggle.isOn = true;
        AutoPlayOnToggle.isOn = false;
        FungusFlowchart.SetIntegerVariable("autoplay", 0);
        settings.autoplay = BookSettings.AUTOPLAY_OFF;
        settings.saveSetting();
    }

    public void TurnAudioOnOff(bool isOn)
    {
        if(isOn)
        {
            AudioOffToggle.isOn = false;
            AudioOnToggle.isOn = true;
            BackgroundMusic.Play();
            settings.audio = BookSettings.AUDIO_ON;
            settings.saveSetting();
            return;
        }
        else
        {
            AudioOffToggle.isOn = true;
            AudioOnToggle.isOn = false;
            BackgroundMusic.Stop();
            settings.audio = BookSettings.AUDIO_OFF;
            settings.saveSetting();
        }
    }

    public void TurnVoiceOnOff(bool isOn)
    {
        if (isOn)
        {
            VoiceOffToggle.isOn = false;
            VoiceOnToggle.isOn = true;
            FungusFlowchart.SetIntegerVariable("voice", 1);
            settings.voice = BookSettings.VOICE_ON;
            settings.saveSetting();
            return;
        }
        else
        {
            VoiceOffToggle.isOn = true;
            VoiceOnToggle.isOn = false;
            FungusFlowchart.SetIntegerVariable("voice", 0);
            settings.voice = BookSettings.VOICE_OFF;
            settings.saveSetting();
        }
    }
    #endregion
}
