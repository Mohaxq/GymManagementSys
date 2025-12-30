using AutoMapper;
using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.TrainerViewModel;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using GymManagmentBLL.ViewModels.TrainerViewModel;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public bool CreateTrainer(CreateTrainerViewModel model)
        {
            var Repo = _unitOfWork.GetRepository<Trainer>();

            if (IsEmailExist(model.Email) || IsPhoneExist(model.Phone)) return false;
            var TrainerEntity = _mapper.Map<CreateTrainerViewModel, Trainer>(model);

            Repo.Add(TrainerEntity);

            return _unitOfWork.SaveChanges() > 0;


        }

        public bool DeleteTrainer(int id)
        {
            try
            {
                if (IsHasActiveSessions(id)) return false;
                var trainerRepo = _unitOfWork.GetRepository<Trainer>();
                var trainer = trainerRepo.GetById(id);
                if (trainer == null) return false;
                trainerRepo.Delete(trainer);
                return _unitOfWork.SaveChanges() > 0;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<TrainerViewModel> GetAllTrainers()
        {
            var trainers = _unitOfWork.GetRepository<Trainer>().GetAll();
            if(trainers is null || !trainers.Any())
            {
                return [];
            }
            var mappedTrainers = _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
            return mappedTrainers;

        }

        public TrainerViewModel? GetTrainerDetails(int id)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(id);
            if (trainer == null) return null;
            var mappedTrainer = _mapper.Map<TrainerViewModel>(trainer);
            return mappedTrainer;
        }

        public TrainerToUpdateViewModel? GetTrainerForUpdate(int id)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(id);
            if (trainer == null) return null;
            var mappedTrainer = _mapper.Map<TrainerToUpdateViewModel>(trainer);
            return mappedTrainer;
        }

        public bool UpdateTrainer(int id, TrainerToUpdateViewModel model)
        {
            var emailExist = _unitOfWork.GetRepository<Member>().GetAll(
                 m => m.Email == model.Email && m.Id != id);

            var PhoneExist = _unitOfWork.GetRepository<Member>().GetAll(
                m => m.Phone == model.Phone && m.Id != id);

            if (emailExist.Any() || PhoneExist.Any()) return false;

            var Repo = _unitOfWork.GetRepository<Trainer>();
            var TrainerToUpdate = Repo.GetById(id);

            if (TrainerToUpdate is null) return false;

            _mapper.Map(model, TrainerToUpdate);
            TrainerToUpdate.UpdatedAt = DateTime.Now;

            return _unitOfWork.SaveChanges() > 0;
        }

        #region Helper
        private bool IsEmailExist(string email)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetAll(t => t.Email == email).FirstOrDefault();
            return trainer != null;
        }
        private bool IsPhoneExist(string phone)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetAll(t => t.Phone == phone).FirstOrDefault();
            return trainer != null;
        }
        private bool IsHasActiveSessions(int trainerId)
        {
            var sessions = _unitOfWork.GetRepository<Session>().GetAll(s => s.TrainerId == trainerId && s.startDate >= DateTime.Now).ToList();
            return sessions.Any();
        }
        #endregion
    }
}
