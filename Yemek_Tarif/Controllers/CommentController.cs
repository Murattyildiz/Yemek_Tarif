using Microsoft.AspNetCore.Mvc;
using Yemek_Tarif.Models;
using Microsoft.EntityFrameworkCore;

namespace Yemek_Tarif.Controllers
{
    public class CommentController : Controller
    {
        private readonly RecipeAppContext _context;

        public CommentController(RecipeAppContext context)
        {
            _context = context;
        }

        // GET: Comment/Create
        public IActionResult Create(int recipeId)
        {
            ViewBag.RecipeId = recipeId;
            return View();
        }

        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId, CommentText, DateCreated, RecipeId, UserId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.DateCreated = DateTime.Now; // Yorumun oluşturulma tarihi
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Recipe", new { id = comment.RecipeId });
            }
            ViewBag.RecipeId = comment.RecipeId; // Eğer yorum eklenemezse, form tekrar gösterilir
            return View(comment);
        }

        // GET: Comment/Index
        public async Task<IActionResult> Index(int recipeId)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.RecipeId == recipeId)
                .ToListAsync();

            return View(comments);
        }
    }
}
