namespace KitchenCompanionWebApi.Models.DTOs.Pagination
{
    public class RecipeListRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? ChefName { get; set; }
        public bool IsCloned { get; set; }
    }
}
