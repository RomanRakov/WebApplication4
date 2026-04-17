using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplication4.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WebApplication4.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApplication4.Controllers
{
    /// <summary>
    /// Контроллер объявлений с принудительной авторизацией
    /// При переходе на любую страницу контроллера незарегистрированный пользователь
    /// будет перенаправлен на страницу логина /Account/Login
    /// </summary>
    [Authorize]  // 🔒 ЭТОТ АТРИБУТ ОБЕСПЕЧИВАЕТ ПЕРЕНАПРАВЛЕНИЕ НА ВХОД БЕЗ ДОСТУПА
    public class AnnouncementsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AnnouncementsController> _logger;

        public AnnouncementsController(
            AppDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<AnnouncementsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Проверка роли Admin
            bool isAdmin = await IsCurrentUserAdminAsync(userId);
            ViewData["IsAdmin"] = isAdmin;

            _logger.LogInformation($"User {userId} accessing Announcements/Index");

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Forbid();

            var user = _context.Users.Find(userId);
            if (user != null)
            {
                // Проверяем, не является ли пользователь админом
                // Администраторы не создают предложения - они только модераторы
                var isAdminTask = _userManager.IsInRoleAsync(user, "Admin");
                isAdminTask.Wait();
                var isAdmin = isAdminTask.Result;

                if (isAdmin)
                {
                    TempData["Info"] = "Администраторы не добавляют объявления. Используйте панель модерации.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]  
        public async Task<IActionResult> Create([Bind] Suggestion suggestion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    // Устанавливаем статус на "На рассмотрении"
                    suggestion.UserId = userId;
                    suggestion.Status = Models.SuggestionStatus.Pending;
                    suggestion.CreatedAt = DateTime.UtcNow;

                    await _context.Suggestions.AddAsync(suggestion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "✅ Ваше предложение отправлено на модерацию!";
                    _logger.LogInformation($"User {userId} submitted new suggestion");

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Ошибка при создании предложения: " + ex.Message;
                    _logger.LogError(ex, "Ошибка создания предложения");
                    return View(suggestion);
                }
            }

            return View(suggestion);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]  
        public async Task<IActionResult> Moderation()
        {
            // Получаем все ожидающие рассмотрения предложения
            var pendingSuggestions = await _context.Suggestions
                .Where(s => s.Status == Models.SuggestionStatus.Pending)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            ViewData["Title"] = "Модерация объявлений";
            return View(pendingSuggestions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);
            if (suggestion == null || suggestion.Status != Models.SuggestionStatus.Pending)
            {
                TempData["Error"] = "Предложение уже обработано или не найдено.";
                return RedirectToAction(nameof(Moderation));
            }

            try
            {
                // 1. Создаём опубликованное объявление
                var flat = new Flat
                {
                    Title = suggestion.Title,
                    PropertyType = suggestion.PropertyType,
                    Area = suggestion.Area,
                    Rooms = suggestion.Rooms,
                    Price = suggestion.Price,
                    Address = suggestion.Address,
                    Condition = suggestion.Condition,
                    Description = suggestion.Description ?? "",
                    ImageUrl = suggestion.ImageUrl ?? "",
                    PublishedAt = DateTime.UtcNow,
                    IsPublished = true,
                    ModeratorUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                };

                await _context.Flats.AddAsync(flat);

                // 2. Помечаем предложение как одобренное
                suggestion.Status = Models.SuggestionStatus.Approved;
                suggestion.ApprovedAt = DateTime.UtcNow;
                suggestion.ApprovedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                TempData["Success"] = $"✅ Объявление «{suggestion.Title}» успешно опубликовано!";
                return RedirectToAction(nameof(Moderation));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка при публикации: " + ex.Message;
                _logger.LogError(ex, "Ошибка публикации объявления");
                return RedirectToAction(nameof(Moderation));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);
            if (suggestion == null || suggestion.Status != Models.SuggestionStatus.Pending)
            {
                TempData["Error"] = "Предложение уже обработано или не найдено.";
                return RedirectToAction(nameof(Moderation));
            }

            try
            {
                suggestion.Status = Models.SuggestionStatus.Rejected;
                suggestion.RejectedAt = DateTime.UtcNow;
                suggestion.RejectedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                TempData["Info"] = "❌ Предложение отклонено.";
                return RedirectToAction(nameof(Moderation));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка при отклонении: " + ex.Message;
                return RedirectToAction(nameof(Moderation));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);
            if (suggestion == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = await IsCurrentUserAdminAsync(userId);

            // Пользователь видит своё предложение или только одобренные
            if (!isAdmin && suggestion.UserId != userId && suggestion.Status != Models.SuggestionStatus.Approved)
            {
                TempData["Info"] = "Доступ запрещён.";
                return RedirectToAction(nameof(Index));
            }

            return View(suggestion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);
            if (suggestion == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (suggestion.UserId != userId)
                return Forbid();

            // Удаляем только если не опубликовано
            if (suggestion.Status == Models.SuggestionStatus.Pending ||
                suggestion.Status == Models.SuggestionStatus.Rejected)
            {
                _context.Suggestions.Remove(suggestion);
                await _context.SaveChangesAsync();
                TempData["Info"] = "Предложение удалено.";
            }
            else
            {
                TempData["Error"] = "Нельзя удалить опубликованное объявление.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> IsCurrentUserAdminAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, "Admin");
        }
    }
}