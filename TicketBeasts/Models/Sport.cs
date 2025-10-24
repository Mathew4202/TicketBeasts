using System.ComponentModel.DataAnnotations;

namespace TicketBeasts.Models
{
    public class Sport
    {
        public int Id { get; set; }

        [Required, StringLength(160)]
        [Display(Name = "Game Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(4000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Event Date & Time")]
        public DateTime EventDateTime { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        // Foreign keys
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }   // navigation

        [Required]
        [Display(Name = "Owner")]
        public int OwnerId { get; set; }
        public Owner? Owner { get; set; }         // navigation

        // For newest → oldest ordering on home page
        [Display(Name = "Created At")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [StringLength(300)]
        [Display(Name = "Image")]
        public string? ImagePath { get; set; }   // e.g. "/uploads/abcd1234.jpg"
    }
}
