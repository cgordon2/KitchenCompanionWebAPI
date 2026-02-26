using KitchenCompanionWebApi.Models;
using KitchenCompanionWebApi.Models.DatabaseFirst;
using KitchenCompanionWebApi.Models.DTOs;
using System.Runtime.CompilerServices;

namespace KitchenCompanionWebApi.Services
{
    public interface IRecipeService
    {
        Task<List<RecipeDto>> GetRecipes();
        Task<List<RecipeDto>> GetFavoriteRecipes(); 
        Task<RecipeDto> GetSingleRecipe(int recipeId); 
        Task<List<IngredientDto>> GetAllIngredients();
        Task<bool> DeleteRecipe(RecipeDto dto);
        Task<RecipeDto> EditRecipe(RecipeDto recipe);
        Task<RecipeDto> AddRecipe(string username, RecipeDto recipe);
        Task<bool> UnfavoriteRecipe(int recipeId, int userId);
        Task<List<RecipeDto>> GetFavRecipesPagination(string chefId, int skip, int pageSize); 
        Task<bool> FavoriteRecipe(int recipe, int userId); 
        Task<List<RecipeDto>> GetClonedRecipes(); 
        Task<IngredientDto> GetIngredientById(int ingredientId);
        Task<bool> DeleteIngredient(int ingredientId);
        Task<IngredientDto> AddIngredient(IngredientDto ingredient);
        Task<List<RecipeDto>> SearchForRecipes(string searchQuery);
        Task<bool> IsRecipeAuthed(int userId, RecipeDto dto); 

        Task<List<ShoppingListDto>> GetShoppingList(string username);
        Task<bool> DeleteShoppingList(int shoppingListId);
        Task<bool> MarkShoppingListComplete(int shoppingListId);
        Task<ShoppingListDto> CreateShoppingListItem(string text, string category, string username);

        Task<List<RecipeDto>> GetRecipesByUserId(int page, int pageSize, int userId);
        Task<List<RecipeDto>> GetRecipesByUserIdPagination(int page, int pageSize, int userId);

        Task<List<RecipeDto>> SearchRecipesAsync(RecipeSearchDto search);
        Task<List<RecipeDto>> GetRecipesPagination(int page, int pageSize, string? chefName, bool? isCloned = false); 
        Task BatchUpdateShoppingStatus(
    string username,
    List<string> markDone,
    List<string> markUndone);

        Task<List<RecipeDto>> SearchRecipesAsyncPagination(string query, int page, int pageSIze, string chefName);
        Task<List<Unit>> GetUnits();
        Task<List<Category>> GetCategories();

        Task<List<PantryDto>> GetPantryItems(string username);
        Task UpdatePantryQuantity(PantryDto dto);
        Task CreatePantryItem(List<PantryDto> dtos, string username);
        Task<List<PantryDto>> UpdatePantryByRecipe(string username, int pantryId, int quantity, int ingredientGuid);
        Task UpdatePantryByUser(string username, int pantryID, int quantity); 
    }
}
