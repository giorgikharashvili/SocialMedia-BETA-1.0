using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Enums;
using SocialMedia.Models;
using SocialMedia.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Controllers
{
    public class FriendsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FriendsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //  Send Friend Request & Create Notification
        [HttpPost("send-request")]
        public async Task<IActionResult> SendFriendRequests(string? data)
        {
            var userId = _userManager.GetUserId(User);

            if (data == null)
                return BadRequest("Recipient ID is null");

            if (userId == null || userId == data)
                return BadRequest("You cannot add yourself as a friend");

            if (_context.Friendships == null)
                return Problem("Friendships DbSet is null");

            var existingRequest = await _context.Friendships
                .FirstOrDefaultAsync(f => (f.RequesterId == userId && f.AddresseeID == data) ||
                                          (f.RequesterId == data && f.AddresseeID == userId));

            if (existingRequest != null)
                return BadRequest("Friend request already exists");

            var friendship = new Friendship
            {
                RequesterId = userId,
                AddresseeID = data,
                Status = FriendshipStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            //  Create a Notification for the Friend Request
            var notification = new Notification
            {
                UserId = data,  // Recipient gets notified
                Message = $"You have a new friend request from {_userManager.Users.FirstOrDefault(u => u.Id == userId)?.UserName}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok("Friend request sent");
        }

        //  Get Pending Friend Requests
        [HttpGet("requests")]
        public async Task<IActionResult> GetFriendRequests()
        {
            var userId = _userManager.GetUserId(User);

            if (_context.Friendships == null)
                return Problem("Friendships DbSet is null");

            var friendRequests = await _context.Friendships
                .Where(f => f.AddresseeID == userId && f.Status == FriendshipStatus.Pending)
                .Select(f => new FriendRequestViewModel
                {
                    RequestId = f.Id,
                    SenderId = f.RequesterId,
                    SenderName = _context.Users
                        .Where(u => u.Id == f.RequesterId)
                        .Select(u => u.UserName)
                        .FirstOrDefault() ?? "Unknown User"
                })
                .ToListAsync();

            return View(friendRequests);  
        }



        [HttpPost]
        public IActionResult AcceptFriendRequest([FromForm] int requestId)
        {
            Console.WriteLine($"Received Request ID: {requestId}");

            if (requestId == 0)
            {
                Console.WriteLine("Invalid data received");
                return BadRequest("Invalid request data");
            }

            var friendRequest = _context.Friendships.FirstOrDefault(f => f.Id == requestId);

            if (friendRequest == null)
            {
                return NotFound("Friend request not found");
            }

            friendRequest.Status = FriendshipStatus.Accepted;
            _context.SaveChanges();

            return RedirectToAction("Index", "Home"); // Redirect back to home
        }




        public class AcceptFriendRequestModel
        {
            public int RequestId { get; set; }
        }



    }
}
