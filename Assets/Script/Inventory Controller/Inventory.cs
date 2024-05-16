using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 10;

    [SerializeField]
    public List<IInventoryItem> mItem = new List<IInventoryItem>();
    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;
    public InventoryItemDataList inventoryItemDataList;

    [Header("Pilihan Inventory")]
    public ItemClickHandler[] itemSelected;
    public int defaultSelectedItemIndex = -1;

    public int myItemId;
    public string ID_KEY = "ID_KEY";

    private void Start()
    {
        ChangedSelectedSlot(0);

        LoadInventoryItem();
    }

    public void OnApplicationQuit()
    {
        SaveSystem.SaveInventory(inventoryItemDataList.slotData);
    }
    
    void Update() 
    {
        LoadInventoryItem();
    }

    public void ChangedSelectedSlot(int newValue)
    {
        if (defaultSelectedItemIndex >= 0)
        {
            itemSelected[defaultSelectedItemIndex].Deselected();
        }

        itemSelected[newValue].Selected();
        defaultSelectedItemIndex = newValue;
    }

    public void AddItem(IInventoryItem item)
    {
        Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
        myItemId = PlayerPrefs.GetInt(ID_KEY, 1);

        if (inventoryItemDataList.slotData.Count < SLOTS)
        {
            if (collider.enabled)
            {
                if (!GameVariable.isQuestStarting)
                {
                    collider.enabled = false;

                    List<int> usedSlotNumbers = new List<int>();
                    foreach (InventoryItemData itemData in inventoryItemDataList.slotData)
                    {
                        usedSlotNumbers.Add(itemData.slotNumber);
                    }

                    // Menemukan nomor slot yang tersedia
                    int availableSlotNumber = 1;
                    while (usedSlotNumbers.Contains(availableSlotNumber))
                    {
                        availableSlotNumber++;
                    }

                    mItem.Add(item);
                    InventoryItemData inventoryItemData = new InventoryItemData(availableSlotNumber, myItemId, item.itemName, item.image, item.typeSampah, item.jenisSampah, item.jumlahItem);
                    inventoryItemDataList.slotData.Add(inventoryItemData);

                    if (ItemAdded != null)
                    {
                        ItemAdded(this, new InventoryEventArgs(item));
                    }

                    SaveSystem.SaveInventory(inventoryItemDataList.slotData);

                    LoadInventoryItem();

                    item.OnPickup();

                    IncrementAndSaveItemId();
                }
            }
        }
    }

    // public void RemoveItem(IInventoryItem item)
    // {
    //     if (mItem.Contains(item))
    //     {
    //         mItem.Remove(item);
    //         // item.OnDrop();

    //         Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
    //         if (collider != null)
    //         {
    //             collider.enabled = true;
    //         }

    //         if (ItemRemoved != null)
    //         {
    //             ItemRemoved(this, new InventoryEventArgs(item));
    //         }

    //         // SaveSystem.SaveInventory(myItemId, item);

    //         LoadInventoryItem();
    //     }
    // }

    public bool RemoveItem(TrashcanController trashcanController, MainCharMovement nPanelShow)
    {
        Transform imageTransform = transform.GetChild(defaultSelectedItemIndex).GetChild(0).GetChild(0);
        Image image = imageTransform.GetChild(0).GetComponent<Image>();
        InventoryVariable inventoryVariable = imageTransform.GetComponent<InventoryVariable>();
        
        int indexToRemove = inventoryItemDataList.slotData.FindIndex(item => {            
            return item.itemId == inventoryVariable.itemId && item.jenisSampah == trashcanController.jenisTempatSampah;
        });

        Debug.Log("Index yang dihapus : " + indexToRemove);

        if (indexToRemove != -1)
        {
            inventoryItemDataList.slotData.RemoveAt(indexToRemove);

            SaveSystem.SaveInventory(inventoryItemDataList.slotData);
            LoadInventoryItem();

            image.enabled = false;
            image.sprite = null;
            inventoryVariable.itemId = 0;
            inventoryVariable.itemName = "";
            inventoryVariable.jenisSampah = "";
            inventoryVariable.totalSampah = 0;

            Debug.Log("Buang Sampah Berhasil");

            return true;
        }
        else
        {
            // Item with the specified itemId not found
            Debug.Log("Gagal Buang Sampah");
            nPanelShow.showNotification("SALAH GOBLOK!!!");

            return false;
        }
    }

    void LoadInventoryItem() 
    {
        List<InventoryItemData> loadedItemData = SaveSystem.LoadInventory();

        if (loadedItemData != null)
        {
            inventoryItemDataList.slotData = loadedItemData;

            foreach (InventoryItemData itemData in loadedItemData)
            {
                int slotNumberItem = itemData.slotNumber - 1;

                Transform imageTransform = transform.GetChild(slotNumberItem).GetChild(0).GetChild(0);
                Image image = imageTransform.GetChild(0).GetComponent<Image>();
                InventoryVariable inventoryVariable = imageTransform.GetComponent<InventoryVariable>();

                if (!image.enabled) {
                    image.enabled = true;

                    inventoryVariable.itemId = itemData.itemId;
                    inventoryVariable.itemName = itemData.itemName;
                    inventoryVariable.jenisSampah = itemData.jenisSampah;
                    inventoryVariable.totalSampah = itemData.jumlahItem;
                    
                    image.sprite = itemData.itemImage;
                }
            }
        }
    }

    private void IncrementAndSaveItemId() {
        myItemId++;
        PlayerPrefs.SetInt(ID_KEY, myItemId);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class InventoryItemData
{
    public int slotNumber;
    public int itemId;
    public string itemName;
    public Sprite itemImage;
    public string typeSampah;
    public string jenisSampah;
    public int jumlahItem;

    public InventoryItemData(
        int slotNumber,
        int itemId,
        string itemName,
        Sprite itemImage,
        string typeSampah,
        string jenisSampah,
        int jumlahItem
    )
    {
        this.slotNumber = slotNumber;
        this.itemId = itemId;
        this.itemName = itemName;
        this.itemImage = itemImage;
        this.typeSampah = typeSampah;
        this.jenisSampah = jenisSampah;
        this.jumlahItem = jumlahItem;
    }
}

[System.Serializable]
public class InventoryItemDataList
{
    public List<InventoryItemData> slotData;
}