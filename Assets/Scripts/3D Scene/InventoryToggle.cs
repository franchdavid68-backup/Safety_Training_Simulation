using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryToggle : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUIPanel;
    [SerializeField] private bool startOpen = false;

    private bool isOpen;

    private void Start()
    {
        SetState(startOpen);
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.iKey.wasPressedThisFrame || keyboard.tabKey.wasPressedThisFrame)
        {
            SetState(!isOpen);
        }
    }

    public void SetState(bool open)
    {
        isOpen = open;
        if (inventoryUIPanel != null)
        {
            inventoryUIPanel.SetActive(isOpen);
        }

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }
}