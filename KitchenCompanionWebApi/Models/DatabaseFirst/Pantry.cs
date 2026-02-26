using System;
using System.Collections.Generic;

namespace KitchenCompanionWebApi.Models.DatabaseFirst;

public partial class Pantry
{
    public int Id { get; set; }

    public string? IngredientGuid { get; set; }

    public int Quantity { get; set; }

    public string? UserName { get; set; }
}
