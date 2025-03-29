using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using SocialMedia.Data;
using SocialMedia.Enums;
using SocialMedia.Models;

namespace SocialMedia.Models
{
   

    public class Friendship
    {


        public int Id { get; set; }
     

        // The user who sent the friend request
        public string RequesterId { get; set; }

        public IdentityUser Requester { get; set; }

        // The user who received the request
        public string AddresseeID { get; set; }

        public IdentityUser Addresse { get; set; }

        // Status of the friendship (Pending, Accepted, Declined)
        public FriendshipStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
