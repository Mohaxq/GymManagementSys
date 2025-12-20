using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.ViewModels.MemberViewModel
{
    internal class MemberViewModels
    {
        public int Id { get; set; }
        public string? Photo { get; set; }
        public string name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? PlanName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? MembershipStart { get; set; }
        public string? MembershipEnd { get; set; }
        public string? Address { get; set; } 
    }
}
