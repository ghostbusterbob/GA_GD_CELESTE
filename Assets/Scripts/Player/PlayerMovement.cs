using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float wallSlideSpeed = 1.5f;
    [SerializeField] private float wallClimbSpeed = 3f;
    [SerializeField] private float wallJumpForceX = 5f;
    [SerializeField] private float wallJumpForceY = 7f;

    [Header("Wall Settings")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;
    private bool canDash = true;

    private bool isTouchingWall;
    private bool isWallSliding;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        WallCheck();

<<<<<<< Updated upstream
        if (Input.GetKeyDown(KeyCode.Q) && canDash)
=======
        // Springen / Wall jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isWallSliding)
                WallJump();
            else if (isGrounded)
                Jump();
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
>>>>>>> Stashed changes
            StartDash();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        float move = Input.GetAxisRaw("Horizontal");

        // Normale horizontale beweging
        if (isGrounded || !isWallSliding)
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        HandleWallMovement();
    }

    private void WallCheck()
    {
        Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        isTouchingWall = Physics2D.Raycast(wallCheck.position, dir, wallCheckDistance, wallLayer);
        Debug.DrawRay(wallCheck.position, dir * wallCheckDistance, Color.green);
    }

    private void HandleWallMovement()
    {
        if (isTouchingWall && !isGrounded)
        {
            float vertical = Input.GetAxisRaw("Vertical");

            // Wall climb omhoog
            if (vertical > 0)
            {
                rb.gravityScale = 0;
                rb.linearVelocity = new Vector2(0, wallClimbSpeed);
                isWallSliding = false;
            }
            // Wall slide naar beneden
            else
            {
                rb.gravityScale = 1;
                rb.linearVelocity = new Vector2(0, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
                isWallSliding = true;
            }
        }
        else
        {
            rb.gravityScale = 1;
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        float dir = transform.localScale.x > 0 ? -1 : 1; // spring van muur weg
        rb.linearVelocity = new Vector2(dir * wallJumpForceX, wallJumpForceY);
        isWallSliding = false;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = true;
    }
}
