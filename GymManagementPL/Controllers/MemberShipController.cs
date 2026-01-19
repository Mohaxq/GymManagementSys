using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.MemberShipViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class MemberShipController : Controller
    {
        private readonly IMemberShipService _memberShipService;

        public MemberShipController(IMemberShipService memberShipService)
        {
            _memberShipService = memberShipService;
        }

        public ActionResult Index()
        {
            var memberships = _memberShipService.GetAllMemberShips();
            return View(memberships);
        }

        public ActionResult Details(int memberId, int planId)
        {
            if (memberId <= 0 || planId <= 0)
            {
                TempData["Error"] = "Invalid Membership";
                return RedirectToAction(nameof(Index));
            }

            var membership = _memberShipService.GetMemberShipById(memberId, planId);
            if (membership is null)
            {
                TempData["Error"] = "Membership not found";
                return RedirectToAction(nameof(Index));
            }

            return View(membership);
        }

        public ActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateMemberShipViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropDowns();
                return View(model);
            }

            var (success, message) = _memberShipService.CreateMemberShip(model);

            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = message;
                PopulateDropDowns();
                return View(model);
            }
        }

        public ActionResult Cancel(int memberId, int planId)
        {
            if (memberId <= 0 || planId <= 0)
            {
                TempData["Error"] = "Invalid Membership";
                return RedirectToAction(nameof(Index));
            }

            var membership = _memberShipService.GetMemberShipById(memberId, planId);
            if (membership is null)
            {
                TempData["Error"] = "Membership not found";
                return RedirectToAction(nameof(Index));
            }

            return View(membership);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelConfirmed(int memberId, int planId)
        {
            var (success, message) = _memberShipService.CancelMemberShip(memberId, planId);

            if (success)
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction(nameof(Index));
        }

        #region Private Helper Methods
        private void PopulateDropDowns()
        {
            var members = _memberShipService.GetMembersForDropDown();
            var plans = _memberShipService.GetActivePlansForDropDown();

            ViewBag.Members = new SelectList(members, "Id", "Name");
            ViewBag.Plans = new SelectList(
                plans.Select(p => new
                {
                    p.Id,
                    DisplayText = $"{p.Name} - ${p.Price} ({p.DurationInDays} days)"
                }),
                "Id",
                "DisplayText"
            );
        }
        #endregion
    }
}
