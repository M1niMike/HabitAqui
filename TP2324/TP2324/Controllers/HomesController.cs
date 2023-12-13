using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TP2324.Data;
using TP2324.Models;
using TP2324.ViewModels;

namespace TP2324.Controllers
{
    public class HomesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(PesquisaHabitacaoViewModel viewModel)
        {
            IQueryable<Home> homesQuery = _context.Homes.Include(m => m.Category).Include(m => m.typeResidence).Include(m => m.District).Include(m => m.Company);

            if (!string.IsNullOrEmpty(viewModel.TextoAPesquisar))
            {
                homesQuery = homesQuery.Where(c => c.typeResidence.Name == viewModel.TextoAPesquisar);
            }


            if (!string.IsNullOrEmpty(viewModel.LocalizacaoSelecionada))
            {
                homesQuery = homesQuery.Where(c => c.District.Name == viewModel.LocalizacaoSelecionada);
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







        // GET: Homes/Details/5
        public async Task<IActionResult> Details(int? id)
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


        [HttpGet]
        public async Task<IActionResult> Search(string? TextoAPesquisar, string? TipoResidenciaSelecionado, string? CategoriaSelecinada, string? PeriodoMinimoSelecionado, string? LocalizacaoSelecionada)
        {
            PesquisaHabitacaoViewModel pesquisaVM = new PesquisaHabitacaoViewModel();
            ViewData["Title"] = "Pesquisa habitações";

            var typeResidences = _context.TypeResidences.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeTypes = new SelectList(typeResidences);

            var category = _context.Category.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeCategory = new SelectList(category);

            var districts = _context.Districts.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeDistrict = new SelectList(districts);

            var companies = _context.Companies.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeCompany = new SelectList(companies);

            if (string.IsNullOrWhiteSpace(TextoAPesquisar))
            {

                if(string.IsNullOrEmpty(TipoResidenciaSelecionado) && string.IsNullOrEmpty(CategoriaSelecinada) && string.IsNullOrEmpty(PeriodoMinimoSelecionado) && string.IsNullOrEmpty(LocalizacaoSelecionada))
                {
                    pesquisaVM.Homeslist = await _context.Homes
                   .Include(m => m.Category)
                   .Include(m => m.typeResidence)
                   .Include(m => m.District)
                   .Include(m => m.Company)
                   .OrderBy(c => c.Category.Name)
                   .ToListAsync();
                }
                else if (!string.IsNullOrEmpty(TipoResidenciaSelecionado))
                {
                    pesquisaVM.Homeslist = await _context.Homes
                    .Include(m => m.Category)
                    .Include(m => m.typeResidence)
                    .Include(m => m.District)
                    .Include(m => m.Company)
                    .Where(c =>
                        (c.typeResidence != null && c.typeResidence.Name.Contains(TipoResidenciaSelecionado)))
                    .ToListAsync();

                    pesquisaVM.TextoAPesquisar = TipoResidenciaSelecionado;
                }
                else if (!string.IsNullOrEmpty(CategoriaSelecinada))
                {
                    pesquisaVM.Homeslist = await _context.Homes
                    .Include(m => m.Category)
                    .Include(m => m.typeResidence)
                    .Include(m => m.District)
                    .Include(m => m.Company)
                    .Where(c =>
                        (c.typeResidence != null && c.Category.Name.Contains(CategoriaSelecinada)))
                    .ToListAsync();

                    pesquisaVM.TextoAPesquisar = CategoriaSelecinada;
                }
                else if (!string.IsNullOrEmpty(PeriodoMinimoSelecionado))
                {
                    pesquisaVM.Homeslist = await _context.Homes
                    .Include(m => m.Category)
                    .Include(m => m.typeResidence)
                    .Include(m => m.District)
                    .Include(m => m.Company)
                    .Where(c =>
                        (c.typeResidence != null && c.MinimumPeriod.ToString().Contains(PeriodoMinimoSelecionado)))
                    .ToListAsync();

                    pesquisaVM.TextoAPesquisar = PeriodoMinimoSelecionado;
                }
                else if (!string.IsNullOrEmpty(LocalizacaoSelecionada))
                {
                    pesquisaVM.Homeslist = await _context.Homes
                    .Include(m => m.Category)
                    .Include(m => m.typeResidence)
                    .Include(m => m.District)
                    .Include(m => m.Company)
                    .Where(c =>
                        (c.typeResidence != null && c.District.Name.Contains(LocalizacaoSelecionada)))
                    .ToListAsync();

                    pesquisaVM.TextoAPesquisar = LocalizacaoSelecionada;
                }


            }
            else
            {
                pesquisaVM.Homeslist = await _context.Homes
                    .Include(m => m.Category)
                    .Include(m => m.typeResidence)
                    .Include(m => m.District)
                    .Include(m => m.Company)
                    .Where(c =>
                        (c.typeResidence != null && c.typeResidence.Name.Contains(TextoAPesquisar)) ||
                        c.Description.Contains(TextoAPesquisar) ||
                        c.PriceToRent.ToString().Contains(TextoAPesquisar) ||
                        c.MinimumPeriod.ToString().Contains(TextoAPesquisar) ||
                        c.Category.Name.Contains(TextoAPesquisar) ||
                        c.District.Name.Contains(TextoAPesquisar)
                    )
                    .ToListAsync();

                pesquisaVM.TextoAPesquisar = TextoAPesquisar;
            }

            pesquisaVM.NumResultados = pesquisaVM.Homeslist.Count();

            return View(pesquisaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([Bind("TextoAPesquisar,TipoResidenciaSelecionado,CategoriaSelecinada,PeriodoMinimoSelecionado")] PesquisaHabitacaoViewModel pesquisaHabitacao)
        {
            var typeResidences = _context.TypeResidences.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeTypes = new SelectList(typeResidences);

            var category = _context.Category.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeCategory = new SelectList(category);

            var districts = _context.Districts.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeDistrict = new SelectList(districts);

            var companies = _context.Companies.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeCompany = new SelectList(companies);

            if (string.IsNullOrEmpty(pesquisaHabitacao.TextoAPesquisar))
            {
                if (string.IsNullOrEmpty(pesquisaHabitacao.TipoResidenciaSelecionado) && string.IsNullOrEmpty(pesquisaHabitacao.CategoriaSelecinada) && string.IsNullOrEmpty(pesquisaHabitacao.PeriodoMinimoSelecionado) && string.IsNullOrEmpty(pesquisaHabitacao.LocalizacaoSelecionada))
                {
                    pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence).Include(m => m.District).Include(m => m.Company).OrderBy(c => c.Category.Name).ToListAsync();
                    pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
                }
                else if(!string.IsNullOrEmpty(pesquisaHabitacao.TipoResidenciaSelecionado))
                {
                    pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence).Include(m => m.District).Include(m => m.Company)
                    .Where(e => e.typeResidence.Name.Contains(pesquisaHabitacao.TipoResidenciaSelecionado)).OrderBy(c => c.typeResidence.Name).ToListAsync();
                    pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
                }
                else if (!string.IsNullOrEmpty(pesquisaHabitacao.CategoriaSelecinada))
                {
                    pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence).Include(m => m.District).Include(m => m.Company)
                    .Where(e => e.Category.Name.Contains(pesquisaHabitacao.CategoriaSelecinada)).OrderBy(c => c.typeResidence.Name).ToListAsync();
                    pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
                }
                else if (!string.IsNullOrEmpty(pesquisaHabitacao.PeriodoMinimoSelecionado))
                {
                    pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence).Include(m => m.District).Include(m => m.Company)
                    .Where(e => e.MinimumPeriod.ToString().Contains(pesquisaHabitacao.PeriodoMinimoSelecionado)).OrderBy(c => c.typeResidence.Name).ToListAsync();
                    pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
                }
                else if (!string.IsNullOrEmpty(pesquisaHabitacao.LocalizacaoSelecionada))
                {
                    pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence).Include(m => m.District).Include(m => m.Company)
                    .Where(e => e.District.Name.Contains(pesquisaHabitacao.LocalizacaoSelecionada)).OrderBy(c => c.typeResidence.Name).ToListAsync();
                    pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
                }


            }
            else
            {
                pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence).Include(m => m.District).Include(m => m.Company)
                    .Where(e => e.typeResidence.Name.Contains(pesquisaHabitacao.TextoAPesquisar) ||
                                e.Description.Contains(pesquisaHabitacao.TextoAPesquisar) ||
                                e.PriceToRent.ToString().Contains(pesquisaHabitacao.TextoAPesquisar) ||
                                e.Category.Name.Contains(pesquisaHabitacao.TextoAPesquisar) ||
                                e.District.Name.Contains(pesquisaHabitacao.TextoAPesquisar)
                    ).OrderBy(c => c.typeResidence.Name).ToListAsync();
                pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
            }

            return View(pesquisaHabitacao);
        }


        // GET: Homes/Create
        [Authorize(Roles = "Employee")]
        public IActionResult Create()
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
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create([Bind("Id,TypeResidenceId,CategoryId,DistrictId,CompanyId,PriceToRent,NumWC,Address,SquareFootage,NumParks,Wifi,Description,MinimumPeriod,Available,ImgUrl,Ratings")] Home home)
        {
            ModelState.Remove(nameof(home.Category));
            ModelState.Remove(nameof(home.typeResidence));
            ModelState.Remove(nameof(home.Rentings));
            ModelState.Remove(nameof(home.District));
            ModelState.Remove(nameof(home.Company));



            if (ModelState.IsValid)
            {
                _context.Add(home);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", home.CategoryId);
            ViewData["TypeResidenceId"] = new SelectList(_context.TypeResidences, "Id", "Name", home.TypeResidenceId);
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name", home.DistrictId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", home.CompanyId);
            return View(home);
        }



        // GET: Homes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Homes == null)
            {
                return NotFound();
            }

            var home = await _context.Homes.FindAsync(id);
            if (home == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", home.CategoryId);
            ViewData["TypeResidenceId"] = new SelectList(_context.TypeResidences, "Id", "Name", home.TypeResidenceId);
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name", home.DistrictId);
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", home.CompanyId);

            return View(home);
        }

        // POST: Homes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TypeResidenceId,CategoryId,DistrictId,CompanyId,PriceToRent,NumWC,Address,SquareFootage,NumParks,Wifi,Description,BeginDate,EndDate,MinimumPeriod,Available,ImgUrl")] Home home)
        {
            if (id != home.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(home.Category));
            ModelState.Remove(nameof(home.typeResidence));
            ModelState.Remove(nameof(home.District));
            ModelState.Remove(nameof(home.Rentings));
            ModelState.Remove(nameof(home.Company));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(home);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomeExists(home.Id))
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
            return View(home);
        }

        // GET: Homes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Homes == null)
            {
                return NotFound();
            }

            var home = await _context.Homes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (home == null)
            {
                return NotFound();
            }

            return View(home);
        }

        // POST: Homes/Delete/5
        [HttpPost, ActionName("Delete")]
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
            return RedirectToAction(nameof(Index));
        }

        private bool HomeExists(int id)
        {
          return (_context.Homes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
