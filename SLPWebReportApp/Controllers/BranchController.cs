using Microsoft.AspNetCore.Mvc;
using SLPDBLibrary;
using System.Diagnostics;

namespace SLPWebReportApp.Controllers
{
    
    [Controller]
    public class BranchController : Controller
    {
        private readonly ILogger<BranchController> _loger;

        public BranchController(ILogger<BranchController> loger)
        {
            _loger = loger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
