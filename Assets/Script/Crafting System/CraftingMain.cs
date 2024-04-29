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
        InventoryExt inventoryExtData = transform.parent.GetComponent<InventoryExt>();
        myItemIdExt = PlayerPrefs.GetInt(ID_KEY);

        var recipeToCraft = recipes[defaultSelectedItemIndex];

        bool itemExists = false;

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

            // Periksa ketersediaan semua bahan yang diperlukan
            foreach (RequiredIngredients ingredient in recipeToCraft.requiredIngredients)
            {
                bool ingredientFound = false;

                // Iterasi melalui setiap item dalam inventori
                foreach (InventoryExtItemData existingItem in inventoryExtData.inventoryExtItemDataList.slotData)
                {
                    // Jika nama item cocok
                    if (existingItem.itemName == ingredient.itemName)
                    {
                        // Periksa jumlah yang cukup
                        if (existingItem.jumlahItem >= ingredient.requiredQuantity)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                }

                // Jika salah satu bahan tidak ditemukan atau jumlahnya tidak mencukupi, set allIngredientsAvailable menjadi false
                if (!ingredientFound)
                {
                    allIngredientsAvailable = false;
                    break;
                }
            }

            // Jika semua bahan tersedia, buat item baru dan kurangi jumlah bahan dari inventori
            if (allIngredientsAvailable)
            {
                // Kurangi jumlah item yang digunakan dari inventori
                foreach (RequiredIngredients ingredient in recipeToCraft.requiredIngredients)
                {
                    foreach (InventoryExtItemData existingItem in inventoryExtData.inventoryExtItemDataList.slotData)
                    {
                        if (existingItem.itemName == ingredient.itemName)
                        {
                            existingItem.jumlahItem -= ingredient.requiredQuantity;
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
