using System;
using System.ComponentModel.DataAnnotations;

namespace TicketBeasts.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        // FK to Sport (your event)
        public int EventId { get; set; }
        public Sport? Event { get; set; }

        public int Quantity { get; set; }

        [Required, StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [StringLength(4)]
        public string CreditCardLast4 { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
