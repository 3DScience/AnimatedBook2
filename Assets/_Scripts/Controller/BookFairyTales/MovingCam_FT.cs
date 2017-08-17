using UnityEngine;
using System.Collections;

public class MovingCam_FT : MonoBehaviour {
	
	public Transform startScenceMarker;
	public Transform endScenceMarker;

    public Transform startStoryTellScenceMarker;
    public Transform endStoryTellScenceMarker;

    public float phase1_speed = 0.6F;
    public float phase2_speed = 1F;

    private float startTime;
	private float journeyLength;

    private bool isStartScence = false;
    private bool isStoryTellScence = false;

    void Start() {
        //transform.LookAt(GameObject.Find("Book").transform);
	}

	void Update() {
        if(isStartScence)   
            Move(startScenceMarker, endScenceMarker);

        if (isStoryTellScence) {
            transform.rotation = Quaternion.Lerp(transform.rotation, endStoryTellScenceMarker.rotation, Time.deltaTime * phase2_speed);
            transform.position = Vector3.Lerp(transform.position, endStoryTellScenceMarker.position, Time.deltaTime * phase2_speed);
        }     
    }

    public void StartScence()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startScenceMarker.position, endScenceMarker.position);
        isStartScence = true;
    }

    public void StoryTellScence()
    {
        isStartScence = false;
        isStoryTellScence = true;       
    }

    void Move(Transform startMarker, Transform endMarker)
    {
        if(startMarker != null && endMarker != null) {
            float distCovered = (Time.time - startTime) * phase1_speed;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
        }
    }

    void Rotate() {
        Quaternion target = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * phase2_speed);
    }
}
