using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public interface IRepository<T>
    {
        Task<T> GetById(object id);
        List<T> GetAll();
        Task<bool> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
    }
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DemoDbContext dbContext;
        public Repository(DemoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        private DbSet<T> _entities;
        private DbSet<T> Entites
        {
            get
            {
                if (_entities == null)
                {
                    _entities = dbContext.Set<T>();
                }
                return _entities;
            }
        }
        public async Task<bool> Delete(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                this.Entites.Remove(entity);
                await this.dbContext.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public List<T> GetAll()
        {
            return this.Entites.ToList();
        }

        public async Task<T> GetById(object id)
        {
            return await this.Entites.FindAsync(id);
        }

        public async Task<bool> Add(T entity)
        {
  
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                await this.Entites.AddAsync(entity);
                await this.dbContext.SaveChangesAsync();
                return true;
 
        }

        public async Task<bool> Update(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                this.Entites.Update(entity);
                await this.dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}