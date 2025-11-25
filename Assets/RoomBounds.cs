using UnityEngine;

public class CameraRoom : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CelesteCamera cam = Camera.main.GetComponent<CelesteCamera>();
            cam.SetRoomBounds(GetComponent<Collider2D>().bounds);
        }
    }
}