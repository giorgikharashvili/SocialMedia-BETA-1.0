using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SocialMedia.Models
{
    public class ReelPostings
    {
        [Key] // manually setting the Id variable as the primary key
        public int Id { get; set; } // entity framework automatically registers this property here as the primary key
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }

        [AllowNull]
        public string? ImagePath { get; set; }

        [NotMapped] // this property will not be added to the database and will not cause any errors
        [AllowNull]
        public string? ImageBase64Url { get; set; }

        [Required]
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        [Required]
        public string UserId { get; set; } // depending on this UserId we are able to get an access to the identity user of this job posting 
                                           // vgulisxmob mtlian ROW-s databaseshi
        [ForeignKey(nameof(UserId))] // what is a ForeignKey
        public IdentityUser User { get; set; }

        
    }
}
