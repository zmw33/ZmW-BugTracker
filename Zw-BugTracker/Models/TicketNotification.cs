using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zw_BugTracker.Models
{
    public class TicketNotification
    {
        //PK
        public int Id { get; set; }

        //Regular Properties
        public string Body { get; set; }

        //FK
        public int TicketId { get; set; }
        public string RecipientId { get; set; }
        public string SenderId { get; set; }

        //Navigation - Parents
        public virtual ApplicationUser Recipient { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual Ticket Ticket { get; set; }

    }
}