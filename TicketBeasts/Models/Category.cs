using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace TicketBeasts.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(60)]
        public string Name { get; set; } = string.Empty;

        // Navigation
        public List<Sport> Events { get; set; } = new();
    }
}
