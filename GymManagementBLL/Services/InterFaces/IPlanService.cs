using GymManagementBLL.ViewModels.PlanViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.InterFaces
{
    internal interface IPlanService
    {
        IEnumerable<PlanViewModel> GetAllPlans();
        PlanViewModel? GetPlanById(int id);
        UpdatePlanViewModel? GetPlanForUpdate(int id);
        bool UpdatePlan(int id, UpdatePlanViewModel updatePlanViewModel);
        bool TogglePlanStatus(int id);
    }
}
