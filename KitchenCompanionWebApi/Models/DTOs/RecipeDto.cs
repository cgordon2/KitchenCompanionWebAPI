using System.Text.Json.Serialization;

namespace KitchenCompanionWebApi.Models.DTOs
{
    public class RecipeDto
    {
        public int RecipeId { get; set; }
        [JsonPropertyName("recipeName")]
        public string RecipeName { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        public string ChefName { get; set; }
        public string ChefEmail { get; set; }
        public string Category { get; set; } 
        public string Favorite { get; set; }
        public List<RecipeIngredientDto> Ingredients { get; set; } = new();
    }

    public class RecipeIngredientDto
    {
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public int Quantity { get; set; }
        public int? UnitId { get; set; }
        public string IngredientName { get; set; }
        public string StoreName { get; set; }
        public string StoreUrl { get; set; }
        public string? UnitName { get; set; }
    }
}
