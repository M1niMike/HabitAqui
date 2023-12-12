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
                if(viewModel.TextoAPesquisar == "Todos" || viewModel.TextoAPesquisar == "todos")
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
                         // Adiciona o usuário à role "Manager"
                        await _userManager.AddToRoleAsync(newManager, Roles.Manager.ToString());

                        var manager = new Manager
                        {
                            Name = $"{model.FirstName} {model.LastName}",
                            CompanyId = model.Company.Id,  // Associa à nova empresa
                            ApplicationUserId = newManager.Id  // Associa ao novo usuário
                        };

                        model.Company.Managers = new List<Manager>{ manager };

                        _context.Add(model.Company);

                        _context.Managers.Add(manager);
                        await _context.SaveChangesAsync();

                        Console.Write("Contador:",model.Company.Managers.Count());

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Rating,State")] Company company)
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


        // GET: Homes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Homes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Companies == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Homes'  is null.");
            }
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
