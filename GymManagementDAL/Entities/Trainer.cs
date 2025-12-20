using GymManagementDAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Entities
{
    public class Trainer : GymUser
    {
        public Specialist Specialist { get; set; }
        #region Session-Trainer
        public ICollection<Session> TrainerSession { get; set; } = null!;
        
        #endregion
    }
}
