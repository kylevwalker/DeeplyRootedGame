using System;
using UnityEngine;

public class ObjectCraftingController : MonoBehaviour
{
    [Header("Recipes")]
    public CraftingRecipe[] craftingRecipes;

    [Header("Debug")]
    public bool enableDebugMode;

    public void CheckInventoryForRecipes()
    {
        try
        {
            foreach (CraftingRecipe recipeToCheck in craftingRecipes)
            {
                bool allItemsPickedUp = true;
                foreach (GameObject materialToCheck in recipeToCheck.craftingMaterialObjects)
                {
                    if (!materialToCheck.GetComponent<InteractableItemController>().itemInInventory)
                    {
                        allItemsPickedUp = false;
                    }
                }
                if (allItemsPickedUp)
                {
                    foreach (GameObject recipeMaterial in recipeToCheck.craftingMaterialObjects)
                    {
                        //recipeMaterial.GetComponent<InteractableItemController>().consumedCraftingMaterial = true;
                        recipeMaterial.SetActive(false);
                    }
                    CraftObjects(recipeToCheck);
                }
            }
        }
        catch
        {
            if (enableDebugMode)
            {
                Debug.LogWarning("No Existing Crafting Recipes To Check");
            }
        }
    }

    [Serializable]
    public class CraftingRecipe
    {
        [Header("Recipe Data")]
        public string recipeName;
        public GameObject[] objectsToCraft;
        public GameObject[] craftingMaterialObjects;
        [HideInInspector]
        public bool crafted = false;
    }

    public void CraftObjects(CraftingRecipe craftingRecipe)
    {
        foreach (GameObject currentObjectToCraft in craftingRecipe.objectsToCraft)
        {
            craftingRecipe.crafted = true;
            currentObjectToCraft.SetActive(true);
            gameObject.GetComponent<PlayerInventoryController>().AddToInventory(currentObjectToCraft);
        }
    }
}
