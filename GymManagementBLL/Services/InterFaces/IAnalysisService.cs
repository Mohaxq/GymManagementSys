using GymManagementBLL.ViewModels.AnalysisViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.InterFaces
{
    public interface IAnalysisService
    {
        AnalysisViewModel GetAnalysisData();
    }
}
