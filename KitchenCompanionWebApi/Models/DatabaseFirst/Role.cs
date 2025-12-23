using System;
using System.Collections.Generic;

namespace KitchenCompanionWebApi.Models.DatabaseFirst;

public partial class Role
{
    public int RoleId { get; set; }

    public string? Class { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
