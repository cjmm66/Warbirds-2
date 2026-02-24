using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool startMovingRight = true;

    [Header("Turn Around Limits")]
    [Tooltip("How far past the camera edge the plane goes before flipping direction.")]
    [SerializeField] private float extraOffscreenDistance = 2f;
    [Tooltip("Optional fixed world X limit. If > 0, this is used instead of camera bounds.")]
    [SerializeField] private float fixedWorldXLimit = 0f;
    [SerializeField] private GameObject targetCamera;

    private float direction = 1f;

    private void Awake()
    {
        direction = startMovingRight ? 1f : -1f;

        if (targetCamera == null)
        {
            targetCamera = Camera.main.gameObject;
        }

        ApplyFacing();
    }

    private void Update()
    {
        transform.Translate(Vector3.right * (direction * moveSpeed * Time.deltaTime), Space.World);

        float xLimit = GetTurnAroundXLimit();
        if (xLimit <= 0f)
        {
            return;
        }

        if (direction > 0f && transform.position.x >= xLimit)
        {
            FlipDirection();
        }
        else if (direction < 0f && transform.position.x <= -xLimit)
        {
            FlipDirection();
        }
    }

    private float GetTurnAroundXLimit()
    {
        if (fixedWorldXLimit > 0f)
        {
            return fixedWorldXLimit;
        }

        if (targetCamera == null)
        {
            return 0f;
        }

        Vector3 rightEdgeWorld = targetCamera.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(1f, 0.5f, Mathf.Abs(targetCamera.transform.position.z)));
        return Mathf.Abs(rightEdgeWorld.x) + Mathf.Max(0f, extraOffscreenDistance);
    }

    private void FlipDirection()
    {
        direction *= -1f;
        ApplyFacing();
    }

    private void ApplyFacing()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
        transform.localScale = scale;
    }
}
