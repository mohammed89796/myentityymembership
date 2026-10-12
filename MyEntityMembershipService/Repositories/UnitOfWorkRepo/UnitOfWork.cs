using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Repositories.GenericRepo;
using System.Collections;

namespace MyEntityMembershipService.Repositories.UnitOfWorkRepo
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly EntityMembershipDbContext context;
        private IDbContextTransaction _transaction;

        private Hashtable repositories;
        public UnitOfWork(EntityMembershipDbContext context)
        {
            this.context = context;
            repositories = new Hashtable();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity).Name;
            if (!repositories.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity>(context);
                repositories.Add(type, repository);
            }
            return repositories[type] as IGenericRepository<TEntity>;
        }

        public async Task<int> Save()
            => await context.SaveChangesAsync();
        //public async Task BeginTransactionAsync()
        //{
            //_transaction = await context.Database.BeginTransactionAsync();
        //}

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
            return _transaction;
        }
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            context.Dispose();
        }
    }
}
