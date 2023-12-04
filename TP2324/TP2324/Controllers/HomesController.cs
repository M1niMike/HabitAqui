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

        // GET: Homes
        public async Task<IActionResult> Index(/*string? tipo*/)
        {
            IQueryable<Home> homesQuery = _context.Homes.Include(m => m.Category);

            //if (tipo.CompareTo("apartamento") == 1)
            //{
            //    homesQuery.Where(c => c.Type.Equals(tipo));
            //}
            //else if (tipo.CompareTo("casa") == 1)
            //{
            //    homesQuery.Where(c => c.Type.Equals(tipo));
            //}
            //else if (tipo == null)
            //{
            //    var homesAll = await homesQuery.ToListAsync();
            //    return View(homesAll);
            //}

            var homes = await homesQuery.ToListAsync();
            return View(homes);
        }


        // GET: Homes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Homes == null)
            {
                return NotFound();
            }

            var home = await _context.Homes.Include(m => m.Category)
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

        // GET: Cursos/Search
        public async Task<IActionResult> Search(string? TextoAPesquisar)
        {

            PesquisaHabitacaoViewModel pesquisaVM = new PesquisaHabitacaoViewModel();
            ViewData["Title"] = "Pesquisa cursos";

            if (string.IsNullOrWhiteSpace(TextoAPesquisar))
                pesquisaVM.Homeslist = await _context.Homes.Include(m => m.Category).OrderBy(c => c.Category.Name).ToListAsync();
            else
            {
                pesquisaVM.Homeslist =
                    await _context.Homes.Include(m => m.Category).Where(c => c.Type.Contains(TextoAPesquisar) ||
                                                                          c.Description.Contains(TextoAPesquisar)
                                                                         || c.PriceToRent.ToString().Contains(TextoAPesquisar)
                                                                         || c.Category.Name.Contains(TextoAPesquisar)
                                                                         ).ToListAsync();
                pesquisaVM.TextoAPesquisar = TextoAPesquisar;

            }
            pesquisaVM.NumResultados = pesquisaVM.Homeslist.Count();



            return View(pesquisaVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([Bind("TextoAPesquisar")] PesquisaHabitacaoViewModel pesquisaHabitacao)
        {
            if (string.IsNullOrEmpty(pesquisaHabitacao.TextoAPesquisar))
            {
                pesquisaHabitacao.Homeslist = await _context.Homes.OrderBy(c => c.Category.Name).ToListAsync();
                pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();
            }
            else
            {
                pesquisaHabitacao.Homeslist = await _context.Homes.Include(m => m.Category)
                .Where(e => e.Type.Contains(pesquisaHabitacao.TextoAPesquisar)||
                            e.Description.Contains(pesquisaHabitacao.TextoAPesquisar) ||
                            e.PriceToRent.ToString().Contains(pesquisaHabitacao.TextoAPesquisar) ||
                            e.Category.Name.Contains(pesquisaHabitacao.TextoAPesquisar)
                            )/*.OrderBy(c => c.Type)*/.ToListAsync();
                pesquisaHabitacao.NumResultados = pesquisaHabitacao.Homeslist.Count();

            }

            return View(pesquisaHabitacao);
        }


        // POST: Homes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,CategoryId,PriceToRent,PriceToPurchase,NumWC,Address,SquareFootage,NumParks,Wifi,Description,toRent,toPurchase")] Home home)
        {
            ModelState.Remove(nameof(home.Category));
            if (ModelState.IsValid)
            {
                _context.Add(home);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", home.CategoryId);
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

            return View(home);
        }

        // POST: Homes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,CategoryId,PriceToRent,PriceToPurchase,NumWC,Address,SquareFootage,NumParks,Wifi,Description,toRent,toPurchase")] Home home)
        {
            if (id != home.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(home.Category));

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
