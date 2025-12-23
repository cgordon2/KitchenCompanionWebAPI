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

        Task<IngredientDto> GetIngredientById(int ingredientId);
        Task<bool> DeleteIngredient(int ingredientId);
        Task<IngredientDto> AddIngredient(IngredientDto ingredient);
        Task<List<RecipeDto>> SearchForRecipes(string searchQuery);
    }
}
