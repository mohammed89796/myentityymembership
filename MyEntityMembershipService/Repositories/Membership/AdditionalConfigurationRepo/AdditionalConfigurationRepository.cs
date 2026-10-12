using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.GenericRepo;

namespace MyEntityMembershipService.Repositories.Membership.AdditionalConfigurationRepo
{
    public class AdditionalConfigurationRepository : GenericRepository<AdditionalConfiguration>, IAdditionalConfigurationRepository
    {
        private readonly EntityMembershipDbContext context;

        public AdditionalConfigurationRepository(EntityMembershipDbContext context) : base(context)
        {
            this.context = context;
        }
        //public async Task<List<AdditionalConfiguration>> SyncAdditionalConfigurations(int packageId, List<AdditionalConfigurationDto> newConfigurations)
        //{
        //    // Fetch the existing configurations from the database including SubMemberAdditionalConfigurations
        //    var existingConfigurations = await context.AdditionalConfigurations
        //        .Where(c => c.PackageId == packageId)
        //        .Include(c => c.SubMemberAdditionalConfigurations)
        //        .ToListAsync();

        //    // Find configurations to delete (existing ones not in the new list)
        //    var configurationsToDelete = existingConfigurations
        //        .Where(ec => !newConfigurations.Any(nc => nc.PackageId == ec.Id))
        //        .ToList();

        //    // Find configurations to add (new ones not in the existing list)
        //    var configurationsToAdd = newConfigurations
        //        .Where(nc => !existingConfigurations.Any(ec => ec.Id == nc.Id))
        //        .ToList();

        //    // Find configurations to update (existing ones that are modified)
        //    var configurationsToUpdate = existingConfigurations
        //        .Where(ec => newConfigurations.Any(nc => nc.Id == ec.Id))
        //        .ToList();

        //    // Handle deletions
        //    foreach (var configToDelete in configurationsToDelete)
        //    {
        //        _context.AdditionalConfigurations.Remove(configToDelete);
        //    }

        //    // Handle additions
        //    foreach (var configToAdd in configurationsToAdd)
        //    {
        //        var entityToAdd = Mapper.Map<AdditionalConfiguration>(configToAdd);
        //        _context.AdditionalConfigurations.Add(entityToAdd);
        //    }

        //    // Handle updates
        //    foreach (var existingConfig in configurationsToUpdate)
        //    {
        //        var newConfigDto = newConfigurations.First(nc => nc.Id == existingConfig.Id);

        //        // Update the main configuration fields (e.g., Name)
        //        existingConfig.Name = newConfigDto.Name;

        //        // For each SubMemberAdditionalConfiguration, track changes
        //        foreach (var subConfigDto in newConfigDto.SubMemberAdditionalConfigurations)
        //        {
        //            var existingSubConfig = existingConfig.SubMemberAdditionalConfigurations
        //                .FirstOrDefault(sc => sc.SubMemberId == subConfigDto.SubMemberId && sc.MemberType == subConfigDto.MemberType);

        //            if (existingSubConfig != null)
        //            {
        //                // Update the existing SubMemberAdditionalConfiguration
        //                existingSubConfig.Price = subConfigDto.Price;
        //                existingSubConfig.UnitId = subConfigDto.UnitId;
        //            }
        //            else
        //            {
        //                // Add new SubMemberAdditionalConfiguration
        //                var newSubConfig = Mapper.Map<SubMemberAdditionalConfiguration>(subConfigDto);
        //                newSubConfig.AdditionalConfigurationId = existingConfig.Id; // set the foreign key
        //                existingConfig.SubMemberAdditionalConfigurations.Add(newSubConfig);
        //            }
        //        }
        //    }

        //    // Save changes
        //    await _context.SaveChangesAsync();

        //    return existingConfigurations;
        //}
    
            public async Task<AdditionalConfiguration> GetAdditionalConfigurationWithSubMemeberByIdAsync(int Id,int packageId)
            => await context.AdditionalConfigurations.Where(ac=>ac.Id==Id&&ac.PackageId==packageId)
            .Include(ac=>ac.SubMemberAdditionalConfigurations)
            .FirstOrDefaultAsync();


        public async Task<double?> GetAdditionFeeByPackageIdAndSubMembersIdAsync(int packageId, int? subMemberId = null)
        {
            var entity = await context.Set<AdditionalConfiguration>()
             .Where(config => config.PackageId == packageId)
             .Include(config => config.SubMemberAdditionalConfigurations)
             .FirstOrDefaultAsync();

            var additionFee = entity.SubMemberAdditionalConfigurations.Where(subMember => subMember.SubMemberId == subMemberId)
                .Select(subMember => subMember.Price)
                .FirstOrDefault();

            return additionFee;
            
        }
    }
}

