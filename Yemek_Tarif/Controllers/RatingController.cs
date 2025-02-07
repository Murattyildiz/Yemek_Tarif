using Microsoft.AspNetCore.Mvc;
using Yemek_Tarif.Models;
using Microsoft.EntityFrameworkCore;

namespace Yemek_Tarif.Controllers
{
    public class RatingController : Controller
    {
        private readonly RecipeAppContext _context;

        public RatingController(RecipeAppContext context)
        {
            _context = context;
        }

        // Puan ekleme veya güncelleme işlemi (Yorum yaparken puan da eklenir)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRating(int recipeId, int ratingValue)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId);

            if (existingRating != null)
            {
                // Kullanıcı zaten puan verdiyse, güncelle
                existingRating.RatingValue = ratingValue;
                _context.Update(existingRating);
            }
            else
            {
                // Yeni puan ekle
                var newRating = new Rating
                {
                    RecipeId = recipeId,
                    UserId = userId.Value,
                    RatingValue = ratingValue,
                    DateCreated = DateTime.Now
                };
                _context.Add(newRating);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Recipe", new { id = recipeId });
        }

        // Belirli bir tarifin puanlarını listeleme
        public async Task<IActionResult> Index(int recipeId)
        {
            var ratings = await _context.Ratings
                .Include(r => r.User)
                .Where(r => r.RecipeId == recipeId)
                .ToListAsync();

            return View(ratings);
        }

        // Belirli bir tarifin ortalama puanını hesaplama
        public async Task<double> GetAverageRating(int recipeId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.RecipeId == recipeId)
                .ToListAsync();

            if (ratings.Any())
            {
                return ratings.Average(r => r.RatingValue);
            }
            return 0;
        }
    }
}
