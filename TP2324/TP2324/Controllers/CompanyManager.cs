using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TP2324.Data;
using TP2324.Models;
using TP2324.ViewModels;

namespace TP2324.Controllers
{
    public class CompanyManager : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        //private readonly ILogger<RegisterModel> _logger;

        public CompanyManager(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _userStore = userStore;
        }



        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies.ToListAsync();
            return View(companies);
        }


        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserId = userId;
            ViewBag.UserName = user.UserName;
            var model = new List<ManageUserRolesViewModel>();

            // Modifique o loop para incluir apenas as roles "Manager"
            foreach (var role in _roleManager.Roles.Where(r => r.Name == "Manager"))
            {
                var userRolesManager = new ManageUserRolesViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                userRolesManager.Selected = await _userManager.IsInRoleAsync(user, role.Name);
                model.Add(userRolesManager);
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Details(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Certifique-se de remover apenas as roles "Manager"
            var managerRolesToRemove = roles.Where(r => r == "Manager").ToList();
            var result = await _userManager.RemoveFromRolesAsync(user, managerRolesToRemove);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove");
                return View(model);
            }

            // Adicione apenas as roles "Manager" selecionadas
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected && x.RoleName == "Manager").Select(x => x.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        private List<string> GetAllRoles()
        {
            return _roleManager.Roles.Select(r => r.Name).ToList();
        }


        public IActionResult Create()
        {
            return View();
        }

        // POST: Homes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Company,FirstName,LastName,UserName,Password")] CreateCompanyViewModel model)
        {

            var company = model.Company;

            if (ModelState.IsValid)
            {
                

                // Verifica se o usuário já existe pelo e-mail
                var user = await _userManager.FindByEmailAsync(model.UserName);
               
                if (user == null)
                {
                    // Cria um novo usuário
                    var newManager = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.UserName,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };


                    await _userStore.SetUserNameAsync(newManager,newManager.UserName, CancellationToken.None);
                    var identityResult = await _userManager.CreateAsync(newManager, model.Password);

                  

                    if (identityResult.Succeeded)
                    {

                        _context.Add(model.Company);
                        await _context.SaveChangesAsync();

                        // Adiciona o usuário à role "Manager"
                        await _userManager.AddToRoleAsync(newManager, Roles.Manager.ToString());

                        var manager = new Manager
                        {
                            Name = $"{model.FirstName} {model.LastName}",
                            CompanyId = model.Company.Id,  // Associa à nova empresa
                            ApplicationUserId = newManager.Id  // Associa ao novo usuário
                        };

                        model.Company.Managers = new List<Manager>
                        {
                            manager
                        };

                        _context.Managers.Add(manager);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                   
                }
                else
                {
                    // Trata o caso em que o usuário já existe
                    ModelState.AddModelError(string.Empty, "Um usuário com o mesmo e-mail já existe.");
                }
            }
         
            return View(model);
        }

    }
}
