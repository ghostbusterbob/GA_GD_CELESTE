using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float airDrag = 0.05f;
    [SerializeField, Range(0f, 20f)] private float airControlMultiplier = 0.4f;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;
    private bool canDash = true;

    private float dashTimeLeft;
    private float lastDash = -100f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash || Input.GetKeyDown(KeyCode.C) && canDash || Input.GetKeyDown(KeyCode.X) && canDash)
            StartDash();
    }

    void FixedUpdate()
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
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
    }

    private void StartDash()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput == 0) return;

        isDashing = true;
        canDash = false;
        dashTimeLeft = dashDuration;

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
    {

        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Elevator"))
            isGrounded = true;
    }
}
