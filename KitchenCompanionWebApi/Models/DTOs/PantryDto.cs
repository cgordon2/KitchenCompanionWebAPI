namespace KitchenCompanionWebApi.Models.DTOs
{
    public class PantryDto
    {
        public string IngredientGUID { get; set; } 
        public int PantryID { get; set; } 
        public int Quantity { get; set; }
        public int? RecipeId { get; set; } 
        public string UserName { get; set; } 
        public string? IngredientName { get; set; } 
    }
}
