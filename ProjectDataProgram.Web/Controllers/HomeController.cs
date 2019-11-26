using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectDataProgram.Web.Models;

namespace ProjectDataProgram.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("AdminAupervisor"))
            {
                return View("IndexAdmn");
            }
            else if (User.IsInRole("ProjectManager"))
            {
                return View("IndexPM");
            }
            else if (User.IsInRole("Employee"))
            {
                return View("IndexEmp");
            }
            else
            {
                return View();
            }
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
