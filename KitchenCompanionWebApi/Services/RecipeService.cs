using KitchenCompanionWebApi.Models.DatabaseFirst;
using KitchenCompanionWebApi.Models.DTOs; 
using Microsoft.EntityFrameworkCore;

namespace KitchenCompanionWebApi.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly RecipeEntitiesContext _recipeEntitiesContext;

        public RecipeService(RecipeEntitiesContext context)
        {
            _recipeEntitiesContext = context;
        }

        public async Task<List<RecipeDto>> SearchForRecipes(string searchQuery)
        {
            var foundRecipes = await _recipeEntitiesContext.Recipes
                .Where(r => r.RecipeName.Contains(searchQuery) ||
                            r.RecipeDescription.Contains(searchQuery))
                .Select(r => new RecipeDto
                {
                    RecipeId = r.RecipeId,   
                    RecipeName = r.RecipeName,
                    Description = r.RecipeDescription
                })
                .Take(1000)
                .ToListAsync();

            return foundRecipes; 
        }



        public async Task<bool> DeleteRecipe(int id)
        {
            var recipe = await _recipeEntitiesContext.Recipes
                        .FirstOrDefaultAsync(r => r.RecipeId == id);

            if (recipe == null)
            {
                return false; // not found
            }
            else
            {
                _recipeEntitiesContext.Recipes.Remove(recipe);
                await _recipeEntitiesContext.SaveChangesAsync();
            }

                return true; 
        }

        public async Task<RecipeDto> EditRecipe(RecipeDto dto)
        {
            var recipe = await _recipeEntitiesContext.Recipes
                 .FirstOrDefaultAsync(r => r.RecipeId == dto.RecipeId);

            if (recipe == null)
                return new RecipeDto { };

            recipe.RecipeName = dto.RecipeName;
            recipe.RecipeDescription = dto.Description;

            await _recipeEntitiesContext.SaveChangesAsync();

            return new RecipeDto
            {
                RecipeId = recipe.RecipeId, 
                RecipeName = recipe.RecipeName,
                Description = recipe.RecipeDescription
            };
        }

        public async Task<RecipeDto> AddRecipe(RecipeDto dto)
        {
            var recipe = new Recipe
            {
                RecipeName = dto.RecipeName, 
                RecipeDescription = dto.Description, 
                ChefId = 2,
                CategoryId = 1,
                DishId = 1, 
                Favorite = new Favorite()
                {
                    Favorite1 = "Yes"
                },

                // Leave this empty:
                RecipeIngredients = new List<RecipeIngredient>()
            };

            _recipeEntitiesContext.Recipes.Add(recipe);
            _recipeEntitiesContext.SaveChanges(); 

            return dto;
        }

        public async Task<bool> DeleteIngredient(int id)
        {
            var ingredient = await _recipeEntitiesContext.Ingredients
                 .FirstOrDefaultAsync(r => r.IngredientId == id);

            if (ingredient == null)
            {
                return false; 
            }

            _recipeEntitiesContext.Ingredients.Remove(ingredient);
            _recipeEntitiesContext.SaveChanges(); 

            return true; 
        }

        public async Task<IngredientDto> AddIngredient(IngredientDto dto)
        {
            var ingredient = new Ingredient
            {
                IngredientName = dto.IngredientName, 
                UnitId = 1,     // Use ID of an existing Unit
                StoreId = 1,
                Quantity = 3,
               // RecipeIngredients = new List<RecipeIngredient>()
            };

            // Create the RecipeIngredient mapping
            // make default recipe that it links to and dont pull it from db
            /*var recipeIngredient = new RecipeIngredient
            {
                RecipeId = 11,
                Ingredient = ingredient,   // EF links them together
                Quantity = 3,
            };**/

            //ingredient.RecipeIngredients.Add(recipeIngredient);

            _recipeEntitiesContext.Ingredients.Add(ingredient);
            await _recipeEntitiesContext.SaveChangesAsync();

            // return with new ID
            dto.IngredientId = ingredient.IngredientId;
            /* var ingredient = new Ingredient
             {
                 RecipeIngredients = new List<RecipeIngredient>(),
                 IngredientName = dto.IngredientName,
                 UnitId = 12,
                 StoreId = 1,
                 Quantity = 3,
                 Unit = new Unit()
             } 


             _recipeEntitiesContext.Ingredients.Add(ingredient);
             await _recipeEntitiesContext.SaveChangesAsync();**/

            return dto; 
        }

        public async Task<IngredientDto> GetIngredientById(int id)
        {
            var ingredient = await _recipeEntitiesContext.Ingredients.FirstOrDefaultAsync(r => r.IngredientId == id); 

            if (ingredient == null)
                return new IngredientDto { };

            return new IngredientDto
            {
                IngredientId = ingredient.IngredientId,
                IngredientName = ingredient.IngredientName,
                UnitName = ingredient.Unit?.Unit1,       // assuming property name in Unit entity
                StoreName = ingredient.Store?.StoreName, // assuming property name in Store entity
                StoreUrl = ingredient.Store?.StoreUrl
            };
        }

        public async Task<List<IngredientDto>> GetAllIngredients()
        {
            var results = await (
                from ig in _recipeEntitiesContext.Ingredients
                join u in _recipeEntitiesContext.Units on ig.UnitId equals u.UnitId
                join so in _recipeEntitiesContext.Stores on ig.StoreId equals so.StoreId
                select new IngredientDto
                {
                    IngredientId = ig.IngredientId,
                    IngredientName = ig.IngredientName,
                    UnitName = u.Unit1,
                    StoreName = so.StoreName,
                    StoreUrl = so.StoreUrl
                }
            ).ToListAsync();

            return results;
        }

        public async Task<RecipeDto> GetSingleRecipe(int recipeId)
        {
            var recipe = await _recipeEntitiesContext.Recipes
                .Include(r => r.Chef)
                .Include(r => r.Favorite)
                .Include(r => r.Category)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                        .ThenInclude(i => i.Store)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Unit)
                .Where(r => r.RecipeId == recipeId)
                .Select(r => new RecipeDto
                {
                    RecipeId = r.RecipeId,
                    RecipeName = r.RecipeName,
                    Description = r.RecipeDescription,
                    ChefName = r.Chef.UserName,
                    ChefEmail = r.Chef.Email,
                    Category = r.Category.Category1,
                    Favorite = r.Favorite.Favorite1,
                    Ingredients = r.RecipeIngredients.Select(ri => new RecipeIngredientDto
                    {
                        RecipeId = ri.RecipeId,
                        IngredientId = ri.IngredientId,
                        Quantity = ri.Quantity,
                        UnitId = ri.UnitId,
                        IngredientName = ri.Ingredient.IngredientName,
                        StoreName = ri.Ingredient.Store.StoreName,
                        StoreUrl = ri.Ingredient.Store.StoreUrl,
                        UnitName = ri.Unit != null ? ri.Unit.Unit1 : null
                    }).ToList()
                })
                .FirstOrDefaultAsync(); 

            if (recipe == null)
            {
                return new RecipeDto(); 
            }

            return recipe; 
        }

        public async Task<List<RecipeDto>> GetFavoriteRecipes()
        {
            var recipes = await _recipeEntitiesContext.Recipes
                                .Include(r => r.Chef)
                                .Include(r => r.Favorite)
                                .Include(r => r.Category)
                                .Include(r => r.RecipeIngredients)
                                    .ThenInclude(ri => ri.Ingredient)
                                        .ThenInclude(i => i.Store)
                                .Include(r => r.RecipeIngredients)
                                    .ThenInclude(ri => ri.Unit)
                                .Where(r => r.Favorite.Favorite1 == "Yes")
                                .Where(r => r.ChefId == 12)
                                .Select(r => new RecipeDto
                                {
                                    RecipeId = r.RecipeId,
                                    RecipeName = r.RecipeName,
                                    Description = r.RecipeDescription,
                                    ChefName = r.Chef.UserName,
                                    ChefEmail = r.Chef.Email,
                                    Category = r.Category.Category1,
                                    Favorite = r.Favorite.Favorite1,
                                    Ingredients = r.RecipeIngredients.Select(ri => new RecipeIngredientDto
                                    {
                                        RecipeId = ri.RecipeId,
                                        IngredientId = ri.IngredientId,
                                        Quantity = ri.Quantity,
                                        UnitId = ri.UnitId,
                                        IngredientName = ri.Ingredient.IngredientName,
                                        StoreName = ri.Ingredient.Store.StoreName,
                                        StoreUrl = ri.Ingredient.Store.StoreUrl,
                                        UnitName = ri.Unit != null ? ri.Unit.Unit1 : null
                                    }).ToList()
                                })
                                .OrderBy(r => r.RecipeId)
                                .ToListAsync();

            return recipes; 
        }

        /** Fetch batch then process on device ***/
        public async Task<List<RecipeDto>> GetRecipes()
        {
            var recipes = await _recipeEntitiesContext.Recipes
                .Include(r => r.Chef) 
                .Include(r => r.Favorite) 
                .Include(r => r.Category)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                        .ThenInclude(i => i.Store)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Unit) 
                .Select(r => new RecipeDto
                {
                    RecipeId = r.RecipeId,
                    RecipeName = r.RecipeName,
                    Description = r.RecipeDescription,
                    ChefName = r.Chef.UserName,
                    ChefEmail = r.Chef.Email,
                    Category = r.Category.Category1, 
                    Favorite = r.Favorite.Favorite1,
                    Ingredients = r.RecipeIngredients.Select(ri => new RecipeIngredientDto
                    {
                        RecipeId = ri.RecipeId,
                        IngredientId = ri.IngredientId,
                        Quantity = ri.Quantity,
                        UnitId = ri.UnitId,
                        IngredientName = ri.Ingredient.IngredientName,
                        StoreName = ri.Ingredient.Store.StoreName,
                        StoreUrl = ri.Ingredient.Store.StoreUrl,
                        UnitName = ri.Unit != null ? ri.Unit.Unit1 : null
                    }).ToList()
                })
                .OrderBy(r => r.RecipeId)
                .ToListAsync();

            return recipes; 
        }
    }
}
