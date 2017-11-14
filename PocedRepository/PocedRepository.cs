using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using PocedRepository.Contexts;
using Repository;

namespace PocedRepository
{
    public class PocedRepository<T> : IRepository<T> where T : class
    {
        protected DbContext ObjectContext;
        protected DbSet<T> ObjectSet;

        public PocedRepository(string connectionString)
        {
            ObjectContext = new PocedContext(connectionString);
            ObjectSet = ObjectContext.Set<T>();
        }

        public IQueryable<T> Entities => ObjectSet;

        public T New()
        {
            return ObjectSet.Create<T>();
        }

        public void Add(T entity)
        {
            ObjectSet.Attach(entity);
        }

        public void Create(T entity)
        {
            ObjectSet.Add(entity);
        }

        public void Delete(T entity)
        {
            ObjectSet.Remove(entity);
        }

        public void Save()
        {
            ObjectContext.SaveChanges();
        }

        public void Dispose()
        {
            ObjectContext.Dispose();
        }
    }
}