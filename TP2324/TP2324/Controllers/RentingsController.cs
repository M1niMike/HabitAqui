using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TP2324.Data;
using TP2324.Models;

namespace TP2324.Controllers
{
    public class RentingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IWebHostEnvironment _env;

        public RentingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Rentings
        public async Task<IActionResult> RentingsIndex()
        {

            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Employee"))
            {


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

                var applicationDbContext = _context.Rentings
                    .Include(r => r.Homes)
                        .ThenInclude(m => m.Company)
                    .Include(r => r.ApplicationUser)
                    .Where(m => m.Homes.CompanyId == company.Id);

                return View(await applicationDbContext.ToListAsync());
            }
            else if (User.IsInRole("Manager"))
            {
                // Encontre o funcionario associado ao usuário autenticado
                var manager = await _context.Managers
                    .Include(m => m.Company) // Inclua a empresa associada ao funcionario
                    .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

                if (manager == null)
                {
                    // O usuário autenticado não é um funcionario
                    return NotFound();
                }

                // Obtenha a empresa associada ao gestor
                var company = manager.Company;

                var applicationDbContext = _context.Rentings
                    .Include(r => r.Homes)
                        .ThenInclude(m => m.Company)
                    .Include(r => r.ApplicationUser)
                    .Where(m => m.Homes.CompanyId == company.Id);

                return View(await applicationDbContext.ToListAsync());
            }

            return NotFound();
        }

        // GET: Rentings/Details/5
        public async Task<IActionResult> DetailsRentings(int? id)
        {
            if (id == null || _context.Rentings == null)
            {
                return NotFound();
            }

            var renting = await _context.Rentings
                .Include(r => r.Homes)
                    .ThenInclude(h => h.typeResidence)
                .Include(r => r.Homes)
                    .ThenInclude(h => h.Category)
                .Include(r => r.Homes)
                    .ThenInclude(h => h.Company)
                .Include(r => r.Homes)
                    .ThenInclude(h => h.District)
                .Include(h => h.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (renting == null)
            {
                return NotFound();
            }

            return View(renting);
        }

        // GET: Rentings/Create
        [Authorize(Roles = "Client")]
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Só utilizadores autenticados que podem efetuar arrendamentos";
                return Redirect("/Identity/Account/Login");
            }

            ViewData["HomeId"] = new SelectList(_context.Homes, "Id", "Address");
            return View();
        }

        // POST: Rentings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Price,BeginDate,EndDate,MinimumPeriod,MaximumPeriod,HomeId,UserId")] Renting renting)
        {

            ModelState.Remove(nameof(renting.Homes));
            ModelState.Remove(nameof(renting.ApplicationUser));


            if (ModelState.IsValid)
            {
                renting.ApplicationUserId = _context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault().Id;

                // Verifica se BeginDate e EndDate têm valores antes de calcular a diferença TimeSpan
                if (renting.BeginDate.HasValue && renting.EndDate.HasValue)
                {
                    TimeSpan duration = renting.EndDate.Value - renting.BeginDate.Value;

                    int durationInDays = (int)duration.TotalDays;

                    int minimumPeriod = _context.Homes.Where(h => h.Id == renting.HomeId)
                                                         .Select(h => h.MinimumPeriod)
                                                         .FirstOrDefault();

                    if (durationInDays < minimumPeriod)
                    {

                        // Adiciona uma mensagem de erro informando que as datas são obrigatórias
                        ModelState.AddModelError(string.Empty, "As datas selecionadas não respeitam o período minímo de arrendamento da habitação selecionada");

                        // Retorna a view com as mensagens de erro
                        ViewData["HomeId"] = new SelectList(_context.Homes, "Id", "Address", renting.HomeId);
                        return View(renting);
                    }

                    decimal pricePerDay = (decimal)_context.Homes.Where(h => h.Id == renting.HomeId)
                                                         .Select(h => h.PriceToRent)
                                                         .FirstOrDefault();

                    decimal totalPrice = durationInDays * pricePerDay;

                    renting.Price = totalPrice;
                    renting.IsApproved = false;

                }
                else
                {
                    // Adiciona uma mensagem de erro informando que as datas são obrigatórias
                    ModelState.AddModelError(string.Empty, "As datas de início e término são obrigatórias.");


                    // Retorna a view com as mensagens de erro
                    ViewData["HomeId"] = new SelectList(_context.Homes, "Id", "Address", renting.HomeId);
                    return View(renting);
                }

                _context.Add(renting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyRentings));
            }
            ViewData["HomeId"] = new SelectList(_context.Homes, "Id", "Address", renting.HomeId);
            return View(renting);
        }

        // GET: Rentings/Edit/5
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rentings == null)
            {
                return NotFound();
            }

            var renting = await _context.Rentings.FindAsync(id);
            if (renting == null)
            {
                return NotFound();
            }
            ViewData["HomeId"] = new SelectList(_context.Homes, "Id", "Address", renting.HomeId);
            return View(renting);
        }

        // POST: Rentings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Employee,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Price,BeginDate,EndDate,MinimumPeriod,MaximumPeriod,HomeId")] Renting renting)
        {
            if (id != renting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(renting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentingExists(renting.Id))
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
            ViewData["HomeId"] = new SelectList(_context.Homes, "Id", "Address", renting.HomeId);
            return View(renting);
        }

        // GET: Rentings/Delete/5
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Rentings == null)
            {
                return NotFound();
            }

            var renting = await _context.Rentings
                .Include(r => r.Homes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (renting == null)
            {
                return NotFound();
            }

            return View(renting);
        }

        // POST: Rentings/Delete/5
        [Authorize(Roles = "Employee,Manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rentings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Rentings'  is null.");
            }
            var renting = await _context.Rentings.FindAsync(id);
            if (renting != null)
            {
                _context.Rentings.Remove(renting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(RentingsIndex));
        }

        private bool RentingExists(int id)
        {
            return (_context.Rentings?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> ConfirmRenting(int? id)
        {
            if (id == null || _context.Rentings == null)
            {
                return NotFound();
            }
            var renting = await _context.Rentings
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (renting == null)
            {
                return NotFound();
            }
            ViewData["HomeId"] = new SelectList(_context.Homes, "Id", "Address", renting.HomeId);
            return View(renting);
        }

        [Authorize(Roles = "Employee,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRenting(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingRenting = await _context.Rentings.FindAsync(id);

                    if (existingRenting == null)
                    {
                        return NotFound();
                    }

                    existingRenting.IsApproved = true;



                    if (User.IsInRole("Employee"))
                    {

                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


                        var employee = await _context.Employees
                            .Include(m => m.Company)
                            .Include(m => m.Rentings)
                            .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

                        if (employee == null)
                        {
                            return NotFound();
                        }

                        existingRenting.ResponsibleId = employee.ApplicationUserId;


                        // Adiciona o arrendamento à lista de arrendamentos do cliente
                        employee.Rentings.Add(existingRenting);


                    }
                    else if (User.IsInRole("Manager"))
                    {

                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                        var manager = await _context.Managers
                            .Include(m => m.Company)
                            .Include(m => m.Rentings)
                            .FirstOrDefaultAsync(m => m.ApplicationUserId == userId);

                        if (manager == null)
                        {
                            return NotFound();
                        }


                        existingRenting.ResponsibleId = manager.ApplicationUserId;



                        //adiciona o arrendamento a lista de arrendamentos do manager
                        manager.Rentings = new List<Renting> { existingRenting };
                    }

                    _context.Update(existingRenting);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(RentingsIndex));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentingExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Se o modelo não for válido, adicione uma mensagem à ModelState
            ModelState.AddModelError(string.Empty, "Ocorreu um erro ao confirmar o arrendamento.");

            // Retorna a view com a mensagem de erro
            return RedirectToAction(nameof(RentingsIndex));
        }

        //#############################
        //
        // Forms Estados
        //
        //#############################

        // GET: Rentings/Index
        [Authorize(Roles = "Employee,Manager")]
        public IActionResult FormIndex()
        {
            var homeStatusList = _context.HomeStatus
                .Include(h => h.Renting)
                .Include(h => h.ApplicationUser)
                .ToList();

            return View(homeStatusList);
        }




        // GET: Rentings/Create
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> CreateForm()
        {


            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var rentings = _context.Rentings.Where(c => c.ResponsibleId == userId).Select(c => c.Id).Distinct().ToList();
            ViewBag.Rentings = new SelectList(rentings);

            return View();
        }

        // POST: Rentings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateForm([Bind("Id,Equipments,Damage,Observation,ApplicationUserId,Files,RentingId")] HomeStatus homeStatus)
        {
            try
            {
                ModelState.Remove(nameof(homeStatus.ApplicationUser));
                ModelState.Remove(nameof(homeStatus.Renting));

                if (ModelState.IsValid)
                {
                    // Obtenha o ID do usuário autenticado
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var rentings = _context.Rentings.Where(c => c.ResponsibleId == userId).Select(c => c.Id).Distinct().ToList();
                    ViewBag.Rentings = new SelectList(rentings);

                    if (User.IsInRole("Manager") || User.IsInRole("Employee"))
                    {
                        var renting = await _context.Rentings.FindAsync(homeStatus.RentingId);

                        if (renting == null)
                        {
                            return NotFound();
                        }

                        // Salvar imagens no sistema de arquivos
                        if (homeStatus.Files != null && homeStatus.Files.Count > 0)
                        {
                            var uploadedImageUrls = new List<string>();

                            foreach (var file in homeStatus.Files)
                            {
                                var imageUrl = await UploadImage(homeStatus.Id, file);
                                uploadedImageUrls.Add(imageUrl);
                            }

                            // Associe a URL da imagem ao modelo
                            homeStatus.ImgUrls = uploadedImageUrls.FirstOrDefault();
                        }

                        homeStatus.ApplicationUser = await _context.Users.FindAsync(userId);
                        homeStatus.Renting = renting;

                        _context.Add(homeStatus);
                        await _context.SaveChangesAsync();

                        if (User.IsInRole("Manager") || User.IsInRole("Employee"))
                        {
                            return RedirectToAction(nameof(FormIndex));
                        }
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

            return View(homeStatus);
        }





        private async Task<List<string>> UploadImages(int formId, List<IFormFile> imageFiles)
        {
            if (imageFiles == null || imageFiles.Count == 0)
                return null;

            // Diretório base para salvar as imagens (assumindo que 'imgForms' seja o diretório virtual para as imagens dos formulários)
            var baseUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imgForms");

            // Diretório específico para o formulário usando seu identificador único
            var formUploadsFolder = Path.Combine(baseUploadsFolder, formId.ToString());

            // Garante que o diretório de destino existe, caso contrário, cria-o
            if (!Directory.Exists(formUploadsFolder))
            {
                Directory.CreateDirectory(formUploadsFolder);
            }

            var uploadedImageUrls = new List<string>();

            foreach (var imageFile in imageFiles)
            {
                if (imageFile.Length == 0)
                    continue;

                // Gera um nome de arquivo único para evitar colisões
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;

                // Caminho completo para o arquivo no sistema de arquivos
                var filePath = Path.Combine(formUploadsFolder, uniqueFileName);

                // Salva a imagem no sistema de arquivos
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Adiciona a URL da imagem à lista
                var imageUrl = Path.Combine("imgForms", formId.ToString(), uniqueFileName);
                uploadedImageUrls.Add(imageUrl);
            }

            return uploadedImageUrls;
        }

        private async Task<string> UploadImage(int formId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            // Diretório base para salvar as imagens (assumindo que 'imgForms' seja o diretório virtual para as imagens dos formulários)
            var baseUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imgForms");

            // Diretório específico para o formulário usando seu identificador único
            var formUploadsFolder = Path.Combine(baseUploadsFolder, formId.ToString());

            // Garante que o diretório de destino existe, caso contrário, cria-o
            if (!Directory.Exists(formUploadsFolder))
            {
                Directory.CreateDirectory(formUploadsFolder);
            }

            // Gera um nome de arquivo único para evitar colisões
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;

            // Caminho completo para o arquivo no sistema de arquivos
            var filePath = Path.Combine(formUploadsFolder, uniqueFileName);

            // Salva a imagem no sistema de arquivos
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Retorna a URL da imagem específica do formulário
            return Path.Combine("imgForms", formId.ToString(), uniqueFileName);
        }







        //#############################
        //
        // Rentings do Cliente
        //
        //#############################


        // GET: Rentings/Details/5
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> DetailsMyRentings(int? id)
        {
            if (id == null || _context.Rentings == null)
            {
                return NotFound();
            }

            var renting = await _context.Rentings
                .Include(r => r.Homes)
                    .ThenInclude(h => h.typeResidence)
                .Include(r => r.Homes)
                    .ThenInclude(h => h.Category)
                .Include(r => r.Homes)
                    .ThenInclude(h => h.Company)
                .Include(r => r.Homes)
                    .ThenInclude(h => h.District)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (renting == null)
            {
                return NotFound();
            }

            return View(renting);
        }

        [Authorize(Roles = "Client")]
        public IActionResult MyRentings()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtém os arrendamentos do cliente com base no UserId
            var myRentings = _context.Rentings
                .Include(r => r.Homes)
                .Include(r => r.ApplicationUser)
                .Where(a => a.ApplicationUserId == userId)
                .ToList();

            return View(myRentings);
        }

    }
}
