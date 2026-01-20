using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpForce = 4.5f;
    [SerializeField] private float airControlMultiplier = 10f;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 3f;

    [Header("Wall Climb Settings")]
    [SerializeField] private float wallClimbSpeed = 3f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;
    private bool canDash = true;
    private bool hasDashed = false;

    private bool wallClimbing = false;
    private Transform wallClimbPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleJump();
        HandleDash();
        HandleWallClimbCheck();
        HandleWallClimbMovement();
        HandleSpriteFlip();
    }

    void FixedUpdate()
    {
        if (isDashing || wallClimbing) return;

        float move = 0;

        if (Input.GetKey(KeyCode.A))
            move = -1;
        else if (Input.GetKey(KeyCode.D))
            move = 1;

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

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (wallClimbing)
            {
                // Jump off the wall
                wallClimbing = false;
                rb.gravityScale = 1;
                rb.linearVelocity = new Vector2(transform.localScale.x * -jumpForce, jumpForce);
            }
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canDash && !hasDashed)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        float moveInput = 0;

        if (Input.GetKey(KeyCode.A))
            moveInput = -1;
        else if (Input.GetKey(KeyCode.D))
            moveInput = 1;

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

    private void HandleWallClimbCheck()
    {
        if (Input.GetKeyDown(KeyCode.G) && !wallClimbing)
        {
            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Vector2 origin = (Vector2)transform.position + direction * 0.1f;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, 0.5f);
            Debug.DrawRay(origin, direction * 0.5f, Color.red, 1f);

            if (hit.collider != null && hit.collider.CompareTag("wallclimbpoint"))
            {
                wallClimbPoint = hit.collider.transform;
                StartWallClimb();
            }
        }
    }

    private void StartWallClimb()
    {
        wallClimbing = true;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        transform.position = wallClimbPoint.position;
    }

    private void HandleWallClimbMovement()
    {
        if (!wallClimbing) return;

        float vertical = 0;
        if (Input.GetKey(KeyCode.W))
            vertical = 1;
        else if (Input.GetKey(KeyCode.S))
            vertical = -1;

        rb.linearVelocity = new Vector2(0, vertical * wallClimbSpeed);

        // Release wall if moving away
        if ((Input.GetKey(KeyCode.A) && transform.localScale.x > 0) || 
            (Input.GetKey(KeyCode.D) && transform.localScale.x < 0))
        {
            wallClimbing = false;
            rb.gravityScale = 1;
        }
    }

    private void HandleSpriteFlip()
    {
        if (wallClimbing) return;

        float move = 0;
        if (Input.GetKey(KeyCode.A))
            move = -1;
        else if (Input.GetKey(KeyCode.D))
            move = 1;

        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Elevator"))
        {
            isGrounded = true;
            hasDashed = false;
            canDash = true;
            wallClimbing = false;
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
