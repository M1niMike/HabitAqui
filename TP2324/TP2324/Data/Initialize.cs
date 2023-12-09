using Microsoft.AspNetCore.Identity;
using System;
using TP2324.Models;

namespace TP2324.Data
{
    public enum Roles
    {
        Admin,
        Manager,
        Employee,
        Client
    }

    public static class Initialize
    {

        public static async Task CreateInitialDatas(UserManager<ApplicationUser>
       userManager, RoleManager<IdentityRole> roleManager)
        {
            //Adicionar default Roles 
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Manager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Employee.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Client.ToString()));
            ////Adicionar Default User - Admin
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@localhost.com",
                Email = "admin@localhost.com",
                FirstName = "Administrador",
                LastName = "Local",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Abcd1234@");
                await userManager.AddToRoleAsync(defaultUser,
                Roles.Admin.ToString());
            }
        }
    }
}
