using AutoMapper;
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
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IEnumerable<SessionViewModel> GetAllSessions()
        {
            var sessions = _unitOfWork.sessionReposistory.GetAllSessionsWithTrainerAndCategory();
            if (sessions is null || !sessions.Any())
            {
                return [];
            }
            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
            foreach (var sessionViewModel in MappedSessions)
            {
                var bookedSpots = _unitOfWork.sessionReposistory.GetCountBookedSpots(sessionViewModel.Id);
                sessionViewModel.AvailableSpots = sessionViewModel.Capacity - bookedSpots;
            }
            return MappedSessions;
        }

        public SessionViewModel GetSessionById(int id)
        {
            var session = _unitOfWork.sessionReposistory.GetSessionWithDetailsById(id);
            if (session is null)
            {
                return null;
            }
            var MappedSession = _mapper.Map<SessionViewModel>(session);
            MappedSession.AvailableSpots = MappedSession.Capacity - _unitOfWork.sessionReposistory.GetCountBookedSpots(MappedSession.Id);
            return MappedSession;
        }

        public bool CreateSession(CreateSessionViewModel createSessionViewModel)
        {
            if (IsTrainerExits(createSessionViewModel.TrainerId) &&
               IsCategoryExits(createSessionViewModel.CategoryId) &&
               IsTimeValid(createSessionViewModel.StartDate, createSessionViewModel.EndDate))
            {
                var session = _mapper.Map<Session>(createSessionViewModel);
                _unitOfWork.GetRepository<Session>().Add(session);
                return _unitOfWork.SaveChanges() > 0;
            }
            if (createSessionViewModel.Capacity < 0 || createSessionViewModel.Capacity > 25) return false;
            return false;
        }
        #region Helper
        private bool IsTrainerExits(int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);
            return trainer is not null;
        }
        private bool IsCategoryExits(int categoryId)
        {
            var category = _unitOfWork.GetRepository<Category>().GetById(categoryId);
            return category is not null;
        }
        private bool IsTimeValid(DateTime startDate, DateTime endDate)
        {
            return startDate < endDate;
        }
        private bool IsSessionAvilableToUpdata(Session session)
        {
            if (session is null) return false;

            if (session.EndDate < DateTime.Now)
            {
                return false;
            }

            if (session.startDate <= DateTime.Now)
            {
                return false;
            }
            var HasActiveBooking = _unitOfWork.sessionReposistory.GetCountBookedSpots(session.Id) > 0;

            if (HasActiveBooking) return false;

            return true;
        }

        private bool IsSessionAvilableToDelete(Session session)
        {
            if (session is null) return false;
            if (session.startDate <= DateTime.Now && session.EndDate>DateTime.Now)
            {
                return false;
            }
            if(session.startDate>DateTime.Now)
            {
                return false;
            }
            var HasActiveBooking = _unitOfWork.sessionReposistory.GetCountBookedSpots(session.Id) > 0;
            if (HasActiveBooking) return false;
            return true;
        }
        #endregion
        public UpdateSessionViewModel? GetSessionForUpdate(int id)
        {
            var session = _unitOfWork.sessionReposistory.GetById(id);
            if (session is null || !IsSessionAvilableToUpdata(session))
            {
                return null;
            }
            var MappedSession = _mapper.Map<UpdateSessionViewModel>(session);
            return MappedSession;
        }

        public bool UpdateSession(int id, UpdateSessionViewModel updateSessionViewModel)
        {
            try
            {
                var session = _unitOfWork.sessionReposistory.GetById(id);
                if (session is null || !IsSessionAvilableToUpdata(session!))
                {
                    return false;
                }
                if (!IsTrainerExits(updateSessionViewModel.TrainerId))
                {
                    return false;
                }
                if (!IsTimeValid(updateSessionViewModel.StartDate, updateSessionViewModel.EndDate))
                {
                    return false;
                }
                _mapper.Map(updateSessionViewModel, session);
                session.UpdatedAt = DateTime.Now;
                _unitOfWork.GetRepository<Session>().Update(session);
                return _unitOfWork.SaveChanges() > 0;

            }
            catch (Exception)
            {
                return false;
            }

        }
        public bool DeleteSession(int id)
        {
            try
            {
                var session = _unitOfWork.sessionReposistory.GetById(id);
                if (session is null || !IsSessionAvilableToDelete(session!))
                {
                    return false;
                }
                _unitOfWork.GetRepository<Session>().Delete(session!);
                return _unitOfWork.SaveChanges() > 0;

            } 
            catch (Exception)
            {                 
                return false; 
            }
        }

        public IEnumerable<TrainerSelectViewModel> GetAllTrainersFromDropDown()
        {
           var Trainers= _unitOfWork.GetRepository<Trainer>().GetAll();
            
            var MappedTrainers = _mapper.Map<IEnumerable<TrainerSelectViewModel>>(Trainers);
            return MappedTrainers;
        }

        public IEnumerable<CategorySelectViewModel> GetAllCategoriesFromDropDown()
        {
            var Categories = _unitOfWork.GetRepository<Category>().GetAll();
            var MappedCategories = _mapper.Map<IEnumerable<CategorySelectViewModel>>(Categories);
            return MappedCategories;
        }
    }
}
