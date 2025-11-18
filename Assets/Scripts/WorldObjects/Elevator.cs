using System;
using UnityEngine;

public class Eleve : MonoBehaviour
{
    [SerializeField] private float metersUp = 3f;
    [SerializeField] private float waitTime = 3f; 
    [SerializeField] private float speed;

    private float timer = 0f;
    private bool shouldLerpUp = false;

    private Vector2 initialPosition;
    private Vector2 finalPos;

    private void Start()
    {
        initialPosition = transform.position;
        finalPos = initialPosition; 
    }

    private void Update()
    {
        finalPos = shouldLerpUp ? new Vector2(initialPosition.x, initialPosition.y + metersUp) : initialPosition;

        LerpEffect();

        if (shouldLerpUp)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                shouldLerpUp = false;
                timer = 0f; 
            }
        }
    }

    private void LerpEffect()
    {
        transform.position = Vector2.Lerp(transform.position, finalPos, Time.deltaTime * speed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            shouldLerpUp = true;
            timer = 0f;
        }
    }
}