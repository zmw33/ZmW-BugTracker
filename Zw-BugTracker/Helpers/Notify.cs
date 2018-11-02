using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Zw_BugTracker.Models;


namespace Zw_BugTracker.Helpers
{
    public static class Notify
    {
        public static async Task TriggerNotifications(this Ticket tick, Ticket oldTicket)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            //Let's begin with notifications for Ticket Assignment/UnAssignment
            var newAssignment = (tick.AssignedToUserId != null && oldTicket.AssignedToUserId == null);
            var unAssignment = (tick.AssignedToUserId == null && oldTicket.AssignedToUserId != null);
            var reAssignment = ((tick.AssignedToUserId != null && oldTicket.AssignedToUserId != null) &&
                               (tick.AssignedToUserId != oldTicket.AssignedToUserId));

            //Compose the body of the email
            var body = new StringBuilder();
            body.AppendFormat("<p>Email From: <bold>{0}</bold>({1})</p>", "Administrator", WebConfigurationManager.AppSettings["emailfrom"]);
            body.AppendLine("<br/><p><u><b>Message:</b></u></p>");
            //body.AppendFormat("<p><b>Project Name:</b> {0}</p>", db.Projects.FirstOrDefault(p => p.Id == tick.ProjectId).Name);
            body.AppendFormat("<p><b>Ticket Title:</b> {0} | Id: {1}</p>", tick.Title, tick.Id);
            body.AppendFormat("<p><b>Ticket Created:</b> {0}</p>", tick.Created);
            //body.AppendFormat("<p><b>Ticket Type:</b> {0}</p>", db.TicketTypes.Find(tick.TicketTypeId).Name);
            //body.AppendFormat("<p><b>Ticket Status:</b> {0}</p>", db.TicketStatuses.Find(tick.TicketStatusId).Name);
            //body.AppendFormat("<p><b>Ticket Priority:</b> {0}</p>", db.TicketPriorities.Find(tick.TicketPriorityId).Name);

            //Generate email
            IdentityMessage email = null;
            if (newAssignment)
            {
                //Generate 1 email to the new Developer letting them know they have been assigned
                email = new IdentityMessage()
                {
                    Subject = "Notification: A Ticket has been assigned to you...",
                    Body = body.ToString(),
                    Destination = db.Users.Find(tick.AssignedToUserId).Email
                };

                var svc = new EmailService();
                await svc.SendAsync(email);
            }
            else if (unAssignment)
            {
                //Generate email to old Developer for unassignment of ticket
                email = new IdentityMessage()
                {
                    Subject = "Notification: You have been REMOVED from a Ticket...",
                    Body = body.ToString(),
                    Destination = db.Users.Find(oldTicket.AssignedToUserId).Email
                };

                var svc = new EmailService();
                await svc.SendAsync(email);
            }
            else if (reAssignment)
            {
                //Generate email to new Developer for newly assigned ticket
                email = new IdentityMessage()
                {
                    Subject = "Notification: You have been ASSIGNED a Ticket...",
                    Body = body.ToString(),
                    Destination = db.Users.Find(tick.AssignedToUserId).Email
                };

                var svc = new EmailService();
                await svc.SendAsync(email);

                //Generate 1 email to the old Developer letting them know they have been unassigned
                email = new IdentityMessage()
                {
                    Subject = "Notification: You have been REMOVED from a Ticket...",
                    Body = body.ToString(),
                    Destination = db.Users.Find(oldTicket.AssignedToUserId).Email
                };

                svc = new EmailService();
                await svc.SendAsync(email);
            }

            //Generate Notification
            TicketNotification notification = null;
            if (newAssignment)
            {
                notification = new TicketNotification
                {
                    Body = "Notification: You have been ASSIGNED a Ticket...<br />" + body.ToString(),
                    RecipientId = tick.AssignedToUserId,
                    TicketId = tick.Id
                };
                db.TicketNotifications.Add(notification);
            }
            else if (unAssignment)
            {
                notification = new TicketNotification
                {
                    Body = "Notification: You have been REMOVED from a Ticket...<br />" + body.ToString(),
                    RecipientId = oldTicket.AssignedToUserId,
                    TicketId = tick.Id
                };
                db.TicketNotifications.Add(notification);
            }
            else if (reAssignment)
            {
                notification = new TicketNotification
                {
                    Body = "Notification: You have been ASSIGNED a Ticket...<br />" + body.ToString(),
                    RecipientId = tick.AssignedToUserId,
                    TicketId = tick.Id
                };
                db.TicketNotifications.Add(notification);

                notification = new TicketNotification
                {
                    Body = "Notification: You have been REMOVED from a Ticket...<br />" + body.ToString(),
                    RecipientId = oldTicket.AssignedToUserId,
                    TicketId = tick.Id
                };
                db.TicketNotifications.Add(notification);
            }
            db.SaveChanges();
        }

    }
}