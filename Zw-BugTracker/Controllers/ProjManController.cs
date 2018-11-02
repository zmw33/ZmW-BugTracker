using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zw_BugTracker.Helpers;
using Zw_BugTracker.Models;

namespace Zw_BugTracker.Controllers
{
    public class ProjManController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketsHelper tickHelper = new TicketsHelper();
        private UserRolesHelper roleHelper = new UserRolesHelper();

        // GET: Admin
        [Authorize(Roles = "Project Manager")]
        public ActionResult TicketAssignment()
        {
            //Load up a SelectList data structure into a ViewBag property
            var developers = roleHelper.UsersInRole("Developer").ToList();
            ViewBag.Developers = new SelectList(developers,"Id", "Email");
            ViewBag.Tickets = new MultiSelectList(db.Tickets.ToList(), "Id", "Title");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> TicketAssignment(List<int> tickets, string developers, IdentityMessage message, Ticket oldTicket, Ticket tick)
         {
            foreach (var ticket in tickets)
            {
                //Step 1: Unassign everyone from selected ticket
                var tickUsers = tickHelper.ListUsersOnTicket(ticket);
                if (tickUsers.Count > 0)
                {
                    foreach (var user in tickUsers)
                    {
                        tickHelper.RemoveUserFromTicket(user.Email, ticket);
                    }
                }
                //Step 2: Assign selected users to ticket
                tickHelper.AddUserToTicket(developers, ticket);
            }
            db.SaveChanges();
            await Notify.TriggerNotifications(tick, oldTicket);
            //Step 3: Take user somewhere else
            return RedirectToAction("Index", "Tickets");
        }
    }
}