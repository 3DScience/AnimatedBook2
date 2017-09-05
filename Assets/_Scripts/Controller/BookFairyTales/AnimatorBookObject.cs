using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBookObject : MonoBehaviour {

    #region Classes
    [System.Serializable]
    public class MoveTo
    {
        public bool isMove = false;
        public Vector3 newPosition = new Vector3(0, 0, 0);
        public float speed = 1.0f;
        public float delay = 0;   
        
        public Vector3 originalPosition { get; set; }
    } 

    [System.Serializable]
    public class RotateTo
    {
        public bool isRotate = false;
        public float angle = 90;
        public float speed = 1.0f;
        public float delay = 0;

        public float changeAngle { get; set; }
        public Vector3 currentAngle { get; set; }
    }

    [System.Serializable]
    public class ScaleTo
    {             
        [Header("Scale main setting")]       
        public bool isScale = false;
        public Vector3 newScale = new Vector3(0, 0, 0);       
        public float speed = 1.0f;
        public float delay = 0;
        [Header("Object will bouncing when scale done")]
        public bool isBouncing = false;
        public float bounceRange = 0.2f;
        [Header("This is need for decrease scale setting when reverse")]
        public bool reverseDelay = true;
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
        public float speed = 1.0f;
        public float delay = 0;
    }

    [System.Serializable]
    public class Dissolve
    {
        [Header("Use for plane with dissolve texture")]
        public bool isDissolve = false;
        public float speed = 1.0f;
        public float delay = 0;
    }
    #endregion

    public bool playTest = false;
    public bool flipObject = true;

    public MoveTo moveTo = new MoveTo();
    public RotateTo rotateTo = new RotateTo();
    public ScaleTo scaleTo = new ScaleTo();
    public Fade fade = new Fade();
    public Dissolve dissolve = new Dissolve();

    private Coroutine move_coroutine;
    private Coroutine rotate_coroutine;
    private Coroutine scale_coroutine;   
    private Coroutine fade_coroutine;
    private Coroutine dissolve_coroutine;

    void Start () {
        if (playTest)
            Play();
    }

    void Update()
    {
        if (playTest)
        {
            Play();
            playTest = false;
        }
    }

    #region Play Function
    public void Play()
    {
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
    }

    public void PlayReverse()
    {
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
    }   
    #endregion

    #region Move Object Function
    public void Move()
    {
        moveTo.originalPosition = transform.localPosition;

        if (move_coroutine != null)
            StopCoroutine(move_coroutine);
        move_coroutine = StartCoroutine(Move_Coroutine(moveTo.newPosition));
    }

    IEnumerator Move_Coroutine(Vector3 newPosition)
    {
        if (moveTo.delay != 0)
            yield return new WaitForSeconds(moveTo.delay);

        float startTime = Time.time;
        float endTime = startTime + moveTo.speed;

        while (Time.time < endTime)
        {
            float timeProgressed = (Time.time - startTime) / moveTo.speed;
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, timeProgressed);

            yield return new WaitForFixedUpdate();
        }
    }

    public void ReverseMove()
    {
        if (move_coroutine != null)
            StopCoroutine(move_coroutine);
        move_coroutine = StartCoroutine(Move_Coroutine(moveTo.originalPosition));
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
       
        if (angle > 0)
        {
            while (rotateTo.changeAngle < angle)
            {
                rotateTo.changeAngle = rotateTo.changeAngle + (1 * rotateTo.speed);
                rotateTo.currentAngle += new Vector3((1 * rotateTo.speed), 0, 0);
                transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                yield return null;
            }
            yield break;
        }
        else
        {
            while (rotateTo.changeAngle > angle)
            {
                rotateTo.changeAngle = rotateTo.changeAngle - (1 * rotateTo.speed);
                rotateTo.currentAngle -= new Vector3((1 * rotateTo.speed), 0, 0);
                transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                yield return null;
            }
            yield break;
        }     
    }

    IEnumerator ReverseRotate_Coroutine(float angle)
    {                   
        if (angle > 0)
        {
            while (rotateTo.changeAngle < angle)
            {
                if (Math.Abs(rotateTo.changeAngle) < 40)
                {
                    rotateTo.changeAngle = rotateTo.changeAngle + (0.6f * rotateTo.speed);
                    rotateTo.currentAngle += new Vector3((0.6f * rotateTo.speed), 0, 0);
                    transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                    yield return null;
                }
                else
                {
                    rotateTo.changeAngle = rotateTo.changeAngle + (2f * rotateTo.speed);
                    if(rotateTo.changeAngle > angle) {
                        float x = (2f * rotateTo.speed) - (rotateTo.changeAngle - angle);
                        rotateTo.currentAngle += new Vector3(x, 0, 0);
                    }
                    else { 
                        rotateTo.currentAngle += new Vector3((2f * rotateTo.speed), 0, 0);
                    }
                    transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                    yield return null;
                }
            }
            transform.localScale = new Vector3(0, 0, 0);
            yield break;
        }
        else
        {
            while (rotateTo.changeAngle > angle)
            {
                if (Math.Abs(rotateTo.changeAngle) < 40)
                {
                    rotateTo.changeAngle = rotateTo.changeAngle - (0.6f * rotateTo.speed);
                    rotateTo.currentAngle -= new Vector3((0.6f * rotateTo.speed), 0, 0);
                    transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                    yield return null;
                }
                else
                {
                    rotateTo.changeAngle = rotateTo.changeAngle - (2f * rotateTo.speed);
                    if (rotateTo.changeAngle < angle)
                    {
                        float x = (2f * rotateTo.speed) - Math.Abs(rotateTo.changeAngle - angle);
                        rotateTo.currentAngle -= new Vector3(x, 0, 0);
                    }
                    else { 
                        rotateTo.currentAngle -= new Vector3((2f * rotateTo.speed), 0, 0);
                    }
                    transform.localRotation = Quaternion.Euler(rotateTo.currentAngle);
                    yield return null;
                }
            }
            transform.localScale = new Vector3(0, 0, 0);
            yield break;
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

        if (transform.localScale.x >= scaleTo.newScale.x)
            scaleTo.isScaleUp = false;
        else
            scaleTo.isScaleUp = true;

        if (scale_coroutine != null)
            StopCoroutine(scale_coroutine);
        scale_coroutine = StartCoroutine(Scale_Coroutine(scaleTo.newScale, true));
    }

    IEnumerator Scale_Coroutine(Vector3 newScale, bool isDelay)
    {       
        if (scaleTo.delay != 0 && isDelay)
            yield return new WaitForSeconds(scaleTo.delay);

        if (scaleTo.isScaleUp)
            while (transform.localScale.x < newScale.x)
            {
                float constant_scale = 0.1f;
                Vector3 currentScale = transform.localScale;
                currentScale += new Vector3(constant_scale * scaleTo.speed, constant_scale * scaleTo.speed, constant_scale * scaleTo.speed);
                transform.localScale = currentScale;
                yield return null;
            }

        else       
            while (transform.localScale.x > newScale.x)
            {
                float constant_scale = 0.1f;
                Vector3 currentScale = transform.localScale;
                currentScale -= new Vector3(constant_scale * scaleTo.speed, constant_scale * scaleTo.speed, constant_scale * scaleTo.speed);
                transform.localScale = currentScale;
                yield return null;
            }        

        if (scaleTo.isScaleUp && scaleTo.isBouncing)
        {
            scaleTo.bounceState = 1;
            StartCoroutine(Bouncing(newScale));
        }

        yield break;
    }

    IEnumerator Bouncing(Vector3 aroundVector)
    {       
        if (scaleTo.bounceState == 1)
        {
            while (transform.localScale.y < aroundVector.y + scaleTo.bounceRange)
            {
                float constant_bounce = 0.1f;
                Vector3 currentScale = transform.localScale;
                currentScale += new Vector3(0, constant_bounce * scaleTo.speed, 0);
                transform.localScale = currentScale;
                yield return null;
            }
            scaleTo.bounceState++;
            yield return null;
        }
        if (scaleTo.bounceState == 2)
        {
            while (transform.localScale.y > aroundVector.y - scaleTo.bounceRange)
            {
                float constant_bounce = 0.1f;
                Vector3 currentScale = transform.localScale;
                currentScale -= new Vector3(0, constant_bounce * scaleTo.speed, 0);
                transform.localScale = currentScale;
                yield return null;
            }
            scaleTo.bounceState++;
            yield return null;
        }
        if (scaleTo.bounceState == 3)
        {
            while (transform.localScale.y < aroundVector.y)
            {
                float constant_bounce = 0.1f;
                Vector3 currentScale = transform.localScale;
                currentScale += new Vector3(0, constant_bounce * scaleTo.speed, 0);
                transform.localScale = currentScale;
                yield return null;
            }
            yield break;
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
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a + (Time.deltaTime / fade.speed));
                yield return null;
            }
        }
        else
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
            //if (fadeIn.delay != 0)
            //    yield return new WaitForSeconds(fadeIn.delay);

            while (sprite.color.a > 0.0f)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - (Time.deltaTime / fade.speed * 3));
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
                dissolveLevel -= Time.deltaTime / dissolve.speed;
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
                dissolveLevel += Time.deltaTime / dissolve.speed;
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
