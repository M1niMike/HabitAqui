using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TP2324.Data;
using TP2324.Models;
using TP2324.ViewModels;

namespace TP2324.Controllers;

public class HomeController : Controller
{
    //private readonly ILogger<HomeController> _logger;

    //public HomeController(ILogger<HomeController> logger)
    //{
    //    _logger = logger;
    //}

    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var typeResidences = _context.TypeResidences.Select(c => c.Name).Distinct().ToList();
        ViewBag.HomeTypes = new SelectList(typeResidences);

        var category = _context.Category.Select(c => c.Name).Distinct().ToList();
        ViewBag.HomeCategory = new SelectList(category);

        IQueryable<Home> homes = _context.Homes.Include(m => m.Category).Include(m => m.typeResidence);
        return View(homes.ToList());

    }


    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Client()
    {
        return View();
    }

    public IActionResult Employee()
    {
        return View();
    }

    public IActionResult Manager()
    {
        return View();
    }

    public IActionResult Rent()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

