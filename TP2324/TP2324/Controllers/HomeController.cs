using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP2324.Data;
using TP2324.Models;

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
        IQueryable<Home> homes = _context.Homes;
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

