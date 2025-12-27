using System;
using System.Collections.Generic;

namespace KitchenCompanionWebApi.Models.DatabaseFirst;

public partial class ShoppingList
{
    public int ShoppingListId { get; set; }

    public string UserName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public bool IsDone { get; set; }
}
