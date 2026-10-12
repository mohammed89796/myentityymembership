using Microsoft.EntityFrameworkCore.Query;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.SpecificationPattern;
using System.Linq.Expressions;
using static Dapper.SqlMapper;

namespace MyEntityMembershipService.Repositories.GenericRepo
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> AsQueryableAsync();
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

        Task<T> FindAsync(Expression<Func<T, bool>> predicate);

        Task<T> FindAsyncWithNoTracking(Expression<Func<T, bool>> predicate);
        Task AddRange(ICollection<T> entity);
        Task Add(T entity);
        void AddRange(List<T> entitise);
        void Update(T entity);
        void UpdateField(T entity, params Expression<Func<T, object>>[] properties);
        void Delete(T entity);
        void RemoveRange(ICollection<T> entits);

        void DeleteRange(List<T> entity);
        Task<int> SaveAsync();

        Task<T> GetWithIncludesAsync(int value, string filterColumn, params Expression<Func<T, object>>[] includes);
        Task<T> GetWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<int> GetCountAsync();
        
        Task<int> GetCountByCriteriaAsync(Expression<Func<T, bool>> criteria);

        void SoftDelete(T entity);
        void SoftDeleteRange(List<T> entities);
        void UnSoftDelete(T entity);
        void UnSoftDeleteRange(List<T> entities);

        Task<bool> IsExistsAsync(Expression<Func<T, bool>> criteria, bool ignoreQueryFilter = false);

        Task<List<T>> GetAllWithSpecifications(Specifications<T> specifications);
        Task<List<TResult>> GetAllWithSpecificationsAndProjection<TResult>(Specifications<T> specifications,Expression<Func<T, TResult>> selector) where TResult : class;
        Task<T> GetWithSpecifications(Specifications<T> specifications);
        Task<TResult> GetWithSpecificationsAndProjection<TResult>(Specifications<T> specifications, Expression<Func<T, TResult>> selector) where TResult : class;
        Task<T> FindByCriteriaAsync(Expression<Func<T, bool>> criteria);
        Task<List<T>> FindAllByCriteriaAsync(Expression<Func<T, bool>> criteria);

        Task<IReadOnlyList<T>> GetAllWithIncludesAsync(
    Expression<Func<T, bool>>? criteria = null,
    int? pageSize = null,
    int? pageNumber = null,
    params Expression<Func<T, object>>[]? includes);


        Task<List<AuditLogDto<TEntity>>> GetLogs<TEntity>(int id) where TEntity : BaseEntity;

        Task<int> ExecuteUpdateAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setProperties);
    }
}
