using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Entity.SystemDtos.FedrationEntityTypes;


using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Entity.System.FedrationEntityTypesRepo
{
    public interface IFedrationEntityTypesRepo : IGenericRepository<FedrationEntityType>
    {
        public Task<List<FedrationEntityTypesViewModel>> GetFedEntityTypes();
    }
}
