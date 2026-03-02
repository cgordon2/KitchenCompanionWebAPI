using KitchenCompanionWebApi.Models.DatabaseFirst;
using KitchenCompanionWebApi.Models.DTOs;
using KitchenCompanionWebApi.Models.DTOs.Pagination;
using KitchenCompanionWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
namespace KitchenCompanionWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController(IRecipeService recipeService): ControllerBase
    {
        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpPost("Search")]
        public async Task<ActionResult<List<RecipeDto>>> SearchRecipes(RecipeSearchDto search)
        {
            var foundRecipes = await recipeService.SearchRecipesAsync(search);

            return foundRecipes; 
        }

	[Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("GetPantryItems")]
        public async Task<ActionResult<List<PantryDto>>> GetPantryItems()
        {
            var test = User.Identity?.Name;

            if (test == null)
                return Unauthorized();

            var items = await recipeService.GetPantryItems(test);

            return items; 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("SearchWeb")]
        public async Task<ActionResult<List<RecipeDto>>> SearchRecipesWeb(string query, int page = 1,
    int pageSize = 20, string? chefName = null)
        {
            var test = User.Identity?.Name;

            if (test == null)
                return Unauthorized(); 

            var recipes = await recipeService.SearchRecipesAsyncPagination(query, page, pageSize, chefName); 
            return new OkObjectResult(recipes); 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpPost("CreatePantryItems")]
        public async Task<ActionResult<bool>> CreatePantryItem(List<PantryDto> pantryDtos)
        {
	    var test = User.Identity?.Name; 
	    if (test == null)
		return Unauthorized(); 

            await recipeService.CreatePantryItem(pantryDtos, test);
            return true; 
        }

        [HttpPost("UpdatePantryQuantity")]
        public async Task<ActionResult<bool>> UpdatePantryQuantity(PantryDto dto)
        {
            await recipeService.UpdatePantryQuantity(dto); 

            return true; 
        }

        [HttpPost("UpdatePantryByUser")]
        public async Task<ActionResult<bool>> UpdatePantryByUser(PantryDto dto)
        {
            await recipeService.UpdatePantryByUser("cameron", dto.PantryID, dto.Quantity); 

            return true;
        }

        [HttpPost("UpdatePantryByRecipe")]
        public async Task<ActionResult<List<PantryDto>>> UpdatePantryByRecipe(PantryDto dto)
        {
            // @TODO: Need to pass in qty from dto.quantity
            var pantryItems = await recipeService.UpdatePantryByRecipe("cameron", Convert.ToInt32(dto.PantryID), dto.Quantity, Convert.ToInt32(dto.IngredientGUID));  // then do select to dbo.ingredients to find out the qty = 1002, and 4018

            return pantryItems; 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("ShoppingList")]
        public async Task<ActionResult<List<ShoppingListDto>>> GetShoppingList(string username)
        {
            var test = User.Identity?.Name;

            if (test == null)
                return Unauthorized();

            var shoppingList = await recipeService.GetShoppingList(test);

            return new OkObjectResult(shoppingList);
        }

        public class FavoriteRequest
        {
            public int RecipeId { get; set; }
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpPost("FavRecipe")]
        public async Task<ActionResult<bool>> FavRecipe(FavoriteRequest dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            int id = int.Parse(userId);

            await recipeService.FavoriteRecipe(dto.RecipeId, id);
            return new OkObjectResult(dto); 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpPost("UnfavRecipe")]
        public async Task<ActionResult<bool>> UnfavRecipe(FavoriteRequest recipeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            int id = int.Parse(userId);


            await recipeService.UnfavoriteRecipe(recipeId.RecipeId, id);

            return new OkObjectResult(false);
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpPost("DeleteShoppingListItem")]
        public async Task<ActionResult<bool>> DeleteShoppingItem(ShoppingListDto dto)
        {
            await recipeService.DeleteShoppingList(dto.Id); 
            return true; 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpPost("UpdateShoppingList")]
        public async Task<ActionResult<bool>> MarkShoppingListItemComplete([FromBody] ShoppingListBatchUpdateDto dto)
        {
            if (!dto.MarkDone.Any() && !dto.MarkUndone.Any())
                return BadRequest("No updates provided");
            var test = User.Identity?.Name;

            if (test == null)
                return Unauthorized();
            await recipeService.BatchUpdateShoppingStatus(test, dto.MarkDone, dto.MarkUndone); 


            return true; 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpPost("CreateShoppingListItem")]
        public async Task<ActionResult<ShoppingListDto>> CreateShoppingListItem(ShoppingListDto dto)
        { 
            var username = User.Identity?.Name;

            if (username == null)
                return Unauthorized(); 

            var test = await recipeService.CreateShoppingListItem(dto.Text, dto.Category, username);

            return new OkObjectResult(test); 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("IngredientId")]
        public async Task<ActionResult<IngredientDto>> GetIngredientById(int id)
        {
            var ingredientDto = await recipeService.GetIngredientById(id); 

            return ingredientDto;
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("FavByPag")]
        public async Task<ActionResult<List<RecipeDto>>> GetFavoritesPagination([FromQuery] RecipeListRequest re)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
                return Unauthorized();


            re.Page = Math.Max(re.Page, 1);

            re.PageSize = Math.Clamp(re.PageSize, 1, 10); // safer than 100

            if (!string.IsNullOrEmpty(re.ChefName))
            {
                if (re.ChefName.Length > 64)
                    return BadRequest("Chef name too long.");
            }

            // Prevent absurd offsets (DB protection)
            var offset = (re.Page - 1) * re.PageSize;

            if (offset > 100_000)
                return BadRequest("Page too large."); 

            var recipes = await recipeService.GetFavRecipesPagination(re.ChefName, re.Page, re.PageSize); 

            return new OkObjectResult(recipes); 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("Ingredients")]
        public async Task<ActionResult<IngredientDto>> GetIngredients()
        {
            var ingredients = await recipeService.GetAllIngredients();

            return new OkObjectResult(ingredients);
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("FavoriteList")]
        public async Task<ActionResult<List<RecipeDto>>> GetFavoriteRecipes()
        {
            var recipes = await recipeService.GetFavoriteRecipes();

            return recipes; 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("Categories")]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var categories = await recipeService.GetCategories();

            return categories; 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("ListByUserIdPagination")]
        public async Task<ActionResult<List<RecipeDto>>> ListByUserIdPagination(int page, int userId)
        {
            var recipes = await recipeService.GetRecipesByUserIdPagination(page, 5, userId);

            return recipes;
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("Units")]
        public async Task<ActionResult<List<Unit>>> GetUnits()
        {
            var units = await recipeService.GetUnits();

            return units; 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("ListByUserId")]
        public async Task<ActionResult<List<RecipeDto>>> GetRecipesById(int page, int userId)
        {

            var recipes = await recipeService.GetRecipesByUserId(page, 3, userId);

            return recipes; 
        }

        /** int page = 1,
    int pageSize = 20, string? chefName = null, bool? isCloned = false**/
        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("ListWebsite")]
        public async Task<ActionResult<List<RecipeDto>>> GetRecipesWebsite([FromQuery] RecipeListRequest req)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
                return Unauthorized();

            // ----- VALIDATION -----

            req.Page = Math.Max(req.Page, 1);

            req.PageSize = Math.Clamp(req.PageSize, 1, 10); // safer than 100

            if (!string.IsNullOrEmpty(req.ChefName))
            {
                if (req.ChefName.Length > 64)
                    return BadRequest("Chef name too long.");
            }

            // Prevent absurd offsets (DB protection)
            var offset = (req.Page - 1) * req.PageSize;

            if (offset > 100_000)
                return BadRequest("Page too large.");

            // ----- SERVICE CALL -----

            var recipes = await recipeService.GetRecipesPagination(
                req.Page,
                req.PageSize,
                req.ChefName,
                req.IsCloned
            );

            
            return Ok(recipes);
            /*var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(); 
            }

            if (userId != null)
            {
                int id = int.Parse(userId);

                req.Page = Math.Max(req.Page, 1);

                req.PageSize = Math.Clamp(req.PageSize, 1, 100); // HARD LIMIT

                var recipes = await recipeService.GetRecipesPagination(page, pageSize, chefName, isCloned);

                return Ok(recipes);
            }**/  
        }


        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("List")]
        public async Task<ActionResult<List<RecipeDto>>> GetRecipes()
        {
            var recipes = await recipeService.GetRecipes();

            return new OkObjectResult(recipes);
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
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

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]

        [HttpPost("DeleteIngredient")]

        public async Task<ActionResult<IngredientDto>> DeleteIngredient(IngredientDto dto)
        {
            var isDeleted = await recipeService.DeleteIngredient(dto.IngredientId);

            return new OkObjectResult(isDeleted); 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("RecipeId")]
        public async Task<ActionResult<RecipeDto>> GetSingleRecipe(int id)
        {
            var test = User.Identity?.Name;

            if (test == null)
                return Unauthorized();

            var recipe = await recipeService.GetSingleRecipe(id);

            if (recipe != null)
            {
                if (recipe.IsDeleted == true)
                    return Forbid();

                return new OkObjectResult(recipe);
            }

            return new OkObjectResult(false); 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpGet("ListClones")]
        public async Task<ActionResult<List<RecipeDto>>> GetClonedRecipes()
        {
            var cloned = await recipeService.GetClonedRecipes(); 

            return new OkObjectResult(cloned); 
        }

        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpPost("EditRecipe")]
        public async Task<ActionResult<bool>> EditRecipe(RecipeDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            if (userId != null)
            {
                int id = int.Parse(userId);

                if (await recipeService.IsRecipeAuthed(id, dto))
                {
                    await recipeService.EditRecipe(dto);
                    return new OkObjectResult(true);
                    // 
                }
            }

            return new OkObjectResult(false); 
        }

        [HttpPost("UploadImage")]
        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
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
        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        public async Task<ActionResult<bool>> AddRecipe(RecipeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var test = User.Identity?.Name;

            if (test == null)
                return Unauthorized(); 

            await recipeService.AddRecipe(test, dto); 

            return await Task.FromResult(false); 
        }

        // http://127.0.0.1:5500/pages/#detail?id=2008
        [Authorize(AuthenticationSchemes = "JwtBearer,JwtCookie")]
        [HttpPost("DeleteRecipe")]
        public async Task<ActionResult<bool>> DeleteRecipe(RecipeDto dto)
        { 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            var test = User.Identity?.Name;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            if (userId != null)
            {
                int id = int.Parse(userId);

                if (await recipeService.IsRecipeAuthed(id, dto))
                {
                    await recipeService.DeleteRecipe(dto); 
                    return new OkObjectResult(true);
                    // 
                }
            }

            return await Task.FromResult(false);
        }
    }
}
