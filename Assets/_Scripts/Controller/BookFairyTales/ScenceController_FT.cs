using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenceController_FT : MonoBehaviour {

    public Camera camera;
    public Light light;
    public GameObject book;
    public GameObject GUI;
    public Flowchart dialog;

    // Use this for initialization
    void Start () {
        GUI.SetActive(false);

        //Move camera
        camera.GetComponent<MovingCam_FT>().StartScence();

        //Start the light
        light.intensity = 0f;
        InvokeRepeating("StartScenceLight", 0f, 0.015f);
    }
	
	// Update is called once per frame
	void Update () {

    }

    void StartScenceLight()
    {
        if (light.intensity < 5.7)
            light.intensity = light.intensity + 0.035f;
        else {
            CancelInvoke();

            book.GetComponent<BookController_FT>().open();
            InvokeRepeating("BookOpenScenceLight", 0f, 0.015f);                   
        }
    }

    void BookOpenScenceLight()
    {
        if (light.spotAngle > 70)
            light.spotAngle = light.spotAngle - 0.15f;
        else
        {
            light.spotAngle = 70;         
            CancelInvoke();
            GUI.SetActive(true);
            dialog.ExecuteBlock("StartScence");
            //Debug.Log("Book is opened");
        }
    }

    public void StartStoryTellScence()
    {
        light.intensity = 4.5f;
        camera.GetComponent<MovingCam_FT>().StoryTellScence();
    }
}
