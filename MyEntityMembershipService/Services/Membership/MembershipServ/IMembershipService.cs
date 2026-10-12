using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.Dtos;
using MyEntityMembershipService.DTOs.Membership.DashboardDtos;
using MyEntityMembershipService.DTOs.Membership.FederationMmebrshipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.HistoricalTournamentDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipRequestsDtos;
using MyEntityMembershipService.DTOs.Membership.NewMembershipDtos;
using MyEntityMembershipService.DTOs.Membership.PackageDtos;
using MyEntityMembershipService.DTOs.Membership.Swimming;
using MyEntityMembershipService.Repositories.Common;

namespace MyEntityMembershipService.Services.Membership.MembershipServ
{
    public interface IMembershipService
    {
        Task<Response<MembershipDto>> CreateMembership(MembershipDto membershipDto);
        Task<Response<MembershipDto>> CreateSubMemberMembership(subMemberMembershipDto subMemberMembershipDto);
        Task<Response<List<MembershipDto>>> GeMembershipsByPackageId(int PackageId);
        Task<Response<MembershipDto>> GeMembershipsById(int Id);
        Task<Response<List<MembershipDto>>> GeMembershipsByTenantId(int tenantId);
        Task<Response<NumberOfMembershpsDto>> GetMembershipsGroupedByPackageNameAsync(int year, int? tenantId);
        Task<Response<MembershipGrowthRateDto>> MembershipsGrowthRatePerPackage(int year, int? tenantId);
        Task<Response<List<MembershipSegmentationDto>>> GetMembershipsSegmentationByPackageAsync(int year, int tenantId);
        Task<PackageDto> GetPackageByIdAsync(int packageId);
        Task<Response<List<TenantMemberDto>>> GetTenantMembers
            (GetTenantMembersParamDto parameters);
        Task<Response<List<DropdownDto>>> GetBelts();
        Task<Response<List<FederationMemberDto>>> GetFederationMembers(GetFederationMembersParamDto parameters);

        Task<Response<PaginationToReturnDto<MembershipInjuryStatusDto>>> GetSwimmingMembers(GetSwimmingMembersParamDto parameters);
        Task<Response<MembershipInjuryStatusDto>> GetMembershipInjuryStatusAsync(int membershipId);
        Task<Response<List<MembershipBasicDto>>> SearchMembershipsAsync(string? searchTerm);
        Task<Response<MembershipDto>> CreateSwimmingMembership(CreateMembershipDto createMembershipDto);
        Task<Response<MembershipDto>> UpdateSwimmingMembership(UpdateMembershipDto updateDto);
        Task<Response<string>> SoftDeleteSwimmingMembership(int membershipId);
        Task<Response<string>> UnSoftDeleteSwimmingMembership(int membershipId);
        Task<Response<List<GetAllUserMemberships>>> GetAllUserMembershipsAsync(List<string> ChildrenIds);
        Task<Response<PaginationToReturnDto<MemberSportLevelDto>>> GetMembersSportLevels(GetMemberSportLevelsParamDto parameters);

        Task<Response<List<MembershipInjuryStatusDto>>> GetAllSwimmingMembers();

        Task<Response<SportDistributionDto>> GetSportDistribution(SportDistributionFilterDto filterDto, int? tenantId);

        Task<Response<bool>> UserHasMainMembershipAsync(int tenantId);
        Task<Response<List<EntitiesResponseDto>>> GetEntitiesByUserIdAsync(string userId);

        Task<Response<List<SportLevelsResponseDto>>> GetAllSportLevelByUserId(string userId, SportLevelSearchDto searchDto);
        Task<Response<List<SportLevelTenantResponseDto>>> GetAllSportLevelTenantsByUserId(string userId,int sportId, SportLevelTenantSearchDto searchDto);
    }
}
