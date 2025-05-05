using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public float handleRange = 1f;

    private Vector2 input = Vector2.zero;

    void Start()
    {
        // Center the handle initially
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = RectTransformUtility.WorldToScreenPoint(null, joystickBackground.position);
        Vector2 radius = joystickBackground.sizeDelta / 2;
        input = (eventData.position - position) / (radius * handleRange);
        input = Vector2.ClampMagnitude(input, 1f);
        joystickHandle.anchoredPosition = input * radius * handleRange;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    public Vector2 GetInput()
    {
        return input;
    }
}