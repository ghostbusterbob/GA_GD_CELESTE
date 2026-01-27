using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 4.5f;
    [SerializeField] private float airControlMultiplier = 10f;

    [Header("Dash")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 3f;

    [Header("Wall Climb")]
    [SerializeField] private float wallClimbSpeed = 3f;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallCheckDistance = 0.4f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Wall Jump Lock")]
    [SerializeField] private float wallGrabDisableTime = 0.2f;

    private Rigidbody2D rb;
    private CapsuleCollider2D col;

    private bool isGrounded;
    private bool isDashing;
    private bool canDash = true;
    private bool hasDashed;

    private bool wallClimbing;
    private bool isTouchingWall;
    private int wallDirection; // -1 = left, 1 = right

    private float wallGrabTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        if (wallGrabTimer > 0)
            wallGrabTimer -= Time.deltaTime;

        CheckWall();
        HandleJump();
        HandleDash();
        HandleWallClimb();
        HandleSpriteFlip();
    }

    void FixedUpdate()
    {
        if (isDashing || wallClimbing) return;

        float move = Input.GetAxisRaw("Horizontal");

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        }
        else
        {
            float targetX = move * speed;
            float newX = Mathf.Lerp(rb.linearVelocity.x, targetX, airControlMultiplier * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }
    }

    // ---------------- WALL CHECK ----------------
    private void CheckWall()
    {
        Bounds bounds = col.bounds;
        float skin = 0.05f;

        Vector2 leftOrigin = new Vector2(bounds.min.x - skin, bounds.center.y);
        Vector2 rightOrigin = new Vector2(bounds.max.x + skin, bounds.center.y);

        RaycastHit2D left = Physics2D.Raycast(leftOrigin, Vector2.left, wallCheckDistance, wallLayer);
        RaycastHit2D right = Physics2D.Raycast(rightOrigin, Vector2.right, wallCheckDistance, wallLayer);

        Debug.DrawRay(leftOrigin, Vector2.left * wallCheckDistance, Color.red);
        Debug.DrawRay(rightOrigin, Vector2.right * wallCheckDistance, Color.red);

        if (left)
        {
            isTouchingWall = true;
            wallDirection = -1;
        }
        else if (right)
        {
            isTouchingWall = true;
            wallDirection = 1;
        }
        else
        {
            isTouchingWall = false;
        }
    }

    // ---------------- JUMP ----------------
    private void HandleJump()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        // Ground jump
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            return;
        }

        // Wall jump (works even while holding G)
        if (isTouchingWall)
        {
            wallClimbing = false;
            rb.gravityScale = 1;

            wallGrabTimer = wallGrabDisableTime;

            rb.linearVelocity = new Vector2(
                -wallDirection * speed,
                jumpForce
            );
        }
    }

    // ---------------- DASH ----------------
    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canDash && !hasDashed)
        {
            float move = Input.GetAxisRaw("Horizontal");
            if (move == 0) return;

            isDashing = true;
            canDash = false;
            if (!isGrounded) hasDashed = true;

            rb.linearVelocity = new Vector2(move * dashForce, 0);

            Invoke(nameof(EndDash), dashDuration);
            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    private void EndDash()
    {
        isDashing = false;
    }

    private void ResetDash()
    {
        canDash = true;
    }

    // ---------------- WALL CLIMB ----------------
    private void HandleWallClimb()
    {
        if (wallGrabTimer > 0)
            return;

        if (!isTouchingWall || isGrounded)
        {
            if (wallClimbing)
            {
                wallClimbing = false;
                rb.gravityScale = 1;
            }
            return;
        }

        if (Input.GetKey(KeyCode.G))
        {
            wallClimbing = true;
            rb.gravityScale = 0;

            float vertical = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = new Vector2(0, vertical * wallClimbSpeed);

            // Slight stick so collider doesn't drift
            rb.position = new Vector2(
                rb.position.x + wallDirection * 0.01f,
                rb.position.y
            );
        }
        else
        {
            // Wall slide
            rb.gravityScale = 1;
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed)
            );

            wallClimbing = false;
        }
    }

    // ---------------- SPRITE FLIP ----------------
    private void HandleSpriteFlip()
    {
        if (wallClimbing) return;

        float move = Input.GetAxisRaw("Horizontal");
        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
    }

    // ---------------- COLLISIONS ----------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Elevator"))
        {
            isGrounded = true;
            hasDashed = false;
            canDash = true;
            rb.gravityScale = 1;
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

    public void Die()
    {
        Destroy(gameObject);
    }
}
