using UnityEngine;

public class CelesteCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playerTransform;

    private CameraRoom activeRoom;

    private float camHalfHeight;
    private float camHalfWidth;

    private void Start()
    {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RoomBounds"))
        {
            CameraRoom room = other.transform.parent.GetComponent<CameraRoom>();
            if (room != null)
            {
                activeRoom = room;
                SnapToBounds();   // instant Celeste snap
            }
        }
    }

    private void LateUpdate()
    {
        if (activeRoom == null) return;
        FollowInsideBounds();
    }

    private void FollowInsideBounds()
    {
        Vector3 pos = cameraTransform.position;

        // Follow player
        pos.x = playerTransform.position.x;
        pos.y = playerTransform.position.y;

        // Get room bounds
        Bounds b = activeRoom.bounds.bounds;

        float left   = b.min.x + camHalfWidth;
        float right  = b.max.x - camHalfWidth;
        float bottom = b.min.y + camHalfHeight;
        float top    = b.max.y - camHalfHeight;

        pos.x = Mathf.Clamp(pos.x, left, right);
        pos.y = Mathf.Clamp(pos.y, bottom, top);

        cameraTransform.position = pos;
    }

    private void SnapToBounds()
    {
        FollowInsideBounds();  // instant snap = Celeste
    }
}