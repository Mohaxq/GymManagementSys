using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.SessionViewModel;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<SessionViewModel> GetAllSessions()
        {
            var sessions = _unitOfWork.sessionReposistory.GetAllSessionsWithTrainerAndCategory();
            if(sessions is null || !sessions.Any())
            {
                return [];
            }
            var sessionViewModels = sessions.Select(session => new SessionViewModel
            {
                Id = session.Id,
                EndDate = session.EndDate,
                StartDate = session.startDate,
                Capacity = session.capacity,
                Description = session.Description,
                TrainerName = session.SessionTrainer.Name,
                CategoryName = session.SessionCategory.CategoryName.ToString(),
                AvailableSpots = session.capacity - _unitOfWork.sessionReposistory.GetCountBookedSpots(session.Id)

            });
            return sessionViewModels;
        }
    }
}
