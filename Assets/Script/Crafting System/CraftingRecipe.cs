using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftingRecipe
{
    [SerializeField] public Sprite imageItemCraft;
    [SerializeField] private InventoryExtItemData output;
    [SerializeField] public bool isHaveItem;
    public List<RequiredIngredients> requiredIngredients = new List<RequiredIngredients>();
}

[System.Serializable]
public class RequiredIngredients
{
    public Sprite imageIngredient;
    public string itemName;
    public int requiredQuantity;
}
