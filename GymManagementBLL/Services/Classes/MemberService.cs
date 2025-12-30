using AutoMapper;
using GymManagementBLL.Services.InterFaces;
using GymManagementBLL.ViewModels.MemberViewModel;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public bool CreateMember(CreateMemberViewModel createMemberViewModel)
        {
            try
            {
                if (IsEmailExists(createMemberViewModel.Email) || IsPhoneExists(createMemberViewModel.Phone))
                {
                    return false;
                }

                var memberMapped = _mapper.Map<Member>(createMemberViewModel);
                _unitOfWork.GetRepository<Member>().Add(memberMapped);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }


        public IEnumerable<MemberViewModels> GetAllMembers()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll();
            if (members is null || !members.Any())
            {
                return [];
            }
           var member = _unitOfWork.GetRepository<Member>().GetAll();
            if(member is null || !member.Any())
            {
                return [];
            }
            var memberMapped = _mapper.Map<IEnumerable<MemberViewModels>>(members);
            return memberMapped;
        }

        public MemberViewModels? GetMemberDetails(int Memid)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(Memid);
            if (member is null)
            {
                return null;
            }
            var ViewModel = _mapper.Map<MemberViewModels>(member);
            var ActiveMembership = _unitOfWork.GetRepository<MemberShip>().GetAll(ms => ms.MemberId == Memid && ms.EndDate > DateTime.Now).FirstOrDefault();
            if(ActiveMembership is not null)
            {
                ViewModel.MembershipStart = ActiveMembership.CreatedAt.ToShortDateString();
                ViewModel.MembershipEnd = ActiveMembership.EndDate.ToShortDateString();
                var plan = _unitOfWork.GetRepository<Plan>().GetById(ActiveMembership.PlanId);
                ViewModel.PlanName = plan?.Name;
            }
            return ViewModel;
        }

        public MemberToUpdateViewModel? GetMemberForUpdate(int Memid)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(Memid);
            if (member is null)
            {
                return null;
            }
            return _mapper.Map<MemberToUpdateViewModel>(member);
        }

        public HealthRecordViewModel? GetMemberHealthRecord(int Memid)
        {
            var memberHealthRecord = _unitOfWork.GetRepository<HealthRecord>().GetById(Memid);
            if (memberHealthRecord is null)
            {
                return null;
            }
            return _mapper.Map<HealthRecordViewModel>(memberHealthRecord);
        }

        public bool UpdateMember(int Memid, MemberToUpdateViewModel memberToUpdate)
        {
            try
            {
                var PhoneExist = _unitOfWork.GetRepository<Member>().GetAll(
                    m => m.Phone == memberToUpdate.Phone && m.Id != Memid);
                var EmailExist = _unitOfWork.GetRepository<Member>().GetAll(
                    m => m.Email == memberToUpdate.Email && m.Id != Memid);
                if (PhoneExist.Any() || EmailExist.Any())
                {
                    return  false;
                }


                    var member = _unitOfWork.GetRepository<Member>().GetById(Memid);
                if (member is null)
                {
                    return false;
                }
                _mapper.Map(memberToUpdate, member);
                _unitOfWork.GetRepository<Member>().Update(member);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool DeleteMember(int Memid)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(Memid);
            if (member is null)
            {
                return false;
            }
            var HasActiveMembership = _unitOfWork.GetRepository<MemberSession>().GetAll(ms => ms.MemberId == Memid && ms.Session.startDate > DateTime.Now).Any();
            if (HasActiveMembership)
            {
                return false;
            }
            var MemberShip = _unitOfWork.GetRepository<MemberShip>().GetAll(ms => ms.MemberId == Memid);
            try
            {
                if (MemberShip.Any())
                {
                    foreach (var ms in MemberShip)
                    {
                        _unitOfWork.GetRepository<MemberShip>().Delete(ms);
                    }
                }
                _unitOfWork.GetRepository<Member>().Delete(member);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch 
            {
                return false;
            }

        }
        #region Helper Method
        private bool IsEmailExists(string email)
        {
            return _unitOfWork.GetRepository<Member>().GetAll(m => m.Email == email).Any();
        }
        private bool IsPhoneExists(string phone)
        {
            return _unitOfWork.GetRepository<Member>().GetAll(m => m.Phone == phone).Any();
        }
        #endregion
    }
}
