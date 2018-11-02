using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Zw_BugTracker.Helpers;
using Zw_BugTracker.Models;

namespace Zw_BugTracker.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();

        // GET: Admin
        [Authorize(Roles ="Admin")]
        public ActionResult RoleAssignment()
        {
            //Load up a SelectList data structure into a ViewBag property
            ViewBag.Users = new SelectList(db.Users.ToList(), "Id", "Email");
            ViewBag.Roles = new SelectList(db.Roles.ToList(), "Name", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAssignment(string users, string roles)
        {
            //Step 1: Remove any and all current role assignment
            var currentRoles = roleHelper.ListUserRoles(users);
            if(currentRoles.Count > 0)
            {
                foreach(var role in currentRoles)
                {
                    roleHelper.RemoveUserFromRole(users, role);
                }
            }

            //Step 2: Assign selected ROLE to selected USER
            roleHelper.AddUserToRole(users, roles);

            //Step 3: Update cookies
            //SignInManager.SignIn(User, false, false);

            //Step 4: Redirect the user somewhere else
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ProjectAssignment()
        {
            //Load up SelectList
            ViewBag.Users = new SelectList(db.Users.ToList(), "Id", "Email");
            ViewBag.Projects = new SelectList(db.Projects.ToList(), "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectAssignment(int projects, List<string> users)
        {
            //Step 1: Unassign everyone from selected project
            var projUsers = projHelper.UsersOnProject(projects);
            if (projUsers.Count > 0)
            {
                foreach (var user in projUsers)
                {
                    projHelper.RemoveUserFromProject(user.Email, projects);
                }
            }

            //Step 2: Assign selected users to project
            foreach (var user in users)
            {
                projHelper.AddUserToProject(user, projects);
            }


            //Step 3: Take user somewhere else
            return RedirectToAction("Index", "Home");
        }
    }
}