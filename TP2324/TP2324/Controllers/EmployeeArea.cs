using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP2324.Data;
using TP2324.Models;
using TP2324.ViewModels;

namespace TP2324.Controllers
{
    public class EmployeeArea : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        //private readonly ILogger<RegisterModel> _logger;

        public EmployeeArea(
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

        public async Task<IActionResult> HomesList()
        {


            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Encontre o funcionario associado ao usuário autenticado
            var employee = await _context.Employees
                .Include(m => m.Company) // Inclua a empresa associada ao funcionario
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

            if (employee == null)
            {
                // O usuário autenticado não é um funcionario
                return NotFound();
            }

            // Obtenha a empresa associada ao gestor
            var company = employee.Company;

            // Obtenha a lista de gestores associados à empresa
            var homes = await _context.Homes
                .Include(m => m.Company).Include(m => m.typeResidence).Include(m => m.Category).Include(m => m.District)
                .Where(m => m.CompanyId == company.Id)
                .ToListAsync();

            return View(homes);
        }


        // GET: Homes/Create
        //[Authorize(Roles = "Employee")]
        public IActionResult CreateHomes()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["TypeResidenceId"] = new SelectList(_context.TypeResidences, "Id", "Name");
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            return View();
        }


        // POST: Homes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Employee")]
        public async Task<IActionResult> CreateHomes([Bind("Id,TypeResidenceId,CategoryId,DistrictId,CompanyId,PriceToRent,NumWC,Address,SquareFootage,NumParks,Wifi,Description,MinimumPeriod,Available,ImgUrl,Ratings")] Home home)
        {
            
            ModelState.Remove(nameof(home.Category));
            ModelState.Remove(nameof(home.typeResidence));
            ModelState.Remove(nameof(home.Rentings));
            ModelState.Remove(nameof(home.District));
            ModelState.Remove(nameof(home.Company));

            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Encontre o gestor associado ao usuário autenticado
            var employee = await _context.Employees
                .Include(m => m.Company) // Inclua a empresa associada ao gestor
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);


            try
            {
                

                if (employee == null)
                {
                    // O usuário autenticado não é um gestor
                    return NotFound();
                }

                // Obtenha a empresa associada ao gestor
                var company = employee.Company;

                if (ModelState.IsValid)
                {
                    //var newHome = new Home
                    //{
                    //    PriceToRent = viewModel.Price,
                    //    NumWC = viewModel.numWc,
                    //    Address = viewModel.Address,
                    //    SquareFootage = viewModel.SquareFootage,
                    //    NumParks = viewModel.numParks,
                    //    Wifi = viewModel.Wifi,
                    //    Description = viewModel.Description,
                    //    MinimumPeriod = viewModel.minimumPeriod,
                    //    Ratings = viewModel.ratings,
                    //    Available = true,
                    //    ImgUrl = viewModel.ImgUrl,
                    //    CategoryId = viewModel.categoryId,
                    //    TypeResidenceId = viewModel.typeResidenceId,
                    //    DistrictId = viewModel.districtId,
                    //    CompanyId = employee.Company.Id
                    //};

                    // Atualiza apenas as propriedades desejadas
                    home.CompanyId = company.Id;
                
                    employee.Company.Homes = new List<Home> { home };

                    _context.Homes.Add(home);
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Contador: " + employee.Company.Homes.Count());

                    return RedirectToAction(nameof(HomesList));
                }
            }
            catch (Exception ex)
            {
                // Imprime detalhes da exceção
                Console.WriteLine("Exceção ao criar home: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Exceção ao criar home: " + ex.Message);
            }

            // Adiciona os erros do ModelState à coleção de erros
            foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("Erro no modelo: " + modelError.ErrorMessage);
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", home.CategoryId);
            ViewData["TypeResidenceId"] = new SelectList(_context.TypeResidences, "Id", "Name", home.TypeResidenceId);
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name", home.DistrictId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", home.CompanyId);
            return View(employee);
        }
    }
}
