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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories = new();
        private readonly GymDbContext _dbContext;

        public UnitOfWork(GymDbContext dbContext, ISessionReposistory sessionReposistory)
        {
            _dbContext = dbContext;
            this.sessionReposistory = sessionReposistory;
        }

        public ISessionReposistory sessionReposistory { get; }

        public IBaseRepository<T> GetRepository<T>() where T : BaseEntity, new()
        {
            var EntityType = typeof(T);
            if(_repositories.TryGetValue(EntityType, out var repository))
            {
                return (IBaseRepository<T>)repository;
            }
            var newRepository = new BaseRepository<T>(_dbContext);
            _repositories[EntityType] = newRepository;
            return newRepository;
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
