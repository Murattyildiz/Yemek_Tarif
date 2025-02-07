using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yemek_Tarif.Models;

namespace Yemek_Tarif.Controllers
{
    public class RecipeController : Controller
    {
        private readonly RecipeAppContext _context;

        public RecipeController(RecipeAppContext context)
        {
            _context = context;
        }

        // GET: Recipe
        public async Task<IActionResult> Index()
        {
            var recipes = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .ToListAsync();
            return View(recipes);
        }

        // GET: Recipe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var recipe = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.RecipeId == id);

            if (recipe == null) return NotFound();

            return View(recipe);
        }

        // GET: Recipe/Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: Recipe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeId,Title,Description,Ingredients,Instructions,CategoryId")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                recipe.UserId = (int)HttpContext.Session.GetInt32("UserId"); // Giriş yapan kullanıcı
                recipe.DateCreated = DateTime.Now;

                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(recipe);
        }

        // GET: Recipe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();

            // Kullanıcı yetki kontrolü
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (recipe.UserId != userId)
            {
                TempData["ErrorMessage"] = "Yetkiniz yok"; // Hata mesajını ekle
                return RedirectToAction(nameof(Index));  // Kullanıcıyı ana sayfaya yönlendir
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(recipe);
        }

        // POST: Recipe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecipeId,Title,Description,Ingredients,Instructions,DateCreated,CategoryId")] Recipe recipe)
        {
            if (id != recipe.RecipeId) return NotFound();

            // Kullanıcı yetki kontrolü
            int? userId = HttpContext.Session.GetInt32("UserId");
            var originalRecipe = await _context.Recipes.AsNoTracking().FirstOrDefaultAsync(r => r.RecipeId == id);
            if (originalRecipe == null || originalRecipe.UserId != userId)
            {
                TempData["ErrorMessage"] = "Yetkiniz yok"; // Hata mesajını ekle
                return RedirectToAction(nameof(Index));  // Kullanıcıyı ana sayfaya yönlendir
            }

            if (ModelState.IsValid)
            {
                try
                {
                    recipe.UserId = originalRecipe.UserId; // Kullanıcı ID'si değişmesin
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.RecipeId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(recipe);
        }

        // GET: Recipe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var recipe = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.RecipeId == id);

            if (recipe == null) return NotFound();

            // Kullanıcı yetki kontrolü
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (recipe.UserId != userId)
            {
                TempData["ErrorMessage"] = "Yetkiniz yok"; // Hata mesajını ekle
                return RedirectToAction(nameof(Index));  // Kullanıcıyı ana sayfaya yönlendir
            }

            return View(recipe);
        }

        // POST: Recipe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);

            // Kullanıcı yetki kontrolü
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (recipe == null || recipe.UserId != userId)
            {
                TempData["ErrorMessage"] = "Yetkiniz yok"; // Hata mesajını ekle
                return RedirectToAction(nameof(Index));  // Kullanıcıyı ana sayfaya yönlendir
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Yorum Ekleme (AddComment)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int recipeId, string commentText)
        {
            if (string.IsNullOrEmpty(commentText))
            {
                TempData["ErrorMessage"] = "Yorum alanı boş olamaz!";
                return RedirectToAction("Details", new { id = recipeId });
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var comment = new Comment
            {
                RecipeId = recipeId,
                UserId = (int)userId,
                CommentText = commentText,
                DateCreated = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Recipe", new { id = recipeId });
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.RecipeId == id);
        }
    }
}
