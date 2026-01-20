namespace KitchenCompanionWebApi.Models.DTOs
{
    public class RecipeSearchDto
    {
        public string AllWords { get; set; }
        public string ExactPhrase { get; set; }
        public string NoneWords { get; set; }
        public int loggedInUserGuid { get; set; } 
        public bool SearchOnlyUser { get; set; } 
    }
}
