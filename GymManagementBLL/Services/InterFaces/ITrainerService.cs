using GymManagementBLL.ViewModels.TrainerViewModel;
using GymManagmentBLL.ViewModels.TrainerViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.InterFaces
{
    public interface ITrainerService
    {
        bool CreateTrainer(CreateTrainerViewModel model);
        bool UpdateTrainer(int id, TrainerToUpdateViewModel model);
        bool DeleteTrainer(int id);
        TrainerViewModel? GetTrainerDetails(int id);
        TrainerToUpdateViewModel? GetTrainerForUpdate(int id);

        IEnumerable<TrainerViewModel> GetAllTrainers();
        
    }
}
