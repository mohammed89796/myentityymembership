
using MyEntityMembershipService.Dtos;


using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Entity.SportsService
{
    public interface ISportsService
    {
        Task<Response<bool>> CreateSport(string sportName);
        Task<Response<List<Sports>>> GetSportsDropDown();

    }
}
