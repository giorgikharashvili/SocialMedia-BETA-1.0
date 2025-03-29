using SocialMedia.Models;
namespace SocialMedia.ViewModels
{
    public class MessageViewModel
    {
       
            public string Content { get; set; }
            public string FriendId { get; set; }
            public List<Message> Messages { get; set; }
        public string UserId { get; internal set; }
    }
}
