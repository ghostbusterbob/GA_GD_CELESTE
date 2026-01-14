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

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;
    private bool canDash = true;
    private bool hasDashed = false;

    [SerializeField] private bool wallclimbing = false;

    [SerializeField]private Transform wallclimbpoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        if (Input.GetKeyDown(KeyCode.Q) && canDash && !hasDashed)
            StartDash();
        
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            CheckWallClimb();
        }

        if (wallclimbing)
        {
            Debug.Log("Wallclimbing");
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            transform.position = wallclimbpoint.position;
            
            
        }
        
      
    }

    void FixedUpdate()
    {
        if (isDashing) return;

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

        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);


        
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
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

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
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

    private void CheckWallClimb()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Offset the ray outside the player collider
        Vector2 origin = (Vector2)transform.position + direction * 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, .1f);

        Debug.DrawRay(origin, direction * 1f, Color.red, 1f);

        if (hit.collider != null && hit.collider.CompareTag("wallclimbpoint"))
        {
            Debug.Log("Wall detected");

            wallclimbpoint = hit.collider.transform;    
            wallclimbing = true;
        }
    }


}
