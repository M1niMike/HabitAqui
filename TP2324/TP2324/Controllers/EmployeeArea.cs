using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        //private readonly ILogger<RegisterModel> _logger;

        public EmployeeArea(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _userStore = userStore;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ActionResult> HomesList(PesquisaHabitacaoViewModel viewModel)
        {

            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Encontre o funcionario associado ao usuário autenticado
            var employee =  await _context.Employees
                .Include(m => m.Company) // Inclua a empresa associada ao funcionario
                .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

            if (employee == null)
            {
                // O usuário autenticado não é um funcionario
                return NotFound();
            }

            // Obtenha a empresa associada ao gestor
            var company = employee.Company;

            IQueryable<Home> homesQuery =  _context.Homes
                .Include(m => m.Company).Include(m => m.typeResidence).Include(m => m.Category).Include(m => m.District)
                .Where(m => m.CompanyId == company.Id);
                 

            if (!string.IsNullOrEmpty(viewModel.TipoResidenciaSelecionado))
            {
                homesQuery = homesQuery.Where(c => c.typeResidence.Name == viewModel.TipoResidenciaSelecionado);
            }


            if (!string.IsNullOrEmpty(viewModel.CategoriaSelecinada))
            {
                homesQuery = homesQuery.Where(c => c.Category.Name == viewModel.CategoriaSelecinada);
            }

            if (!string.IsNullOrEmpty(viewModel.Ordenacao))
            {
                if (viewModel.Ordenacao == "MenorPreco")
                {
                    homesQuery = homesQuery.OrderBy(c => c.PriceToRent);
                }
                else if (viewModel.Ordenacao == "MaiorPreco")
                {
                    homesQuery = homesQuery.OrderByDescending(c => c.PriceToRent);
                }
                if (viewModel.Ordenacao == "Ativo")
                {
                    homesQuery = homesQuery.Where(c => c.Available).OrderBy(c => c.Address);
                }
                else if (viewModel.Ordenacao == "Inativo")
                {
                    homesQuery = homesQuery.Where(c => !c.Available).OrderBy(c => c.Address);
                }
            }


            var typeResidences = _context.TypeResidences.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeTypes = new SelectList(typeResidences);

            var category = _context.Category.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeCategory = new SelectList(category);

            var districts = _context.Districts.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeDistrict = new SelectList(districts);

            var companies = _context.Companies.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeCompany = new SelectList(companies);

            viewModel.Homeslist = homesQuery.ToList();
            viewModel.NumResultados = viewModel.Homeslist.Count;

            return View(viewModel);
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
        public async Task<IActionResult> CreateHomes([Bind("Id,TypeResidenceId,CategoryId,DistrictId,CompanyId,PriceToRent,NumWC,Address,SquareFootage,NumParks,Wifi,Description,MinimumPeriod,Available,ImgUrl,Ratings,ImageFile")] Home home)
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

                    if (home.ImageFile != null)
                    {

                        // Processar o upload da imagem
                        home.ImgUrl = await UploadImage(home.ImageFile);
                    }

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


        // GET: Homes/Edit/5
        public async Task<IActionResult> HomesEdit(int? id)
        {
            if (id == null || _context.Homes == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["TypeResidenceId"] = new SelectList(_context.TypeResidences, "Id", "Name");
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");

            var home = await _context.Homes.FindAsync(id);
            if (home == null)
            {
                return NotFound();
            }


            return View(home);
        }


        // POST: Homes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HomesEdit(int id, [Bind("Id,TypeResidenceId,CategoryId,DistrictId,CompanyId,PriceToRent,NumWC,Address,SquareFootage,NumParks,Wifi,Description,BeginDate,EndDate,MinimumPeriod,Available,ImgUrl,Ratings,ImageFile")] Home home)
        {
            if (id != home.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(home.Category));
            ModelState.Remove(nameof(home.typeResidence));
            ModelState.Remove(nameof(home.Rentings));
            ModelState.Remove(nameof(home.District));
            ModelState.Remove(nameof(home.Company));

            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var existingHome = await _context.Homes.FindAsync(id);
                        if (existingHome == null)
                        {
                            return NotFound();
                        }

                        //Atualiza apenas as propriedades desejadas
                        existingHome.TypeResidenceId = home.TypeResidenceId;
                        existingHome.CategoryId = home.CategoryId;
                        existingHome.DistrictId = home.DistrictId;
                        existingHome.PriceToRent = home.PriceToRent;
                        existingHome.MinimumPeriod = home.MinimumPeriod;
                        existingHome.NumWC = home.NumWC;
                        existingHome.Address = home.Address;
                        existingHome.SquareFootage = home.SquareFootage;
                        existingHome.NumParks = home.NumParks;
                        existingHome.Description = home.Description;
                        existingHome.Wifi = home.Wifi;
                        existingHome.Available = home.Available;


                        if (home.ImageFile != null)
                        {
                            // Processar o upload da imagem
                            existingHome.ImgUrl = await UploadImage(home.ImageFile);
                        }


                        _context.Update(existingHome);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!HomesExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            Console.WriteLine($"Concurrency exception: {ex.Message}");
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                        throw;
                    }
                    return RedirectToAction(nameof(HomesList));
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

            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", home.CategoryId);
            ViewData["TypeResidenceId"] = new SelectList(_context.TypeResidences, "Id", "Name", home.TypeResidenceId);
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name", home.DistrictId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", home.CompanyId);
            return View(home);

        }



        // GET: Homes/Details/5
        public async Task<IActionResult> HomesDetails(int? id)
        {
            if (id == null || _context.Homes == null)
            {
                return NotFound();
            }

            var home = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence).Include(m => m.District).Include(m => m.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (home == null)
            {
                return NotFound();
            }

            return View(home);
        }



        private bool HomesExists(int id)
        {
            return (_context.Homes?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        // GET: Homes/Delete/5
        public async Task<IActionResult> DeleteHomes(int? id)
        {
            if (id == null || _context.Homes == null)
            {
                return NotFound();
            }

            var home = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence).Include(m => m.District).Include(m => m.Company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (home == null)
            {
                return NotFound();
            }

            return View(home);
        }

        // POST: Homes/Delete/5
        [HttpPost, ActionName("DeleteHomes")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Homes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Homes'  is null.");
            }
            var home = await _context.Homes.FindAsync(id);
            if (home != null)
            {
                _context.Homes.Remove(home);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(HomesList));
        }

        private async Task<string> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            // Diretório para salvar as imagens (assumindo que 'img' seja o diretório virtual para as imagens)
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");

            // Garante que o diretório de destino existe, caso contrário, cria-o
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Gera um nome de arquivo único para evitar colisões
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;

            // Caminho completo para o arquivo no sistema de arquivos
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Salva a imagem no sistema de arquivos
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Retorna a URL da imagem (assumindo que 'img' seja o diretório virtual para as imagens)
            return uniqueFileName;
        }

    }
}
