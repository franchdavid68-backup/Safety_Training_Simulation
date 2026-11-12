using UnityEngine;


public static class UIState
{
    private static int openCount;

    public static bool IsBlocking => openCount > 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        openCount = 0;
    }

    public static void Push()
    {
        openCount++;
        Refresh();
    }

    public static void Pop()
    {
        openCount = Mathf.Max(0, openCount - 1);
        Refresh();
    }

    public static void Refresh()
    {
        Cursor.lockState = IsBlocking ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsBlocking;
    }
}
