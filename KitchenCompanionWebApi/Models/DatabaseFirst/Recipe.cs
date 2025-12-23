using System;
using System.Collections.Generic;

namespace KitchenCompanionWebApi.Models.DatabaseFirst;

public partial class Recipe
{
    public int RecipeId { get; set; }

    public int ChefId { get; set; }

    public int? FavoriteId { get; set; }

    public int DishId { get; set; }

    public int CategoryId { get; set; }

    public string? RecipeName { get; set; }

    public string? RecipeDescription { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User Chef { get; set; } = null!;

    public virtual MealType Dish { get; set; } = null!;

    public virtual Favorite? Favorite { get; set; }

    public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}
