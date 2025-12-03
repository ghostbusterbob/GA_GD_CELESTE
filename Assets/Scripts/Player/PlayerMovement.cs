using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 4.5f;
    [SerializeField] private float airDrag = 0.05f;
    [SerializeField, Range(0f, 20f)] private float airControlMultiplier = 10f;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 3f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;
    private bool canDash = true;
    private bool hasDashed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Input voor Jumpen en Dashen
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.X)) 
            && canDash && !hasDashed)
        {
            StartDash();
        }
    }

    void FixedUpdate()
        //Dash mechanics
    {
        if (isDashing) return;

        float move = Input.GetAxisRaw("Horizontal");

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        }
        else
        {
            if (move != 0)
            {
                float targetX = move * speed;
                float newX = Mathf.Lerp(rb.linearVelocity.x, targetX, airControlMultiplier * Time.fixedDeltaTime);
                rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(
                    Mathf.Lerp(rb.linearVelocity.x, 0, airDrag),
                    rb.linearVelocity.y
                );
            }
        }
    }

    private void Jump()
    //nog meer jump mechanics
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
    }

    private void StartDash()
    //Nog meer dash mechanics
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput == 0) return;

        isDashing = true;
        canDash = false;

        if (!isGrounded)
            hasDashed = true;

        rb.linearVelocity = new Vector2(moveInput * dashForce, 0);

        Invoke(nameof(EndDash), dashDuration);
        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void EndDash()
    {
        isDashing = false;
    }

    private void ResetDash()
    {
        canDash = true;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    //zorgt ervoor dat de speler niet kan blijven dashen in de lucht
    {
        
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Elevator"))
        {
            isGrounded = true;
            hasDashed = false;
            canDash = true;
        }

       
        if (collision.collider.CompareTag("Spike"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Spike"))
        {
            Die();
        }
    }
}
