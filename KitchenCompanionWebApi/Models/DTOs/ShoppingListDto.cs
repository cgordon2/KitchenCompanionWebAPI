namespace KitchenCompanionWebApi.Models.DTOs
{
    public class ShoppingListDto
    {
        public int Id { get; set; } 

        public string Text { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsDone { get; set; } 

        public string UserName { get; set; } 
    }
}
