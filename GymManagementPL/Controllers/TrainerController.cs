using GymManagementBLL.Services.Classes;
using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.TrainerViewModel;
using GymManagmentBLL.ViewModels.TrainerViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainerController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }
        public IActionResult Index()
        {
            var trainers = _trainerService.GetAllTrainers();
            return View(trainers);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateTrainer(CreateTrainerViewModel createTrainerView)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataMissed", "Check data and missing field");
                return View("Create", createTrainerView);
            }
            var isCreated = _trainerService.CreateTrainer(createTrainerView);
            if (!isCreated)
            {
                TempData["Error"] = "Something went wrong";
                return View("Create", createTrainerView);
            }
            TempData["Success"] = "Trainer Created Successfully";
            return RedirectToAction("Index");

        }

        public ActionResult TrainerDetails(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid Trainer Id";
                return RedirectToAction("Index");
            }
            var trainer = _trainerService.GetTrainerDetails(id);
            if (trainer is null)
            {
                TempData["Error"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        public ActionResult TrainerEdit(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid Trainer Id";
                return RedirectToAction("Index");
            }
            var trainer = _trainerService.GetTrainerForUpdate(id);
            if (trainer is null)
            {
                TempData["Error"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        [HttpPost]  
        public ActionResult UpdateTrainer([FromRoute]int id, TrainerToUpdateViewModel updateTrainerView)
        {
            if (!ModelState.IsValid)
            {
                return View("TrainerEdit", updateTrainerView);
            }
            bool isUpdated = _trainerService.UpdateTrainer(id, updateTrainerView);
            if (!isUpdated)
            {
                TempData["Error"] = "Something went wrong";
                return View("TrainerEdit", updateTrainerView);
            }
            TempData["Success"] = "Trainer Updated Successfully";
            return RedirectToAction("Index");
        }
        public ActionResult DeleteTrainer(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid Trainer Id";
                return RedirectToAction("Index");
            }
            var trainer = _trainerService.GetTrainerDetails(id);

            if (trainer is null)
            {
                TempData["Error"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TrainerId = id;
            return View();
        }
        [HttpPost]
        public ActionResult ConfirmDeleteTrainer(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Invalid Trainer Id";
                return RedirectToAction("Index");
            }
            bool isDeleted = _trainerService.DeleteTrainer(id);
            if (!isDeleted)
            {
                TempData["Error"] = "Something went wrong or Trainer has active sessions";
                return RedirectToAction("DeleteTrainer", new { id = id });
            }
            TempData["Success"] = "Trainer Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
