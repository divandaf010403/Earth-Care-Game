using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemClickHandler : MonoBehaviour
{
    public Image image;
    public Color selectedColor, deselectedColor;
    Inventory inventory;

    private void Start()
    {
        Transform getInvComponent = transform.parent.parent;
        inventory = getInvComponent.GetComponent<Inventory>();
    }

    private void Awake()
    {
        Deselected();
    }

    public void Selected()
    {
        image.color = selectedColor;
    }

    public void Deselected()
    {
        image.color = deselectedColor;
    }

    public void ChangeActiveInventory()
    {
        if (inventory != null)
        {
            inventory.ChangedSelectedSlot(Array.IndexOf(inventory.itemSelected, this));
        }
    }

    public void OnItemClick()
    {
        InventoryVariable inventoryVariable = gameObject.transform.Find("InvContent").GetComponent<InventoryVariable>();

        string item = inventoryVariable.jenisSampah;

        Debug.Log(item);
    }


}
