using System.Linq.Expressions;

namespace FBAdsManager.Common.Database.Repository
{
    public interface IRepository<TEntity>
    {
        #region Get

        IQueryable<TEntity> GetQuery();

        IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> expression);

        IQueryable<TEntity> GetQueryNoTracking();

        IQueryable<TEntity> GetQueryNoTracking(Expression<Func<TEntity, bool>> expression);

        //DbContext GetDbContext();

        TEntity Single(Expression<Func<TEntity, bool>> criteria);

        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> criteria);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> criteria);

        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> criteria);

        #endregion Get

        #region Find

        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> criteria);

        TEntity FindOne(Expression<Func<TEntity, bool>> criteria);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> criteria);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> expression,
            Expression<Func<TEntity, object>>[] includeExpressions);

        #endregion Find

        #region Count

        int Count();

        int Count(Expression<Func<TEntity, bool>> criteria);

        Task<int> CountAsync();

        Task<int> CountAsync(Expression<Func<TEntity, bool>> criteria);

        Task<bool> GetAny(Expression<Func<TEntity, bool>> expression);

        #endregion Count

        #region Update

        void Update(TEntity entity, Expression<Func<TEntity, bool>> criteria);

        void Update(TEntity entity);

        void UpdateRange(IEnumerable<TEntity> entities);

        #endregion Update

        #region Add

        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        #endregion Add

        #region Remove

        void Remove(TEntity entity);

        void RemoveRange(IEnumerable<TEntity> entities);

        #endregion Remove

        #region Sql

        IQueryable<TEntity> GetDataFromQuery(string query);

        #endregion Sql

        void Reload(TEntity entity);
    }
}
