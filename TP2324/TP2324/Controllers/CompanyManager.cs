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



        //public async Task<IActionResult> Index()
        //{
        //    var companies = await _context.Companies.Include(c => c.Managers).ToListAsync();
        //    return View(companies);
        //}


        public IActionResult Index(CreateCompanyViewModel viewModel)
        {
            IQueryable<Company> CompaniesQuery = _context.Companies.Include(m => m.Managers).Include(m => m.Employees).Include(m => m.Homes);

            if (!string.IsNullOrEmpty(viewModel.TextoAPesquisar))
            {
                if (viewModel.TextoAPesquisar == "Todos" || viewModel.TextoAPesquisar == "todos")
                {
                    CompaniesQuery = CompaniesQuery.OrderBy(c => c.Name);
                }
                else
                {
                    CompaniesQuery = CompaniesQuery.Where(c => c.Name.Contains(viewModel.TextoAPesquisar)).OrderBy(c => c.Name);
                }

            }

            if (!string.IsNullOrEmpty(viewModel.Ordenacao))
            {
                if (viewModel.Ordenacao == "Ativo")
                {
                    CompaniesQuery = CompaniesQuery.Where(c => c.State).OrderBy(c => c.Name);
                }
                else if (viewModel.Ordenacao == "Inativo")
                {
                    CompaniesQuery = CompaniesQuery.Where(c => !c.State).OrderBy(c => c.Name);
                }
            }

            viewModel.companiesList = CompaniesQuery.ToList();

            return View(viewModel);
        }




        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
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
        public async Task<IActionResult> Create([Bind("Company,FirstName,LastName,Password")] CreateCompanyViewModel model)
        {
            var company = model.Company;


            // Cria um novo usuário
            var newManager = new ApplicationUser
            {
                UserName = $"gestorPrincipal_{model.FirstName}{company.EmailDomain}.com",
                Email = $"gestorPrincipal_{model.FirstName}{company.EmailDomain}.com",
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            // Verifica se o usuário já existe pelo e-mail
            var user = await _userManager.FindByEmailAsync(newManager.Email);


            try
            {
                if (ModelState.IsValid)
                {
                    
                    if (user == null)
                    {
                       

                        await _userStore.SetUserNameAsync(newManager, newManager.UserName, CancellationToken.None);
                        var identityResult = await _userManager.CreateAsync(newManager, model.Password);

                        if (identityResult.Succeeded)
                        {
                            // Adiciona o usuário à role "Manager"
                            await _userManager.AddToRoleAsync(newManager, Roles.Manager.ToString());

                            var manager = new Manager
                            {
                                Name = $"{model.FirstName} {model.LastName}",
                                CompanyId = model.Company.Id,  // Associa à nova empresa
                                ApplicationUserId = newManager.Id,  // Associa ao novo usuário
                                Available = true
                            };

                            model.Company.Managers = new List<Manager> { manager };

                            _context.Add(model.Company);
                            _context.Managers.Add(manager);
                            await _context.SaveChangesAsync();

                            Console.WriteLine("Contador: " + model.Company.Managers.Count());

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

        

        // GET: Homes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }


            return View(company);
        }


        // POST: Homes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Rating,State,EmailDomain")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            //ModelState.Remove(nameof(home.Category));
            //ModelState.Remove(nameof(home.typeResidence));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        // GET: Homes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var companies = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companies == null)
            {
                return NotFound();
            }

            return View(companies);
        }


        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.Homes).Include(c => c.Managers).Include(c => c.Employees) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Homes/Delete/5
        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies
            .Include(c => c.Homes)
            .Include(c => c.Managers)
                .ThenInclude(m => m.ApplicationUser)  // Inclua a propriedade ApplicationUser
            .Include(c => c.Employees)
                .ThenInclude(e => e.ApplicationUser)  // Inclua a propriedade ApplicationUser
            .FirstOrDefaultAsync(m => m.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            if (company.Homes != null && company.Homes.Count >= 1)
            {
                ModelState.AddModelError(string.Empty, "Não é possível excluir a empresa, pois ela possui pelo menos uma habitação.");
                return View(company);
            }
            // Remover o locador
            var manager = await _context.Managers.FirstOrDefaultAsync(l => l.CompanyId == id);

            var employee = await _context.Employees.FirstOrDefaultAsync(l => l.CompanyId == id);

           
            ApplicationUser userManager = await _userManager.FindByIdAsync(manager.ApplicationUser.Id); // Usando operadores de navegação segura
           

            if (company.Employees != null && company.Employees.Count >= 1)
            {
                ApplicationUser userEmployee = await _userManager.FindByIdAsync(employee.ApplicationUser.Id); // Substitua pelo campo adequado no seu modelo
                if (userEmployee != null)
                {
                    await _userManager.DeleteAsync(userEmployee);
                }

                if (employee != null)
                {
                    _context.Employees.Remove(employee);
                }
            }


            if (userManager != null)
            {
                await _userManager.DeleteAsync(userManager);
            }


            if (manager != null)
            {
                _context.Managers.Remove(manager);
            }

            

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




    }
}
