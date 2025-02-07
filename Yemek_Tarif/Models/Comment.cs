namespace Yemek_Tarif.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public DateTime DateCreated { get; set; }

        // İlişkiler
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        // Yeni eklenen özellik: Kullanıcının puanı
        public int RatingValue { get; set; } // 1 ile 5 arasında bir değer alacak
    }
}
