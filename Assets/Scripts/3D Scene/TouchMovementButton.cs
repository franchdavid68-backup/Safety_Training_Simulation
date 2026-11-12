using UnityEngine;
using UnityEngine.EventSystems;

public class TouchMovementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isPressed = false;
    private bool consumed = false;

    public bool WasPressedThisFrame()
    {
        if (isPressed && !consumed)
        {
            consumed = true; 
            return true;
        }
        return false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        consumed = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        consumed = false;
    }
}