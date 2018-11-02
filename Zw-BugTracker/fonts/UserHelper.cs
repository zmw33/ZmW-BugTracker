using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zw_BugTracker.Models;

namespace Zw_BugTracker.fonts
{
    [Authorize]
    public static class UserHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin,ProjectManager,Developer,Submitter")]
        public static string GetProfileImagePath(string userId)
        {
            var defaultPath = "/Uploads/default.jpg";
            if (string.IsNullOrEmpty(userId))
                return defaultPath;

            var profileImagePath = db.Users.Find(userId).ProfileImagePath;
            if (string.IsNullOrEmpty(profileImagePath))
                return profileImagePath;

            return profileImagePath;
        }
    }
}