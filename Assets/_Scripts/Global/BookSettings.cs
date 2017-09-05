using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSettings {

    private static readonly string GRAPHIC_SETTING = "PlayerSettings_Graphic";
    private static readonly string AUTOPLAY_SETTING = "PlayerSettings_Autoplay";
    private static readonly string AUDIO_SETTING = "PlayerSettings_Audio";
    private static readonly string VOICE_SETTING = "PlayerSettings_Voice";

    public static readonly int HIGH_QUALITY = 1;
    public static readonly int LOW_QUALITY = 0;
    public static readonly int AUTOPLAY_ON = 1;
    public static readonly int AUTOPLAY_OFF = 0;
    public static readonly int AUDIO_ON = 1;
    public static readonly int AUDIO_OFF = 0;
    public static readonly int VOICE_ON = 1;
    public static readonly int VOICE_OFF = 0;

    public int graphic { get; set; }
    public int autoplay { get; set; }
    public int audio { get; set; }
    public int voice { get; set; }


    public void saveSetting()
    {
        PlayerPrefs.SetInt(GRAPHIC_SETTING, graphic);
        PlayerPrefs.SetInt(AUTOPLAY_SETTING, autoplay);
        PlayerPrefs.SetInt(AUDIO_SETTING, audio);
        PlayerPrefs.SetInt(VOICE_SETTING, voice);
    }

    public void loadSetting()
    {
        if (PlayerPrefs.HasKey(GRAPHIC_SETTING))
            graphic = PlayerPrefs.GetInt(GRAPHIC_SETTING);
        else
            graphic = -1;

        if (PlayerPrefs.HasKey(AUTOPLAY_SETTING))
            autoplay = PlayerPrefs.GetInt(AUTOPLAY_SETTING);
        else
            autoplay = AUTOPLAY_ON;

        if (PlayerPrefs.HasKey(AUDIO_SETTING))
            audio = PlayerPrefs.GetInt(AUDIO_SETTING);
        else       
            audio = AUDIO_ON;

        if (PlayerPrefs.HasKey(VOICE_SETTING))
            voice = PlayerPrefs.GetInt(VOICE_SETTING);
        else
            voice = VOICE_ON;
    }

}
