using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemClickHandlerCrafting : MonoBehaviour
{
    public Image image;
    public Color selectedColor, deselectedColor;
    CraftingMain craftingMain;

    private void Start()
    {
        Transform getInvComponent = transform.parent.parent.parent;
        craftingMain = getInvComponent.GetComponent<CraftingMain>();
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
        if (craftingMain != null)
        {
            craftingMain.ChangedSelectedSlot(Array.IndexOf(craftingMain.itemSelected, this));
        }
    }
}
