using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBookObject : MonoBehaviour {

    #region Serializable Classes
    [System.Serializable]
    public class MoveTo
    {
        public bool isMove = false;
        public Vector3 newPosition = new Vector3(0, 0, 0);
        public float time = 1.0f;
        public float delay = 0f;   
        
        public Vector3 originalPosition { get; set; }
    } 

    [System.Serializable]
    public class RotateTo
    {
        public bool isRotate = false;
        public float angle = 90;
        public float time = 1.0f;
        public float delay = 0f;

        public float changeAngle { get; set; }
        public Vector3 currentAngle { get; set; }
    }

    [System.Serializable]
    public class ScaleTo
    {             
        [Header("Scale main setting")]       
        public bool isScale = false;
        public Vector3 newScale = new Vector3(0, 0, 0);       
        public float time = 1.0f;
        public float delay = 0f;
        [Header("Object will bouncing when scale done")]
        public bool isBouncing = false;
        public float bounceRange = 0f;
        public float bounceTime = 1f;
        [Header("This is need for decrease scale setting when reverse")]
        public bool reverseDelay = false;
        public bool forceToZero = false;
        public Vector3 reverseScale = new Vector3(0, 0, 0);

        public bool isScaleUp { get; set; }
        public int bounceState { get; set; }
        public Vector3 originalScale { get; set; }
    }
    
    [System.Serializable]
    public class Fade
    {
        [Header("Use for sprite")]
        public bool isFade = false;
        public float time = 1.0f;
        public float delay = 0f;
    }

    [System.Serializable]
    public class Dissolve
    {
        [Header("Use for plane with dissolve texture")]
        public bool isDissolve = false;
        public float time = 1.0f;
        public float delay = 0f;
    }
    #endregion

    public bool playTest = false;
    [Header("Only use with static (outpage) content")]
    public bool flipObject = true;

    public MoveTo moveTo = new MoveTo();
    public RotateTo rotateTo = new RotateTo();
    public ScaleTo scaleTo = new ScaleTo();
    public Fade fade = new Fade();
    public Dissolve dissolve = new Dissolve();

    private Coroutine move_coroutine;
    private Coroutine rotate_coroutine;
    private Coroutine scale_coroutine;
    private Coroutine bounce_coroutine;
    private Coroutine fade_coroutine;
    private Coroutine dissolve_coroutine;
    private int state = 0;

    void Start () {
        if (playTest)
            Play();
    }

    void Update()
    {
        if (playTest)
        {
            if (state == 0)
                Play();
            else if (state == 1)
                PlayReverse();

            playTest = false;
        }
    }

    #region Play Function
    public void Play()
    {
        if (state == 0) {
            if (moveTo.isMove)
                Move();
            if (rotateTo.isRotate)
                Rotate();
            if (scaleTo.isScale)
                Scale();
            if (fade.isFade)
                FadeIn();
            if (dissolve.isDissolve)
                DissolveIn();

            state = 1;   
        }
    }

    public void PlayReverse()
    {
        if (state == 1) {
            if (moveTo.isMove)
                ReverseMove();
            if (rotateTo.isRotate)
                ReverseRotate();
            if (scaleTo.isScale)
                ReverseScale();
            if (fade.isFade)
                FadeOut();
            if (dissolve.isDissolve)
                DissolveOut();

            state = 0;
        }
    }

    public void StraightReverse()
    {
        if (state == 1)
        {
            if (moveTo.isMove)
                transform.localPosition = moveTo.originalPosition;
            if (rotateTo.isRotate)
                ;
            if (scaleTo.isScale)
                transform.localScale = scaleTo.reverseScale;
            if (fade.isFade)
                GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0);
            if (dissolve.isDissolve)
                ;

            state = 0;
        }
    }
    #endregion

    #region Move Object Function
    public void Move()
    {
        moveTo.originalPosition = transform.localPosition;

        if (move_coroutine != null)
            StopCoroutine(move_coroutine);
        move_coroutine = StartCoroutine(Move_Coroutine(moveTo.newPosition, true));
    }

    IEnumerator Move_Coroutine(Vector3 newPosition, bool isDelay)
    {
        if (isDelay)
            yield return new WaitForSeconds(moveTo.delay);

        float startTime = Time.time;
        float endTime = startTime + moveTo.time;

        while (Time.time < endTime)
        {
            float timeProgressed = (Time.time - startTime) / moveTo.time;
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, timeProgressed);

            yield return null;
        }
    }

    public void ReverseMove()
    {
        if (move_coroutine != null)
            StopCoroutine(move_coroutine);
        move_coroutine = StartCoroutine(Move_Coroutine(moveTo.originalPosition, false));
    }
    #endregion

    #region Rotate Object Function
    public void Rotate()
    {
        rotateTo.changeAngle = 0;
        if(rotateTo.currentAngle == null)
            rotateTo.currentAngle = new Vector3(transform.localEulerAngles.x, 0, 0);

        if (rotate_coroutine != null)
            StopCoroutine(rotate_coroutine);
        rotate_coroutine = StartCoroutine(Rotate_Coroutine(rotateTo.angle));    
    }

    IEnumerator Rotate_Coroutine(float angle)
    {     
        if (rotateTo.delay != 0)
            yield return new WaitForSeconds(rotateTo.delay);

        Vector3 toRotation = rotateTo.currentAngle + new Vector3(angle, 0, 0);
        
        if (angle > 0)
        {            
            while (rotateTo.changeAngle < angle)
            {
                float angle_change_perframe = Math.Abs(angle) * Time.deltaTime;
                rotateTo.changeAngle = rotateTo.changeAngle + (angle_change_perframe / rotateTo.time);
                if((rotateTo.changeAngle < angle)) { 
                    rotateTo.currentAngle += new Vector3((angle_change_perframe / rotateTo.time), 0, 0);
                    transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                }
                yield return null;
            }
            rotateTo.currentAngle = toRotation;
            transform.localRotation = Quaternion.Euler(toRotation);            
        }
        else
        {            
            while (rotateTo.changeAngle > angle)
            {
                float angle_change_perframe = Math.Abs(angle) * Time.deltaTime;
                rotateTo.changeAngle = rotateTo.changeAngle - (angle_change_perframe / rotateTo.time);
                if ((rotateTo.changeAngle > angle))
                {
                    rotateTo.currentAngle -= new Vector3((angle_change_perframe / rotateTo.time), 0, 0);
                    transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                }
                yield return null;
            }
            rotateTo.currentAngle = toRotation;
            transform.localRotation = Quaternion.Euler(toRotation);
        }     
    }

    IEnumerator ReverseRotate_Coroutine(float angle)
    {
        Vector3 toRotation = rotateTo.currentAngle + new Vector3(angle, 0, 0);
       
        if (angle > 0)
        {           
            while (rotateTo.changeAngle < angle)
            {
                if (Math.Abs(rotateTo.changeAngle) < 40)
                {
                    float angle_change_perframe = Math.Abs(angle) * Time.deltaTime * 0.6f;
                    rotateTo.changeAngle = rotateTo.changeAngle + (angle_change_perframe / rotateTo.time);
                    rotateTo.currentAngle += new Vector3((angle_change_perframe / rotateTo.time), 0, 0);
                    transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                    yield return null;
                }
                else
                {
                    float angle_change_perframe = Math.Abs(angle) * Time.deltaTime * 2f;
                    rotateTo.changeAngle = rotateTo.changeAngle + (angle_change_perframe / rotateTo.time);
                    if (rotateTo.changeAngle < angle)
                    {
                        rotateTo.currentAngle += new Vector3((angle_change_perframe / rotateTo.time), 0, 0);
                        transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                    }
                    yield return null;
                }
            }
            rotateTo.currentAngle = toRotation;
            transform.localRotation = Quaternion.Euler(toRotation);
            if (scale_coroutine != null)
                StopCoroutine(scale_coroutine);
            transform.localScale = new Vector3(0, 0, 0);
        }

        else
        {           
            while (rotateTo.changeAngle > angle)
            {
                if (Math.Abs(rotateTo.changeAngle) < 40)
                {
                    float angle_change_perframe = Math.Abs(angle) * Time.deltaTime * 0.6f;
                    rotateTo.changeAngle = rotateTo.changeAngle - (angle_change_perframe / rotateTo.time);
                    rotateTo.currentAngle -= new Vector3((angle_change_perframe / rotateTo.time), 0, 0);
                    transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                    yield return null;
                }
                else
                {
                    float angle_change_perframe = Math.Abs(angle) * Time.deltaTime * 1.5f;
                    rotateTo.changeAngle = rotateTo.changeAngle - (angle_change_perframe / rotateTo.time);
                    if (rotateTo.changeAngle > angle)
                    {
                        rotateTo.currentAngle -= new Vector3((angle_change_perframe / rotateTo.time), 0, 0);
                        transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                    }
                    yield return null;
                }
            }
            rotateTo.currentAngle = toRotation;
            transform.localRotation = Quaternion.Euler(toRotation);
            if (scale_coroutine != null)
                StopCoroutine(scale_coroutine);
            transform.localScale = new Vector3(0, 0, 0);
        }      
    }

    public void ReverseRotate()
    {
        rotateTo.changeAngle = 0;
        if (rotateTo.currentAngle == null)
            rotateTo.currentAngle = new Vector3(transform.localEulerAngles.x, 0, 0);

        if (rotate_coroutine != null)
            StopCoroutine(rotate_coroutine);
        rotate_coroutine = StartCoroutine(ReverseRotate_Coroutine(-rotateTo.angle));      
    }   
    #endregion

    #region Scale Object Function
    public void Scale()
    {
        scaleTo.originalScale = transform.localScale;

        if (transform.localScale.y >= scaleTo.newScale.y)
            scaleTo.isScaleUp = false;
        else
            scaleTo.isScaleUp = true;

        if (scale_coroutine != null)
            StopCoroutine(scale_coroutine);
        scale_coroutine = StartCoroutine(Scale_Coroutine(scaleTo.newScale, true));
    }

    public void Bounce()
    {
        if (scaleTo.isBouncing)
        {
            scaleTo.bounceState = 1;
            if (bounce_coroutine != null) { 
                StopCoroutine(bounce_coroutine);
                transform.localScale = scaleTo.newScale;
            }
            bounce_coroutine = StartCoroutine(Bouncing(transform.localScale));
        }
    }

    IEnumerator Scale_Coroutine(Vector3 newScale, bool isDelay)
    {       
        if (isDelay)
            yield return new WaitForSeconds(scaleTo.delay);
       
        float endTime = Time.time + scaleTo.time;
        float deltaX = newScale.x - transform.localScale.x;
        float deltaY = newScale.y - transform.localScale.y;
        float deltaZ = newScale.z - transform.localScale.z;
        float velocityX = 0;
        float velocityY = 0;
        float velocityZ = 0;

        while (Time.time < endTime)
        {
            Vector3 currentScale = transform.localScale;
            velocityX += deltaX * Time.deltaTime / scaleTo.time;
            velocityY += deltaY * Time.deltaTime / scaleTo.time;
            velocityZ += deltaZ * Time.deltaTime / scaleTo.time;
            if(Math.Abs(velocityX) <= Math.Abs(deltaX) && Math.Abs(velocityY) <= Math.Abs(deltaY) && Math.Abs(velocityZ) <= Math.Abs(deltaZ)) { 
                currentScale += new Vector3(deltaX * Time.deltaTime / scaleTo.time, deltaY * Time.deltaTime / scaleTo.time, deltaZ * Time.deltaTime / scaleTo.time);
                transform.localScale = currentScale;
            }
            yield return null;
        }

        transform.localScale = newScale;

        if (!scaleTo.isScaleUp && scaleTo.forceToZero)
            transform.localScale = new Vector3(0, 0, 0);

        if (scaleTo.isScaleUp && scaleTo.isBouncing)
        {
            scaleTo.bounceState = 1;
            StartCoroutine(Bouncing(newScale));
        }      
    }

    IEnumerator Bouncing(Vector3 aroundVector)
    {       
        if (scaleTo.bounceState == 1)
        {
            float deltaY = scaleTo.bounceRange;

            while (transform.localScale.y < aroundVector.y + scaleTo.bounceRange)
            {               
                Vector3 currentScale = transform.localScale;
                currentScale += new Vector3(0, deltaY * Time.deltaTime / scaleTo.bounceTime, 0);
                transform.localScale = currentScale;
                yield return null;
            }
            scaleTo.bounceState++;          
        }

        if (scaleTo.bounceState == 2)
        {
            float deltaY = scaleTo.bounceRange * 2;

            while (transform.localScale.y > aroundVector.y - scaleTo.bounceRange)
            {                
                Vector3 currentScale = transform.localScale;
                currentScale -= new Vector3(0, deltaY * Time.deltaTime / scaleTo.bounceTime, 0);
                transform.localScale = currentScale;
                yield return null;
            }
            scaleTo.bounceState++;          
        }

        if (scaleTo.bounceState == 3)
        {
            float deltaY = scaleTo.bounceRange;

            while (transform.localScale.y < aroundVector.y)
            {               
                Vector3 currentScale = transform.localScale;
                if (currentScale.y < aroundVector.y)
                {
                    currentScale += new Vector3(0, deltaY * Time.deltaTime / scaleTo.bounceTime, 0);
                    transform.localScale = currentScale;
                }
                yield return null;
            }
            transform.localScale = aroundVector;
        }
    }

    public void ReverseScale()
    {
        if (transform.localScale.x >= scaleTo.reverseScale.x)
            scaleTo.isScaleUp = false;
        else
            scaleTo.isScaleUp = true;

        if (scale_coroutine != null)
            StopCoroutine(scale_coroutine);
        scale_coroutine = StartCoroutine(Scale_Coroutine(scaleTo.reverseScale, scaleTo.reverseDelay));      
    }
    #endregion

    #region Fade Object Function
    public void FadeIn()
    {
        if (fade_coroutine != null)
            StopCoroutine(fade_coroutine);

        fade_coroutine = StartCoroutine(Fade_Coroutine(true));
    }

    public IEnumerator Fade_Coroutine(bool isFadeIn)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (isFadeIn)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
            if (fade.delay != 0)
                yield return new WaitForSeconds(fade.delay);

            while (sprite.color.a < 1.0f)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a + (Time.deltaTime / fade.time));
                yield return null;
            }
        }
        else
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);

            while (sprite.color.a > 0.0f)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - (Time.deltaTime / fade.time * 3));
                yield return null;
            }
        }
    }

    public void FadeOut()
    {
        if (fade_coroutine != null)
            StopCoroutine(fade_coroutine);

        fade_coroutine = StartCoroutine(Fade_Coroutine(false));
    }
    #endregion

    #region Dissolve Object Function
    public void DissolveIn()
    {
        if (dissolve_coroutine != null)
            StopCoroutine(dissolve_coroutine);

        dissolve_coroutine = StartCoroutine(Dissolve_Coroutine(true));
    }

    public IEnumerator Dissolve_Coroutine(bool isIn)
    {       
        if (isIn)
        {
            if (dissolve.delay != 0)
                yield return new WaitForSeconds(dissolve.delay);

            float dissolveLevel = 1f;
            GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel);     
                 
            while (dissolveLevel > 0)
            {
                dissolveLevel -= Time.deltaTime / dissolve.time;
                GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel);
                yield return null;
            }
        }

        else
        {
            float dissolveLevel = 0f;
            GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel);

            while (dissolveLevel < 1)
            {
                dissolveLevel += Time.deltaTime / dissolve.time;
                GetComponent<Renderer>().material.SetFloat("_Level", dissolveLevel);
                yield return null;
            }
        }
    }

    public void DissolveOut()
    {
        if (dissolve_coroutine != null)
            StopCoroutine(dissolve_coroutine);

        dissolve_coroutine = StartCoroutine(Dissolve_Coroutine(false));
    }
    #endregion
}
