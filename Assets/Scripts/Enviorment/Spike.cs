using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    //zorgt ervoor dat de speler sterft als die de spikes raakt
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().Die();
        }
    }
}