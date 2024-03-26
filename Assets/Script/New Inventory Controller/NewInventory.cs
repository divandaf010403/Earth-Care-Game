using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class NewInventory : MonoBehaviour
{
    [Header("UI")]
    public GameObject inventory;
    private List<NewSlot> allInventorySlots = new List<NewSlot>();
    public List<NewSlot> inventorySlots = new List<NewSlot>();
    public List<NewSlot> hotbarSlots = new List<NewSlot>();
    public Image crosshair;
    public TMP_Text itemHoverText;

    [Header("Raycast")]
    public float raycastDistance = 5f;
    public LayerMask itemLayer;
    public Transform dropLocation; // The location items will be dropped from.

    [Header("Drag and Drop")]
    public Image dragIconImage;
    private NewItem currentDraggedItem;
    private int currentDragSlotIndex = -1;

    [Header("Equippable Items")]
    public List<GameObject> equippableItems = new List<GameObject>();
    public Transform selectedItemImage;

    [Header("Crafting")]
    public List<NewRecipe> itemRecipes = new List<NewRecipe>();

    [Header("Save/Load")]
    public List<GameObject> allItemPrefabs = new List<GameObject>();
    private string saveFileName = "inventorySave.json";

    // Start is called before the first frame update
    void Start()
    {
        toggleInventory(false);

        allInventorySlots.AddRange(hotbarSlots);
        allInventorySlots.AddRange(inventorySlots);

        foreach (NewSlot uiSlot in allInventorySlots)
        {
            uiSlot.initialiseSlot();
        }

        loadInventory();
    }

    // Update is called once per frame
    void Update()
    {
        itemRaycast(Input.GetMouseButtonDown(0));

        if (Input.GetKeyDown(KeyCode.E))
            toggleInventory(!inventory.activeInHierarchy);

        if (inventory.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            dragInventoryIcon();
        }
        else if (currentDragSlotIndex != -1 && Input.GetMouseButtonUp(0) || currentDragSlotIndex != -1 && !inventory.activeInHierarchy) // If we are hovered over a slot and release, if we are dragging an item and close the inventory
        {
            dropInventoryIcon();
        }

        if (Input.GetKeyDown(KeyCode.Q)) // The button we need to press to drop items from the inventory
            dropItem();

        for (int i = 1; i < hotbarSlots.Count + 1; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                enableHotbarItem(i-1);
            }
        }

        dragIconImage.transform.position = Input.mousePosition;
    }

    private void itemRaycast(bool hasClicked = false)
    {
        itemHoverText.text = "";
        Ray ray = Camera.main.ScreenPointToRay(crosshair.transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, itemLayer))
        {
            if (hit.collider != null)
            {
                if (hasClicked) // Pick up
                {
                    NewItem newItem = hit.collider.GetComponent<NewItem>();
                    if (newItem)
                    {
                        addItemToInventory(newItem);
                    }
                }
                else // Get the name
                {
                    NewItem newItem = hit.collider.GetComponent<NewItem>();

                    if (newItem)
                    {
                        itemHoverText.text = newItem.name;
                    }
                }
            }
        }
    }

    private void addItemToInventory(NewItem itemToAdd, int overrideIndex = -1)
    {
        if (overrideIndex != -1)
        {
            allInventorySlots[overrideIndex].setItem(itemToAdd);
            itemToAdd.gameObject.SetActive(false);
            allInventorySlots[overrideIndex].updateData();
            return;
        }

        int leftoverQuantity = itemToAdd.currentQuantity;
        NewSlot openSlot = null;
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            NewItem heldItem = allInventorySlots[i].getItem();

            if (heldItem != null && itemToAdd.name == heldItem.name)
            {
                int freeSpaceInSlot = heldItem.maxQuantity - heldItem.currentQuantity;

                if (freeSpaceInSlot >= leftoverQuantity)
                {
                    heldItem.currentQuantity += leftoverQuantity;
                    Destroy(itemToAdd.gameObject);
                    allInventorySlots[i].updateData();
                    return;
                }
                else // Add as much as we can to the current slot
                {
                    heldItem.currentQuantity = heldItem.maxQuantity;
                    leftoverQuantity -= freeSpaceInSlot;
                }
            }
            else if (heldItem == null)
            {
                if(!openSlot)
                    openSlot = allInventorySlots[i];
            }

            allInventorySlots[i].updateData();
        }

        if (leftoverQuantity > 0 && openSlot)
        {
            openSlot.setItem(itemToAdd);
            itemToAdd.currentQuantity = leftoverQuantity;
            itemToAdd.gameObject.SetActive(false);
        }
        else
        {
            itemToAdd.currentQuantity = leftoverQuantity;
        }
    }

    private void toggleInventory(bool enable)
    {
        inventory.SetActive(enable);

        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;

        // Disable the rotation of the camera.
        // Camera.main.GetComponent<FirstPersonLook>().sensitivity = enable ? 0 : 2;

        if (!enable) // Bug Fix 
        {
            foreach (NewSlot curSlot in allInventorySlots)
                curSlot.hovered = false;
        }
    }

    private void dropItem()
    {
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            NewSlot curSlot = allInventorySlots[i];
            if (curSlot.hovered && curSlot.hasItem())
            {
                curSlot.getItem().gameObject.SetActive(true);
                curSlot.getItem().transform.position = dropLocation.position;
                curSlot.setItem(null);
                break;
            }
        }
    }

    private void dragInventoryIcon()
    {
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            NewSlot curSlot = allInventorySlots[i];
            if (curSlot.hovered && curSlot.hasItem())
            {
                currentDragSlotIndex = i; // Update the current drag slot index variable.

                currentDraggedItem = curSlot.getItem(); // Get the item from the current slot
                dragIconImage.sprite = currentDraggedItem.icon;
                dragIconImage.color = new Color(1, 1, 1, 1); // Make the follow mouse icon opaque (visible).

                curSlot.setItem(null); // Remove the item from the slot we just picked up the item from.
            }
        }
    }

    private void dropInventoryIcon()
    {
        // Reset our drag item variables
        dragIconImage.sprite = null;
        dragIconImage.color = new Color(1, 1, 1, 0); // Make invisible.

        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            NewSlot curSlot = allInventorySlots[i];
            if (curSlot.hovered)
            {
                if (curSlot.hasItem()) // Swap the items.
                {
                    NewItem itemToSwap = curSlot.getItem();

                    curSlot.setItem(currentDraggedItem);

                    allInventorySlots[currentDragSlotIndex].setItem(itemToSwap);

                    resetDragVariables();
                    return;
                }
                else // Place the item with no swap.
                {
                    curSlot.setItem(currentDraggedItem);
                    resetDragVariables();
                    return;
                }
            }
        }

        // If we get to this point we dropped the item in an invalid location (or closed the inventory).
        allInventorySlots[currentDragSlotIndex].setItem(currentDraggedItem);
        resetDragVariables();
    }

    private void resetDragVariables()
    {
        currentDraggedItem = null;
        currentDragSlotIndex = -1;
    }

    private void enableHotbarItem(int hotbarIndex)
    {
        foreach (GameObject a in equippableItems)
        {
            a.SetActive(false);
        }

        NewSlot hotbarSlot = hotbarSlots[hotbarIndex];

        if (hotbarSlot.hasItem())
        {
            if (hotbarSlot.getItem().equippableItemIndex != -1)
            {
                equippableItems[hotbarSlot.getItem().equippableItemIndex].SetActive(true);
            }
        }
    }

    public void craftItem(string itemName)
    {
        foreach (NewRecipe recipe in itemRecipes)
        {
            if (recipe.createdItemPrefab.GetComponent<NewItem>().name == itemName)
            {
                bool haveAllIngredients = true;
                for (int i = 0; i < recipe.requiredIngredients.Count; i++)
                {
                    if (haveAllIngredients)
                        haveAllIngredients = haveIngredient(recipe.requiredIngredients[i].itemName, recipe.requiredIngredients[i].requiredQuantity);
                }

                if (haveAllIngredients)
                {
                    for (int i = 0; i < recipe.requiredIngredients.Count; i++)
                    {
                        removeIngredient(recipe.requiredIngredients[i].itemName, recipe.requiredIngredients[i].requiredQuantity);
                    }

                    GameObject craftedItem = Instantiate(recipe.createdItemPrefab, dropLocation.position, Quaternion.identity);
                    craftedItem.GetComponent<NewItem>().currentQuantity = recipe.quantityProduced;

                    addItemToInventory(craftedItem.GetComponent<NewItem>());
                }

                break;
            }
        }
    }

    private void removeIngredient(string itemName, int quantity)
    {
        if (!haveIngredient(itemName, quantity))
            return;

        int remainingQuantity = quantity;

        foreach (NewSlot curSlot in allInventorySlots)
        {
            NewItem item = curSlot.getItem();

            if (item != null && item.name == itemName)
            {
                if(item.currentQuantity >= remainingQuantity)
                {
                    item.currentQuantity -= remainingQuantity;

                    if (item.currentQuantity == 0)
                    {
                        curSlot.setItem(null);
                        curSlot.updateData();
                    }

                    return;
                }
                else
                {
                    remainingQuantity -= item.currentQuantity;
                    curSlot.setItem(null);
                }
            }
        }
    }

    private bool haveIngredient(string itemName, int quantity)
    {
        int foundQuantity = 0;
        foreach (NewSlot curSlot in allInventorySlots)
        {
            if (curSlot.hasItem() && curSlot.getItem().name == itemName)
            {
                foundQuantity += curSlot.getItem().currentQuantity;

                if (foundQuantity >= quantity)
                    return true;
            }
        }

        return false;
    }

    private void saveInventory()
    {
        InventoryData data = new InventoryData();

        foreach (NewSlot slot in allInventorySlots)
        {
            NewItem item = slot.getItem();
            if (item != null)
            {
                ItemData itemData = new ItemData(item.name, item.currentQuantity, allInventorySlots.IndexOf(slot));
                data.slotData.Add(itemData);
            }
        }

        string jsonData = JsonUtility.ToJson(data);

        File.WriteAllText(saveFileName, jsonData);
    }

    private void loadInventory()
    {
        if (File.Exists(saveFileName))
        {
            string jsonData = File.ReadAllText(saveFileName);

            InventoryData data = JsonUtility.FromJson<InventoryData>(jsonData);

            clearInventory();

            foreach (ItemData itemData in data.slotData)
            {
                GameObject itemPrefab = allItemPrefabs.Find(prefab => prefab.GetComponent<NewItem>().name == itemData.itemName);

                if (itemPrefab != null)
                {
                    GameObject createdItem = Instantiate(itemPrefab, dropLocation.position, Quaternion.identity);
                    NewItem item = createdItem.GetComponent<NewItem>();

                    item.currentQuantity = itemData.quantity;

                    addItemToInventory(item, itemData.slotIndex);
                }
            }
        }

        foreach (NewSlot slot in allInventorySlots)
        {
            slot.updateData();
        }
    }

    private void clearInventory()
    {
        foreach (NewSlot slot in allInventorySlots)
        {
            slot.setItem(null);
        }
    }
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public int quantity;
    public int slotIndex;

    public ItemData(string itemName, int quantity, int slotIndex)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.slotIndex = slotIndex;
    }
}

[System.Serializable]
public class InventoryData
{
    public List<ItemData> slotData = new List<ItemData>();
}