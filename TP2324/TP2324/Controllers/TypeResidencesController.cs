using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TP2324.Data;
using TP2324.Models;

namespace TP2324.Controllers
{
    public class TypeResidencesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TypeResidencesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TypeResidences
        public async Task<IActionResult> typeResidenceList()
        {
              return _context.TypeResidences != null ? 
                          View(await _context.TypeResidences.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.TypeResidences'  is null.");
        }

        // GET: TypeResidences/Details/5
        public async Task<IActionResult> typeResidenceDetails(int? id)
        {
            if (id == null || _context.TypeResidences == null)
            {
                return NotFound();
            }

            var typeResidence = await _context.TypeResidences
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeResidence == null)
            {
                return NotFound();
            }

            return View(typeResidence);
        }

        // GET: TypeResidences/Create
        public IActionResult typeResidenceCreate()
        {
            return View();
        }

        // POST: TypeResidences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> typeResidenceCreate([Bind("Id,Name,Available,Homes")] TypeResidence typeResidence)
        {
            ModelState.Remove(nameof(typeResidence.Homes));

            if (ModelState.IsValid)
            {
                _context.Add(typeResidence);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(typeResidenceList));
            }
            return View(typeResidence);
        }

        // GET: TypeResidences/Edit/5
        public async Task<IActionResult> typeResidenceEdit(int? id)
        {
            if (id == null || _context.TypeResidences == null)
            {
                return NotFound();
            }

            var typeResidence = await _context.TypeResidences.FindAsync(id);
            if (typeResidence == null)
            {
                return NotFound();
            }
            return View(typeResidence);
        }

        // POST: TypeResidences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> typeResidenceEdit(int id, [Bind("Id,Name,Available,Homes")] TypeResidence typeResidence)
        {
            if (id != typeResidence.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(typeResidence.Homes));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(typeResidence);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeResidenceExists(typeResidence.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(typeResidenceList));
            }
            return View(typeResidence);
        }

        // GET: TypeResidences/Delete/5
        public async Task<IActionResult> typeResidenceDelete(int? id)
        {
            if (id == null || _context.TypeResidences == null)
            {
                return NotFound();
            }

            var typeResidence = await _context.TypeResidences
                .FirstOrDefaultAsync(m => m.Id == id);
            if (typeResidence == null)
            {
                return NotFound();
            }

            return View(typeResidence);
        }

        // POST: TypeResidences/Delete/5
        [HttpPost, ActionName("typeResidenceDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TypeResidences == null)
            {
                return Problem("Entity set 'ApplicationDbContext.TypeResidences'  is null.");
            }
            var typeResidence = await _context.TypeResidences.FindAsync(id);
            if (typeResidence != null)
            {
                _context.TypeResidences.Remove(typeResidence);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(typeResidenceList));
        }

        private bool TypeResidenceExists(int id)
        {
          return (_context.TypeResidences?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
