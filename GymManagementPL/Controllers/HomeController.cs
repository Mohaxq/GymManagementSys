using GymManagementBLL.Services.InterFaces;
using GymManagementDAL.Entities;
using GymManagementDAL.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class HomeController : Controller
    {
        private readonly IAnalysisService _analysisService;

        public HomeController(IAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }
        public ViewResult Index()
        {
            var data = _analysisService.GetAnalysisData();
            return View(data);
        }

    }
}
