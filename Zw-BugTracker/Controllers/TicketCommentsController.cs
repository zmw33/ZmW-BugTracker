using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zw_BugTracker.Helpers;
using Zw_BugTracker.Models;

namespace Zw_BugTracker.Controllers
{
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private UserRolesHelper roleHelper = new UserRolesHelper();


        // GET: TicketComments
        public ActionResult Index()
        {
            var ticketComments = db.TicketComments.Include(t => t.Ticket).Include(t => t.User);
            return View(ticketComments.ToList());
        }

        // GET: TicketComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
        public ActionResult Create(int id)
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            //return View();
            var comment = new TicketComment
            {
                TicketId = id
            };
            var userId = User.Identity.GetUserId();
            var project = db.Projects.Find(id);
            var ticketComments = db.TicketComments.Find(id);
            var ticket = db.Tickets.Find(id);
            var myRole = roleHelper.ListUserRoles(userId).ToList().FirstOrDefault();
            switch (myRole)
            {
                case "Admin":
                    return View(comment);
                case "Developer":
                    if (ticketComments.TicketId == ticket.Id && ticket.AssignedToUserId == userId)
                        return View(comment);
                    break;
                case "Submitter":
                    if (ticketComments.TicketId == ticket.Id && ticket.OwnerUserId == userId)
                        return View(comment);
                    break;
                case "Project Manager":
                    if (ticketComments.TicketId == ticket.Id && ticket.ProjectId == project.Id)
                        return View(comment);
                    break;
                default:
                    return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TicketId,Title,Body")] TicketComment ticketComment, Ticket tick, Ticket oldTicket)
        {
            if (ModelState.IsValid)
            {
                ticketComment.Created = DateTimeOffset.Now;
                ticketComment.UserId = User.Identity.GetUserId();
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                await Notify.TriggerNotifications(tick, oldTicket);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);

            var userId = User.Identity.GetUserId();
            var project = db.Projects.Find(id);
            var ticket = db.Tickets.Find(id);
            var myRole = roleHelper.ListUserRoles(userId).ToList().FirstOrDefault();
            switch (myRole)
            {
                case "Developer":
                    if (ticket.AssignedToUserId != userId)
                        return RedirectToAction("Index", "Home");
                    break;
                case "Submitter":
                    if (ticket.OwnerUserId != userId)
                        return RedirectToAction("Index", "Home");
                    break;
                case "Project Manager":
                    //if (ticket.ProjectId == project.Id && userId == project.OwnerUserId)
                    //    return View();
                    break;
                default:
                    break;
            }

            if (ticket == null)
            {
                return HttpNotFound();
            }

            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,Body,Created,Updated,Title")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            db.TicketComments.Remove(ticketComment);
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
