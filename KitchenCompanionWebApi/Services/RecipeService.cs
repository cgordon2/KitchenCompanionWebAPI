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

        public async Task<List<RecipeDto>> SearchRecipesAsync(RecipeSearchDto search)
        {
            IQueryable<Recipe> query = _recipeEntitiesContext.Recipes;

            // ALL of these words (AND)
            if (!string.IsNullOrWhiteSpace(search.AllWords))
            {
                var words = search.AllWords
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words)
                {
                    query = query.Include(r => r.Category).Include(r => r.Favorite).Include(r => r.RecipeIngredients).Include(r => r.Chef).Where(r =>
                        r.RecipeName.Contains(word) ||
                        r.RecipeDescription.Contains(word));
                }
            }

            // EXACT phrase
            if (!string.IsNullOrWhiteSpace(search.ExactPhrase))
            {
                query = query.Include(r => r.Category).Include(r => r.Favorite).Include(r => r.RecipeIngredients).Include(r => r.Chef).Where(r =>
                    r.RecipeName.Contains(search.ExactPhrase) ||
                    r.RecipeDescription.Contains(search.ExactPhrase));
            }

            // NONE of these words (NOT)
            if (!string.IsNullOrWhiteSpace(search.NoneWords))
            {
                var excluded = search.NoneWords
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                query = query.Include(r => r.RecipeIngredients).Include(r => r.Favorite).Include(r => r.Category).Include(r => r.Chef).Where(r =>
    !r.RecipeName.Contains(search.NoneWords) &&
    !r.RecipeDescription.Contains(search.NoneWords)); 
                /*foreach (var word in excluded)
                {
                    query = query.Include(r => r.RecipeIngredients).Where(r =>
                        !r.RecipeName.Contains(word) &&
                        !r.RecipeDescription.Contains(word));
                }**/
            }

            if (search.SearchOnlyUser)
            {
                query = query.Include(r => r.RecipeIngredients).Include(r => r.Favorite).Include(r => r.Chef).Include(r => r.Category).Where(r => r.ChefId == search.loggedInUserGuid); 
            }
            return await query
                .Select(r => new RecipeDto
                {
                    RecipeId = r.RecipeId,
                    RecipeName = r.RecipeName,
                    Description = r.RecipeDescription,
                    ChefName = r.Chef.UserName, 
                    Photo = r.Photo, 
                    ChefEmail = r.Chef.Email, 
                    Stars = r.Stars, 
                    CookTime = r.CookTime, 
                    Prep = r.Prep, 
                    Serves = r.Serves,
                    Category = r.Category.Category1,
                    IsClone = r.IsClone, 
                    Favorite = r.Favorite.Favorite1,
                    Ingredients = r.RecipeIngredients
                        .Select(ri => new RecipeIngredientDto
                        {
                            RecipeId = r.RecipeId,
                            IngredientId = ri.IngredientId,
                            Quantity = ri.Quantity,
                            UnitId = ri.UnitId,

                            IngredientName = ri.Ingredient.IngredientName,
                            StoreName = ri.Ingredient.Store.StoreName,
                            StoreUrl = ri.Ingredient.Store.StoreUrl
                        })
                        .ToList()
                })
                .ToListAsync();

            ;
        }


        public async Task<List<ShoppingListDto>> GetShoppingList(string username)
        {
            var foundRecipes = await _recipeEntitiesContext.ShoppingLists
                .Where(r => r.UserName == username)
                .Select(r => new ShoppingListDto
                { 
                    IsDone = r.IsDone, 
                    Category = r.Category, 
                    Text = r.Description, 
                    UserName = username
                }) 
                .ToListAsync();

            return foundRecipes;
        }

        public async Task<ShoppingListDto> CreateShoppingListItem(string text, string category, string username)
        {
            var shoppingList = new ShoppingList();

            shoppingList.IsDone = false;
            shoppingList.Category = category;
            shoppingList.Description = text;
            shoppingList.UserName = username;
            _recipeEntitiesContext.ShoppingLists.Add(shoppingList);
            _recipeEntitiesContext.SaveChanges();

            var dto = new ShoppingListDto();

            dto.IsDone = shoppingList.IsDone;
            dto.Category = category;
            dto.Text = text;

            dto.Id = shoppingList.ShoppingListId; 

            return dto; 
        }

        public async Task<bool> DeleteShoppingList(int shoppingListId)
        {
            try
            {
                var listItem = await _recipeEntitiesContext.ShoppingLists.FirstOrDefaultAsync(r => r.ShoppingListId == shoppingListId);

                _recipeEntitiesContext.ShoppingLists.Remove(listItem);
                await _recipeEntitiesContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false; 
            }

            return true; 
        }

        public async Task<bool> MarkShoppingListComplete(int shoppingListId)
        {
            try
            {
                var listItem = await _recipeEntitiesContext.ShoppingLists.FirstOrDefaultAsync(r => r.ShoppingListId == shoppingListId);

                listItem.IsDone = true;

                await _recipeEntitiesContext.SaveChangesAsync(); 
            }
            catch (Exception ex)
            {
                return false; 
            }

            return true; 
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

        public async Task<bool> FavoriteRecipe(int recipeId)
        {
            var recipe = await _recipeEntitiesContext.Recipes.Include(r => r.Favorite)
            .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            recipe.Favorite.Favorite1 = "Yes";

            await _recipeEntitiesContext.SaveChangesAsync(); 

            return true; 
        }

        public async Task<bool> UnfavoriteRecipe(int recipeId)
        {
            var recipe = await _recipeEntitiesContext.Recipes.Include(r => r.Favorite)
             .FirstOrDefaultAsync(r => r.RecipeId == recipeId);

            recipe.Favorite.Favorite1 = "No"; 

            await _recipeEntitiesContext.SaveChangesAsync();

            return true;
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
            var recipe = await _recipeEntitiesContext.Recipes.Include(r => r.RecipeIngredients)
                 .FirstOrDefaultAsync(r => r.RecipeId == dto.RecipeId);

            if (recipe == null)
                return new RecipeDto { };

            recipe.RecipeName = dto.RecipeName;
            recipe.RecipeDescription = dto.Description;
            recipe.CategoryId = Convert.ToInt32(dto.Category); 

            _recipeEntitiesContext.RecipeIngredients.RemoveRange(
                recipe.RecipeIngredients
            );
             
            recipe.RecipeIngredients = dto.Ingredients
                .Select(i => new RecipeIngredient
                {
                    IngredientId = i.IngredientId,
                    Quantity = i.Quantity, 
                    UnitId = i.UnitId, 
                    RecipeId = recipe.RecipeId
                })
                .ToList(); 

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
            var recipeIngredients = dto.Ingredients;

            var user = await _recipeEntitiesContext.Users.FirstOrDefaultAsync(r => r.UserName == dto.ChefName);

            dto.Photo = "food.jpg"; 

            var isCloned = dto.IsClone; 

            var recipe = new Recipe
            {
                RecipeName = dto.RecipeName, 
                RecipeDescription = dto.Description, 
                ChefId = user.ChefId,
                CategoryId = Convert.ToInt32(dto.Category),
                DishId = 1, 
                Photo = dto.Photo, 
                Stars = dto.Stars, 
                CookTime = dto.CookTime,
                Prep = dto.Prep, 
                Serves = dto.Serves,
                IsClone = isCloned, 
                IsDeleted = false, 
                Favorite = new Favorite()
                {
                    Favorite1 = "No"
                },

                // Leave this empty: IF the user did not select an ingredient
                RecipeIngredients = new List<RecipeIngredient>()
            };

            _recipeEntitiesContext.Recipes.Add(recipe);
            _recipeEntitiesContext.SaveChanges();

            int recipeId = recipe.RecipeId;
            var riIngredients = new List<RecipeIngredient>(); 

            foreach (var item in recipeIngredients)
            {
                var riDB = new RecipeIngredient(); 

                riDB.IngredientId = item.IngredientId;
                riDB.RecipeId = recipeId;
                riDB.Quantity = 1;
                riDB.UnitId = 1;

                riIngredients.Add(riDB);
            }

            recipe.RecipeIngredients = riIngredients;
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
                StoreId = Convert.ToInt32(dto.StoreName),
                Quantity = 3,
                Photo = dto.Photo, 
                CreatedBy = dto.CreatedBy, 
                Stars = dto.Stars, 
                Preptime = dto.PrepTime, 
                CookTime = dto.CookTime, 
                Serves = dto.Serves
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
                StoreUrl = ingredient.Store?.StoreUrl, 
                Stars = ingredient.Stars, 
                Photo = ingredient.Photo, 
                PrepTime = ingredient.Preptime, 
                CookTime = ingredient.CookTime, 
                CreatedBy = ingredient.CreatedBy
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
                    StoreUrl = so.StoreUrl, 
                    CreatedBy = ig.CreatedBy, 
                    Stars = ig.Stars, 
                    PrepTime = ig.Preptime, 
                    Photo = ig.Photo, 
                    IngredientGUID = Convert.ToString(ig.IngredientId), 
                    CookTime = ig.CookTime, 
                    Serves = ig.Serves
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
                                    Stars = r.Stars,
                                    Photo = r.Photo,
                                    Prep = r.Prep,
                                    CookTime = r.CookTime,
                                    Serves = r.Serves,
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

        public async Task<List<RecipeDto>> GetClonedRecipes()
        {
            var recipes = await _recipeEntitiesContext.Recipes
    .Where(r => !r.IsDeleted)
    .Where(r => r.IsClone)
    .Where(r => !r.IsSetupRecipe)
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
        Stars = r.Stars,
        Photo = r.Photo,
        Prep = r.Prep,
        CookTime = r.CookTime,
        Serves = r.Serves,
        IsClone = r.IsClone,
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

        public async Task<List<RecipeDto>> GetRecipesByUserId(int page, int pageSize, int userId)
        {
            int skip = (page - 1) * pageSize;

            var recipes = await _recipeEntitiesContext.Recipes
                .Where(r => !r.IsDeleted)
                .Where(r => !r.IsSetupRecipe)
                .Where(r => r.ChefId == userId)
                .Include(r => r.Chef)
                .Include(r => r.Favorite)
                .Include(r => r.Category)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                        .ThenInclude(i => i.Store)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Unit)
                .OrderBy(r => r.RecipeId) // REQUIRED for pagination
                .Skip(skip)
                .Take(pageSize)
                .Select(r => new RecipeDto
                {
                    RecipeId = r.RecipeId,
                    RecipeName = r.RecipeName,
                    Description = r.RecipeDescription,
                    ChefName = r.Chef.UserName,
                    ChefEmail = r.Chef.Email,
                    Category = r.Category.Category1,
                    Favorite = r.Favorite.Favorite1,
                    Stars = r.Stars,
                    Photo = r.Photo,
                    Prep = r.Prep,
                    CookTime = r.CookTime,
                    Serves = r.Serves,
                    IsClone = r.IsClone,
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
                .Where(r => !r.IsDeleted)
                .Where(r => !r.IsClone)
                .Where(r => !r.IsSetupRecipe)
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
                    Stars = r.Stars, 
                    Photo = r.Photo, 
                    Prep = r.Prep, 
                    CookTime = r.CookTime,
                    Serves = r.Serves,
                    IsClone = r.IsClone,
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
