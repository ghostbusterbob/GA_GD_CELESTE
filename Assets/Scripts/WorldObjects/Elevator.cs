using System;
using UnityEngine;
using System.Collections;
public class Eleve : MonoBehaviour
{
    [SerializeField] private float metersUp = 3f;
    [SerializeField] private float waitTime = 3f; 
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator animatorGears;

    [SerializeField] private CameraShake cameraShake;

    private float timer = 0f;
    private bool shouldLerpUp = false;

    private Vector2 initialPosition;
    private Vector2 finalPos;
    
    private bool hasShaken = false;
    private bool reachedTopOnce = false;
    
    [SerializeField] private float shakeMagnitude;
    [SerializeField] private float shakeTime;
    
    [SerializeField] private float startDelay = 1f;
    
    



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

                animator.SetBool("Down", true);
                animator.SetBool("Up", false);
                animator.SetBool("Idle", false);

                animatorGears.SetBool("Down", true);
                animatorGears.SetBool("Up", false);
                animatorGears.SetBool("Idle", false);
            }
        }

        bool reachedBottom = Vector2.Distance(transform.position, initialPosition) < 0.15f; 

        if (reachedBottom)
        {
            animator.SetBool("Down", false);
            animator.SetBool("Up", false);
            animator.SetBool("Idle", true);

            animatorGears.SetBool("Down", false);
            animatorGears.SetBool("Up", false);
            animatorGears.SetBool("Idle", true);
        }
        
    }
    // slowdown effect
    private void LerpEffect()
    {
        animator.SetBool("Down", false);
        animator.SetBool("Up", true);
        animator.SetBool("Idle", false);

        animatorGears.SetBool("Down", false);
        animatorGears.SetBool("Up", true);
        animatorGears.SetBool("Idle", false);
        transform.position = Vector2.MoveTowards(
            transform.position,
            finalPos,
            speed * Time.deltaTime
        );

        bool reachedTop = Vector2.Distance(transform.position, initialPosition + Vector2.up * metersUp) < 0.05f;

        if (reachedTop)
        {
            if (!hasShaken)
            {
                hasShaken = true;
                if (cameraShake != null)
                {
                    StartCoroutine(cameraShake.Shake(.3f, 2));
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
            StartCoroutine(cameraShake.Shake(1, 0.2f));

            StartCoroutine(StartElevatorAfterDelay());
        }
    }
    private IEnumerator StartElevatorAfterDelay()
    {
        hasShaken = false;
        reachedTopOnce = false;

        // Optional: play idle animation during delay
        animator.SetBool("Idle", true);
        animatorGears.SetBool("Idle", true);

        yield return new WaitForSeconds(startDelay);

        shouldLerpUp = true;
    }



}