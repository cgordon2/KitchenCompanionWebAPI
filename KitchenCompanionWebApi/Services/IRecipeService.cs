using KitchenCompanionWebApi.Models;
using KitchenCompanionWebApi.Models.DatabaseFirst;
using KitchenCompanionWebApi.Models.DTOs;

namespace KitchenCompanionWebApi.Services
{
    public interface IRecipeService
    {
        Task<List<RecipeDto>> GetRecipes();
        Task<List<RecipeDto>> GetFavoriteRecipes(); 
        Task<RecipeDto> GetSingleRecipe(int recipeId); 
        Task<List<IngredientDto>> GetAllIngredients();
        Task<bool> DeleteRecipe(int recipeId);
        Task<RecipeDto> EditRecipe(RecipeDto recipe);
        Task<RecipeDto> AddRecipe(RecipeDto recipe);
        Task<List<RecipeDto>> GetClonedRecipes(); 
        Task<IngredientDto> GetIngredientById(int ingredientId);
        Task<bool> DeleteIngredient(int ingredientId);
        Task<IngredientDto> AddIngredient(IngredientDto ingredient);
        Task<List<RecipeDto>> SearchForRecipes(string searchQuery);

        Task<List<ShoppingListDto>> GetShoppingList(string username);
        Task<bool> DeleteShoppingList(int shoppingListId);
        Task<bool> MarkShoppingListComplete(int shoppingListId);
        Task<ShoppingListDto> CreateShoppingListItem(string text, string category, string username); 
    }
}
