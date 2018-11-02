﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Zw_BugTracker.Models;

namespace Zw_BugTracker.Helpers
{
    public class ProjectsHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.Users.Any(u => u.Id == userId);
            return (flag);
        }

        public ICollection<Project> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);

            var projects = user.Projects.ToList(); return (projects);
        }

        public void AddUserToProject(string userId, int projectId)
        {
            if (!IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId); var newUser = db.Users.Find(userId);

                proj.Users.Add(newUser); db.SaveChanges();
            }
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            if (IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var delUser = db.Users.Find(userId);

                proj.Users.Remove(delUser);
                db.Entry(proj).State = EntityState.Modified; // just saves this obj instance.
                db.SaveChanges();
            }
        } 

        public ICollection<ApplicationUser> UsersOnProject(int projectId)
        {
            return db.Projects.Find(projectId).Users;
        }

        public ICollection<ApplicationUser> UsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }

        public string GetProjectPm(int projectId)
        {
            var blank = "No PM Assigned";
            var projectAssignedUserId = db.Projects.Find(projectId).AssignedUserId;
            var projectAssignedUser = db.Users.Find(projectAssignedUserId).Email;
            if (projectAssignedUser == null)
            {
                return blank;
            }
            if (string.IsNullOrEmpty(projectAssignedUser))
            {
                return blank;
            }
            return projectAssignedUser;
        }
    }

  
}