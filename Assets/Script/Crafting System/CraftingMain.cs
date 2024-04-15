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

            // Loop through recipes to create buttons for each recipe
            for (int i = 0; i < recipes.Count; i++)
            {
                GameObject recipeButton = Instantiate(btnTemplate, btnRecipe);
                recipeButton.transform.SetParent(btnRecipe, false);
                Button buttonComponent = recipeButton.transform.GetChild(0).GetComponent<Button>();

                // Add a listener with the corresponding recipe
                CraftingRecipe currentRecipe = recipes[i];
                buttonComponent.onClick.AddListener(() => DisplayIngredients(currentRecipe));

                // Assign image to the button
                Image buttonImage = recipeButton.transform.GetChild(0).GetChild(0).GetComponent<Image>();
                buttonImage.sprite = currentRecipe.imageItemCraft;

                itemSelected[i] = recipeButton.transform.GetChild(0).GetComponent<ItemClickHandlerCrafting>();
            }

            // Display ingredients for the default recipe
            DisplayIngredients(recipes[0]);
        }
        else
        {
            Debug.LogError("No recipes available!");
        }
    }

    // Method to display ingredients for a recipe
    public void DisplayIngredients(CraftingRecipe recipe)
    {
        // Clear previous ingredients
        foreach (Transform child in imgIngredient)
        {
            Destroy(child.gameObject);
        }

        // Instantiate and display ingredients
        foreach (RequiredIngredients ingredient in recipe.requiredIngredients)
        {
            GameObject ingredientImage = Instantiate(imgIngredientTemplate, imgIngredient);
            ingredientImage.transform.SetParent(imgIngredient, false);
            Image imageComponent = ingredientImage.transform.GetChild(0).GetComponent<Image>();
            imageComponent.sprite = ingredient.imageIngredient;
            // You can add more customization here if needed, like displaying the name or quantity
        }
    }
}
