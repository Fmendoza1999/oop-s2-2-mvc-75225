using FoodSafetyTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Diagnostics;

namespace FoodSafetyTracker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Log.Information("Home page visited by {UserName}",
                User?.Identity?.Name ?? "Anonymous");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            Log.Error("Error page displayed. RequestId={RequestId}",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}