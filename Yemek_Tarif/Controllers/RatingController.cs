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

        // GET: Rating/Create
        public IActionResult Create(int recipeId)
        {
            ViewBag.RecipeId = recipeId;
            return View();
        }

        // POST: Rating/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RatingId, RatingValue, RecipeId, UserId")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                // Aynı kullanıcı, aynı tarife zaten oy vermişse, eski puanını güncelleyelim
                var existingRating = await _context.Ratings
                    .FirstOrDefaultAsync(r => r.RecipeId == rating.RecipeId && r.UserId == rating.UserId);

                if (existingRating != null)
                {
                    existingRating.RatingValue = rating.RatingValue;
                    _context.Update(existingRating);
                }
                else
                {
                    rating.DateCreated = DateTime.Now;
                    _context.Add(rating);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Recipe", new { id = rating.RecipeId });
            }
            ViewBag.RecipeId = rating.RecipeId;
            return View(rating);
        }

        // GET: Rating/Index
        public async Task<IActionResult> Index(int recipeId)
        {
            var ratings = await _context.Ratings
                .Include(r => r.User)
                .Where(r => r.RecipeId == recipeId)
                .ToListAsync();

            return View(ratings);
        }
    }
}
