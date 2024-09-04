using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using FBAdsManager.Common.Database.DbContexts;

namespace FBAdsManager.Common.Database.Repository
{
    public class Repository<TEntity> : IRepository<TEntity>, IDisposable, IAsyncDisposable where TEntity : class
    {
        protected readonly DbAdsmanagerContext Context;
        private readonly Type _type;

        public Repository(DbAdsmanagerContext context)
        {
            Context = context;
            DbSet = Context.Set<TEntity>();
            _type = typeof(TEntity);
        }

        protected DbSet<TEntity> DbSet { get; }

        public void Update(TEntity entity, Expression<Func<TEntity, bool>> criteria)
        {
            TEntity? original = FindOne(criteria);
            Context.Entry(original).CurrentValues.SetValues(entity);
        }

        public void Update(TEntity entity)
        {
            Context.Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            Context.UpdateRange(entities);
        }

        //public void BulkUpdateRangeAsync(IEnumerable<TEntity> entities)
        //{
        //    Context.BulkUpdateAsync(entities);
        //}

        public void Add(TEntity entity)
        {
            if (entity != null) DbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        //public void BulkAddRangeAsync(IEnumerable<TEntity> entities)
        //{
        //    Context.BulkInsertAsync(entities);
        //}

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().CountAsync(criteria);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Context.Database.BeginTransaction();
        }

        public IQueryable<TEntity> GetQuery()
        {
            return DbSet;
        }

        public IQueryable<TEntity> GetQueryNoTracking()
        {
            return DbSet.AsNoTracking();
        }

        public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> expression)
        {
            return Context.Set<TEntity>().Where(expression);
        }

        public IQueryable<TEntity> GetQueryNoTracking(Expression<Func<TEntity, bool>> expression)
        {
            return Context.Set<TEntity>().AsNoTracking().Where(expression);
        }

        public async Task<bool> GetAny(Expression<Func<TEntity, bool>> expression)
        {
            return await Context.Set<TEntity>().AnyAsync(expression);
        }

        public DbContext GetDbContext()
        {
            return Context;
        }

        public TEntity Single(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().Single(criteria);
        }

        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().SingleAsync(criteria);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().SingleOrDefault(criteria);
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().SingleOrDefaultAsync(criteria);
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().Where(criteria);
        }

        public TEntity FindOne(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().Where(criteria).FirstOrDefault();
        }

        public Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().Where(criteria).FirstOrDefaultAsync();
        }

        public Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>>[] includeExpressions)
        {
            IQueryable<TEntity> includeQueryable = DbSet;
            foreach (Expression<Func<TEntity, object>>? includeExpression in includeExpressions)
            {
                includeQueryable = includeQueryable.Include(includeExpression);
            }

            return includeQueryable.FirstOrDefaultAsync(expression);
        }

        public int Count()
        {
            return GetQuery().Count();
        }

        public Task<int> CountAsync()
        {
            return GetQuery().CountAsync();
        }

        public int Count(Expression<Func<TEntity, bool>> criteria)
        {
            return GetQuery().Count(criteria);
        }

        public void Remove(TEntity entity)
        {
            if (entity != null) DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }

        //public void BulkDeleteAsync(IEnumerable<TEntity> entities)
        //{
        //    Context.BulkDeleteAsync(entities);
        //}

        public void Reload(TEntity entity)
        {
            Context.Entry(entity).Reload();
        }

        public IQueryable<TEntity> GetDataFromQuery(string query)
        {
            return Context.Set<TEntity>().FromSqlRaw(query);
        }

        /// <summary>
        /// Check model have a specific property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected bool HasProperty(string property)
        {
            return _type.GetProperty(property) != null;
        }

        protected void SetProperty(TEntity entity, string property, object value)
        {
            if (value != null)
            {
                object? parseValue = Guid.TryParse(value.ToString(), out Guid guid) ? guid : value;
                entity.GetType().GetProperty(property).SetValue(entity, parseValue);
            }
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
        }
    }
}
