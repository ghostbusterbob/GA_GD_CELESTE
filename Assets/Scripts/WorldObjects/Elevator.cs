using System;
using UnityEngine;

public class Eleve : MonoBehaviour
{
    [SerializeField] private float metersUp = 3f;
    [SerializeField] private float waitTime = 3f; 
    [SerializeField] private float speed;

    [SerializeField] private CameraShake cameraShake;

    private float timer = 0f;
    private bool shouldLerpUp = false;

    private Vector2 initialPosition;
    private Vector2 finalPos;
    
    private bool hasShaken = false;
    private bool reachedTopOnce = false;
    
    [SerializeField] private float shakeMagnitude;
    [SerializeField] private float shakeTime;


    private void Start()
        //zorgt ervoor dat de elevator van positie verplaatst
    {
        initialPosition = transform.position;
        finalPos = initialPosition; 
        
        cameraShake = FindObjectOfType<CameraShake>();

        
    }

    private void Update()
    {
        finalPos = shouldLerpUp ? 
            new Vector2(initialPosition.x, initialPosition.y + metersUp) : 
            initialPosition;

        LerpEffect();

        if (reachedTopOnce)
        //Zorgt ervoor dat de elevator na een tijdje weer naar beneden gaat
        {
            timer += Time.deltaTime;

            if (timer >= waitTime)
            {
                shouldLerpUp = false;  
                timer = 0f;
                reachedTopOnce = false; 
            }
        }
    }
    // slowdown effect

    private void LerpEffect()
    {
        transform.position = Vector2.Lerp(transform.position, finalPos, Time.deltaTime * speed);

        bool reachedTop = Vector2.Distance(transform.position, initialPosition + Vector2.up * metersUp) < 0.15f;

        if (reachedTop)
        {
            if (!hasShaken)
            {
                hasShaken = true;
                if (cameraShake != null)
                {
                    StartCoroutine(cameraShake.Shake(shakeTime, shakeMagnitude));

                }
            }

            if (!reachedTopOnce)
            {
                reachedTopOnce = true;
                timer = 0f;
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            shouldLerpUp = true;

            hasShaken = false;
            reachedTopOnce = false; 
        }
    }

}