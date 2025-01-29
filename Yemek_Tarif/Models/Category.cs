namespace Yemek_Tarif.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        // İlişki
        public ICollection<Recipe> Recipes { get; set; }
    }
}
