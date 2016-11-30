using BlogSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogSystem.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;
using BlogSystem.Models.ViewModels;
using BlogSystem.Models.DataModels;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;

namespace BlogSystem.WebApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        //
        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        // GET: User/List
        public ActionResult List()
        {
            using (var database = new BlogDbContext())
            {
                var users = database.Users
                    .ToList();

                var admins = GetAdminNames(users, database);
                ViewBag.Admins = admins;

                return View(users);
            }
        }

        //
        // GET: User/Edit
        public ActionResult Edit(string id)
        {
            // Validate Id
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            using (var database = new BlogDbContext())
            {
                // Get user from database
                var user = database.Users
                    .Where(u => u.Id == id)
                    .First();

                // Check if user exists
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Create a view model
                var viewModel = new EditUserViewModel();
                viewModel.User = user;
                viewModel.Roles = GetUserRoles(user, database);

                // Pass the model to the view
                return View(viewModel);
            }
        }

        //
        // POST: User/Edit
        [HttpPost]
        public ActionResult Edit(string id, EditUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            using (var database = new BlogDbContext())
            {
                if (!string.IsNullOrEmpty(viewModel.Password))
                {
                    ChangeUserPassword(id, viewModel);
                }

                var user = database.Users.FirstOrDefault(u => u.Id == id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                this.SetUserRoles(viewModel, user, database);

                database.Entry(user).State = EntityState.Modified;
                database.SaveChanges();

                return RedirectToAction("List");
            }
        }

        private void SetUserRoles(EditUserViewModel viewModel, ApplicationUser user, BlogDbContext context)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            foreach (var role in viewModel.Roles)
            {
                if (role.IsSelected && !userManager.IsInRole(user.Id, role.Name))
                {
                    userManager.AddToRole(user.Id, role.Name);
                }
                else if (!role.IsSelected && userManager.IsInRole(user.Id, role.Name))
                {
                    userManager.RemoveFromRole(user.Id, role.Name);
                }
            }
        }

        private void ChangeUserPassword(string userId, EditUserViewModel viewModel)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var token = userManager.GeneratePasswordResetToken(userId);
            var result = userManager.ResetPassword(userId, token, viewModel.Password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors));
            }
        }

        private List<Role> GetUserRoles(ApplicationUser user, BlogDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            var roles = roleManager.Roles
                .Select(r => r.Name)
                .OrderBy(r => r)
                .ToList();

            var userRoles = new List<Role>();

            foreach (var roleName in roles)
            {
                var role = new Role { Name = roleName };

                if (userManager.IsInRole(user.Id, roleName))
                {
                    role.IsSelected = true;
                }

                userRoles.Add(role);
            }

            return userRoles;
        }

        private HashSet<string> GetAdminNames(List<ApplicationUser> users, BlogDbContext context)
        {
            var admins = new HashSet<string>();

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            foreach (var user in users)
            {
                if (userManager.IsInRole(user.Id, "Admin"))
                {
                    admins.Add(user.UserName);
                }
            }

            return admins;
        }
    }
}