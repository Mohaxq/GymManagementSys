using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.PlanViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }
        public IActionResult Index()
        {
            var Plans = _planService.GetAllPlans();
            return View(Plans);
        }
        public ActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Plan ID.";
                return RedirectToAction("Index");
            }
            var plan = _planService.GetPlanById(id);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return NotFound();
            }
            return View(plan);
        }
        public ActionResult Edit(int id)
        {
            if(id<=0)
            {
                TempData["ErrorMessage"] = "Invalid Plan ID.";
                return RedirectToAction("Index");
            }
            var plan = _planService.GetPlanForUpdate(id);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return NotFound();
            }
            return View(plan);
        }
        [HttpPost]
        public  ActionResult Edit([FromRoute]int id , UpdatePlanViewModel model)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("WrongData", "Check data and missing field");
                return View(model);

            }
            var result = _planService.UpdatePlan(id, model);
            if(!result)
            {
                TempData["ErrorMessage"] = "Failed to update plan.";
                
            }
            else
            {
                TempData["SuccessMessage"] = "Plan updated successfully.";
                
            }
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult Activate([FromRoute]int id)
        {
            var result = _planService.TogglePlanStatus(id);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to change plan status.";
            }
            else
            {
                TempData["SuccessMessage"] = "Plan status changed successfully.";
            }
            return RedirectToAction("Index");
        }
    }
}
