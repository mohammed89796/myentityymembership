using Microsoft.AspNetCore.Mvc;
using MyEntityMembershipService.APi.Repositories.Series_Repo;
using MyEntityMembershipService.Authorization.AuthorizationService;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Errors;
using MyEntityMembershipService.Helper.MappingProfile.Membership;
using MyEntityMembershipService.Repositories.Entity.AccessibilityOptionsRepos;
using MyEntityMembershipService.Repositories.Entity.AmenitiesRepo;
using MyEntityMembershipService.Repositories.Entity.BookMarkRepo;
using MyEntityMembershipService.Repositories.Entity.BranchLocationRepo;
using MyEntityMembershipService.Repositories.Entity.BranchMediaRepo;
using MyEntityMembershipService.Repositories.Entity.BranchOperatingHoursRepo;
using MyEntityMembershipService.Repositories.Entity.BranchRepo;
using MyEntityMembershipService.Repositories.Entity.BranchServicesRepo;
using MyEntityMembershipService.Repositories.Entity.CommentRepo;
using MyEntityMembershipService.Repositories.Entity.CoreServicesRepo;
using MyEntityMembershipService.Repositories.Entity.CoreUtilitiesRepo;
using MyEntityMembershipService.Repositories.Entity.CountryCodeRepo;
using MyEntityMembershipService.Repositories.Entity.EventDetailRepo;
using MyEntityMembershipService.Repositories.Entity.EventRepo;
using MyEntityMembershipService.Repositories.Entity.FacilityOperatingHoursRepo;
using MyEntityMembershipService.Repositories.Entity.FacilityRepo;
using MyEntityMembershipService.Repositories.Entity.FacilityServicesRepo;
using MyEntityMembershipService.Repositories.Entity.FacilityTypesRepo;
using MyEntityMembershipService.Repositories.Entity.NewVenueRepositoy;
using MyEntityMembershipService.Repositories.Entity.PackageRepo;
using MyEntityMembershipService.Repositories.Entity.PollRepo;
using MyEntityMembershipService.Repositories.Entity.PostRepo;
using MyEntityMembershipService.Repositories.Entity.SearchRepo;
using MyEntityMembershipService.Repositories.Entity.SportsRepo;
using MyEntityMembershipService.Repositories.Entity.System.CityRepo;
using MyEntityMembershipService.Repositories.Entity.System.CountryRep;
using MyEntityMembershipService.Repositories.Entity.System.FedrationBranchesRepo;
using MyEntityMembershipService.Repositories.Entity.System.FedrationEntityTypesRepo;
using MyEntityMembershipService.Repositories.Entity.System.StateRepo;
using MyEntityMembershipService.Repositories.Entity.TeamMemberRepo;
using MyEntityMembershipService.Repositories.Entity.TeamsRepo;
using MyEntityMembershipService.Repositories.Entity.TenantRepo;
using MyEntityMembershipService.Repositories.Entity.TurfTypesRepo;
using MyEntityMembershipService.Repositories.Entity.VenueBookingRepo;
using MyEntityMembershipService.Repositories.Entity.VenueBookingSlotRepo;
using MyEntityMembershipService.Repositories.Entity.VenueBookingTypesRepo;
using MyEntityMembershipService.Repositories.Entity.VenueConfigurationRepo;
using MyEntityMembershipService.Repositories.Entity.VenueOperatingHoursRepo;
using MyEntityMembershipService.Repositories.Entity.VenueRepositoy;
using MyEntityMembershipService.Repositories.Entity.VenueReviewsRepo;
using MyEntityMembershipService.Repositories.Entity.VenueTypesRepo;
using MyEntityMembershipService.Repositories.Membership.AdditionalConfigurationRepo;
using MyEntityMembershipService.Repositories.Membership.CancellationConfigurationRepository;
using MyEntityMembershipService.Repositories.Membership.ConfigurationRepo;
using MyEntityMembershipService.Repositories.Membership.FieldRepo;
using MyEntityMembershipService.Repositories.Membership.FormRepo;
using MyEntityMembershipService.Repositories.Membership.FormUserResponseRepo;
using MyEntityMembershipService.Repositories.Membership.FreezeConfigurationRepository;
using MyEntityMembershipService.Repositories.Membership.GetPackageMemberTypesRepository;
using MyEntityMembershipService.Repositories.Membership.MembershipRepo;
using MyEntityMembershipService.Repositories.Membership.MembershipRequestRepository;
using MyEntityMembershipService.Repositories.Membership.MemberTypeRepo;
using MyEntityMembershipService.Repositories.Membership.PaakageMemberFormRepo;
using MyEntityMembershipService.Repositories.Membership.PackageRequestAuthourizationsRepo;
using MyEntityMembershipService.Repositories.Membership.PaymentInvoiceRepository;
using MyEntityMembershipService.Repositories.Membership.PaymentRequest;
using MyEntityMembershipService.Repositories.Membership.RemovalConfigurationRepository;
using MyEntityMembershipService.Repositories.Membership.RequestTypeRepo;
using MyEntityMembershipService.Repositories.Membership.UnitRepo;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Entity.AccessibilityOptionsService;
using MyEntityMembershipService.Services.Entity.AmenitiesService;
using MyEntityMembershipService.Services.Entity.AtheleteService;
using MyEntityMembershipService.Services.Entity.BookMarkService;
using MyEntityMembershipService.Services.Entity.BranchService;
using MyEntityMembershipService.Services.Entity.CommentService;
using MyEntityMembershipService.Services.Entity.CoreServices;
using MyEntityMembershipService.Services.Entity.CoreUtilities;
using MyEntityMembershipService.Services.Entity.CountryCode;
using MyEntityMembershipService.Services.Entity.DashboardServ;
using MyEntityMembershipService.Services.Entity.EntityAdminService;
using MyEntityMembershipService.Services.Entity.EventDetailService;
using MyEntityMembershipService.Services.Entity.EventService;
using MyEntityMembershipService.Services.Entity.FacilityService;
using MyEntityMembershipService.Services.Entity.FacilityServicesService;
using MyEntityMembershipService.Services.Entity.FacilityTypesService;
using MyEntityMembershipService.Services.Entity.FederationService;
using MyEntityMembershipService.Services.Entity.GalleryService;
using MyEntityMembershipService.Services.Entity.PackageService;
using MyEntityMembershipService.Services.Entity.PollServices;
using MyEntityMembershipService.Services.Entity.PostService;
using MyEntityMembershipService.Services.Entity.SeriesService;
using MyEntityMembershipService.Services.Entity.SportsService;
using MyEntityMembershipService.Services.Entity.System.CityService;
using MyEntityMembershipService.Services.Entity.System.CountryService;
using MyEntityMembershipService.Services.Entity.System.EntityBranchService;
using MyEntityMembershipService.Services.Entity.System.FedrationEntityTypesService;
using MyEntityMembershipService.Services.Entity.System.StateService;
using MyEntityMembershipService.Services.Entity.Teams;
using MyEntityMembershipService.Services.Entity.TenantReviewService;
using MyEntityMembershipService.Services.Entity.TenantService;
using MyEntityMembershipService.Services.Entity.TermsOfServiceService;
using MyEntityMembershipService.Services.Entity.TurfTypesService;
using MyEntityMembershipService.Services.Entity.VenueBookingTypes;
using MyEntityMembershipService.Services.Entity.VenueConfigurationService;
using MyEntityMembershipService.Services.Entity.VenueReviewService;
using MyEntityMembershipService.Services.Entity.VenueService;
using MyEntityMembershipService.Services.Entity.VenueTypesService;
using MyEntityMembershipService.Services.Membership.AdditionalConfigurationServ;
using MyEntityMembershipService.Services.Membership.AreaServ;
using MyEntityMembershipService.Services.Membership.BulckUploadXML;
using MyEntityMembershipService.Services.Membership.CancellationConfigurationService;
using MyEntityMembershipService.Services.Membership.ConfigrationFieldResponseService;
using MyEntityMembershipService.Services.Membership.EmailServ;
using MyEntityMembershipService.Services.Membership.EntityService;
using MyEntityMembershipService.Services.Membership.FederationMemershipRequestSev;
using MyEntityMembershipService.Services.Membership.FetchService;
using MyEntityMembershipService.Services.Membership.FieldServ;
using MyEntityMembershipService.Services.Membership.FieldTypeServ;
using MyEntityMembershipService.Services.Membership.FormServ;
using MyEntityMembershipService.Services.Membership.FreezeConfigurationService;
using MyEntityMembershipService.Services.Membership.GetConfigurationService;
using MyEntityMembershipService.Services.Membership.GetPackageMemberTypesService;
using MyEntityMembershipService.Services.Membership.HistoricalTournamentService;
using MyEntityMembershipService.Services.Membership.MembershipRequestServ;
using MyEntityMembershipService.Services.Membership.MembershipRequestWebServ;
using MyEntityMembershipService.Services.Membership.MembershipServ;
using MyEntityMembershipService.Services.Membership.MemberTypesServ;
using MyEntityMembershipService.Services.Membership.NewMembershipConfigurationService;
using MyEntityMembershipService.Services.Membership.PaakageMemberFormService;
using MyEntityMembershipService.Services.Membership.PackageRequestAuthorizationService;
using MyEntityMembershipService.Services.Membership.ParentApplicationFormService;
using MyEntityMembershipService.Services.Membership.PaymentInvoiceServ;
using MyEntityMembershipService.Services.Membership.PublishService;
using MyEntityMembershipService.Services.Membership.RemovalConfigurationService;
using MyEntityMembershipService.Services.Membership.RenewalConfigurationService;
using MyEntityMembershipService.Services.Membership.RequestPaymentServ;
using MyEntityMembershipService.Services.Membership.RequestTypeServ;
using MyEntityMembershipService.Services.Membership.SearchService;
using MyEntityMembershipService.Services.Membership.UnitService;
using MyEntityMembershipService.Services.PaymentService;

namespace MyEntityMembershipService.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(typeof(MappingProfile));
            #region Membership
            services.AddScoped<IMembershipRequestWebService, MembershipRequestWebService>();
            services.AddScoped<IMembershipRequestService, MembershipRequestService>();
            services.AddScoped<IMembershipRequestRepository, MembershipRequestRepository>();
            services.AddScoped<IMembershipRepository, MembershipRepository>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IPaymentRequestService, PaymentRequestService>();
            services.AddScoped<IPaymentInvoiceService, PaymentInvoiceService>();

            services.AddScoped<IGetPackageMemberTypesRepository, GetPackageMemberTypesRepository>();
            services.AddScoped<IGetPackageMemberTypesService, GetPackageMemberTypesService>();
            services.AddScoped<IGetConfigurationService, GetConfigurationService>();

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IFederationMemershipRequestService, FederationMemershipRequestService>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAreaService, AreaService>();



            services.AddScoped<IRequestTypeService, RequestTypeService>();
            services.AddScoped<IRequestTypeService, RequestTypeService>();
            services.AddScoped<IRequestTypeRepository, RequestTypeRepository>();

            services.AddScoped<IFieldTypeService, FieldTypeService>();
            services.AddScoped<IMemberTypeService, MemberTypeService>();
            services.AddScoped<IFormService, FormService>();
            services.AddScoped<IFormRepository, FormRepository>();
            services.AddScoped<IPackageRequestAuthorizationService, PackageRequestAuthorizationService>();
            services.AddScoped<IPackageRequestAuthorizationRepository, PackageRequestAuthorizationRepository>();

            services.AddScoped<IFieldRepository, FieldRepository>();
            services.AddScoped<IFormUserResponseRepository, FormUserResponseRepository>();
            services.AddScoped<IConfigrationFieldResponseService, ConfigrationFieldResponseService>();
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
            services.AddScoped<IPaakageMemberFormService, PaakageMemberFormService>();
            services.AddScoped<IMemberTypeRepo, MemberTypeRepo>();
            services.AddScoped<IPaakageMemberFormRepo, PaakageMemberFormRepo>();
            services.AddScoped<IPaymentRequestService, PaymentRequestService>();
            services.AddScoped<IPaymentRequestRepo, PaymentRequestRepo>();
            services.AddScoped<IFieldService, FieldService>();



            services.AddScoped<IFreezeConfigurationService, FreezeConfigurationService>();
            services.AddScoped<IFreezeConfigurationRepository, FreezeConfigurationRepository>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IAdditionalConfigurationRepository, AdditionalConfigurationRepository>();
            services.AddScoped<IAdditionalConfigurationService, AdditionalConfigurationService>();



            services.AddScoped<IRemovalConfigurationService, RemovalConfigurationService>();
            services.AddScoped<IRemovalConfigurationRepository, RemovalConfigurationRepository>();
            services.AddScoped<IPaymentInvoiceRepo, PaymentInvoiceRepo>();


            services.AddScoped<ICancellationConfigurationRepository, CancellationConfigurationRepository>();
            services.AddScoped<ICancellationConfigurationService, CancellationConfigurationService>();

            services.AddScoped<INewMembershipConfigurationService, NewMembershipConfigurationService>();
            services.AddScoped<IRenewalConfigurationService, RenewalConfigurationService>();

            services.AddScoped<IExcelTemplateService, ExcelTemplateService>();

            services.AddScoped<IPublishService, PublishService>();
            services.AddScoped<IParentApplicationFormService, ParentApplicationFormService>();

            #endregion
            #region Entity
            services.AddScoped<ISeriesRepo, SeriesRepo>();
            services.AddScoped<IEventRepo, EventRepo>();
            services.AddScoped<IEventDetailRepo, EventDetailRepo>();
            services.AddScoped<ISeriesService, SeriesService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IEventDetailService, EventDetailService>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IPostRepo, PostRepo>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IBranchLocationRepository, BranchLocationRepository>();
            services.AddScoped<IBranchOperatingHoursRepository, BranchOperatingHoursRepository>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IBranchServicesRepository, BranchServicesRepository>();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IVenueService, VenueService>();
            services.AddScoped<IVenueRepository, VenueReposiory>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICountryRepo, CountryRepo>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<IStateRepo, StateRepo>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<ICityRepo, CityRepo>();
            services.AddScoped<ICoreServicesService, CoreServicesService>();
            services.AddScoped<ICoreServicesRepository, CoreServicesRepository>();
            services.AddScoped<ICoreUtilitiesService, CoreUtilitiesService>();
            services.AddScoped<ICoreUtilitiesRepository, CoreUtilitiesRepository>();
            services.AddScoped<IFacilityService, FacilityService>();
            services.AddScoped<IFacilityRepository, FacilityRepository>();
            services.AddScoped<ISportsRepository, SportsRepository>();
            services.AddScoped<ISportsService, SportsService>();
            services.AddScoped<IFacilityTypesRepository, FacilityTypesRepository>();
            services.AddScoped<IFacilityTypesService, FacilityTypesService>();
            services.AddScoped<IFacilityServicesService, FacilityServicesService>();
            services.AddScoped<IFacilityServicesRepository, FacilityServicesRepository>();
            services.AddScoped<IAmenitiesService, AmenitiesService>();
            services.AddScoped<IAmenitiesRepository, AmenitiesRepository>();
            services.AddScoped<IAccessibilityOptionsService, AccessibilityOptionsService>();
            services.AddScoped<INewVenueRepository, NewVenueReposiory>();
            services.AddScoped<IAccessibilityOptionsRepository, AccessibilityOptionsRepository>();
            services.AddScoped<IBookMarkRepository, BookMarkRepository>();
            services.AddScoped<IBookMarkService, BookMarkService>();
            services.AddScoped<IDashboardServ, DashboardServ>();
            services.AddScoped<IPollRepository, PollRepository>();
            services.AddScoped<IPollService, PollService>();

            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ITurfTypesService, TurfTypesService>();
            services.AddScoped<ITurfTypesRepository, TurfTypesRepository>();
            services.AddScoped<IVenueTypesService, VenueTypesService>();
            services.AddScoped<IVenueTypesRepository, VenueTypesRepository>();
            services.AddScoped<IBranchMediaRepository, BranchMediaRepository>();
            services.AddScoped<ICountryCodesRepository, CountryCodesRepository>();
            services.AddScoped<ICountryCodesService, CountryCodesService>();
            services.AddScoped<IFacilityOperatingHoursRepository, FacilityOperatingHoursRepository>();
            services.AddScoped<IVenueOperatingHoursRepository, VenueOperatingHoursRepository>();
            services.AddScoped<IVenueReviewsRepository, VenueReviewsRepository>();
            services.AddScoped<IVenueReviewService, VenueReviewService>();
            services.AddScoped<IVenueConfigurationService, VenueConfigurationService>();
            services.AddScoped<IVenueConfigurationRepository, VenueConfigurationRepository>();
            services.AddScoped<IVenueBookingSlotRepository, VenueBookingSlotRepository>();
            services.AddScoped<IVenueBookingRepository, VenueBookingRepository>();
            services.AddScoped<IVenueBookingTypesService, VenueBookingTypesService>();
            services.AddScoped<IVenueBookingTypesRepository, VenueBookingTypesRepository>();
            services.AddScoped<ITeamsService, TeamsService>();
            services.AddScoped<ITeamsRepository, TeamsRepository>();
            services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
            services.AddScoped<IAtheleteService, AtheleteService>();
            services.AddScoped<IFedrationBranchService, FedrationBranchService>();
            services.AddScoped<IFedrationBranchesRepo, FedrationBranchesRepo>();
            services.AddScoped<IFedrationEntityTypesService, FedrationEntityTypesService>();
            services.AddScoped<IFedrationEntityTypesRepo, FedrationEntityTypesRepo>();
            services.AddScoped<IEntityAdminService, EntityAdminService>();

            services.AddScoped<IFederationService, FederationService>();

            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<ISearchRepository, SearchRepository>();
            services.AddScoped<IGalleryService,GalleryService>();
            services.AddScoped<ITermsOfServiceService, TermsOfServiceService>();
            services.AddScoped<ITenantReviewService, TenantReviewService>();
            #endregion

            #region Polly

            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddHttpClient("ResilientClient")
               .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
               .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy())
               .AddPolicyHandler(PollyPolicies.GetTimeoutPolicy())
               .AddHttpMessageHandler<AuthorizationDelegatingHandler>();


            services.AddHttpClient("ResilientClientWithoutTimeOut")
                .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
                .AddHttpMessageHandler<AuthorizationDelegatingHandler>();
            services.AddTransient<AuthorizationDelegatingHandler>();
            #endregion

            #region ModelState
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(model => model.Value.Errors.Any())
                                                         .SelectMany(model => model.Value.Errors)
                                                         .Select(error => error.ErrorMessage).ToList();
                    var apiValidationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(apiValidationErrorResponse);
                };
            });
            #endregion

            //TODO: Delete Them
            services.AddScoped<IFetchService, FetchService>();

            services.AddScoped<IEntityService, EntityService>();
            services.AddScoped<ITenantProvider, TenantProvider>();
            services.AddScoped<IHistoricalTournamentService, HistoricalTournamentService>();


            services.AddServicesByConvention();

            return services;


        }
    }
}
