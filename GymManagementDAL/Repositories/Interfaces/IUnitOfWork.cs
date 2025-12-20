using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        public ISessionReposistory sessionReposistory { get; }
        IBaseRepository<T> GetRepository<T>() where T : BaseEntity,new();
        int SaveChanges();

    }
}
