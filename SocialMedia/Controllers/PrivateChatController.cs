using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models;
using SocialMedia.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Controllers
{
    public class PrivateChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PrivateChatController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> PrivateChat(string friendId)
        {
            var userId = _userManager.GetUserId(User);
            if (friendId == null || userId == null)
            {
                return BadRequest("Friend Id or user Id is null");
            }

            var messages = await _context.Messages
                .Where(m => (m.SenderId == userId && m.ReceiverId == friendId) ||
                            (m.SenderId == friendId && m.ReceiverId == userId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            var model = new MessageViewModel
            {
                Messages = messages,
                FriendId = friendId,
                UserId = userId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null || model.FriendId == null || string.IsNullOrWhiteSpace(model.Content))
            {
                return RedirectToAction("PrivateChat", new { friendId = model.FriendId });
            }

            var message = new Message
            {
                SenderId = userId,
                ReceiverId = model.FriendId,
                Content = model.Content,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("PrivateChat", new { friendId = model.FriendId });
        }
    }
}
