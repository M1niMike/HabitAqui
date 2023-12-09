using Microsoft.AspNetCore.Mvc;
using TP2324.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TP2324.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesManager : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesManager(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return View(roles);
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string roleName)
        {
            var role = new IdentityRole(roleName);

            return RedirectToAction("Index");
        }
    }
}
