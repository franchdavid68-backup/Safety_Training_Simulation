using UnityEngine;
using UnityEngine.Rendering;

public class InventoryUI : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject uiItemPrefab;

    [Header("References")]
    [SerializeField] private Transform uiInventoryContainer;
    [SerializeField] private Inventory inventory;

    [Header("State")]
    [SerializeField] private SerializedDictionary<string, GameObject> inventoryUI = new();

    public void AddItem(string inventoryId, Item item)
    {
        if (uiInventoryContainer == null || uiItemPrefab == null)
        {
            Debug.LogError("[InventoryUI] Missing uiInventoryContainer or uiItemPrefab reference!", this);
            return;
        }

        GameObject itemObj = Instantiate(uiItemPrefab, uiInventoryContainer);

        if (itemObj.TryGetComponent<ItemUI>(out var itemUI))
        {
            inventoryUI[inventoryId] = itemObj;

            itemUI.Initialize(inventoryId, item, (id) =>
            {
                if (inventory != null)
                {
                    inventory.DropItem(id);
                }
                else
                {
                    Debug.LogError("[InventoryUI] Missing Inventory reference on drop callback.", this);
                }
            });
        }
    }

    public void RemoveUIItem(string inventoryId)
    {
        if (inventoryUI.TryGetValue(inventoryId, out GameObject itemObj))
        {
            inventoryUI.Remove(inventoryId);
            if (itemObj != null)
            {
                Destroy(itemObj);
            }
        }
    }
}