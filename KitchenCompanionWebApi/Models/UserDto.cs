namespace KitchenCompanionWebApi.Models
{
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;

        public string? Password { get; set; } = string.Empty; 

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

        public int? ChefId { get; set; } 

        public string? Email { get; set; } 
    }
}
