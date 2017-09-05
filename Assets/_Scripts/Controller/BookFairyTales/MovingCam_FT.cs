using UnityEngine;
using System.Collections;

public class MovingCam_FT : MonoBehaviour {
	
	public Transform startScence;	
    public Transform viewScence;
  
    public float speed = 1F; 

    private bool isStartScence = false;
    private bool isViewScence = false;   

    void Start() {
        //transform.LookAt(GameObject.Find("Book").transform);
	}

	void Update() {   
        if (isViewScence) {
            transform.rotation = Quaternion.Lerp(transform.rotation, viewScence.rotation, Time.deltaTime * speed);
            transform.position = Vector3.Lerp(transform.position, viewScence.position, Time.deltaTime * speed);
        }
        
        if(isStartScence)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, startScence.rotation, Time.deltaTime * speed);
            transform.position = Vector3.Lerp(transform.position, startScence.position, Time.deltaTime * speed);
        }
    }


    public void GoToViewScence()
    {
        isStartScence = false;
        isViewScence = true;
        GetComponent<DraggableCamera>().cancelDraggable();

        StartCoroutine(startDraggable());
    }

    public void RestartViewScence()
    {        
        if(isViewScence) { 
            GetComponent<DraggableCamera>().cancelDraggable();
            speed = speed * 4;
            StartCoroutine(restartDraggable());
        }
    }

    public void GoToStartScence()
    {
        isStartScence = true;
        isViewScence = false;
        GetComponent<DraggableCamera>().cancelDraggable();
    }

    IEnumerator startDraggable()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<DraggableCamera>().startDraggable();
    }

    IEnumerator restartDraggable()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<DraggableCamera>().startDraggable();
        speed = speed / 4;
    } 
}
