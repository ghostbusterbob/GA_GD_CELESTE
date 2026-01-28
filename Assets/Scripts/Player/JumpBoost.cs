using UnityEngine;

public class JumpBoostTrigger : MonoBehaviour
{
    public bool inJumpBoostZone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<PlayerMovement>().inJumpBoostZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<PlayerMovement>().inJumpBoostZone = false;
        }
    }
}
