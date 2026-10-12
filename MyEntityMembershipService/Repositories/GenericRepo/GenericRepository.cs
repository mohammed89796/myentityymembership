using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.DTOs;
using MyEntityMembershipService.SpecificationPattern;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using static Dapper.SqlMapper;



namespace MyEntityMembershipService.Repositories.GenericRepo
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly EntityMembershipDbContext context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(EntityMembershipDbContext context)
        {
            this.context = context;
            _dbSet = context.Set<T>();
        }


        public async Task Add(T entity)
            => await context.Set<T>().AddAsync(entity);

        public void Delete(T entity)
            => context.Set<T>().Remove(entity);

        public void DeleteRange(List<T> entity)
            => context.Set<T>().RemoveRange(entity);

        public async Task<List<T>> GetAllAsync()
            => await context.Set<T>().ToListAsync();

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public IQueryable<T> AsQueryableAsync()
            => _dbSet.AsQueryable();

        public async Task<T> GetByIdAsync(int id)
            => await context.Set<T>().FindAsync(id);

        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.FirstOrDefaultAsync(predicate);

        public async Task<T> FindAsyncWithNoTracking(Expression<Func<T, bool>> predicate)
           => await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);

        public void Update(T entity)
            => context.Set<T>().Update(entity);

        public void UpdateField(T entity, params Expression<Func<T, object>>[] properties)
        {
            _dbSet.Attach(entity);
            foreach (var property in properties)
            {
                context.Entry(entity).Property(property).IsModified = true;
            }
        }

        public async Task<int> SaveAsync()
            => await context.SaveChangesAsync();



        public void AddRange(List<T> entitise)
            => context.Set<T>().AddRange(entitise);

        public async Task<T> GetWithIncludesAsync(int value, string filterColumn, params Expression<Func<T, object>>[] includes)
        {
            // Create the base query with dynamic filtering using the filterColumn
            IQueryable<T> query = context.Set<T>().Where(entity => EF.Property<int>(entity, filterColumn) == value);

            // Apply each include
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync();
        }
        public async Task<T> GetWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = context.Set<T>().Where(entity => EF.Property<int>(entity, "Id") == id);

            // Apply each include
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync();
        }

        public void SoftDelete(T entity)
        {
            var entry = context.Entry(entity);

            entry.Property("IsDeleted").CurrentValue = true;
        }

        public void UnSoftDelete(T entity)
        {
            var entry = context.Entry(entity);

            entry.Property("IsDeleted").CurrentValue = false;
        }

        public async void SoftDeleteRange(List<T> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var entry = context.Entry(entities[i]);


                entry.Property("IsDeleted").CurrentValue = true;

            }
        }

        public void UnSoftDeleteRange(List<T> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var entry = context.Entry(entities[i]);

                entry.Property("IsDeleted").CurrentValue = false;

            }
        }
        public async Task<int> GetCountAsync()
            => await context.Set<T>().CountAsync();

        

        public async Task<int> GetCountByCriteriaAsync(Expression<Func<T, bool>> criteria)
        => await context.Set<T>().AsNoTracking().CountAsync(criteria);


        public async Task<bool> IsExistsAsync(Expression<Func<T, bool>> criteria, bool ignoreQueryFilter = false)
        {
            if (ignoreQueryFilter)
            {
                return await context.Set<T>().IgnoreQueryFilters().AnyAsync(criteria);
            }
            else
            {
                return await context.Set<T>().AnyAsync(criteria);
            }
        }

        private IQueryable<T> ApplySpecification(Specifications<T> Spec)
            => Spec.GetQuery(context.Set<T>());

        public async Task<List<T>> GetAllWithSpecifications(Specifications<T> specifications)
        {
            return await ApplySpecification(specifications).ToListAsync();
        }

        public async Task<T> GetWithSpecifications(Specifications<T> specifications)
        {
            return await ApplySpecification(specifications).FirstOrDefaultAsync();
        }

        public async Task<TResult> GetWithSpecificationsAndProjection<TResult>(Specifications<T> specifications, Expression<Func<T, TResult>> selector) where TResult : class
        {
            var query = ApplySpecification(specifications);
            var projectedQuery = query.Select(selector);
            return await projectedQuery.FirstOrDefaultAsync();
        }

        public async Task<T> FindByCriteriaAsync(Expression<Func<T, bool>> criteria)
         => await context.Set<T>().Where(criteria).FirstOrDefaultAsync();

        public async Task<List<T>> FindAllByCriteriaAsync(Expression<Func<T, bool>> criteria)
          => await context.Set<T>().AsNoTracking().Where(criteria).ToListAsync();

        public async Task AddRange(ICollection<T> entity)
            => await context.Set<T>().AddRangeAsync(entity);

        public void RemoveRange(ICollection<T> entits)
        => context.Set<T>().RemoveRange(entits);

        public async Task<List<AuditLogDto<TEntity>>> GetLogs<TEntity>(int id) where TEntity : BaseEntity
        {
            var logs = await context.Set<AuditLog>()
                .Where(l => l.TableName == typeof(TEntity).Name && l.EntityId == id)
                .ToListAsync();

            //        var options = new JsonSerializerOptions
            //        {
            //            PropertyNameCaseInsensitive = true,
            //            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            //            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            //            Converters =
            //{
            //    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            //}
            //        };


            return logs.Select(al => new AuditLogDto<TEntity>
            {

                Id = al.Id,
                TableName = al.TableName,
                OriginalValuesJson = JsonSerializer.Deserialize<TEntity>(al.OriginalValuesJson),
                CurrentValuesJson = JsonSerializer.Deserialize<TEntity>(al.CurrentValuesJson),
                ChangedBy = al.ChangedBy,
                ChangedOn = al.ChangedOn,
                EntityId = al.EntityId
            }).ToList();
        }



        public async Task<IReadOnlyList<T>> GetAllWithIncludesAsync(
    Expression<Func<T, bool>>? criteria = null,
    int? pageSize = null,
    int? pageNumber = null,
    params Expression<Func<T, object>>[]? includes)
        {
            IQueryable<T> query = _dbSet;

            if (criteria != null)
            {
                query = query.Where(criteria);
            }

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (pageSize.HasValue && pageNumber.HasValue)
            {
                query = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                             .Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }
        public async Task<List<TResult>> GetAllWithSpecificationsAndProjection<TResult>(
    Specifications<T> specifications,
    Expression<Func<T, TResult>> selector) where TResult : class

        {
            var query = ApplySpecification(specifications);
            var projectedQuery = query.Select(selector);
            return await projectedQuery.ToListAsync();
        }
        public async Task<int> ExecuteUpdateAsync(
        Expression<Func<T, bool>> predicate,
        Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setProperties)
        {
            return await _dbSet
                .Where(predicate)
                .ExecuteUpdateAsync(setProperties);
        }


    }
}
