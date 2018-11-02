using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zw_BugTracker.Models
{
    public class TicketType
    {
        //PK
        public int Id { get; set; }

        //Regular Properties
        public string Name { get; set; }

        //Navigation - Children
        public virtual ICollection<Ticket> Tickets { get; set; }

        //Instantiate ICollections
        public TicketType()
        {
            Tickets = new HashSet<Ticket>();
        }
    }
}