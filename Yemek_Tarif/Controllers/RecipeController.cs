using Microsoft.AspNetCore.Mvc;
using Yemek_Tarif.Models;
using Microsoft.EntityFrameworkCore;

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
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.Comments) // Yorumları da dahil ediyoruz
                .ThenInclude(c => c.User) // Yorum yapan kullanıcıyı ekliyoruz
                .FirstOrDefaultAsync(m => m.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: Recipe/Create
        public IActionResult Create()
        {
            // Kategorileri ViewBag üzerinden gönderiyoruz
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: Recipe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeId,Title,Description,Ingredients,Instructions,UserId,CategoryId")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                // DateCreated alanını ekliyoruz
                recipe.DateCreated = DateTime.Now;

                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Kategoriler tekrar gönderiliyor, çünkü model invalidse yeniden form render edilecek
            ViewBag.Categories = _context.Categories.ToList();
            return View(recipe);
        }

        // GET: Recipe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            // Kategorileri ViewBag üzerinden gönderiyoruz
            ViewBag.Categories = _context.Categories.ToList();
            return View(recipe);
        }

        // POST: Recipe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecipeId,Title,Description,Ingredients,Instructions,DateCreated,UserId,CategoryId")] Recipe recipe)
        {
            if (id != recipe.RecipeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.RecipeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(recipe);
        }

        // GET: Recipe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.RecipeId == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
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
                // Eğer yorum boş ise, kullanıcıya bir hata mesajı dönebiliriz
                TempData["ErrorMessage"] = "Yorum alanı boş olamaz!";
                return RedirectToAction("Details", new { id = recipeId });
            }

            // Yorum eklemek için yeni bir Comment nesnesi oluşturuyoruz
            var comment = new Comment
            {
                RecipeId = recipeId,
                UserId = (int)HttpContext.Session.GetInt32("UserId"), // Kullanıcı ID'sini oturumdan alıyoruz
                CommentText = commentText,
                DateCreated = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Yorum eklendikten sonra, kullanıcının tarifin detay sayfasına yönlendirilmesi sağlanır
            return RedirectToAction("Details", "Recipe", new { id = recipeId });
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.RecipeId == id);
        }
    }
}
