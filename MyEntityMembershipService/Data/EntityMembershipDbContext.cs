using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data.Membership;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyEntityMembershipService.Data
{
    public class EntityMembershipDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantProvider _tenantProvider;

        public EntityMembershipDbContext(DbContextOptions<EntityMembershipDbContext> options, IHttpContextAccessor httpContextAccessor, ITenantProvider tenantProvider) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenantProvider = tenantProvider;
        }


        #region Membership

        public DbSet<PaymentInvoiceType> PaymentInvoiceTypes { get; set; }
        public DbSet<Membership.Entities.Membership> Memberships { get; set; }
        public DbSet<MembershipRequest> MembershipRequests { get; set; }
        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<RequestTypeCategory> RequestTypeCategory { get; set; }
        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<RequestStatusHistory> RequestStatusHistory { get; set; }
        public DbSet<PaymentInvoice> PaymentInvoices { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<PaymentItem> PaymentItems { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<TermsOfService> TermsOfServices { get; set; }

        public DbSet<MembershipRequestComment> MembershipRequestComments { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormMemberType> FormMemberType { get; set; }

        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldType> FieldTypes { get; set; }
        public DbSet<FieldCategory> FieldCategory { get; set; }
        public DbSet<FormUserResponse> FormUserResponse { get; set; }
        public DbSet<FieldResponse> FieldResponse { get; set; }
        public DbSet<RequestTypeFields> RequestTypeFields { get; set; }
        public DbSet<FormFields> FormFields { get; set; }
        public DbSet<PackageRequestAuthourizations> PackageRequestAuthourizations { get; set; }
        public DbSet<PackageConfigurationSection> PackageConfigurationSections { get; set; }
        public DbSet<ConfigrationFieldResponse> ConfigrationFieldResponses { get; set; }
        public DbSet<PaakageMemberForm> PaakageMemberForms { get; set; }
        public DbSet<MemberType> MemberType { get; set; }
        public DbSet<DropdownValue> DropdownValue { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<ConfigurationField> ConfigurationFields { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UnitType> UnitTypes { get; set; }

        public DbSet<MembershipRequestBackgroundJobs> MembershipRequestBackgroundJobs { get; set; }

        public DbSet<FreezeConfiguration> FreezeConfigurations { get; set; }
        public DbSet<AdditionalConfiguration> AdditionalConfigurations { get; set; }
        public DbSet<SubMemberAdditionalConfiguration> SubMemberAdditionalConfigurations { get; set; }
        public DbSet<RenewalConfiguration> RenewalConfigurations { get; set; }
        public DbSet<RemovalConfiguration> RemovalConfigurations { get; set; }
        public DbSet<CancellationConfiguration> CancellationConfigurations { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<NewPaymentRequest> NewPaymentRequests { get; set; }
        public DbSet<PaymentBatch> PaymentBatches { get; set; }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<FedrationEntityType> FedrationEntityTypes { get; set; }
        public DbSet<TenantAdmin> TenantAdmins { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<AreaConfiguration> AreaConfigurations { get; set; }
        public DbSet<AreaPricing> AreaPrices { get; set; }
        public DbSet<TenantAdminArea> TenantAreaAdmins { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<TournamentResult> TournamentResults { get; set; }
        public DbSet<InjuryReport> InjuryReports { get; set; }
        public DbSet<MembershipAccess> MembershipAccesses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PackageProduct> PackageProducts { get; set; }
        public DbSet<MemberRequestType> MemberRequestTypes { get; set; }

        #endregion


        #region Entity

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<SubMemberCategory> SubMemberCategories { get; set; }
        public DbSet<AccessedBranches> AccessedBranches { get; set; }
        public DbSet<AccessedBranchesFacilities> AccessedBranchesFacilities { get; set; }
        public DbSet<AccessedFacilitiesVenues> AccessedFacilitiesVenues { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventDetail> EventDetails { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<UserChoice> UsersChoices { get; set; }
        public DbSet<OrganizationEntity> Organizations { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<NewVenue> NewVenues { get; set; }
        public DbSet<StadFacilities> StadFacilities { get; set; }
        public DbSet<AccessibilityFeatures> AccessibilityFeatures { get; set; }
        public DbSet<VenueStadFacility> VenueStadFacilities { get; set; }
        public DbSet<VenueAccessibilityFeatures> VenueAccessibilityFeatures { get; set; }
        public DbSet<VenueOperatingHours> VenueOperatingHours { get; set; }
        public DbSet<FacilityOperatingHours> FacilityOperatingHours { get; set; }
        public DbSet<FacilityType> FacilityType { get; set; }
        public DbSet<FacilityAccessibilityFeatures> FacilityAccessibilityFeatures { get; set; }
        public DbSet<FacilityAmenities> FacilityAmenities { get; set; }
        public DbSet<FacilitySportsAccommodated> FacilitySportsAccommodated { get; set; }
        public DbSet<SportsAccommodated> SportsAccommodated { get; set; }
        public DbSet<FacilityAccessibilityOptions> FacilityAccessibilityOptions { get; set; }
        public DbSet<TurfTypes> TurfTypes { get; set; }
        public DbSet<NewVenuePricingRules> NewVenuePricingRules { get; set; }
        public DbSet<NewVenueDiscountTypes> NewVenueDiscountTypes { get; set; }
        public DbSet<PackageScopeAssignment> PackageScopeAssignments { get; set; }
        public DbSet<PackageScope> PackageScopes { get; set; }


        //Newley Added DBSets:
        public DbSet<Country> Country { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<BranchLocation> BranchLocation { get; set; }
        public DbSet<BranchServices> BranchServices { get; set; }
        public DbSet<BranchOperatingHours> BranchOperatingHours { get; set; }
        public DbSet<CoreService> CoreServices { get; set; }
        public DbSet<Sports> Sports { get; set; }
        public DbSet<AccessibilityOptions> AccessibilityOptions { get; set; }
        public DbSet<CoreFacilityServices> CoreFacilityServices { get; set; }
        public DbSet<NewFacility> NewFacility { get; set; }
        public DbSet<FacilityLocation> FacilityLocation { get; set; }
        public DbSet<FacilityServices> FacilityServices { get; set; }
        public DbSet<Amenities> Amenities { get; set; }
        public DbSet<NewAccessibilityOptions> NewAccessibilityOptions { get; set; }
        public DbSet<NewFacilityAccessibilityOptions> NewFacilityAccessibilityOptions { get; set; }
        public DbSet<NewFacilitySports> NewFacilitySports { get; set; }
        public DbSet<FacilityImages> FacilityImages { get; set; }
        public DbSet<NewFacilityOperatingHours> NewFacilityOperatingHours { get; set; }
        public DbSet<NewCoreUtilities> NewCoreUtilities { get; set; }
        public DbSet<NewVenue> NewVenue { get; set; }
        public DbSet<VenueTypes> VenueTypes { get; set; }
        public DbSet<BookMark> BookMarks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CountryCodes> CountryCodes { get; set; }
        public DbSet<VenueReviews> VenueReviews { get; set; }
        public DbSet<TenantReview> TenantReviews { get; set; }
        public DbSet<NewVenueBookingConfiguration> NewVenueBookingConfiguration { get; set; }
        public DbSet<VenueSlots> VenueSlots { get; set; }
        public DbSet<VenueBookingTypes> VenueBookingTypes { get; set; }
        public DbSet<VenueBooking> VenueBooking { get; set; }
        public DbSet<VenueBookingSlot> VenueBookingSlot { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMembers> TeamMembers { get; set; }
        public DbSet<EntityType> EntityType { get; set; }
        public DbSet<EntityAdmin> EntityAdmins { get; set; }
        public DbSet<EntityMember> EntityMembers { get; set; }
        public DbSet<Belt> Belts { get; set; }
        public DbSet<FedrationBranch> FedrationBranches { get; set; }

        public DbSet<TenantAdminSport> TenantAdminSports { get; set; }
        public DbSet<TenantAdminFacility> TenantAdminFacilities { get; set; }
        public DbSet<TenantAdminBranch> TenantAdminBranches { get; set; }
        public DbSet<TenantAdminVenue> TenantAdminVenues { get; set; }

        public DbSet<PreRegisteredUser> PreRegisteredUsers { get; set; }

        public DbSet<Attainment> Attainments { get; set; }

        public DbSet<AttainmentCategory> AttainmentCategories { get; set; }
        public DbSet<MemberSportLevel> MemberSportLevels { get; set; }
        public DbSet<ParentApplicationConfiguration> ParentApplicationConfiguration { get; set; }

        #endregion


        public DbSet<AuditLog> AuditLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ConfigureSoftDelete(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            ApplyTenantFilter(modelBuilder);
        }

        private void ApplyTenantFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {

                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var tenantIdProperty = Expression.Property(parameter, nameof(BaseEntity.GlobalTenantId));

                    // Access _tenantProvider dynamically
                    var providerExpr = Expression.Constant(this, typeof(EntityMembershipDbContext));
                    var tenantProviderField = Expression.Field(providerExpr, "_tenantProvider");
                    var allowedIdsProperty = Expression.Property(tenantProviderField, nameof(ITenantProvider.AllowedTenantIds));

                    var containsMethod = typeof(List<int>).GetMethod("Contains", new[] { typeof(int) });
                    var containsCall = Expression.Call(allowedIdsProperty, containsMethod, tenantIdProperty);

                    var lambda = Expression.Lambda(containsCall, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                    ConfigureSoftDelete(modelBuilder);
                }
            }
        }

        protected void ConfigureSoftDelete(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)

                        .Property<bool>("IsDeleted");

                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var body = Expression.Equal(
                        Expression.Call(typeof(EF), nameof(EF.Property), new[] { typeof(bool) }, parameter, Expression.Constant("IsDeleted")),
                        Expression.Constant(false));

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));
                }
            }
        }

        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{


        //    var userId = _httpContextAccessor?.HttpContext?.User.GetUserId();

        //    foreach (var entry in ChangeTracker.Entries<SharedEntity>().Where(e => e.State == EntityState.Added))
        //    {
        //        entry.Entity.CreationDate = DateTime.Now;

        //        entry.Entity.CreatedBy = userId;
        //    }
        //    foreach (var entry in ChangeTracker.Entries<SharedEntity>().Where(e => e.State == EntityState.Modified))
        //    {
        //        entry.Entity.ModificationDate = DateTime.Now;

        //        entry.Entity.ModifiedBy = userId;
        //    }
        //    return base.SaveChangesAsync(cancellationToken);
        //}


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var currentUserName = _httpContextAccessor.HttpContext?.User.GetUserName();
            var auditLogs = new List<AuditLog>();
            SetGlobalTenantIdForEntries();

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                var entityName = entry.Entity.GetType().Name;
                int.TryParse(entry.Property("Id").CurrentValue.ToString(), out int entityId);


                if (entry.State == EntityState.Modified)
                {
                    // Get original and current state as objects
                    var originalEntity = entry.OriginalValues.ToObject();
                    var currentEntity = entry.CurrentValues.ToObject();

                    // Serialize to JSON
                    var originalJson = JsonSerializer.Serialize(originalEntity, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

                    });

                    var currentJson = JsonSerializer.Serialize(currentEntity, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

                    });

                    auditLogs.Add(new AuditLog
                    {
                        TableName = entityName,
                        EntityId = entityId,
                        OriginalValuesJson = originalJson,
                        CurrentValuesJson = currentJson,
                        ChangedBy = currentUserId ?? "UNKNOWN",
                        ChangedOn = DateTime.UtcNow,


                    });

                    entry.Entity.ModificationDate = DateTime.Now;

                    entry.Entity.ModifiedBy = currentUserId;
                }
                else if (entry.State == EntityState.Added)
                {
                    entry.Property(x => x.CreatedBy).CurrentValue = currentUserId;
                    entry.Property(x => x.CreationDate).CurrentValue = DateTime.UtcNow;
                    entry.Property(x => x.CreatedByUsername).CurrentValue = currentUserName;
                }
            }

            if (auditLogs.Any())
                await AuditLogs.AddRangeAsync(auditLogs, cancellationToken);

            return await base.SaveChangesAsync(cancellationToken);
        }

        private void SetGlobalTenantIdForEntries()
        {
            var tenantId = _tenantProvider.GlobalTenantId;

            if (tenantId == null)
                return; // optionally throw if required

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.GlobalTenantId = tenantId.Value;
                }
            }
        }
    }
}
