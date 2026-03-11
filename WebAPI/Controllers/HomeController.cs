using System.Diagnostics;
using BepopStreamProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace BepopStreamProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
