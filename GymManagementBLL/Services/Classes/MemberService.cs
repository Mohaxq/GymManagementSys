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
    internal class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CreateMember(CreateMemberViewModel createMemberViewModel)
        {
            try
            {
                if (IsEmailExists(createMemberViewModel.Email) || IsPhoneExists(createMemberViewModel.Phone))
                {
                    return false;
                }

                var member = new Member
                {
                    Name = createMemberViewModel.Name,
                    Email = createMemberViewModel.Email,
                    Phone = createMemberViewModel.Phone,
                    DateOfBirth = createMemberViewModel.DateOfBirth,
                    Gender = createMemberViewModel.Gender,
                    Address = new Address
                    {
                        BuildingNumber = createMemberViewModel.BuildingNumber,
                        Street = createMemberViewModel.Street,
                        City = createMemberViewModel.City
                    },
                    HealthRecord = new HealthRecord
                    {
                        Weight = createMemberViewModel.HealthRecord.Weight,
                        Height = createMemberViewModel.HealthRecord.Height,
                        BloodType = createMemberViewModel.HealthRecord.BloodType,
                        Note = createMemberViewModel.HealthRecord.Note
                    }
                };
                _unitOfWork.GetRepository<Member>().Add(member);
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
            #region way1
            /*var memberViewModel = new List<MemberViewModels>();
                foreach (var member in members)
                {
                    var MemberViewModel = new MemberViewModels
                    {
                        Id = member.Id,
                        Photo = member.Photo,
                        name = member.Name,
                        Email = member.Email,
                        Phone = member.Phone,
                        Gender = member.Gender.ToString()

                    };
                    memberViewModel.Add(MemberViewModel);

                }*/
            #endregion
            var memberViewModel = members.Select(member => new MemberViewModels
            {
                Id = member.Id,
                Photo = member.Photo,
                name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender.ToString()
            });
            return memberViewModel;
        }

        public MemberViewModels? GetMemberDetails(int Memid)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(Memid);
            if (member is null)
            {
                return null;
            }
            var memberViewModel = new MemberViewModels
            {
                name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Gender = member.Gender.ToString(),
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Address = $"{member.Address.BuildingNumber}, {member.Address.Street}, {member.Address.City}",
                Photo = member.Photo
            };
            var Active = _unitOfWork.GetRepository<MemberShip>().GetAll(x => x.MemberId == Memid && x.Status == "Active").FirstOrDefault();
            if (Active is not null)
            {
                memberViewModel.MembershipStart = Active.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEnd = Active.EndDate.ToShortDateString();
                var plan = _unitOfWork.GetRepository<Plan>().GetById(Active.PlanId);
                memberViewModel.PlanName = plan?.Name;
            }
            return memberViewModel;
        }

        public MemberToUpdateViewModel? GetMemberForUpdate(int Memid)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(Memid);
            if (member is null)
            {
                return null;
            }
            return new MemberToUpdateViewModel
            {
                Name = member.Name,
                Photo = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
                Street = member.Address.Street,
                City = member.Address.City
            };
        }

        public HealthRecordViewModel? GetMemberHealthRecord(int Memid)
        {
            var memberHealthRecord = _unitOfWork.GetRepository<HealthRecord>().GetById(Memid);
            if (memberHealthRecord is null)
            {
                return null;
            }
            return new HealthRecordViewModel
            {
                Height = memberHealthRecord.Height,
                Weight = memberHealthRecord.Weight,
                BloodType = memberHealthRecord.BloodType,
                Note = memberHealthRecord.Note
            };
        }

        public bool UpdateMember(int Memid, MemberToUpdateViewModel memberToUpdate)
        {
            try
            {
                if(IsEmailExists(memberToUpdate.Email) || IsPhoneExists(memberToUpdate.Phone))
                {
                   return false;
                }
                
                var member = _unitOfWork.GetRepository<Member>().GetById(Memid);
                if (member is null)
                {
                    return false;
                }
                member.Email = memberToUpdate.Email;
                member.Phone = memberToUpdate.Phone;
                member.Address.BuildingNumber = memberToUpdate.BuildingNumber;
                member.Address.Street = memberToUpdate.Street;
                member.Address.City = memberToUpdate.City;
                member.UpdatedAt = DateTime.Now;
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
