using Blog_MVC.Data;
using Blog_MVC.Enums;
using Blog_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_MVC.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            // Task: Create the DB from the Migrations
            await _dbContext.Database.MigrateAsync();


            // Task 1: Seed a few roles into the system
            await SeedRolesAsync();

            // Task 2: Seed a few user in to the system
            await SeedUsersAsync();




        }

        private async Task SeedRolesAsync()
        {
            //If are already Roles in the system, do nothing.
            if (_dbContext.Roles.Any())
            {
                return;
            }

            //Otherwise we want to create a few roles.
            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {
                // I need to use the Role Manager to create roles
                await _roleManager.CreateAsync(new IdentityRole(role));

            }


        }

        private async Task SeedUsersAsync()
        {
            // if there are already users in the system do nothing
            if (_dbContext.Users.Any())
            {
                return;
            }

            // otherwise we want to create a few users.
            // Step 1: creates a new instance of BlogUser.
            var adminUser = new BlogUser()
            {
                Email = "lyons.bart@protonmail.com",
                UserName = "lyons.bart@protonmail.com",
                FirstName = "Bart",
                LastName = "Lyons",
                PhoneNumber = "8598433629",
                EmailConfirmed = true,

            };


            // Step 2:  use the user manager to create a new user that is defined by adminUser
            await _userManager.CreateAsync(adminUser, "Abc&123!");


            // Step 3: add this new user to the admin role.

            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());



            // Step 1 repeat: Create the moderator user
            var modUser = new BlogUser()
            {
                Email = "lyons.bart@outlook.com",
                UserName = "lyons.bart@putlook.com",
                FirstName = "Bart",
                LastName = "Lyons",
                PhoneNumber = "8508433629",
                EmailConfirmed = true
            };

            // Step 2:  use the user manager to create a new user that is defined by modUser
            await _userManager.CreateAsync(modUser, "Abc&123!");


            // Step 3: add this new user to the mod role.

            await _userManager.AddToRoleAsync(modUser, BlogRole.Administrator.ToString());

        }
    }
}
