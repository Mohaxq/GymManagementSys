using GymManagementBLL.ViewModels.SessionViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.InterFaces
{
    public interface ISessionService
    {
        IEnumerable<SessionViewModel> GetAllSessions();
        SessionViewModel GetSessionById(int id);
        bool CreateSession(CreateSessionViewModel createSessionViewModel);
        UpdateSessionViewModel? GetSessionForUpdate(int id);
        bool UpdateSession(int id, UpdateSessionViewModel updateSessionViewModel);
        bool DeleteSession(int id);
        IEnumerable<TrainerSelectViewModel> GetAllTrainersFromDropDown();
        IEnumerable<CategorySelectViewModel> GetAllCategoriesFromDropDown();
    }
}
