using System;
using System.Collections.Generic;

namespace KitchenCompanionWebApi.Models.DatabaseFirst;

public partial class Store
{
    public int StoreId { get; set; }

    public string? StoreName { get; set; }

    public string? StoreUrl { get; set; }

    public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
}
