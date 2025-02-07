using System;
using System.ComponentModel.DataAnnotations;

namespace Yemek_Tarif.Models
{
    public class Rating
    {
        public int RatingId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int RatingValue { get; set; } // 1-5 arasında puan

        // İlişkiler
        [Required]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        // Puanın oluşturulma tarihi (Varsayılan olarak şimdi)
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
