using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Entities
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }
        public HealthRecord HealthRecord { get; set; } = null!;

        #region MemberShip-Member
        public ICollection<MemberShip> memberShips { get; set; } = null!;
        #endregion

        #region Member-MemberSession
        public ICollection<MemberSession> MemberSession { get; set; } = null!;
        #endregion
    }
}
