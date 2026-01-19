using GymManagementBLL.ViewModels.AccountViewModel;
using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.InterFaces
{
    public interface IAccountService
    {
        ApplicationUser ValidUser(LoginViewModel loginViewModel);
    }
}
