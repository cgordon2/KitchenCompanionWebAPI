using System;
using System.Collections.Generic;

namespace KitchenCompanionWebApi.Models.DatabaseFirst;

public partial class User
{
    public int ChefId { get; set; }

    public int? RoleId { get; set; }

    public string? UserName { get; set; }

    public string UserPassword { get; set; } = null!;

    public string? Email { get; set; }

    public bool IsSetup { get; set; }

    public int? FollowersCount { get; set; }

    public int? FollowingCount { get; set; }

    public int? TotalRecipes { get; set; }

    public string? ShortBio { get; set; }

    public string? Language { get; set; }

    public string? Created { get; set; }

    public string? Location { get; set; }

    public string? AvatarUrl { get; set; }

    public string? RealName { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public virtual Role? Role { get; set; }
}
