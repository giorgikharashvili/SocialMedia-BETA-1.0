using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Enums;
using SocialMedia.Models;
using SocialMedia.Repositories;
using SocialMedia.ViewModels;
using static SocialMedia.ViewModels.ReelPostingViewModel;

namespace SocialMedia.Controllers
{
    [Route("UserProfilePage")]
    public class UserProfilePageController : Controller
    {
        private readonly ILogger<UserProfilePageController> _logger;
        private readonly IRepository<ReelPostings> _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;


        
        public UserProfilePageController(IRepository<ReelPostings> repository, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _repository = repository;
            _userManager = userManager;
            _context = context;
        }


        [HttpGet("UserProfile/{userId?}")]
        public async Task<IActionResult> UserProfile(string? userId)
        {
            // Get current logged-in user's ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Index", "Home"); // Redirect if not logged in
            }

            // Check if the logged-in user is the same as the post author
            if (currentUserId == userId)
            {
                // Redirect to their own profile
                return RedirectToAction("OwnProfile", "UserProfilePage");
            }
            else
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Index", "Home"); // Redirect if userId is null or empty
                }

                // Fetch author user info
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return RedirectToAction("Index", "Home"); // Redirect if user not found
                }

                // Fetch author’s posts
                var userReels = (await _repository.GetAllAsync())
                    .Where(r => r.UserId == userId)
                    .ToList();

                var viewModel = new ReelPostingViewModel
                {
                    ReelPostings = userReels,
                    tempId = userId
                };


                ViewBag.UserProfileName = user.UserName;
                return View("OtherProfile", viewModel); // Load the author's profile page
            }
        }

        [HttpGet("OwnProfile")]
        public async Task<IActionResult> OwnProfile()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(currentUserId);
            var userReels = (await _repository.GetAllAsync())
                .Where(r => r.UserId == currentUserId)
                .ToList();

            // The list of accepted friendships
            var friendsList = await _context.Friendships
                .Where(f => (f.RequesterId == currentUserId || f.AddresseeID == currentUserId) && f.Status == FriendshipStatus.Accepted)
                .ToListAsync();

            var viewModel = new ReelPostingViewModel
            {
                ReelPostings = userReels,
                FriendsList = friendsList
            };

            ViewBag.UserProfileName = user.UserName;
            return View("UserProfile", viewModel); // Show logged-in user’s profile
        }

        //public async Task<IActionResult> Index()
        //{
        //    var userId = _userManager.GetUserId(User);

        //    // Fetch all posts (assuming there's a ReelPostings model)
        //    var reelPostings = await _context.ReelPostings
        //    .Include(r => r.User)
        //        .ToListAsync();

        //    // Fetch friends of the logged-in user
        //    var friends = await _context.Friendships
        //        .Where(f => (f.RequesterId == userId || f.AddresseeID == userId) && f.Status == FriendshipStatus.Accepted)
        //        .Select(f => f.RequesterId == userId ? f.Addresse : f.Requester) // Get the actual friend
        //        .ToListAsync();

        //    var viewModel = new ReelPostingViewModel
        //    {
        //        ReelPostings = reelPostings,
        //        Friends = friends
        //    };

        //    return View(viewModel);
        //}

    }

  
}
