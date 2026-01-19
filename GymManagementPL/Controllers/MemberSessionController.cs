using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.SessionScheduleViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class MemberSessionController : Controller
    {
        /*
         Session management for booking, cancellation and attendance tracking:
         - View upcoming sessions (for booking and cancellation)
         - View ongoing sessions (for attendance marking)
         - Add members to upcoming sessions
         - Cancel member booking for upcoming sessions
         - Mark member attendance for ongoing sessions
         */

        private readonly IMemberSessionService _memberSessionService;

        public MemberSessionController(IMemberSessionService memberSessionService)
        {
            _memberSessionService = memberSessionService;
        }

        public ActionResult Index()
        {
            var sessions = _memberSessionService.GetSessionsForManagement();
            return View(sessions);
        }

        public ActionResult GetMembersForUpcomingSession(int sessionId)
        {
            if (sessionId <= 0)
            {
                TempData["Error"] = "Invalid session";
                return RedirectToAction(nameof(Index));
            }

            var sessionMembers = _memberSessionService.GetMembersForUpcomingSession(sessionId);
            if (sessionMembers is null)
            {
                TempData["Error"] = "Session not found or is not upcoming";
                return RedirectToAction(nameof(Index));
            }

            return View(sessionMembers);
        }

        public ActionResult GetMembersForOngoingSession(int sessionId)
        {
            if (sessionId <= 0)
            {
                TempData["Error"] = "Invalid session";
                return RedirectToAction(nameof(Index));
            }

            var sessionMembers = _memberSessionService.GetMembersForOngoingSession(sessionId);
            if (sessionMembers is null)
            {
                TempData["Error"] = "Session not found or is not ongoing";
                return RedirectToAction(nameof(Index));
            }

            return View(sessionMembers);
        }

        public ActionResult AddMember(int sessionId)
        {
            if (sessionId <= 0)
            {
                TempData["Error"] = "Invalid session";
                return RedirectToAction(nameof(Index));
            }

            var model = _memberSessionService.GetAddMemberToSessionViewModel(sessionId);
            if (model is null)
            {
                TempData["Error"] = "Session not found, has already started, or is fully booked";
                return RedirectToAction(nameof(GetMembersForUpcomingSession), new { sessionId });
            }

            LoadAvailableMembers(sessionId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMember(AddMemberToSessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadAvailableMembers(model.SessionId);
                // Reload session details
                var sessionModel = _memberSessionService.GetAddMemberToSessionViewModel(model.SessionId);
                if (sessionModel != null)
                {
                    model.SessionName = sessionModel.SessionName;
                    model.TrainerName = sessionModel.TrainerName;
                    model.StartDate = sessionModel.StartDate;
                    model.AvailableSpots = sessionModel.AvailableSpots;
                }
                return View(model);
            }

            var (success, message) = _memberSessionService.BookMemberToSession(model.SessionId, model.MemberId);

            if (success)
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { sessionId = model.SessionId });
        }

        public ActionResult CancelBooking(int sessionId, int memberId)
        {
            if (sessionId <= 0 || memberId <= 0)
            {
                TempData["Error"] = "Invalid parameters";
                return RedirectToAction(nameof(Index));
            }

            var details = _memberSessionService.GetCancelBookingDetails(sessionId, memberId);
            if (details is null)
            {
                TempData["Error"] = "Booking not found or cannot be cancelled";
                return RedirectToAction(nameof(Index));
            }

            return View(details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelBookingConfirmed(int sessionId, int memberId)
        {
            var (success, message) = _memberSessionService.CancelBooking(sessionId, memberId);

            if (success)
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { sessionId });
        }

        public ActionResult MarkAttendance(int sessionId, int memberId)
        {
            if (sessionId <= 0 || memberId <= 0)
            {
                TempData["Error"] = "Invalid parameters";
                return RedirectToAction(nameof(Index));
            }

            var details = _memberSessionService.GetMarkAttendanceDetails(sessionId, memberId);
            if (details is null)
            {
                TempData["Error"] = "Cannot mark attendance for this booking";
                return RedirectToAction(nameof(Index));
            }

            return View(details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkAttendanceConfirmed(int sessionId, int memberId)
        {
            var (success, message) = _memberSessionService.MarkAttendance(sessionId, memberId);

            if (success)
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction(nameof(GetMembersForOngoingSession), new { sessionId });
        }

        #region Private Helper Methods
        private void LoadAvailableMembers(int sessionId)
        {
            var members = _memberSessionService.GetAvailableMembersForSession(sessionId);
            ViewBag.Members = new SelectList(members, "Id", "Name");
        }
        #endregion
    }
}
