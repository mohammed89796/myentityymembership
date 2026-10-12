using AutoMapper;

using MyEntityMembershipService.Data;

using MyEntityMembershipService.Data.Entity.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Entity.AtheleteDtos;
using MyEntityMembershipService.DTOs.Entity.BatchDtos;
using MyEntityMembershipService.DTOs.Entity.BeltDtos;
using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using MyEntityMembershipService.DTOs.Entity.EntityAdminDtos;
using MyEntityMembershipService.DTOs.Entity.FacilityDtos;
using MyEntityMembershipService.DTOs.Entity.FederationDtos;
using MyEntityMembershipService.DTOs.Entity.MobileDtos;
using MyEntityMembershipService.DTOs.Entity.PackageDtos;
using MyEntityMembershipService.DTOs.Entity.PostDtos;
using MyEntityMembershipService.DTOs.Entity.PostTagDtos;
using MyEntityMembershipService.DTOs.Entity.SeriesDtos;
using MyEntityMembershipService.DTOs.Entity.TenantDtos;
using MyEntityMembershipService.DTOs.Entity.VenueDtos;
using MyEntityMembershipService.DTOs.Membership.EntityDtos;
using MyEntityMembershipService.Extensions;
using AddTenantDto = MyEntityMembershipService.DTOs.Entity.TenantDtos.AddTenantDto;
using TenantDetailsDto = MyEntityMembershipService.DTOs.Entity.TenantDtos.TenantDetailsDto;

namespace MyEntityMembershipService.Helper.MappingProfile.Entity
{
    public partial class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddTenantDto, Tenant>().ReverseMap();
            CreateMap<TenantDto, Tenant>().ReverseMap();
            CreateMap<UpdateTenantDto, Tenant>()
                       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Tenant, UpdateTenantDto>();

            CreateMap<Branch, BranchDto>()
            .ForMember(dest => dest.Facilities, opt => opt.MapFrom(src => src.NewFacilities))

                .ReverseMap();
            CreateMap<Branch, AddBranchDto>()
                .ForMember(dest => dest.BranchServices, opt => opt.MapFrom(src => src.BranchServices.Select(bs => bs.CoreServiceID)))
                .ReverseMap()
                .ForMember(dest => dest.BranchServices, opt => opt.MapFrom(src => src.BranchServices.Select(id => new BranchServices { CoreServiceID = id })));

            CreateMap<BranchLocation, BranchLocationDto>().ReverseMap();
            CreateMap<BranchOperatingHours, BranchOperatingHoursDto>().ReverseMap();
            CreateMap<Facility, FaciltityDto>().ReverseMap();
            CreateMap<NewFacility, FaciltityDto>().ReverseMap();

            CreateMap<AnnouncementDto, Post>()
                 .ForMember(d => d.PostType, o => o.MapFrom(_ => PostType.Announcement));


            CreateMap<Post, AnnouncementToReturnDto>()
               .ForMember(d => d.Priority, o => o.MapFrom(s => s.Priority.Value.ToString()))
               .ForMember(d => d.PostPictures, o => o.Ignore());

            CreateMap<NewsFeedDto, Post>()
              .ForMember(d => d.PostType, o => o.MapFrom(_ => PostType.NewsFeed))
              .ForMember(d => d.PostTags, o => o.Ignore());

            CreateMap<Post, NewsFeedToReturnDto>()
               .ForMember(d => d.PostPictures, o => o.Ignore());

            CreateMap<PollDto, Post>()
              .ForMember(d => d.PostType, o => o.MapFrom(_ => PostType.EventPoll))
              .ForMember(d => d.Poll, o => o.Ignore())
              .ForMember(d => d.PostPictureUrl, o => o.Ignore());

            CreateMap<Post, PollToReturnDto>()
               .ForMember(d => d.Question, o => o.Ignore())
               .ForMember(d => d.Choices, o => o.Ignore());

            CreateMap<HighlightDto, Post>()
              .ForMember(d => d.PostType, o => o.MapFrom(_ => PostType.HighLights));

            CreateMap<Post, HightlightToReturnDto>()
               .ForMember(d => d.PostPictures, o => o.Ignore());


            CreateMap<PostTagDto, PostTag>();
            CreateMap<Post, PostToReturnDto>()
                .ForMember(d => d.TenantName, o => o.MapFrom(s => s.Tenant.Name));

            CreateMap<Post, MobilePostToReturnDto>()
                .ForMember(d => d.TenantName, o => o.MapFrom(s => s.Tenant.Name))
                .ForPath(d => d.PostedBy.Id, o => o.MapFrom(s => s.CreatedBy));

            CreateMap<Post, PostWithCommentsToReturnDto>()
                .ForMember(d => d.TenantName, o => o.MapFrom(s => s.Tenant.Name))
                .ForPath(d => d.PostedBy.Id, o => o.MapFrom(s => s.CreatedBy));

            //CreateMap<PostTag, PostTagToRetunDto>()
            //    .ForMember(d => d.PostTitle, o => o.MapFrom(s => s.Post.PostTitle));
            CreateMap<PostDto, Post>()
                 .ForMember(d => d.PostTags, o => o.MapFrom(s => s.PostTags.Select(t => new PostTag { Tag = t }).DistinctBy(pt => pt.Tag).ToList()));

            CreateMap<Venue, AddVenueDto>().ReverseMap();
            CreateMap<Venue, UpdateVenueDto>().ReverseMap();
            CreateMap<Facility, AddFacilityDto>().ReverseMap();
            CreateMap<Facility, UpdateFacilityDto>().ReverseMap();
            CreateMap<NewVenue, AddVenueDto>().ReverseMap();
            CreateMap<NewVenue, UpdateVenueDto>().ReverseMap();




            CreateMap<VenueOperatingHours, SetVenueOperatingDto>().ReverseMap();
            CreateMap<VenueOperatingHours, UpdateVenueOperatingHoursDto>().ReverseMap();
            CreateMap<FacilityOperatingHours, SetFacilityOperatingDto>().ReverseMap();
            CreateMap<FacilityOperatingHours, UpdateFacilityOperatingHoursDto>().ReverseMap();
            CreateMap<FacilityDetailsDto, NewFacility>().ReverseMap();
            CreateMap<AddTenantDto, TenantDto>().ReverseMap();


            CreateMap<AddFacilityDto, NewFacility>()
            .ForMember(dest => dest.FacilityAmenities, opt => opt.MapFrom(src =>
                src.FacilityAmenities.Select(amenityId => new NewFacilityAmenities
                {
                    AmenityID = amenityId
                }).ToList()
            ))
            .ForMember(dest => dest.FacilityAccessibilityOptions, opt => opt.MapFrom(src =>
                src.FacilityAccessibilityOptions.Select(optionId => new NewFacilityAccessibilityOptions
                {
                    AccessibilityOptionID = optionId
                }).ToList()
            ))
            .ForMember(dest => dest.FacilityServices, opt => opt.MapFrom(src =>
                src.FacilityServices.Select(serviceId => new FacilityServices
                {
                    CoreFacilityServiceID = serviceId
                }).ToList()
            ))
            .ForMember(dest => dest.SportsList, opt => opt.MapFrom(src =>
                src.SportsList.Select(sportId => new NewFacilitySports
                {
                    SportID = sportId
                }).ToList()
            ))
            .ForMember(dest => dest.FacilityImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.FacilityUploads, opt => opt.Ignore());

            CreateMap<FacilityLocationDto, FacilityLocation>();

            CreateMap<SeriesDto, Series>();

            CreateMap<Series, SeriesToReturnDto>()
                .ForMember(d => d.RegistrationProcessName, o => o.MapFrom(s => s.RegistrationProcess.ToString()))
                .ForMember(d => d.RegistrationTypeName, o => o.MapFrom(s => s.RegistrationType.ToString()))
                .ForMember(d => d.DayNameOfStartDate, o => o.MapFrom(s => s.StartDate.Value.DayOfWeek.ToString()))
                .ForMember(d => d.DayNameOfEndDate, o => o.MapFrom(s => s.EndDate.Value.DayOfWeek.ToString()))
                .ForMember(d => d.MonthNameOfStartDate, o => o.MapFrom(s => s.StartDate.Value.ToString("MMMM")))
                .ForMember(d => d.MonthNameOfEndDate, o => o.MapFrom(s => s.EndDate.Value.ToString("MMMM")));


            CreateMap<EventDto, Event>();

            CreateMap<Event, EventToReturnDto>()
                .ForMember(e => e.SeriesId, o => o.MapFrom(s => s.Series.Id))
                .ForMember(e => e.SeriesName, o => o.MapFrom(s => s.Series.SeriesName));

            CreateMap<EventDetailDto, EventDetail>();

            CreateMap<EventDetail, EventDetailToReturnDto>()
                .ForMember(ed => ed.EventId, o => o.MapFrom(s => s.Event.Id))
                .ForMember(ed => ed.EventName, o => o.MapFrom(s => s.Event.EventName));

            CreateMap<Package, PackageDto>()
                .ForMember(dest => dest.SubMemberCategories, o => o.MapFrom(s => s.SubMemberCategories))
                .ForMember(dest => dest.AcssedBranches, opt => opt.MapFrom(src => src.Branches))
                .ForMember(dest => dest.Facilities, opt => opt.MapFrom(src => src.Facilities))
                .ForMember(dest => dest.Venues, opt => opt.MapFrom(src => src.Venues))
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.Name : string.Empty))
                .ForMember(dest => dest.TenantImage, opt => opt.MapFrom(src => src.Tenant != null ? ResolveImage(src.Tenant.Image) : string.Empty))
                .ReverseMap();

            CreateMap<AccessedBranchesFacilities, FaciltityDto>()
                .ForMember(dest => dest.Venues, opt => opt.MapFrom(src => src.Facility.AccessedFacilitiesVenues
                .Where(av => av.Venue != null)
                .Select(av => av.Venue)));

            CreateMap<AccessedBranches, BranchDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Branch.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Branch.Name))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Branch.Location))
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.Branch.TenantId))
                .ForMember(dest => dest.BranchImageURL, opt => opt.MapFrom(src => src.Branch.BranchImageURL))

                .ForMember(dest => dest.Facilities, opt => opt.MapFrom(src => src.Branch.AccessedBranchesFacilities.Select(fb => fb.Facility)));

            CreateMap<NewFacility, NewFacilityDto>()
             .ForMember(dest => dest.Venues, opt => opt.MapFrom(src => src.NewVenues))
             .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => src.FacilityType.name))
            .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.FacilityName))
             .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.FacilityImageUrl))


                .ReverseMap();
            CreateMap<NewVenue, NewVenueDto>().ReverseMap();



            CreateMap<FacilitySportsAccommodated, SportsAccommodatedDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.SportsAccommodated.name))
               .ReverseMap();



            CreateMap<Package, AddPackageDto>().ReverseMap();
            CreateMap<Package, PackageDataDto>().ReverseMap();

            CreateMap<AddPackageDto, Package>();

            CreateMap<SubMemberCategory, SubMemberCategoryDto>().ReverseMap();



            CreateMap<AccessedFacilitiesVenues, AddVenueDto>();
            //Mapping for AccessedBranchesDto to AccessedBranches

            CreateMap<AccessedBranchesDto, AccessedBranches>()
               .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.BranchId))
               .ForMember(dest => dest.PackageId, opt => opt.Ignore())
               .ForMember(dest => dest.Package, opt => opt.Ignore())
               .ForMember(dest => dest.Branch, opt => opt.Ignore());

            // Mapping for AccessedBranchesFacilities
            CreateMap<AccessedBranchesFacilityDto, AccessedBranchesFacilities>()
                .ForMember(dest => dest.FacilityId, opt => opt.MapFrom(src => src.FacilityId))
                .ForMember(dest => dest.PackageId, opt => opt.Ignore())
                .ForMember(dest => dest.Package, opt => opt.Ignore())
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore());

            // Mapping for AccessedFacilitiesVenueDto to AccessedFacilitiesVenues
            CreateMap<AccessedFacilitiesVenueDto, AccessedFacilitiesVenues>()
                .ForMember(dest => dest.VenueId, opt => opt.MapFrom(src => src.VenueId))
                .ForMember(dest => dest.PackageId, opt => opt.Ignore())
                .ForMember(dest => dest.Package, opt => opt.Ignore())
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore())
                .ForMember(dest => dest.FacilityId, opt => opt.Ignore())
                .ForMember(dest => dest.Facility, opt => opt.Ignore());

            CreateMap<PackageAuthourizations, CreatePackageAuthourizationsDto>().ReverseMap();
            CreateMap<PackageAuthourizations, PackageAuthourizationsDto>().ReverseMap();


            // new facilities mapping 

            CreateMap<AccessedFacilitiesVenues, NewVenueDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Venue.Id))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Venue.Name))
             .ForMember(dest => dest.VenueImageURL, opt => opt.MapFrom(src => src.Venue.VenueImageURL))
             .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Venue.Description))
             .ForMember(dest => dest.FacilityId, opt => opt.MapFrom(src => src.Venue.FacilityId))
             .ReverseMap();


            //Mapping for AccessedBranchesDto to AccessedBranches
            CreateMap<AccessedBranchesFacilities, NewFacilityDto>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Facility.Id))
             .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Facility.FacilityName))
             .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Facility.FacilityImageUrl))
             .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => src.Facility.FacilityType.name))
             .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Facility.Description))
             .ForMember(dest => dest.Venues, opt => opt.MapFrom(src => src.Facility.AccessedFacilitiesVenues
                 .Where(av => av.Venue != null)  // Ensure non-null venues
                 .Select(av => av)))  // Use AccessedFacilitiesVenues mapping
             .ReverseMap();
            CreateMap<Tenant, AddFederationEntityDto>()
            .ForMember(dest => dest.EntityTypeId, opt => opt.MapFrom(src => src.FedrationEntityTypeId))
                .ReverseMap();
            CreateMap<Tenant, UpdateFederationDto>().ReverseMap();
            CreateMap<UpdateFederationDto, Tenant>()
                .ForMember(dest => dest.FedrationEntityTypeId, opt => opt.MapFrom(src => src.FedrationEntityTypeId));
            CreateMap<Tenant, FederationEntityDto>()
    .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.EntityType != null ? src.EntityType.Name : null))
    .ForMember(dest => dest.EntityTypeName, opt => opt.MapFrom(src => src.FedrationEntityType != null ? src.FedrationEntityType.Name : null))
    .ReverseMap();
            CreateMap<Tenant, AddFederationDto>()
            .ReverseMap();
            CreateMap<Tenant, FederationDto>()
              .ReverseMap();

            CreateMap<Tenant, TenantDetailsDto>().ReverseMap();


            CreateMap<AddAtheleteDto, EntityMember>();
            CreateMap<AddEntityAdminDto, TenantAdmin>()
                .ForMember(t => t.TenantAdminRoles, opt => opt.MapFrom(src => src.RoleIds.Select(b => new TenantAdminRole { RoleId = b })))
                .ForMember(t => t.TenantAreaAdmins, opt => opt.MapFrom(src => src.BranchIds.Select(b => new TenantAdminArea { AreaId = b })))
                .ForMember(t => t.TenantAdminSports , opt => opt.MapFrom(src => src.SportsIds.Select(s => new TenantAdminSport { SportId = s } )))
                .ForMember(t => t.TenantAdminVenues, opt => opt.MapFrom(src => src.VenuesIds.Select(v => new TenantAdminVenue { VenueId = v })))
                .ForMember(t => t.TenantAdminFacilities, opt => opt.MapFrom(src => src.FacilitiesIds.Select(f => new TenantAdminFacility { FacilityId = f })))
                .ForMember(t => t.TenantAdminBranches , opt => opt.MapFrom(src => src.BranchsIds.Select(b => new TenantAdminBranch { BranchId = b })));

            CreateMap<TenantAdmin, TenantAdminDto>()
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant.Name))
                .ForMember(dest => dest.AreaIds, opt => opt.MapFrom(src => src.TenantAreaAdmins.Select(a => a.AreaId)))
                .ForMember(dest => dest.RoleIds, opt => opt.MapFrom(src => src.TenantAdminRoles.Select(r => r.RoleId)))
                .ForMember(dest => dest.SportIds, opt => opt.MapFrom(src => src.TenantAdminSports.Select(s => s.SportId)))
                .ForMember(dest => dest.FacilityIds, opt => opt.MapFrom(src => src.TenantAdminFacilities.Select(f => f.FacilityId)))
                .ForMember(dest => dest.BranchIds, opt => opt.MapFrom(src => src.TenantAdminBranches.Select(b => b.BranchId)))
                .ForMember(dest => dest.VenueIds, opt => opt.MapFrom(src => src.TenantAdminVenues.Select(v => v.VenueId)));



            CreateMap<TenantAdmin, GetEntityAdminDto>()
                .ForMember(t => t.RoleIds, opt => opt.MapFrom(src => src.TenantAdminRoles.Select(r => r.RoleId)))
                .ForMember(t => t.Branches, opt => opt.MapFrom(src => src.TenantAreaAdmins.Select(s => new AdminBranchDto { Id = s.AreaId, Name = s.Area.Name })))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.EntityAdminStatus))
                .ForMember(dest => dest.SportIds, opt => opt.MapFrom(src => src.TenantAdminSports.Select(s => s.SportId)))
                .ForMember(dest => dest.FacilityIds, opt => opt.MapFrom(src => src.TenantAdminFacilities.Select(f => f.FacilityId)))
                .ForMember(dest => dest.BranchIds, opt => opt.MapFrom(src => src.TenantAdminBranches.Select(b => b.BranchId)))
                .ForMember(dest => dest.VenueIds, opt => opt.MapFrom(src => src.TenantAdminVenues.Select(v => v.VenueId)))
                .ReverseMap();

            CreateMap<UpdateEntityAdminDto, TenantAdmin>()
                .ForMember(t => t.TenantAdminRoles, opt => opt.MapFrom(src => src.RoleIds.Select(b => new TenantAdminRole { RoleId = b })))
                .ForMember(t => t.TenantAreaAdmins, opt => opt.MapFrom(src => src.BranchIds.Select(b => new TenantAdminArea { AreaId = b })));

            CreateMap<Batch, BatchToReturnDto>();

            CreateMap<Belt, BeltToReturnDto>();

            CreateMap<EntityMember, AtheleteToReturnDto>()
                .ForMember(dest => dest.MembershipStatus, opt => opt.MapFrom(src => src.MembershipStatus.ToDisplayName()));


            CreateMap<Tenant, GetBranchEntityDto>()
                .ForMember(dest => dest.EntityTypeName, o => o.MapFrom(src => src.FedrationEntityType.Name));

            CreateMap<Tenant, AddFederationDto>()
              .ReverseMap();


            CreateMap<Tenant, FederationDto>()
              .ReverseMap();

            CreateMap<Package, PackageSummaryDto>().ReverseMap();

        }
        private string ResolveImage(string image)
        {
            return DocumentSetting.GetPreSignedUrlAsync(image).GetAwaiter().GetResult();
        }


    }

}

