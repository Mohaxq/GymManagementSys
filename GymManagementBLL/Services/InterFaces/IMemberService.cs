using GymManagementBLL.ViewModels.MemberViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.InterFaces
{
    public interface IMemberService
    {
        IEnumerable<MemberViewModels> GetAllMembers();
        bool CreateMember(CreateMemberViewModel createMemberViewModel);
        MemberViewModels? GetMemberDetails(int Memid);
        HealthRecordViewModel? GetMemberHealthRecord(int Memid);
        MemberToUpdateViewModel? GetMemberForUpdate(int Memid);
        bool UpdateMember(int Memid, MemberToUpdateViewModel memberToUpdate);
        bool DeleteMember(int Memid);
    }
}
