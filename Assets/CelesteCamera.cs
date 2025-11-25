using UnityEngine;
using System.Collections;

public class CelesteCamera : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f;

    private Bounds roomBounds;

    private float camHalfHeight;
    private float camHalfWidth;

    private bool sliding = false;   // prevents normal follow during slide

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (player == null || sliding) return;

        Vector3 target = new Vector3(player.position.x, player.position.y, transform.position.z);

        Vector3 smoothed = Vector3.Lerp(transform.position, target, smoothSpeed * Time.deltaTime);

        float clampedX = Mathf.Clamp(smoothed.x,
            roomBounds.min.x + camHalfWidth,
            roomBounds.max.x - camHalfWidth);

        float clampedY = Mathf.Clamp(smoothed.y,
            roomBounds.min.y + camHalfHeight,
            roomBounds.max.y - camHalfHeight);

        transform.position = new Vector3(clampedX, clampedY, smoothed.z);
    }

    public void SetRoomBounds(Bounds newBounds, bool slide = true)
    {
        if (slide)
            StartCoroutine(SlideToRoom(newBounds));
        else
            roomBounds = newBounds;
    }

    private IEnumerator SlideToRoom(Bounds newBounds)
    {
        sliding = true;
        roomBounds = newBounds;

        Vector3 startPos = transform.position;

        // Compute target position (center of new room, clamped)
        float targetX = Mathf.Clamp(player.position.x,
            newBounds.min.x + camHalfWidth,
            newBounds.max.x - camHalfWidth);

        float targetY = Mathf.Clamp(player.position.y,
            newBounds.min.y + camHalfHeight,
            newBounds.max.y - camHalfHeight);

        Vector3 endPos = new Vector3(targetX, targetY, startPos.z);

        float t = 0f;
        float duration = 0.35f;  // Celeste-like speed

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float eased = Mathf.SmoothStep(0, 1, t);   // smooth easing curve
            transform.position = Vector3.Lerp(startPos, endPos, eased);
            yield return null;
        }

        sliding = false;
    }
}
