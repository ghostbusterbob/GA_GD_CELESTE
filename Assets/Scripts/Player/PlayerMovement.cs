using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float airDrag = 0.05f;
    [SerializeField, Range(0f, 20f)] private float airControlMultiplier = 0.4f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector3 respawnPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        respawnPoint = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
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

    public void Die()
    {
        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Elevator"))
        {
            isGrounded = true;
        }
    }
}