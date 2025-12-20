using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.PlanViewModel;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    internal class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<PlanViewModel> GetAllPlans()
        {
            var plans = _unitOfWork.GetRepository<Plan>().GetAll();
            if (plans is null || !plans.Any())
            {
                return [];
            }
            var planViewModels = plans.Select(plan => new PlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                DurationInDays = plan.DurationInDays,
                IsActive = plan.IsActive
            });
            return planViewModels;
        }

        public PlanViewModel? GetPlanById(int id)
        {
            var plan = _unitOfWork.GetRepository<Plan>().GetById(id);
            if (plan is null)
            {
                return null;
            }
            var planViewModel = new PlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                DurationInDays = plan.DurationInDays,
                IsActive = plan.IsActive
            };
            return planViewModel;
        }

        public UpdatePlanViewModel? GetPlanForUpdate(int id)
        {
           var plan = _unitOfWork.GetRepository<Plan>().GetById(id);
            if (plan is null || HasActiveMembers(id))
            {
                return null;
            }
            var updatePlanViewModel = new UpdatePlanViewModel
            {
                PlanName = plan.Name,
                Description = plan.Description,
                DurationDays = plan.DurationInDays,
                Price = plan.Price
            };
            return updatePlanViewModel;
        }

        public bool TogglePlanStatus(int id)
        {
            var repo = _unitOfWork.GetRepository<Plan>();
            var plan = repo.GetById(id);
            if (plan is null || HasActiveMembers(id))
            {
                return false;
            }
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            try
            {
                repo.Update(plan);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdatePlan(int id, UpdatePlanViewModel updatePlanViewModel)
        {
            var plan = _unitOfWork.GetRepository<Plan>().GetById(id);
            if (plan is null || HasActiveMembers(id))
            {
                return false;
            }
            (plan.Name, plan.Description, plan.DurationInDays, plan.Price) = 
                (updatePlanViewModel.PlanName, updatePlanViewModel.Description, updatePlanViewModel.DurationDays, updatePlanViewModel.Price);
            _unitOfWork.GetRepository<Plan>().Update(plan);
            return _unitOfWork.SaveChanges() > 0;
        }

        #region Helper
        private bool HasActiveMembers(int planId)
        {
            var members = _unitOfWork.GetRepository<MemberShip>().GetAll(m => m.Id == planId && m.Status == "Active");
            return members.Any();
        }
        #endregion
    }
}
