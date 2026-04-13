using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// On-screen virtual joystick for mobile touch input.
/// Attach to a UI Image that acts as the joystick background.
/// Returns a normalized axis value via InputAxis.
/// </summary>
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [Tooltip("The inner knob/handle of the joystick.")]
    [SerializeField] private RectTransform handle;

    [Header("Settings")]
    [SerializeField] private float maxRadius = 50f;
    [SerializeField] private float deadZone = 0.1f;

    /// <summary>
    /// Current joystick axis value. X is horizontal, Y is vertical.
    /// Each axis is in range [-1, 1].
    /// </summary>
    public Vector2 InputAxis { get; private set; }

    private RectTransform baseRect;
    private Canvas parentCanvas;
    private Camera canvasCamera;

    private void Awake()
    {
        baseRect = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvasCamera = parentCanvas.worldCamera;
        }

        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (baseRect == null || handle == null)
        {
            return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            baseRect,
            eventData.position,
            canvasCamera,
            out Vector2 localPoint);

        // Clamp to max radius
        Vector2 clampedPoint = Vector2.ClampMagnitude(localPoint, maxRadius);
        handle.anchoredPosition = clampedPoint;

        // Normalize to [-1, 1]
        Vector2 normalized = clampedPoint / maxRadius;

        // Apply dead zone
        if (normalized.magnitude < deadZone)
        {
            normalized = Vector2.zero;
        }

        InputAxis = normalized;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputAxis = Vector2.zero;

        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }
}
