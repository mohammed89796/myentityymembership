using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Membership.PaakageMemberFormRepo
{
    public interface IPaakageMemberFormRepo : IGenericRepository<PaakageMemberForm>
    {
        Task<List<PaakageMemberForm>> GetAllForms();
        Task<List<PaakageMemberForm>> GetAllFormsWithoughtNew();

        Task<PaakageMemberForm> GetMemberFormByFormIdPackageIdmemberTypeId(int packageId, int formId,int memberTypeId);
    }
}
