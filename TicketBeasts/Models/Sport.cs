using System.ComponentModel.DataAnnotations;

namespace TicketBeasts.Models
{
    public class Sport
    {
        public int Id { get; set; }

        [Required, StringLength(160)]
        public string Title { get; set; } = string.Empty;

        [StringLength(4000)]
        public string? Description { get; set; }

        [Required]
        public DateTime EventDateTime { get; set; }

        [Required, StringLength(200)]
        public string Location { get; set; } = string.Empty;

        // Foreign keys
        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }   // navigation

        [Required]
        public int OwnerId { get; set; }
        public Owner? Owner { get; set; }         // navigation

        // For newest → oldest ordering on home page
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
