using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingMain : MonoBehaviour
{
    [SerializeField] private List<CraftingRecipe> recipes= new List<CraftingRecipe>();
    public Transform btnRecipe;
    public GameObject btnTemplate;
    public Transform imgIngredient;
    public GameObject imgIngredientTemplate;

    [Header("Select Item To Craft")]
    public ItemClickHandlerCrafting[] itemSelected;
    public int defaultSelectedItemIndex = -1;

    [Header("Get Saved ID")]
    public int myItemIdExt;
    public string ID_KEY = "ID_KEY_EXt";

    // Start is called before the first frame update
    void Start()
    {
        InitiateDefaultRecipe();
        ChangedSelectedSlot(0);
    }

    // Update is called once per frame
    void Update()
    {
        
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

    void InitiateDefaultRecipe()
    {
        if (recipes.Count > 0)
        {
            itemSelected = new ItemClickHandlerCrafting[recipes.Count];

            for (int i = 0; i < recipes.Count; i++)
            {
                GameObject recipeButton = Instantiate(btnTemplate, btnRecipe);
                recipeButton.transform.SetParent(btnRecipe, false);
                Button buttonComponent = recipeButton.transform.GetChild(0).GetComponent<Button>();

                CraftingRecipe currentRecipe = recipes[i];
                buttonComponent.onClick.AddListener(() => DisplayIngredients(currentRecipe));

                Image buttonImage = recipeButton.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                buttonImage.sprite = currentRecipe.imageItemCraft;

                itemSelected[i] = recipeButton.transform.GetChild(0).GetComponent<ItemClickHandlerCrafting>();

                buttonComponent.interactable = !recipes[i].isHaveItem;
            }

            DisplayIngredients(recipes[0]);
        }
        else
        {
            Debug.LogError("No recipes available!");
        }
    }

    public void DisplayIngredients(CraftingRecipe recipe)
    {
        foreach (Transform child in imgIngredient)
        {
            Destroy(child.gameObject);
        }

        foreach (RequiredIngredients ingredient in recipe.requiredIngredients)
        {
            GameObject ingredientImage = Instantiate(imgIngredientTemplate, imgIngredient);
            ingredientImage.transform.SetParent(imgIngredient, false);
            Image imageComponent = ingredientImage.transform.GetChild(0).GetComponent<Image>();
            imageComponent.sprite = ingredient.imageIngredient;
        }
    }

    public void CraftingItemBtn()
    {
        InventoryExt inventoryExtData = transform.parent.parent.GetComponent<InventoryExt>();
        myItemIdExt = PlayerPrefs.GetInt(ID_KEY);

        var recipeToCraft = recipes[defaultSelectedItemIndex];

        bool itemExists = false;

        // Check if the item to craft already exists in the inventory
        foreach (InventoryExtItemData existingItem in inventoryExtData.inventoryExtItemDataList.slotData)
        {
            if (existingItem.jenisSampah == recipeToCraft.output.jenisSampah)
            {
                Debug.Log("Item Already Exist");
                itemExists = true;
                break;
            }
        }

        if (!itemExists)
        {
            bool allIngredientsAvailable = true;

            // Check the availability of all required ingredients
            foreach (RequiredIngredients ingredient in recipeToCraft.requiredIngredients)
            {
                bool ingredientFound = false;

                // Iterate through each item in the inventory
                foreach (InventoryExtItemData existingItem in inventoryExtData.inventoryExtItemDataList.slotData)
                {
                    // If the item name matches
                    if (existingItem.jenisSampah == ingredient.itemName)
                    {
                        Debug.Log("Jumlah Item " + existingItem.jumlahItem + " Jumlah Diperlukan : " + ingredient.requiredQuantity);
                        // Check if there is a sufficient quantity
                        if (existingItem.jumlahItem >= ingredient.requiredQuantity)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                }

                // Log the status of each ingredient
                if (!ingredientFound)
                {
                    Debug.Log("Ingredient not found or not enough quantity: " + ingredient.itemName);
                    allIngredientsAvailable = false;
                    break;
                }
                else
                {
                    Debug.Log("Ingredient found and enough quantity: " + ingredient.itemName);
                }
            }

            // If all ingredients are available, create the new item and decrease the ingredient quantities in the inventory
            if (allIngredientsAvailable)
            {
                // Decrease the quantity of used items from the inventory
                foreach (RequiredIngredients ingredient in recipeToCraft.requiredIngredients)
                {
                    foreach (InventoryExtItemData existingItem in inventoryExtData.inventoryExtItemDataList.slotData)
                    {
                        if (existingItem.itemName == ingredient.itemName)
                        {
                            existingItem.jumlahItem -= ingredient.requiredQuantity;
                            Debug.Log("Decreased quantity of: " + ingredient.itemName + " by " + ingredient.requiredQuantity);
                            break;
                        }
                    }
                }

                InventoryExtItemData inventoryExtItemData = new InventoryExtItemData(myItemIdExt, recipeToCraft.output.itemName, recipeToCraft.output.itemImage, recipeToCraft.output.typeSampah, recipeToCraft.output.jenisSampah, recipeToCraft.output.jumlahItem);
                inventoryExtData.inventoryExtItemDataList.slotData.Add(inventoryExtItemData);
            }
            else
            {
                Debug.Log("Not enough ingredients to craft the item!");
            }
        }

        SaveSystem.SaveInventoryExt(inventoryExtData.inventoryExtItemDataList.slotData);
        inventoryExtData.LoadInventoryItem();
    }

    public void IncrementAndSaveItemId() {
        myItemIdExt++;
        PlayerPrefs.SetInt(ID_KEY, myItemIdExt);
        PlayerPrefs.Save();
    }
}
