using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using TP2324.Data;
using TP2324.Models;
using TP2324.ViewModels;

namespace TP2324.Controllers
{
    public class ManagerArea : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        //private readonly ILogger<RegisterModel> _logger;

        public ManagerArea(
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
            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Encontre o gestor associado ao usuário autenticado
            var manager = await _context.Managers
                .Include(m => m.Company) // Inclua a empresa associada ao gestor
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

            if (manager == null)
            {
                // O usuário autenticado não é um gestor
                return NotFound();
            }

            // Obtenha a empresa associada ao gestor
            var company = manager.Company;

            // Obtenha a lista de gestores e funcionários associados à empresa
            var employeesAndManagers = await _context.Managers
                .Include(m => m.Company)
                .Where(m => m.CompanyId == company.Id)
                .ToListAsync();

            var employees = await _context.Employees
                .Include(e => e.Company)
                .Where(e => e.CompanyId == company.Id)
                .ToListAsync();

            // Agora, você tem a lista de gestores e funcionários associados à mesma empresa
            // Faça o que for necessário com as listas (por exemplo, passá-las para a exibição)

            return View(employeesAndManagers);
        }

        public IActionResult CreateManager()
        {

            return View(); // ou qualquer outra ação
        }


        // POST: Homes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateManager([Bind("Manager,FirstName,LastName,UserName,Password")] ManagerAreaViewModel model)
        {
            Console.WriteLine("##################################");

            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Encontre o gestor associado ao usuário autenticado
            var manager = await _context.Managers
                .Include(m => m.Company) // Inclua a empresa associada ao gestor
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

            if (manager == null)
            {
                // O usuário autenticado não é um gestor
                return NotFound();
            }

            // Obtenha a empresa associada ao gestor
            var company = manager.Company;

            Console.WriteLine("##################################");
            Console.WriteLine("##################################");
            Console.WriteLine("ManagerID: " + manager.Id);
            Console.WriteLine("ManagerName: " + manager.Name);
            Console.WriteLine("CompanyID: " + company.Id);
            Console.WriteLine("##################################");
            Console.WriteLine("##################################");

            try
            {
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

                        await _userStore.SetUserNameAsync(newManager, newManager.UserName, CancellationToken.None);
                        var identityResult = await _userManager.CreateAsync(newManager, model.Password);

                        if (identityResult.Succeeded)
                        {
                            // Adiciona o usuário à role "Manager"
                            await _userManager.AddToRoleAsync(newManager, Roles.Manager.ToString());

                            var managerClass = new Manager
                            {
                                Name = $"{model.FirstName} {model.LastName}",
                                CompanyId = manager.CompanyId,  // Usa o mesmo CompanyId do manager atual
                                ApplicationUserId = newManager.Id  // Associa ao novo usuário
                            };

                            _context.Managers.Add(managerClass);
                            await _context.SaveChangesAsync();

                            Console.WriteLine("Contador: " + manager.Company.Managers.Count());

                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            // Exibir todos os erros do modelo
                            foreach (var error in identityResult.Errors)
                            {
                                Console.WriteLine("Erro ao criar usuário: " + error.Description);
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    else
                    {
                        // Trata o caso em que o usuário já existe
                        Console.WriteLine("Um usuário com o mesmo e-mail já existe.");
                        ModelState.AddModelError(string.Empty, "Um usuário com o mesmo e-mail já existe.");
                    }
                }
                else
                {
                    // Model não é válido - exibir todos os erros
                    foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine("Erro no modelo: " + modelError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                // Imprime detalhes da exceção
                Console.WriteLine("Exceção: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Exceção: " + ex.Message);
            }

            return View(model);
        }


    }
}
