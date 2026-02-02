namespace KitchenCompanionWebApi.Models.DTOs
{
    public class ShoppingListBatchUpdateDto
    {
        public List<string> MarkDone { get; set; } = new();
        public List<string> MarkUndone { get; set; } = new();
    }
}
