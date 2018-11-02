using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zw_BugTracker.Extensions;
using Zw_BugTracker.Helpers;
using Zw_BugTracker.Models;

namespace Zw_BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private UserRolesHelper roleHelper = new UserRolesHelper();

        // GET: Tickets
        [Authorize]
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }

        // GET: Tickets2
        [Authorize(Roles = "Project Manager, Submitter, Developer")]
        public ActionResult MyIndex()
        {
            var myTickets = new List<Ticket>();
            var userId = User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(User.Identity.GetUserId());
            switch (myRole.FirstOrDefault())
            { 
                case "Project Manager":
                    var myProjects = projHelper.ListUserProjects(userId);
                    foreach (var project in myProjects)
                    {
                        myTickets.AddRange(db.Tickets.Where(t => t.ProjectId == project.Id));
                    }
                    break;
                case "Developer":
                    myTickets = db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
                    break;
                case "Submitter":
                    myTickets = db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
                    break;
                default:
                    break;
            }
            return View(myTickets);
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin, Submitter, Project Manager, Developer")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,ProjectId,Body,Description,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Title = ticket.Title;
                ticket.Body = ticket.Body;
                ticket.ProjectId = ticket.ProjectId;
                ticket.Description = ticket.Description;
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.Created = DateTimeOffset.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("MyIndex");
            }

            //ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "Name", ticket.AssignedToUserId);
            //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "Name", ticket.OwnerUserId);
            //ViewBag.Project.Name = new SelectList(db.Projects, "Name", "Name", ticket.Project.Name);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin, Submitter, Developer, Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Question 1: What role are you?

            //Question 2: Depending on your role, does the ticket have the appropriate property setting?

            //If I am a submitter, the Ticket.OwnerUserId must be me
            //If I am a developer, the Ticket.AssignedToUserId must be me
            var userId = User.Identity.GetUserId();
            var ticket = db.Tickets.Find(id);
            var myRole = roleHelper.ListUserRoles(userId).ToList().FirstOrDefault();
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

            switch (myRole)
            {
                case "Admin":
                    return View(ticket);
                case "Developer":
                    if(ticket.AssignedToUserId == userId)
                        return View(ticket);
                    break;
                case "Submitter":
                    if(ticket.OwnerUserId == userId)
                        return View(ticket);
                    break;
                case "Project Manager":
                    foreach(var project in projHelper.ListUserProjects(userId))
                    {
                        if (ticket.ProjectId == project.Id)
                            return View(ticket);
                    }
                    break;
                default:
                    break;
            }

            if (ticket == null)
            {
                return HttpNotFound();
            }

            //if(User.IsInRole("Project Manager"))
            //{
            //    ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            //}

            return RedirectToAction("Index");
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Edit([Bind(Include = "Id,Title,Body,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket, Ticket tick, Ticket oldTicket, IdentityMessage message)
        {
            var oldTicket1 = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                await Notify.TriggerNotifications(tick, oldTicket);
                ticket.RecordChanges(oldTicket1);
                return RedirectToAction("Index");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
