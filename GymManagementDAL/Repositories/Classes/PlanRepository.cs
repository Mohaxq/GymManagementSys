using GymManagementDAL.Data.Context;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Classes
{
    public class PlanRepository : IPlanRepository
    {
        private readonly GymDbContext _context;
        public PlanRepository(GymDbContext dbContext)
        {
            _context = dbContext;
        }
        public IEnumerable<Plan> GetAllPlans() => _context.Plans.ToList();

        public Plan? GetPlanById(int planId)=>_context.Plans.Find(planId);

        public int UpdatePlan(Plan plan)
        {
            _context.Plans.Update(plan);
            return _context.SaveChanges();
        }
    }
}
