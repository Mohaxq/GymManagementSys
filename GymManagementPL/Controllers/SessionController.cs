using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.SessionViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public ActionResult Index()
        {
            var sessions = _sessionService.GetAllSessions();
            return View(sessions);
        }
        public ActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid session ID.";
                return RedirectToAction("Index");
            }
            var session = _sessionService.GetSessionById(id);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session not found.";
            }
            return View(session);
        }
        public ActionResult Create()
        {
            LoadDropDownData();
            return View();
        }
        [HttpPost]
        public ActionResult Create(CreateSessionViewModel createSession)
        {
            if (!ModelState.IsValid)
            {
                LoadDropDownData();
                return View(createSession);
            }
            var result = _sessionService.CreateSession(createSession);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to create session.";
                LoadDropDownData();
                return View(createSession);
            }
            else
            {
                TempData["SuccessMessage"] = "Session created successfully.";
                return RedirectToAction("Index");
            }

        }
        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid session ID.";
                return RedirectToAction("Index");
            }
            
            // First check if session exists
            var sessionDetails = _sessionService.GetSessionById(id);
            if (sessionDetails == null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction("Index");
            }
            
            // Then try to get it for update (with business rule validation)
            var session = _sessionService.GetSessionForUpdate(id);
            if (session == null)
            {
                // Session exists but cannot be edited - provide specific reason
                
                if (sessionDetails.Capacity - sessionDetails.AvailableSpots > 0)
                {
                    TempData["ErrorMessage"] = "Cannot edit a session that has active bookings.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Session cannot be edited at this time.";
                }
                return RedirectToAction("Index");
            }
            LoadDropDownData();
            return View(session);
        }
        [HttpPost]
        public ActionResult Edit([FromRoute] int id, UpdateSessionViewModel updateSession)
        {
            if (!ModelState.IsValid)
            {
                LoadDropDownData();
                return View(updateSession);
            }
            var result = _sessionService.UpdateSession(id, updateSession);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to update session.";
                LoadDropDownData();
                return View(updateSession);
            }
            else
            {
                TempData["SuccessMessage"] = "Session updated successfully.";
            }
            return RedirectToAction("Index");

        }
        public ActionResult Delete(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid session ID.";
                return RedirectToAction("Index");
            }
            var session = _sessionService.GetSessionById(id);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction("Index");
            }
            ViewBag.SessionId = session.Id;
            return View();
        }
        [HttpPost]
        public ActionResult ConfirmDelete(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid session ID.";
                return RedirectToAction("Index");
            }
            var result = _sessionService.DeleteSession(id);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to delete session.";
            }
            else
            {
                TempData["SuccessMessage"] = "Session deleted successfully.";
            }
            return RedirectToAction("Index");
        }
        private void LoadDropDownData()
        {
            var Categories = _sessionService.GetAllCategoriesFromDropDown();
            ViewBag.Categories = new SelectList(Categories, "Id", "Name");
            var Trainers = _sessionService.GetAllTrainersFromDropDown();
            ViewBag.Trainers = new SelectList(Trainers, "Id", "Name");
        }

    }
}
