using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace TicketBeasts.Models
{
    public class Owner
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress, StringLength(120)]
        public string? ContactEmail { get; set; }

        // Navigation
        public List<Sport> Events { get; set; } = new();
    }
}
