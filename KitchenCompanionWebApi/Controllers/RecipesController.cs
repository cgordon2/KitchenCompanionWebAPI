using KitchenCompanionWebApi.Models.DatabaseFirst;
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
        public async Task<ActionResult<List<RecipeDto>>> SearchRecipes(RecipeSearchDto search)
        {
            var foundRecipes = await recipeService.SearchRecipesAsync(search);

            return foundRecipes; 
        }

        [HttpGet("ShoppingList")]
        public async Task<ActionResult<List<ShoppingListDto>>> GetShoppingList(string username)
        {
            var shoppingList = await recipeService.GetShoppingList(username); 

            return new OkObjectResult(shoppingList); 
        }

        public class FavoriteRequest
        {
            public int RecipeId { get; set; }
        }

        [HttpPost("FavRecipe")]
        public async Task<ActionResult<bool>> FavRecipe(FavoriteRequest dto)
        {
            await recipeService.FavoriteRecipe(dto.RecipeId);

            return new OkObjectResult(dto); 
        }

        [HttpPost("UnfavRecipe")]
        public async Task<ActionResult<bool>> UnfavRecipe(FavoriteRequest recipeId)
        {
            await recipeService.UnfavoriteRecipe(recipeId.RecipeId);

            return new OkObjectResult(false);
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

        [HttpGet("ListByUserId")]
        public async Task<ActionResult<List<RecipeDto>>> GetRecipesById(int page, int userId)
        {
            var recipes = await recipeService.GetRecipesByUserId(page, 3, userId);

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

        [HttpGet("ListClones")]
        public async Task<ActionResult<List<RecipeDto>>> GetClonedRecipes()
        {
            var cloned = await recipeService.GetClonedRecipes(); 

            return new OkObjectResult(cloned); 
        }

        [HttpPost("EditRecipe")]
        public async Task<ActionResult<bool>> EditRecipe(RecipeDto dto)
        {
            await recipeService.EditRecipe(dto);

            return new OkObjectResult(true); 
        }

        [HttpPost("UploadImage")]
        public async Task<ActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "UploadedImages");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new
            {
                file.FileName,
                file.Length,
                SavedTo = filePath
            });
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
