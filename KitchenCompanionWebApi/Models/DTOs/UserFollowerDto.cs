namespace KitchenCompanionWebApi.Models.DTOs
{
    public class UserFollowerDto
    {
        public int ChefId { get; set; }
        public string? UserName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? ShortBio { get; set; }
        public string? Location { get; set; }
        public int? FollowersCount { get; set; }
        public int? FollowingCount { get; set; }
    }
}
