using Library.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Library.Controllers.MVC
{
    public class AuthorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}