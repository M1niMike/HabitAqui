using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP2324.Models;

namespace TP2324.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
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

