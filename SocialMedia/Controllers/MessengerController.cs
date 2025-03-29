using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models;
using SocialMedia.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Controllers
{
    public class MessengerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MessengerController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet("Messenger/Friends")]
        public async Task<IActionResult> Messages()
        {
            if (_userManager == null)
            {
                return BadRequest("User Manager is null");
            }

            var userId = _userManager.GetUserId(User);

            var friends = await _context.Friendships
                .Where(f => (f.RequesterId == userId || f.AddresseeID == userId) && f.Status == Enums.FriendshipStatus.Accepted)
                .Select(f => new FriendsNamesViewModel
                {
                    Id = f.RequesterId == userId ? f.AddresseeID : f.RequesterId,
                    FriendName = _context.Users
                        .Where(u => u.Id == (f.RequesterId == userId ? f.AddresseeID : f.RequesterId))
                        .Select(u => u.UserName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return View(friends); // Passing a list of FriendsNamesViewModel
        }



    }
}
