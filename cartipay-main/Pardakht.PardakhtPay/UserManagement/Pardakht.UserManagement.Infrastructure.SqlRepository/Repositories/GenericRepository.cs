using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntity 
    {
        protected ParadakhtUserManagementDbContext DbContext;

        public GenericRepository(ParadakhtUserManagementDbContext dbContext)
        {
            DbContext = dbContext;
        }

        // if we need this then we should use iqueryable!
        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await DbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public virtual async Task<TEntity> GetById(int id)
        {
            return await DbContext.Set<TEntity>().FirstOrDefaultAsync(p => p.Id == id);
        }

        public virtual async Task<TEntity> GetByIdAsNoTracking(int id)
        {
            return await DbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public virtual async Task<TEntity> Create(TEntity entity)
        {
            var result = await DbContext.Set<TEntity>().AddAsync(entity);

            return result.Entity;
        }

        public virtual Task<TEntity> Update(TEntity entity)
        {
            return Task.Run(() => DbContext.Set<TEntity>().Update(entity).Entity);
        }

        public async Task<int> Delete(int id)
        {
            var entity = await GetById(id);
            DbContext.Set<TEntity>().Remove(entity);

            return id;
        }

        public async Task CommitChanges()
        {
            await DbContext.SaveChangesAsync();
        }

        public async Task<int> ExecuteSqlCommand(string query)
        {
            return await DbContext.Database.ExecuteSqlCommandAsync(query);
        }


       
    }
}
