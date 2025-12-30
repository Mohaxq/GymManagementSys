using Microsoft.AspNetCore.Mvc;
using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.MemberViewModel;

namespace GymManagementPL.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }
        public ActionResult Index()
        {
            var members = _memberService.GetAllMembers();
            return View(members);
        }
        public ActionResult MemberDetails(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid Member Id";
                return RedirectToAction("Index");
            }
            var member = _memberService.GetMemberDetails(id);
            if(member is null)
            {
                TempData["Error"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
                return View(member);
        }
        public ActionResult HealthRecord(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid Member Id";
                return RedirectToAction("Index");
            }
            var healthRecord = _memberService.GetMemberHealthRecord(id);
            if(healthRecord is null)
            {
                TempData["Error"] = "Health Record Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateMember(CreateMemberViewModel createMember)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataMissed", "Check data and missing field");
                return View(nameof(Create),createMember);
            }
            bool result = _memberService.CreateMember(createMember);
            if (result)
            {
                TempData["Success"] = "Member Created Successfully";
            }
            else 
            {                 
                TempData["Error"] = "Failed to Create Member";
            }
            return RedirectToAction(nameof(Index) );
        }
        public ActionResult MemberEdit(int id)
        {
            if(id <= 0)
            {
                TempData["Error"] = "Invalid Member Id";
                return RedirectToAction("Index");
            }
            var member = _memberService.GetMemberForUpdate(id);
            if(member is null)
            {
                TempData["Error"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        [HttpPost]
        public ActionResult MemberEdit([FromRoute]int id,MemberToUpdateViewModel memberToUpdate)
        {
            if (!ModelState.IsValid)
            {
                return View(memberToUpdate);
            }
            bool result = _memberService.UpdateMember(id, memberToUpdate);
            if (result)
            {
                TempData["Success"] = "Member Updated Successfully";
            }
            else
            {
                TempData["Error"] = "Failed to Update Member";
            }
            return RedirectToAction(nameof(Index));

        }

        public ActionResult MemberDelete(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid Member Id";
                return RedirectToAction("Index");
            }
            var member = _memberService.GetMemberDetails(id);
            
            if(member is null)
            {
                TempData["Error"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MemberId = id;
            return View();
        }
        [HttpPost]
        public ActionResult DeleteConfig([FromForm]int id)
        {
            var resultMember = _memberService.DeleteMember(id);
            if(resultMember)
            {
                TempData["Success"] = "Member Deleted Successfully";
            }
            else
            {
                TempData["Error"] = "Failed to Delete Member";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
