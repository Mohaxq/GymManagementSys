using GymManagementBLL.ViewModels.MemberShipViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.InterFaces
{
    public interface IMemberShipService
    {
        IEnumerable<MemberShipViewModel> GetAllMemberShips();
        MemberShipViewModel? GetMemberShipById(int memberId, int planId);
        (bool Success, string Message) CreateMemberShip(CreateMemberShipViewModel model);
        (bool Success, string Message) CancelMemberShip(int memberId, int planId);
        IEnumerable<MemberSelectListViewModel> GetMembersForDropDown();
        IEnumerable<PlanSelectListViewModel> GetActivePlansForDropDown();
    }
}
