namespace Zw_BugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Zw_BugTracker.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Zw_BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Zw_BugTracker.Models.ApplicationDbContext context)
        {
            //create a few roles (Admin, Submitter)
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(m => m.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(m => m.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            if (!context.Roles.Any(m => m.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            //create a few users (i.e. me and instructor)
            var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

            //assign users to roles (me and instructor)
            // "u => u.email" means that u "goes to u.email"
            //use example email
            if (!context.Users.Any(u => u.Email == "zacharywilsonm@gmail.com"))
            {
                //any user will be identified by ApplicationUser
                userManager.Create(new ApplicationUser
                {
                    UserName = "zacharywilsonm@gmail.com",
                    Email = "zacharywilsonm@gmail.com",
                    FirstName = "Zachary",
                    LastName = "Wilson",
                    DisplayName = "ZwAdmin"
                }, "Pigbull2017!");
            }

            if (!context.Users.Any(u => u.Email == "demoadmin@mailinator.com"))
            {
                //any user will be identified by ApplicationUser
                userManager.Create(new ApplicationUser
                {
                    UserName = "demoadmin@mailinator.com",
                    Email = "demoadmin@mailinator.com",
                    FirstName = "Admin",
                    LastName = "Istrator",
                    DisplayName = "Admin"
                }, "password1");
            }

            if (!context.Users.Any(u => u.Email == "demopm@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demopm@mailinator.com",
                    Email = "demopm@mailinator.com",
                    FirstName = "Project",
                    LastName = "Manager",
                    DisplayName = "ProjMan"
                }, "password1");
            }

            if (!context.Users.Any(u => u.Email == "demosub@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demosub@mailinator.com",
                    Email = "demosub@mailinator.com",
                    FirstName = "Sub",
                    LastName = "Mitter",
                    DisplayName = "Submitter"
                }, "password1");
            }

            if (!context.Users.Any(u => u.Email == "demodev@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demodev@mailinator.com",
                    Email = "demodev@mailinator.com",
                    FirstName = "Dev",
                    LastName = "Eloper",
                    DisplayName = "Developer"
                }, "password1");
            }

            //Demo users for DemoLogin
            if (!context.Users.Any(u => u.Email == "demo.admin@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demo.admin@mailinator.com",
                    Email = "demo.admin@mailinator.com",
                    FirstName = "Admin",
                    LastName = "Istrator",
                    DisplayName = "Admin"
                }, "password1");
            }

            if (!context.Users.Any(u => u.Email == "demo.pm@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demo.pm@mailinator.com",
                    Email = "demo.pm@mailinator.com",
                    FirstName = "Project",
                    LastName = "Manager",
                    DisplayName = "ProjMan"
                }, "password1");
            }

            if (!context.Users.Any(u => u.Email == "demo.sub@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demo.sub@mailinator.com",
                    Email = "demo.sub@mailinator.com",
                    FirstName = "Sub",
                    LastName = "Mitter",
                    DisplayName = "Submitter"
                }, "password1");
            }

            if (!context.Users.Any(u => u.Email == "demo.dev@mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demo.dev@mailinator.com",
                    Email = "demo.dev@mailinator.com",
                    FirstName = "Dev",
                    LastName = "Eloper",
                    DisplayName = "Developer"
                }, "password1");
            }

            //assign role to user
            //var adminId = userManager.FindByEmail("zacharywilsonm@gmail.com").Id;
            //userManager.AddToRole(adminId, "Admin");

            var pmId = userManager.FindByEmail("demopm@mailinator.com").Id;
            userManager.AddToRole(pmId, "Project Manager");

            var devId = userManager.FindByEmail("demodev@mailinator.com").Id;
            userManager.AddToRole(devId, "Developer");

            var subId = userManager.FindByEmail("demosub@mailinator.com").Id;
            userManager.AddToRole(subId, "Submitter");

            //assign demo role to demo user for DemoLogin
            var adminId1 = userManager.FindByEmail("demo.admin@mailinator.com").Id;
            userManager.AddToRole(adminId1, "Admin");

            var pmId1 = userManager.FindByEmail("demo.pm@mailinator.com").Id;
            userManager.AddToRole(pmId1, "Project Manager");

            var devId1 = userManager.FindByEmail("demo.dev@mailinator.com").Id;
            userManager.AddToRole(devId1, "Developer");

            var subId1 = userManager.FindByEmail("demo.sub@mailinator.com").Id;
            userManager.AddToRole(subId1, "Submitter");

            //seed a few project records for demoing the ProjectAssignment view
            context.Projects.AddOrUpdate(p => p.Name,
                new Project { Name = "GiantElf", Description = "Abnormally tall Elf" },
                new Project { Name = "SmallGiant", Description = "Abnormally short Giant" },
                new Project { Name = "2EyedCyclops", Description = "Two eyes on a Cyclops" }
                );

            context.TicketPriorities.AddOrUpdate(TicketPriority => TicketPriority.Name,
                new TicketPriority { Name = "Extreme" },
                new TicketPriority { Name = "High" },
                new TicketPriority { Name = "Considerable" },
                new TicketPriority { Name = "Moderate" },
                new TicketPriority { Name = "Low" }
                );

            context.TicketTypes.AddOrUpdate(tt => tt.Name,
                new TicketType { Name = "View" },
                new TicketType { Name = "Script" },
                new TicketType { Name = "Database" }

            );

            context.TicketStatuses.AddOrUpdate(ts => ts.Name,
                new TicketStatus { Name = "Resolved" },
                new TicketStatus { Name = "In Progress" },
                new TicketStatus { Name = "Docile" }
            );
        }

    }
}
