using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider))]
public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("References")]
    [SerializeField] private InventoryUI ui;
    [SerializeField] private AudioSource audioSource;

    [Header("Prefabs")]
    [SerializeField] private GameObject droppedItemPrefab;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip pickUpItemAudio;
    [SerializeField] private AudioClip dropItemAudio;

    [Header("State")]
    [SerializeField] private SerializedDictionary<string, Item> inventory = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddItem(Item item)
    {
        if (item == null) return;

        string inventoryId = Guid.NewGuid().ToString();
        inventory[inventoryId] = item;
        
        if (ui != null)
        {
            ui.AddItem(inventoryId, item);
        }

        if (audioSource != null && pickUpItemAudio != null)
            audioSource.PlayOneShot(pickUpItemAudio);
    }

    public void DropItem(string inventoryId)
    {
        if (!inventory.TryGetValue(inventoryId, out Item item))
            return;

        Vector3 dropPosition = transform.position + (transform.forward * 1.2f) + (Vector3.up * 0.5f);
        GameObject dropObj = Instantiate(droppedItemPrefab, dropPosition, Quaternion.identity);

        if (dropObj.TryGetComponent<DroppedItem>(out var droppedItem))
        {
            droppedItem.Initialize(item);
        }

        inventory.Remove(inventoryId);
        
        if (ui != null)
        {
            ui.RemoveUIItem(inventoryId);
        }

        if (audioSource != null && dropItemAudio != null)
            audioSource.PlayOneShot(dropItemAudio);
    }

    public bool HasItem(Item targetItem)
    {
        if (targetItem == null) return false;

        foreach (var pair in inventory)
        {
            if (pair.Value == targetItem)
                return true;
        }

        return false;
    }
}