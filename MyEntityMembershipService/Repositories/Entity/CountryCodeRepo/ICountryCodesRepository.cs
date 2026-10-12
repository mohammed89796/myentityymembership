


using MyEntityMembershipService.Repositories.GenericRepo;
using System.Threading.Tasks;

namespace MyEntityMembershipService.Repositories.Entity.CountryCodeRepo
{
    public interface ICountryCodesRepository : IGenericRepository<CountryCodes>
    {
        Task<List<CountryCodes>> GetAllCountryCodesAsync();
    }
}
