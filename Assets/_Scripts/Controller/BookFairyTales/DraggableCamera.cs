using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse drag Orbit with zoom")]
public class DraggableCamera : MonoBehaviour
{   
    public Transform focusPoint;    
    public float distance = 0f;

    #region Drag Setting
    [Header("Drag setting")]
    public bool Draggable = false;
    public float xSpeed = 6.0f;
    public float ySpeed = 5.0f;
    public float xMinLimit = -90f;
    public float xMaxLimit = 90f;
    public float yMinLimit = -90f;
    public float yMaxLimit = 90f;
    public float smoothTime = 5f;
    #endregion

    #region Zoom Setting
    [Header("Zoom setting")]
    public bool Zoomable = false;
    public float zoomSpeed = 10f;
    public float distanceMin = .5f;
    public float distanceMax = 15f; 
    #endregion

    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;
    float velocityX = 0.0f;
    float velocityY = 0.0f; 

   
    void Start()
    {
        //Vector3 angles = transform.eulerAngles;
        //rotationYAxis = angles.y;
        //rotationXAxis = angles.x;

        // Make the rigid body not change rotation
        //Screen.lockCursor = true;

        //distance = Vector3.Distance(focusPoint.position, transform.position);
    }

    void Update()
    {
        //distance = Vector3.Distance(focusPoint.position, transform.position);        
    }

    void LateUpdate()
    {
        //Use navigator key to change camera distance
        if(Zoomable) { 
            transform.position += transform.forward * zoomSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += transform.right * zoomSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
            distance = Vector3.Distance(focusPoint.position, transform.position);
        }

        if (Draggable)
        {
#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                velocityX += xSpeed * Input.GetAxis("Mouse X") * 0.02f;
                velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
            }
#endif
#if UNITY_EDITOR || UNITY_WEBGL
            if (Input.GetMouseButton(0))
            {
                velocityX += xSpeed * Input.GetAxis("Mouse X") * 0.02f;
                velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
            }
#endif
            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;

            rotationXAxis = ClampAngle(rotationXAxis, xMinLimit, xMaxLimit);
            rotationYAxis = ClampAngle(rotationYAxis, yMinLimit, yMaxLimit);

            Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            //distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
            //RaycastHit hit;
            //if (Physics.Linecast(target.position, transform.position, out hit))
            //{
            //    distance -= hit.distance;
            //}

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + focusPoint.position;

            transform.rotation = rotation;
            transform.position = position;
            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
        }
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    public void startDraggable()
    {
        Draggable = true;
        Vector3 angles = transform.eulerAngles;

        rotationYAxis = 0;
        rotationXAxis = 0;
        velocityX = 0;
        velocityY = 0;

        distance = Vector3.Distance(focusPoint.position, transform.position);       
    }

    public void cancelDraggable()
    {
        Draggable = false;        
    }
}