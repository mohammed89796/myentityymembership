using MyEntityMembershipService.Data;
using Microsoft.EntityFrameworkCore;
using Elastic.Apm.Api;
using MyEntityMembershipService.DTOs.Membership.ConfigurationDtos;
using MyEntityMembershipService.Repositories.GenericRepo;
using MyEntityMembershipService.Data.Membership.Entities;

namespace MyEntityMembershipService.Repositories.Membership.ConfigurationRepo
{
    public class ConfigurationRepository : GenericRepository<PackageConfigurationSection>, IConfigurationRepository
    {
        private readonly EntityMembershipDbContext _context;

        public ConfigurationRepository(EntityMembershipDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ConfigurationResponseDto>> GetPackageDetailsByRequestTypeAndPackageID(int requestTypeId, int packageID)
        {
            var configurationResponse = new List<ConfigurationResponseDto>();

            var configurationDetails = await _context.ConfigrationFieldResponses
                .Include(cfr => cfr.ConfigurationField)
                    .ThenInclude(cf => cf.FieldType)
                .Include(cfr => cfr.Configuration)
                    .ThenInclude(cfs => cfs.PackageConfigurationSections)
                .Where(cfr => cfr.Configuration.RequestTypeId == requestTypeId && cfr.PackageId == packageID)
                .ToListAsync();

            var requestDetails = await _context.RequestTypes.Where(rt => rt.Id == requestTypeId).FirstOrDefaultAsync();

            if (configurationDetails == null || !configurationDetails.Any())
            {
                return configurationResponse;
            }

            foreach (var fieldResponse in configurationDetails)
            {
                var field = fieldResponse.ConfigurationField;

                var packageSection = fieldResponse.Configuration.PackageConfigurationSections
                .FirstOrDefault();

                var responseDto = new ConfigurationResponseDto
                {
                    FieldHeader = fieldResponse.ConfigurationField.PackageConfigurationSection.Name,
                    FieldName = field.Name,
                    FieldtypeID = field.FieldTypeId,
                    FieldtypeName = field.FieldType.Name,
                    FieldResponseString = fieldResponse.FieldResponse,
                    FieldResponseBool = bool.TryParse(fieldResponse.FieldResponse, out var boolResponse) ? boolResponse : null
                };

                configurationResponse.Add(responseDto);
            }

            return configurationResponse;
        }

        public async Task<Configuration> GetConfigeWithFields(int Id)
                  => await _context.Configurations.Where(config => config.Id == Id).Include(type => type.ConfigrationFieldResponses).ThenInclude(r => r.ConfigurationField).FirstOrDefaultAsync();



    }
}
