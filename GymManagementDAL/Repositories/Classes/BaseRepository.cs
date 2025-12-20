using GymManagementDAL.Data.Context;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Classes
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext _context;
        public BaseRepository(GymDbContext dbContext)
        {
           _context = dbContext;
        }
        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
           
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            
        }

        public IEnumerable<TEntity> GetAll(Func<TEntity, bool>? condition = null)
        {
            if(condition is null)
            {
                return _context.Set<TEntity>().AsNoTracking().ToList();
            }
                return _context.Set<TEntity>().AsNoTracking().Where(condition).ToList();
        }

        public TEntity? GetById(int id) => _context.Set<TEntity>().Find(id);




        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            
        }
    }
}
