using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using Zw_BugTracker.Models;

namespace Zw_BugTracker.Extensions
{
    public static class History
    {
        private static string GetValueFromKey(string keyName, object key)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var returnValue = "";
            if (key.ToString() == string.Empty)
                return returnValue;

            switch (keyName)
            {
                //case "ProjectId":
                //    returnValue = db.Projects.Find(key).Name;
                case "TicketTypeId":
                    returnValue = db.TicketTypes.Find(key).Name;
                    break;
                case "TicketPrioritiesId":
                    returnValue = db.TicketPriorities.Find(key).Name;
                    break;
                case "TicketStatusId":
                    returnValue = db.TicketStatuses.Find(key).Name;
                    break;
                //case "OwnerUserId":
                //    returnValue = db.OwnerUserId.Find(key).UserName;
                //    break;
                //case "AssignedToUserId":
                //    returnValue = db.AssignedToUserId.Find(key).UserName;
                //    break;
                default:
                    returnValue = key.ToString();
                    break;
            }
            return returnValue;
        }

        public static void RecordChanges(this Ticket ticket, Ticket oldTicket)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            //Grab list of properties
            var propertyList = WebConfigurationManager.AppSettings["TrackedTicketProperties"].Split(',');

            //Itereate over properties of Ticket Type
            foreach(PropertyInfo prop in ticket.GetType().GetProperties())
            {
                //If this property list is not one of the properties I want then loop
                if (!propertyList.Contains(prop.Name))
                    continue;

                //Record current and old property values
                var value = prop.GetValue(ticket, null) ?? "";
                var oldValue = prop.GetValue(oldTicket, null) ?? "";

                //if both properties are empty there is nothing to record
                if (value.ToString() != oldValue.ToString())
                {
                    var ticketHistory = new TicketHistory
                    {
                        Changed = DateTimeOffset.Now,
                        Property = prop.Name,
                        NewValue = GetValueFromKey(prop.Name, value),
                        OldValue = GetValueFromKey(prop.Name, oldValue),
                        TicketId = ticket.Id,
                        UserId = HttpContext.Current.User.Identity.GetUserId()
                    };

                    db.TicketHistories.Add(ticketHistory);
                }
            }
            db.SaveChanges();
        }
    }


}