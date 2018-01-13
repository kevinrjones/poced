using System.Linq;
using Microsoft.EntityFrameworkCore;
using Poced.Repository.Contexts;
using Poced.RepositoryInterfaces;

namespace Poced.Repository
{
    public class PocedRepository<T> : IRepository<T> where T : class, new()
    {
        protected PocedDbContext DbContext;
        protected DbSet<T> DbSet;

        public PocedRepository(PocedDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }

        public IQueryable<T> Entities => DbSet;

        public T New()
        {
            return new T();
        }

        public void Add(T entity)
        {
            DbSet.Attach(entity);
        }

        public void Create(T entity)
        {
            DbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public void Save()
        {
            DbContext.SaveChanges();
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}