using UnityEngine;

public class CameraRoom : MonoBehaviour
{
    public BoxCollider2D bounds;   // The NON-trigger collider

    private void Reset()
    {
        // Auto-assign if user forgets
        bounds = GetComponent<BoxCollider2D>();
    }}