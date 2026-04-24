using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    [Authorize]
    public class AnnouncementsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AnnouncementsController> _logger;
        private readonly IMemoryCache _cache;


        public AnnouncementsController(
            AppDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<AnnouncementsController> logger,
            IMemoryCache cache)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = await IsCurrentUserAdminAsync(userId);
            ViewData["IsAdmin"] = isAdmin;

            string cacheKey = "PublishedFlatsList";

            if (!_cache.TryGetValue(cacheKey, out List<Flat> flats))
            {
                flats = await _context.Flats
                    .Where(f => f.IsPublished)
                    .OrderByDescending(f => f.PublishedAt)
                    .ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(cacheKey, flats, cacheOptions);
            }

            return View(flats);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObjectType,Area,Rooms,Encumbrance,Condition,Address,PhoneNumber,Price,Description")] Suggestion suggestion)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            if (ModelState.IsValid)
            {
                suggestion.UserId = userId;
                suggestion.Status = SuggestionStatus.Pending;
                suggestion.CreatedAt = DateTime.UtcNow;
                _context.Suggestions.Add(suggestion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "✅ Ваше предложение отправлено на модерацию!";
                return RedirectToAction(nameof(Index));
            }

            return View(suggestion);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Moderation()
        {
            var pending = await _context.Suggestions
                .Where(s => s.Status == SuggestionStatus.Pending)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(pending);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);
            if (suggestion == null) return NotFound();

            var flat = new Flat
            {
                ObjectType = suggestion.ObjectType,
                Area = suggestion.Area,
                Rooms = suggestion.Rooms,
                Encumbrance = suggestion.Encumbrance,
                Condition = suggestion.Condition,
                Address = suggestion.Address,
                PhoneNumber = suggestion.PhoneNumber,
                Price = suggestion.Price,
                Description = suggestion.Description,
                ImageUrl = suggestion.ImageUrl,
                PublishedAt = DateTime.UtcNow,
                IsPublished = true
            };

            _context.Flats.Add(flat);
            suggestion.Status = SuggestionStatus.Approved;
            suggestion.ApprovedAt = DateTime.UtcNow;
            suggestion.ApprovedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _context.SaveChangesAsync();
            _cache.Remove("PublishedFlats");

            TempData["Success"] = "✅ Объявление опубликовано!";
            return RedirectToAction(nameof(Moderation));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);
            if (suggestion == null) return NotFound();

            suggestion.Status = SuggestionStatus.Rejected;
            suggestion.RejectedAt = DateTime.UtcNow;
            suggestion.RejectedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _context.SaveChangesAsync();

            TempData["Info"] = "❌ Предложение отклонено";
            return RedirectToAction(nameof(Moderation));
        }

        private async Task<bool> IsCurrentUserAdminAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId)) return false;
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, "Admin");
        }
    }
}