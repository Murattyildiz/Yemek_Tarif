namespace Yemek_Tarif.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int RatingValue { get; set; }

        // İlişkiler
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
