using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.ViewModels.AccountViewModel
{
    public class LoginViewModel
    {
        [Required (ErrorMessage = "Email is required")]
        public string Email { get; set; } = null!;
        [Required (ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
