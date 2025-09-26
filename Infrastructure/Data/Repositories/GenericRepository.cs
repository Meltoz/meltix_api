
using Application.Interfaces.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Exceptions;
using System.Reflection;

namespace Infrastructure.Data.Repositories
{
    public class GenericRepository<T>: IRepository<T> where T : BaseEntity, new()
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() 
            => await _dbSet.ToListAsync();

        public virtual async Task<PagedResult<T>> GetPaginateAsync(int skip, int take)
        {
            var query = _dbSet.OrderBy(f => f.Created);
            return await PaginateAsync<T>(query, skip, take);
        }

        public virtual async Task<T?> GetByIdAsync(Guid id) 
            => await _dbSet.SingleOrDefaultAsync(x => x.Id == id);

        public virtual T? GetById(Guid id) => _dbSet.SingleOrDefault(x => x.Id == id);


        public virtual T? Insert(T entity)
        {
            entity.Created = entity.Updated = DateTime.UtcNow;
            _dbSet.Add(entity);
            this.Save();
            return this.GetById(entity.Id);
        }

        public virtual async Task<T?> InsertAsync(T entity)
        {
            entity.Created = entity.Updated = DateTime.UtcNow;
            _dbSet.Add(entity);
            await this.SaveAsync();
            return await this.GetByIdAsync(entity.Id);
        }

        public virtual async Task InsertWithOutSave(T entity)
        {
            entity.Created = entity.Updated = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task<T> UpdateAsync(T entityToUpdate)
        {
            var entityUpdated = await this.GetByIdAsync(entityToUpdate.Id);
            if (entityUpdated == null)
            {
                throw new NotFoundException();
            }

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name is "Created" or "Updated")
                    continue;

                object updatedValue = property.GetValue(entityToUpdate);
                object originalValue = property.GetValue(entityUpdated);

                if ((updatedValue == null && originalValue != null)
                    || (updatedValue != null && !updatedValue.Equals(originalValue)))
                {
                    property.SetValue(entityUpdated, updatedValue);
                }
            }

            entityUpdated.Updated = DateTime.UtcNow;
            _context.Update(entityUpdated);
            await this.SaveAsync();
            return entityUpdated;
        }

        public virtual async Task UpdateWithOutSaveAsync(T entityToUpdate)
        {
            var entityUpdated = await this.GetByIdAsync(entityToUpdate.Id);

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "Created" || property.Name == "Updated")
                    continue;

                object updatedValue = property.GetValue(entityToUpdate);
                object originalValue = property.GetValue(entityUpdated);

                if ((updatedValue == null && originalValue != null)
                    || (updatedValue != null && !updatedValue.Equals(originalValue)))
                {
                    property.SetValue(entityUpdated, updatedValue);
                }
            }

            entityUpdated.Updated = DateTime.UtcNow;
            _context.Entry(entityUpdated).State = EntityState.Modified;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<T> UpsertAsync(T entity)
        {
            var existingEntity = await GetByIdAsync(entity.Id);

            if (existingEntity == null)
            {
                entity.Created = entity.Updated = DateTime.UtcNow;
                await _dbSet.AddAsync(entity);
            }
            else
            {
                PropertyInfo[] properties = typeof(T).GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    if (property.Name == "Created" || property.Name == "Updated")
                        continue;

                    object updatedValue = property.GetValue(entity);
                    object originalValue = property.GetValue(existingEntity);

                    if ((updatedValue == null && originalValue != null)
                        || (updatedValue != null && !updatedValue.Equals(originalValue)))
                    {
                        property.SetValue(existingEntity, updatedValue);
                    }
                }

                existingEntity.Updated = DateTime.UtcNow;
                _context.Entry(existingEntity).State = EntityState.Modified;
            }

            await SaveAsync();

            return await GetByIdAsync(entity.Id);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<T> UpsertWithOutSaveAsync(T entity)
        {
            var existingEntity = await GetByIdAsync(entity.Id);

            if (existingEntity == null)
            {
                entity.Created = entity.Updated = DateTime.UtcNow;
                await _dbSet.AddAsync(entity);
                return entity;
            }
            else
            {
                PropertyInfo[] properties = typeof(T).GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    if (property.Name == "Created" || property.Name == "Updated")
                        continue;

                    object updatedValue = property.GetValue(entity);
                    object originalValue = property.GetValue(existingEntity);

                    if ((updatedValue == null && originalValue != null)
                        || (updatedValue != null && !updatedValue.Equals(originalValue)))
                    {
                        property.SetValue(existingEntity, updatedValue);
                    }
                }

                existingEntity.Updated = DateTime.UtcNow;
                _context.Entry(existingEntity).State = EntityState.Modified;
                return existingEntity;
            }
        }


        public virtual void Delete(Guid id)
        {
            var entity = _dbSet.Single(e => e.Id == id);
            Delete(entity);
        }

        public virtual void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
            this.Save();
        }

        public virtual void Save()
        {
            _context.SaveChanges();
        }

        public virtual async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        protected virtual async Task<PagedResult<T>> PaginateAsync<T>(IQueryable<T> query, int skip, int take)
        {
            var total = await query.CountAsync();
            var data = await query.Skip(skip).Take(take).ToListAsync();

            return new PagedResult<T>
            {
                Data = data,
                TotalCount = total
            };
        }
    }
}
