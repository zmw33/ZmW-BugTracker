using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zw_BugTracker.Models
{
    public class TicketComment
    {
        //PK
        public int Id { get; set; }

        //FK
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public string OwnerUserId { get; set; }

        //Regular Properties
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Body")]
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }


        //Navigation - Parent
        public virtual ApplicationUser User { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}