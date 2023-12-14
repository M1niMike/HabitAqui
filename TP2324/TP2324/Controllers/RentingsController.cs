using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public RentingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Rentings
        public async Task<IActionResult> EmployeeRentingsIndex()
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

            var applicationDbContext = _context.Rentings
                .Include(r => r.Homes)
                    .ThenInclude(m => m.Company)
                .Include(r => r.ApplicationUser)
                .Where(m => m.Homes.CompanyId == company.Id);

            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Rentings
        public async Task<IActionResult> ManagerRentingsIndex()
        {
            // Obtenha o ID do usuário autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);



            var applicationDbContext = _context.Rentings.Include(r => r.Homes).Include(r => r.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
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

                     if(durationInDays < minimumPeriod)
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
        //[Authorize(Roles = "Admin")]
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
            return RedirectToAction(nameof(EmployeeRentingsIndex));
        }

        private bool RentingExists(int id)
        {
          return (_context.Rentings?.Any(e => e.Id == id)).GetValueOrDefault();
        }

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

                        //adiciona o arrendamento a lista de arrendamentos do cliente
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

                    return RedirectToAction(nameof(EmployeeRentingsIndex));
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
            return RedirectToAction(nameof(EmployeeRentingsIndex));
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

        [Authorize(Roles ="Client")]
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
