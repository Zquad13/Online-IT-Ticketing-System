using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Online_IT_Ticketing_System.Models
{
    public class TicketModel
    {
        [Required]
        public string UserName { get; set; }  // Populated externally (e.g., from session)

        [Required]
        [StringLength(200)]
        public string TicketId { get; set; }

        [Required]
        [StringLength(200)]
        public string Topic { get; set; }


        [Required]
        public string Subject { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }
        [MaxLength(50)]
        public string Status { get; set; } = "Open";
        [MaxLength(50)]
        public string AssignedTo { get; set; }

        public string Message { get; set; }

        public string AttachmentPath { get; set; }

        public byte[] AttachmentData { get; set; }

        // Constructor to automatically set default values
        public TicketModel()
        {
            TicketId = GenerateTicketId();    
            CreationDate = DateTime.Now;
                // Set the current date and time
        }

        // Helper method to generate a Ticket ID
        private string GenerateTicketId()
        {
            return "TKT-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }
        public List<MessageModel> Messages { get; set; } = new List<MessageModel>();

    }
    public class MessageModel
    {
        public int Id { get; set; }
        public string TicketId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }
    }

}
