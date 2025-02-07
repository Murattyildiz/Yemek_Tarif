using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Yemek_Tarif.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Instructions { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        // İlişkiler
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        // Ortalama puanı hesaplayan property (veritabanında saklanmaz)
        [NotMapped]
        public double AverageRating => Ratings.Any() ? Ratings.Average(r => r.RatingValue) : 0;
    }
}
