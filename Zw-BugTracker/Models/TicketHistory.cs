using System;

namespace Zw_BugTracker.Models
{
    public class TicketHistory
    {
        //PK
        public int Id { get; set; }

        //FK
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public int ProjectId { get; set; }

        //Regular Properties
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTimeOffset Changed { get; set; }
        public static string GetValueFromKey { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignedToUserId { get; set; }

        //Navigation - Parent
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}