using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnStartUp : MonoBehaviour {

    private static bool isStartUp = true;

    void Awake()
    {
        if(isStartUp) { 
            DontDestroyOnLoad(transform.gameObject);
            isStartUp = false;
        }
    }
}
