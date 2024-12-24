using System.ComponentModel.DataAnnotations;

namespace Online_IT_Ticketing_System.Models
{
    public class TicketAssignmentViewModel
    {
        public String TicketId { get; set; }

        
        public int SubAdminId { get; set; }

        public List<SubAdminModel> SubAdmins { get; set; }
    }
}