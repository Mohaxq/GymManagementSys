using GymManagementBLL.Services.InterFaces;
using GymManagementDAL.Entities;
using GymManagementDAL.Entities.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
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
