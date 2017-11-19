﻿using System.Data.Entity;
using System.Linq;
using Poced.Repository.Contexts;
using Poced.RepositoryInterfaces;

namespace Poced.Repository
{
    public class PocedRepository<T> : IRepository<T> where T : class
    {
        protected DbContext DbContext;
        protected DbSet<T> DbSet;

        public PocedRepository(string connectionString)
        {
            DbContext = new PocedContext(connectionString);
            DbSet = DbContext.Set<T>();
        }

        public IQueryable<T> Entities => DbSet;

        public T New()
        {
            return DbSet.Create<T>();
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