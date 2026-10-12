using Microsoft.EntityFrameworkCore.Storage;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.UnitOfWorkRepo
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> Save();
        //Task BeginTransactionAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();

        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
