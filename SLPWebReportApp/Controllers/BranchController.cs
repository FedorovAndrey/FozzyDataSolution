using Microsoft.AspNetCore.Mvc;

namespace SLPWebReportApp.Controllers
{
    [Controller]
    public class BranchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
