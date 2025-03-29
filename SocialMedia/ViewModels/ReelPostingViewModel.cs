using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using SocialMedia.Models;

namespace SocialMedia.ViewModels
{
    public class ReelPostingViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public string ImagePath { get; set; }

        public IEnumerable<ReelPostings> ReelPostings { get; set; }
        public IEnumerable<IdentityApplicationUser> Friends { get; set; }

        public IEnumerable<Friendship> FriendsList { get; set; }

        public string tempId { get; set; }


    }
}
