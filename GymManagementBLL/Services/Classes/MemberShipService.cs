using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.MemberShipViewModel;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class MemberShipService : IMemberShipService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberShipService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<MemberShipViewModel> GetAllMemberShips()
        {
            var memberShips = _unitOfWork.GetRepository<MemberShip>().GetAll();
            if (memberShips is null || !memberShips.Any())
            {
                return [];
            }

            var result = new List<MemberShipViewModel>();
            foreach (var ms in memberShips)
            {
                var member = _unitOfWork.GetRepository<Member>().GetById(ms.MemberId);
                var plan = _unitOfWork.GetRepository<Plan>().GetById(ms.PlanId);

                result.Add(new MemberShipViewModel
                {
                    MemberId = ms.MemberId,
                    PlanId = ms.PlanId,
                    MemberName = member?.Name ?? "Unknown",
                    PlanName = plan?.Name ?? "Unknown",
                    PlanPrice = plan?.Price ?? 0,
                    StartDate = ms.CreatedAt,
                    EndDate = ms.EndDate,
                    Status = ms.Status
                });
            }

            return result.OrderByDescending(m => m.StartDate);
        }

        public MemberShipViewModel? GetMemberShipById(int memberId, int planId)
        {
            var memberShip = _unitOfWork.GetRepository<MemberShip>()
                .GetAll(ms => ms.MemberId == memberId && ms.PlanId == planId)
                .FirstOrDefault();

            if (memberShip is null)
            {
                return null;
            }

            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            var plan = _unitOfWork.GetRepository<Plan>().GetById(planId);

            return new MemberShipViewModel
            {
                MemberId = memberShip.MemberId,
                PlanId = memberShip.PlanId,
                MemberName = member?.Name ?? "Unknown",
                PlanName = plan?.Name ?? "Unknown",
                PlanPrice = plan?.Price ?? 0,
                StartDate = memberShip.CreatedAt,
                EndDate = memberShip.EndDate,
                Status = memberShip.Status
            };
        }

        public (bool Success, string Message) CreateMemberShip(CreateMemberShipViewModel model)
        {
            try
            {
                // Check if member exists
                var member = _unitOfWork.GetRepository<Member>().GetById(model.MemberId);
                if (member is null)
                {
                    return (false, "Member not found");
                }

                // Check if plan exists and is active
                var plan = _unitOfWork.GetRepository<Plan>().GetById(model.PlanId);
                if (plan is null)
                {
                    return (false, "Plan not found");
                }

                if (!plan.IsActive)
                {
                    return (false, "Selected plan is not active");
                }

                // Check if member already has an active membership
                var existingMembership = _unitOfWork.GetRepository<MemberShip>()
                    .GetAll(ms => ms.MemberId == model.MemberId && ms.EndDate > DateTime.Now)
                    .FirstOrDefault();

                if (existingMembership is not null)
                {
                    return (false, "Member already has an active membership");
                }

                // Check if this exact membership already exists
                var duplicateMembership = _unitOfWork.GetRepository<MemberShip>()
                    .GetAll(ms => ms.MemberId == model.MemberId && ms.PlanId == model.PlanId)
                    .FirstOrDefault();

                if (duplicateMembership is not null)
                {
                    return (false, "This membership already exists");
                }

                var memberShip = new MemberShip
                {
                    MemberId = model.MemberId,
                    PlanId = model.PlanId,
                    CreatedAt = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(plan.DurationInDays)
                };

                _unitOfWork.GetRepository<MemberShip>().Add(memberShip);
                var result = _unitOfWork.SaveChanges() > 0;

                return result
                    ? (true, "Membership created successfully")
                    : (false, "Failed to create membership");
            }
            catch (Exception)
            {
                return (false, "An error occurred while creating membership");
            }
        }

        public (bool Success, string Message) CancelMemberShip(int memberId, int planId)
        {
            try
            {
                var memberShip = _unitOfWork.GetRepository<MemberShip>()
                    .GetAll(ms => ms.MemberId == memberId && ms.PlanId == planId)
                    .FirstOrDefault();

                if (memberShip is null)
                {
                    return (false, "Membership not found");
                }

                _unitOfWork.GetRepository<MemberShip>().Delete(memberShip);
                var result = _unitOfWork.SaveChanges() > 0;

                return result
                    ? (true, "Membership cancelled successfully")
                    : (false, "Failed to cancel membership");
            }
            catch (Exception)
            {
                return (false, "An error occurred while cancelling membership");
            }
        }

        public IEnumerable<MemberSelectListViewModel> GetMembersForDropDown()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll();
            return members.Select(m => new MemberSelectListViewModel
            {
                Id = m.Id,
                Name = m.Name
            });
        }

        public IEnumerable<PlanSelectListViewModel> GetActivePlansForDropDown()
        {
            var plans = _unitOfWork.GetRepository<Plan>().GetAll(p => p.IsActive);
            return plans.Select(p => new PlanSelectListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                DurationInDays = p.DurationInDays
            });
        }
    }
}
