using System;

namespace Zw_BugTracker.Models
{
    public class TicketAttachment
    {
        //PK
        public int Id { get; set; }

        //Regular Properties
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public string MediaUrl { get; set; }
        public string FilePath { get; set; }

        //FK
        public int TicketId { get; set; }
        public string UserId { get; set; }

        //Navigation - Parent
        public virtual ApplicationUser User { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}