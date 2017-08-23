using UnityEngine;
using System.Collections;

public class MovingCam_FT : MonoBehaviour {
	
	public Transform startScence;
	//public Transform endScenceMarker;
    //public Transform startStoryTellScenceMarker;
    public Transform viewScence;
  
    public float speed = 1F;

    //private float startTime;
	//private float journeyLength;

    private bool isStartScence = false;
    private bool isStoryTellScence = false;   

    void Start() {
        //transform.LookAt(GameObject.Find("Book").transform);
	}

	void Update() {
        //if(isStartScence)   
        //    Move(startScence, endScenceMarker);

        if (isStoryTellScence) {
            transform.rotation = Quaternion.Lerp(transform.rotation, viewScence.rotation, Time.deltaTime * speed);
            transform.position = Vector3.Lerp(transform.position, viewScence.position, Time.deltaTime * speed);
        }
        
        if(isStartScence)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, startScence.rotation, Time.deltaTime * speed);
            transform.position = Vector3.Lerp(transform.position, startScence.position, Time.deltaTime * speed);
        }
    }

    //public void StartScence()
    //{
    //    startTime = Time.time;
    //    journeyLength = Vector3.Distance(startScence.position, endScenceMarker.position);
    //    isStartScence = true;
    //}

    public void GoToViewScence()
    {
        isStartScence = false;
        isStoryTellScence = true;       
    }

    public void GoToStartScence()
    {
        isStartScence = true;
        isStoryTellScence = false;
    }

    //void Move(Transform startMarker, Transform endMarker)
    //{
    //    if(startMarker != null && endMarker != null) {
    //        float distCovered = (Time.time - startTime) * phase1_speed;
    //        float fracJourney = distCovered / journeyLength;
    //        transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
    //    }
    //}

    //void Rotate() {
    //    Quaternion target = Quaternion.Euler(0, 0, 0);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    //}
}
