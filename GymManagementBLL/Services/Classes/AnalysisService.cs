using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.AnalysisViewModel;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class AnalysisService : IAnalysisService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalysisService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public AnalysisViewModel GetAnalysisData()
        {
            var Sessions = _unitOfWork.GetRepository<Session>().GetAll();
            return new AnalysisViewModel
            {
                totalMembers = _unitOfWork.GetRepository<Member>().GetAll().Count(),
                ActiveMembers = _unitOfWork.GetRepository<MemberShip>().GetAll(ms => ms.Status == "Active").Count(),
                TotalTrainers = _unitOfWork.GetRepository<Trainer>().GetAll().Count(),
                UpcomingSessions = Sessions.Count(s => s.startDate > DateTime.Now),
                OngoingSessions = Sessions.Count(s => s.startDate <= DateTime.Now && s.EndDate >= DateTime.Now),
                CompletedSessions = Sessions.Count(s => s.EndDate < DateTime.Now)
            };
        }
    }
}
