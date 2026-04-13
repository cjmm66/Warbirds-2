using UnityEngine;

/// <summary>
/// Player lateral movement controller.
/// Supports both UI button input and VirtualJoystick input.
/// Includes boundary clamping to keep the player on-screen.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;

    [Header("Virtual Joystick")]
    [Tooltip("Optional. If assigned, joystick axis overrides button input.")]
    [SerializeField] private VirtualJoystick virtualJoystick;

    [Header("Boundary Clamping")]
    [Tooltip("Clamp player X position to screen bounds. Disable for infinite levels.")]
    [SerializeField] private bool clampToScreen = true;
    [Tooltip("Extra padding from screen edge in world units.")]
    [SerializeField] private float screenEdgePadding = 0.5f;
    [SerializeField] bool facingRight = true;

    private bool moveLeftPressed;
    private bool moveRightPressed;

    private void Awake()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate()
    {
            SpriteFaceFlip();
        float input = GetMovementInput();

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(input * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            transform.Translate(Vector3.right * (input * moveSpeed * Time.fixedDeltaTime));
        }

        if (clampToScreen)
        {
            ClampPositionToScreen();
        }
    }

    private float GetMovementInput()
    {
        float buttonInput = 0f;

        if (moveLeftPressed)
        {
            buttonInput -= 1f;
            facingRight = false;
        }

        if (moveRightPressed)
        {
            buttonInput += 1f;
            facingRight = true;
        }

        return buttonInput;
    }

    private void ClampPositionToScreen()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            return;
        }

        Vector3 pos = transform.position;
        float camZ = Mathf.Abs(cam.transform.position.z);

        Vector3 minWorld = cam.ViewportToWorldPoint(new Vector3(0f, 0f, camZ));
        Vector3 maxWorld = cam.ViewportToWorldPoint(new Vector3(1f, 1f, camZ));

        pos.x = Mathf.Clamp(pos.x, minWorld.x + screenEdgePadding, maxWorld.x - screenEdgePadding);
        transform.position = pos;
    }

    // ---------- Button Callbacks (UI EventTrigger) ----------

    public void OnLeftButtonDown()
    {
        moveLeftPressed = true;
    }

    public void OnLeftButtonUp()
    {
        moveLeftPressed = false;
    }

    public void OnRightButtonDown()
    {
        moveRightPressed = true;
    }

    public void OnRightButtonUp()
    {
        moveRightPressed = false;
    }

    void SpriteFaceFlip()
    {
        if (facingRight)
        {
        sprite.flipX = false;
        }
        else
        {
       sprite.flipX = true;
        }
    }
}
