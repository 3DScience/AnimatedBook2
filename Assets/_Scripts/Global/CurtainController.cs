using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainController : MonoBehaviour {

    static GameObject Curtain;
    static GameObject curtain_object;

    public bool GetFirstLoad()
    {
        Curtain = GameObject.Find("Curtain");
        if (Curtain != null)
            curtain_object = Curtain.transform.Find("object").gameObject;

        if (curtain_object != null)       
            return curtain_object.GetComponent<CoveredCurtain>().isFirstLoad;       
        else
            return true;
    }

    public void CoverCurtain()
    {
        Curtain = GameObject.Find("Curtain");
        if (Curtain != null)
            curtain_object = Curtain.transform.Find("object").gameObject;

        if (curtain_object != null)
        {
            curtain_object.SetActive(true);
            curtain_object.GetComponent<CoveredCurtain>().Cover();
        }       
    }
	
	public void HideCurtain()
    {
        Curtain = GameObject.Find("Curtain");
        if (Curtain != null)
            curtain_object = Curtain.transform.Find("object").gameObject;

        if (curtain_object != null)
        {           
            curtain_object.SetActive(true);
            curtain_object.GetComponent<CoveredCurtain>().Hide();
        }        
    }
}
