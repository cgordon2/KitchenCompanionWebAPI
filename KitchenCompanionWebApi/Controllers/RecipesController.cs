using KitchenCompanionWebApi.Models.DTOs;
using KitchenCompanionWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace KitchenCompanionWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController(IRecipeService recipeService): ControllerBase
    {
        [HttpPost("Search")]
        public async Task<ActionResult<List<RecipeDto>>> SearchRecipes(string recipe)
        {
            var foundRecipes = await recipeService.SearchForRecipes(recipe);

            return new OkObjectResult(foundRecipes); 
        }

        [HttpGet("ShoppingList")]
        public async Task<ActionResult<List<ShoppingListDto>>> GetShoppingList(string username)
        {
            var shoppingList = await recipeService.GetShoppingList(username); 

            return new OkObjectResult(shoppingList); 
        }

        [HttpPost("DeleteShoppingListItem")]
        public async Task<ActionResult<bool>> DeleteShoppingItem(ShoppingListDto dto)
        {
            await recipeService.DeleteShoppingList(dto.Id); 
            return true; 
        }

        [HttpPost("MarkShoppingComplete")]
        public async Task<ActionResult<bool>> MarkShoppingListItemComplete(ShoppingListDto dto)
        {
            await recipeService.MarkShoppingListComplete(dto.Id);

            return true; 
        }


        [HttpPost("CreateShoppingListItem")]
        public async Task<ActionResult<ShoppingListDto>> CreateShoppingListItem(ShoppingListDto dto)
        {
            var test = await recipeService.CreateShoppingListItem(dto.Text, dto.Category, dto.UserName);

            return new OkObjectResult(test); 
        }


        [HttpGet("IngredientId")]
        public async Task<ActionResult<IngredientDto>> GetIngredientById(int id)
        {
            var ingredientDto = await recipeService.GetIngredientById(id); 

            return ingredientDto;
        } 

        [HttpGet("Ingredients")]
        public async Task<ActionResult<IngredientDto>> GetIngredients()
        {
            var ingredients = await recipeService.GetAllIngredients();

            return new OkObjectResult(ingredients);
        }

        [HttpGet("FavoriteList")]
        public async Task<ActionResult<List<RecipeDto>>> GetFavoriteRecipes()
        {
            var recipes = await recipeService.GetFavoriteRecipes();

            return recipes; 
        }

        [HttpGet("List")]
        public async Task<ActionResult<List<RecipeDto>>> GetRecipes()
        {
            var recipes = await recipeService.GetRecipes();

            return new OkObjectResult(recipes);
        }

        [HttpPost("AddIngredient")]
        public async Task<ActionResult<IngredientDto>> AddIngredient(IngredientDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ingredientDto = await recipeService.AddIngredient(dto);

            return new OkObjectResult(ingredientDto); 
        }

        [HttpPost("DeleteIngredient")]

        public async Task<ActionResult<IngredientDto>> DeleteIngredient(IngredientDto dto)
        {
            var isDeleted = await recipeService.DeleteIngredient(dto.IngredientId);

            return new OkObjectResult(isDeleted); 
        }

        [HttpGet("RecipeId")]
        public async Task<ActionResult<RecipeDto>> GetSingleRecipe(int id)
        {
            var recipe = await recipeService.GetSingleRecipe(id);

            return new OkObjectResult(recipe); 
        }

        [HttpPost("AddRecipe")]
        public async Task<ActionResult<bool>> AddRecipe(RecipeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await recipeService.AddRecipe(dto); 

            return await Task.FromResult(false); 
        }


        [HttpPost("DeleteRecipe")]
        public async Task<bool> DeleteRecipe(int id)
        { 
            return await Task.FromResult(false);
        }
    }
}
