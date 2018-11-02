using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zw_BugTracker.Models
{
    public class Project
    {
        //PK
        public int Id { get; set; }

        //FK
        //public string UserId { get; set; }
        public string AssignedUserId { get; set; }

        //Regular Properties
        [Required]
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        //[Required]
        public string Description { get; set; }

        //Navigation - Children
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        //Navigation - Parent
        //public virtual ApplicationUser OwnerUser { get; set; }

        //Instantiate ICollections
        public Project()
        {
            Tickets = new HashSet<Ticket>();
            Users = new HashSet<ApplicationUser>();
        }
    }
}