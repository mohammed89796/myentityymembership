using MyEntityMembershipService.DTOs.Entity.SystemDtos.FedrationEntityTypes;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.System.FedrationEntityTypesService
{
    public interface IFedrationEntityTypesService
    {
        public Task<Response<bool>> CreateFedrationEntityType(string name);
        public Task<Response<List<FedrationEntityTypesViewModel>>> GetFedEntityTypes();


    }
}