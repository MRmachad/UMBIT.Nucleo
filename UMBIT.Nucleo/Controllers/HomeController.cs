using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UMBIT.Nucleo.Models;

namespace UMBIT.Nucleo.Controllers
{

    [Route("")]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    }