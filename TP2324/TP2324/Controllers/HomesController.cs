using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            IQueryable<Home> homesQuery = _context.Homes.Include(m => m.Category).Include(m => m.typeResidence);

            if (!string.IsNullOrEmpty(viewModel.TextoAPesquisar))
            {
                homesQuery = homesQuery.Where(c => c.typeResidence.Name == viewModel.TextoAPesquisar);
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

            var home = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (home == null)
            {
                return NotFound();
            }

            return View(home);
        }

        // GET: Homes/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["TypeResidenceId"] = new SelectList(_context.TypeResidences, "Id", "Name");
            return View();
        }


        //// Old_SearchBar
        //[HttpPost]
        //public async Task<IActionResult> Index(string? pesquisa)
        //{
        //    return View(await _context.Homes
        //        .Where(e => e.Address.Contains(pesquisa) || e.Category.Name.Contains(pesquisa))
        //        .Include(m => m.Category)
        //        .ToListAsync());
        //}

        //New_SearchBar (GET: Homes/Search)

        [HttpGet]
        public async Task<IActionResult> Search(string? TextoAPesquisar, string? TipoResidenciaSelecionado, string? CategoriaSelecinada, string? PeriodoMinimoSelecionado)
        {
            PesquisaHabitacaoViewModel pesquisaVM = new PesquisaHabitacaoViewModel();
            ViewData["Title"] = "Pesquisa habitações";

            var typeResidences = _context.TypeResidences.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeTypes = new SelectList(typeResidences);

            var category = _context.Category.Select(c => c.Name).Distinct().ToList();
            ViewBag.HomeCategory = new SelectList(category);

            if (string.IsNullOrWhiteSpace(TextoAPesquisar))
            {

                if(string.IsNullOrEmpty(TipoResidenciaSelecionado) && string.IsNullOrEmpty(CategoriaSelecinada) && string.IsNullOrEmpty(PeriodoMinimoSelecionado))
                {
                    pesquisaVM.Homeslist = await _context.Homes
                   .Include(m => m.Category)
                   .Include(m => m.typeResidence)
                   .OrderBy(c => c.Category.Name)
                   .ToListAsync();
                }
                else if (!string.IsNullOrEmpty(TipoResidenciaSelecionado))
                {
                    pesquisaVM.Homeslist = await _context.Homes
                    .Include(m => m.Category)
                    .Include(m => m.typeResidence)
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
                    .Where(c =>
                        (c.typeResidence != null && c.MinimumPeriod.ToString().Contains(PeriodoMinimoSelecionado)))
                    .ToListAsync();

                    pesquisaVM.TextoAPesquisar = PeriodoMinimoSelecionado;
                }


            }
            else
            {
                pesquisaVM.Homeslist = await _context.Homes
                    .Include(m => m.Category)
                    .Include(m => m.typeResidence)
                    .Where(c =>
                        (c.typeResidence != null && c.typeResidence.Name.Contains(TextoAPesquisar)) ||
                        c.Description.Contains(TextoAPesquisar) ||
                        c.PriceToRent.ToString().Contains(TextoAPesquisar) ||
                        c.MinimumPeriod.ToString().Contains(TextoAPesquisar) ||
                        c.Category.Name.Contains(TextoAPesquisar)
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

            if (string.IsNullOrEmpty(pesquisaHabitacao.TextoAPesquisar))
            {
                if (string.IsNullOrEmpty(pesquisaHabitacao.TipoResidenciaSelecionado) && string.IsNullOrEmpty(pesquisaHabitacao.CategoriaSelecinada) && string.IsNullOrEmpty(pesquisaHabitacao.PeriodoMinimoSelecionado))
                {
                    pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence).OrderBy(c => c.Category.Name).ToListAsync();
                    pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
                }
                else if(!string.IsNullOrEmpty(pesquisaHabitacao.TipoResidenciaSelecionado))
                {
                    pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence)
                    .Where(e => e.typeResidence.Name.Contains(pesquisaHabitacao.TipoResidenciaSelecionado)).OrderBy(c => c.typeResidence.Name).ToListAsync();
                    pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
                }
                else if (!string.IsNullOrEmpty(pesquisaHabitacao.CategoriaSelecinada))
                {
                    pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence)
                    .Where(e => e.Category.Name.Contains(pesquisaHabitacao.CategoriaSelecinada)).OrderBy(c => c.typeResidence.Name).ToListAsync();
                    pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
                }
                else if (!string.IsNullOrEmpty(pesquisaHabitacao.PeriodoMinimoSelecionado))
                {
                    pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence)
                    .Where(e => e.MinimumPeriod.ToString().Contains(pesquisaHabitacao.PeriodoMinimoSelecionado)).OrderBy(c => c.typeResidence.Name).ToListAsync();
                    pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
                }


            }
            else
            {
                pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence)
                    .Where(e => e.typeResidence.Name.Contains(pesquisaHabitacao.TextoAPesquisar) ||
                                e.Description.Contains(pesquisaHabitacao.TextoAPesquisar) ||
                                e.PriceToRent.ToString().Contains(pesquisaHabitacao.TextoAPesquisar) ||
                                e.Category.Name.Contains(pesquisaHabitacao.TextoAPesquisar)
                    ).OrderBy(c => c.typeResidence.Name).ToListAsync();
                pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
            }

            return View(pesquisaHabitacao);
        }

        //[HttpPost]
        //public async Task<IActionResult> getTypes()
        //{

        //}


       //[HttpPost]
       //[ValidateAntiForgeryToken]
       //public async Task<IActionResult> Search([Bind("TextoAPesquisar")] PesquisaHabitacaoViewModel pesquisaHabitacao)
       //{
       //    var typeResidences = _context.TypeResidences.Select(c => c.Name).Distinct().ToList();
       //    ViewBag.HomeTypes = new SelectList(typeResidences);


       //    IQueryable<Home> HomesList = _context.Homes.Include("Category").Include("typeResidence");

       //    if (string.IsNullOrEmpty(pesquisaHabitacao.TextoAPesquisar))
       //    {
       //        Console.WriteLine(pesquisaHabitacao.TextoAPesquisar);
       //        HomesList = _context.Homes.Include("Category").Include("typeResidence");
       //        pesquisaHabitacao.Homeslist = await HomesList.ToListAsync();

       //    }
       //    else
       //    {

       //        HomesList =
       //            _context.Homes.Include("Category").Include("typeResidence")
       //            .Where(e => e.typeResidence.Name.Contains(pesquisaHabitacao.TextoAPesquisar) ||
       //                        e.Description.Contains(pesquisaHabitacao.TextoAPesquisar) ||
       //                        e.PriceToRent.ToString().Contains(pesquisaHabitacao.TextoAPesquisar) ||
       //                        e.MinimumPeriod.ToString().Contains(pesquisaHabitacao.TextoAPesquisar) |
       //                        e.Category.Name.Contains(pesquisaHabitacao.TextoAPesquisar)
       //            ).OrderBy(c => c.typeResidence.Name)
       //        pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();

       //        ;
       //        pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category).Include(m => m.typeResidence)
       //            .Where(e => e.typeResidence.Name.Contains(pesquisaHabitacao.TextoAPesquisar) ||
       //                        e.Description.Contains(pesquisaHabitacao.TextoAPesquisar) ||
       //                        e.PriceToRent.ToString().Contains(pesquisaHabitacao.TextoAPesquisar) ||
       //                        e.MinimumPeriod.ToString().Contains(pesquisaHabitacao.TextoAPesquisar) |
       //                        e.Category.Name.Contains(pesquisaHabitacao.TextoAPesquisar)
       //            ).OrderBy(c => c.typeResidence.Name).ToListAsync();
       //        pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
       //    }

       //    return View(pesquisaHabitacao);
       //}



       // POST: Homes/Create
       // To protect from overposting attacks, enable the specific properties you want to bind to.
       // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
       [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TypeResidenceId,CategoryId,PriceToRent,NumWC,Address,SquareFootage,NumParks,Wifi,Description,MinimumPeriod,Available,ImgUrl,Ratings")] Home home)
        {
            ModelState.Remove(nameof(home.Category));
            ModelState.Remove(nameof(home.typeResidence));
            ModelState.Remove(nameof(home.Rentings));


            if (ModelState.IsValid)
            {
                _context.Add(home);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", home.CategoryId);
            ViewData["TypeResidenceId"] = new SelectList(_context.TypeResidences, "Id", "Name", home.TypeResidenceId);
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
            ViewData["TypeResidenceId"] = new SelectList(_context.TypeResidences, "Id", "Name", home.typeResidence);

            return View(home);
        }

        // POST: Homes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TypeResidenceId,CategoryId,PriceToRent,NumWC,Address,SquareFootage,NumParks,Wifi,Description,BeginDate,EndDate,MinimumPeriod,Available,ImgUrl")] Home home)
        {
            if (id != home.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(home.Category));
            ModelState.Remove(nameof(home.typeResidence));

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
