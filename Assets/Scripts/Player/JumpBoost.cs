using UnityEngine;

public class JumpBoostTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.inJumpBoostZone = true;
            Debug.Log("Jump boost ENABLED");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.inJumpBoostZone = false;
            Debug.Log("Jump boost DISABLED");
        }
    }
}
