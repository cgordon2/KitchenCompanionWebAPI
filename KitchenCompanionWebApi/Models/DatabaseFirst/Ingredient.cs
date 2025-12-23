using System;
using System.Collections.Generic;

namespace KitchenCompanionWebApi.Models.DatabaseFirst;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string? IngredientName { get; set; }

    public int? Quantity { get; set; }

    public int UnitId { get; set; }

    public int StoreId { get; set; }

    public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

    public virtual Store Store { get; set; } = null!;

    public virtual Unit Unit { get; set; } = null!;
}
