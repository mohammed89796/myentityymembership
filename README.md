# MyEntityMembershipService

A **versioned**, **multi-tenant** ASP.NET Core Web API that merges **Entity** + **Membership** domains into one service.  
It uses **JWT Bearer authentication** (authority-based), a **client_id authorization policy** for selected endpoints, **tenant scoping** via middleware + claims, and **MassTransit/RabbitMQ** consumers to sync membership fields from user events.

---

## Contents

- [Tech stack](#tech-stack)
- [API versioning & routing](#api-versioning--routing)
- [Auth & security](#auth--security)
  - [JWT Bearer (Authority-based)](#jwt-bearer-authority-based)
  - [ClientIdPolicy](#clientidpolicy)
  - [401/403 JSON responses](#401403-json-responses)
- [Multi-tenancy](#multi-tenancy)
- [Standard response envelope](#standard-response-envelope)
- [Messaging (RabbitMQ / MassTransit)](#messaging-rabbitmq--masstransit)
- [Configuration](#configuration)
- [How to run locally](#how-to-run-locally)
- [API reference](#api-reference)
  - [Entity APIs](#entity-apis)
  - [Membership APIs](#membership-apis)
- [Change notes](#change-notes)

---

## Tech stack

From your provided code/config:

- ASP.NET Core Web API
- `Asp.Versioning` (API versioning)
- JWT Bearer auth: `Microsoft.AspNetCore.Authentication.JwtBearer`
- Authorization policies
- EF Core (`EntityMembershipDbContext`, `Include`, `IgnoreQueryFilters`)
- AutoMapper
- MassTransit + RabbitMQ
- Serilog
- Elastic APM / Elastic logging settings present

---

## API versioning & routing

Most controllers expose both route patterns:

- `api/{version:apiVersion}/[controller]`
- `api/v{version:apiVersion}/[controller]`

All controllers shown are versioned as:

- `v1.0` (`[ApiVersion("1.0")]`)

So endpoints typically look like:

- `/api/v1/<Controller>/<ActionOrRoute>`

---

## Auth & security

### JWT Bearer (Authority-based)

Auth is configured via `AddAppAuthetication(...)`:

- `Authority = ApiSettings:IdentityUrl`
- `RequireHttpsMetadata = false`
- `TokenValidationParameters.ValidateAudience = false`

> Note: Your `appsettings.json` also contains a `Jwt` section with `Secret/Issuer/Audience`, but the shown auth setup validates via **Authority**, not locally via `Jwt:Secret`.

### ClientIdPolicy

A policy exists:

- **Policy name:** `ClientIdPolicy`
- **Requirement:** claim `client_id` must equal `x_inity_mobile_app_react`

Controllers decorated with `[Authorize("ClientIdPolicy")]` enforce this.

### 401/403 JSON responses

JWT bearer events return JSON bodies:

- **401:** `{ "message": "This user is not authorized" }`
- **403:** `{ "message": "Access is forbidden for this user" }`

---

## Multi-tenancy

Tenant scoping is driven by the JWT claim:

- `tenantId`

`TenantMiddleware` builds `tenantProvider.AllowedTenantIds` by:
- Always including `[0, 1]` system/global tenants
- Walking up parents via `SuperEntityId`
- Walking down children via BFS
- Using `IgnoreQueryFilters()` when reading tenants

Several controllers also override request tenant IDs using `User.GetTenantId()` / `User.FindFirst("tenantId")`.

---

## Standard response envelope

You provided:

- `Response<T>` (in `MyEntityMembershipService.Repositories.Common`)
- `GetResponse()` mapping:

```csharp
response.StatusCode switch
{
  OK => Results.Ok(response),
  Created => Results.Created("", response),
  Unauthorized => Results.Unauthorized(),
  BadRequest => Results.BadRequest(response),
  NotFound => Results.NotFound(response),
  Accepted => Results.Accepted("", response),
  UnprocessableEntity => Results.UnprocessableEntity(response),
  _ => Results.BadRequest(response)
}
```

Many controller actions return `Task<IResult>` and end with `result.GetResponse()`.

---

## Messaging (RabbitMQ / MassTransit)

### Consumers

#### `InjuryStatusUpdatedConsumer` (Consumes `InjuryStatusUpdatedEvent`)
- Looks up memberships by `(UserId, TenantId)`
- Sets `Membership.InjuryStatus` to:
  - `Injured` if `ActiveInjury == true`
  - `Uninjured` otherwise
- Retries: `3` attempts, `5s` interval

#### `UserLevelUpdatedConsumer` (Consumes `UserLevelUpdatedEvent`)
- Looks up memberships by `(UserId, TenantId)`
- Updates `Membership.BeltName = LevelOption`
- Retries: `3` attempts, `5s` interval

---

## Configuration

### appsettings.json (redacted)



Key sections:

| Key | Purpose |
|---|---|
| `ApiSettings:Gateway` | Gateway base URL |
| `ApiSettings:IdentityUrl` | Identity server authority used by JWT bearer |
| `Orchestration:OrchestrationUrl` | Orchestration route suffix |
| `ConnectionStrings:DatabaseConnection:*` | DB connection pieces (server/user/password/etc.) |
| `Jwt:*` | JWT settings present (not shown being used by bearer validation) |
| `Authorization:*` | Internal endpoints to MyAuthService for permission checks/user registration |
| `ElasticConfiguration:*` | Elastic logging config |
| `ElasticApm:*` | APM settings |
| `RabbitMq:*` | RabbitMQ host/credentials |
| `StorageConnectionString` | Storage connection string |
| `AllowedHosts` | Allowed hosts |

**Recommended:** keep secrets in environment variables / secret store and never commit them.

### launchSettings.json (redacted)

Profiles include:
- `http` → `http://localhost:5070`
- `https` → `https://localhost:7080;http://localhost:5070`
- `IIS Express` → `http://localhost:8940` + SSL port `44386`
- `Container (Dockerfile)` → ports `8080/8081`

Environment variables include DB credentials, storage keys, Paymob keys, JWT secret, etc. These must be treated as secrets and stored securely.

---

## How to run locally

1. Restore + run:
   ```bash
   dotnet restore
   dotnet run
   ```

2. Using launch profiles:
   - HTTP: `http://localhost:5070/swagger`
   - HTTPS: `https://localhost:7080/swagger`

3. Required dependencies (based on config):
   - Identity server reachable at `ApiSettings:IdentityUrl`
   - Database reachable (via `Db_*` env vars in launch profile)
   - RabbitMQ reachable if consumers are enabled/configured

---

## API reference

Conventions used below:
- **Auth**
  - `Authorize` = `[Authorize]`
  - `ClientIdPolicy` = `[Authorize("ClientIdPolicy")]`
  - `Anonymous` = no `[Authorize]` present in provided code
- **Request**
  - `Body` = JSON body
  - `Form` = `multipart/form-data`
  - `Query` = query string params
  - `Route` = URL route params

> Responses are typically `IResult` carrying a `Response<T>` envelope via `GetResponse()` unless the action returns `IActionResult`.

---

# Entity APIs

## EntityAdminController
**Base route:** `/api/v{version}/EntityAdmin`  
**Auth:** Authorize

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| POST | `AddEntityAdmin` | Adds a new entity admin or related record. | Body: `AddEntityAdminDto` | If `TenantId` not set, defaults to user `tenantId` claim |
| POST | `Set-entity-admin` | Performs the requested operation. | Body: `AddEntityAdminDto` | Calls same service method as AddEntityAdmin |
| PUT | `UpdateEntityAdmin` | Updates an existing entity admin. | Body: `UpdateEntityAdminDto` |  |
| PUT | `DeactiveEntityAdmin/{userId}` | Updates an existing entity admin. | Route: `userId` |  |
| GET | `GetEntityAdmins` | Retrieves entity admins data. | Query: `GetEntityAdminsParametersDto` | If `TenantId` not set, defaults to user `tenantId` |
| GET | `GetFederationEntitiesAdmins` | Retrieves entity admins data. | Query: `GetFederationEntitiesAdminsParametersDto` | Uses `User.GetTenantId()`; returns 400 if invalid |
| GET | `GetTenantAdmin/{userId}` | Retrieves entity admins data. | Route: `userId` |  |
| PUT | `UpdateEntityAdminStatus` | Updates an existing entity admin. | none | Uses `User.GetUserId()` |

---

## AmenitiesController
**Base route:** `/api/v{version}/Amenities`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreateAmenity` | Retrieves amenities data. | Query: `typename` |
| GET | `GetAmenitiesDropDown` | Returns a lightweight list of amenities suitable for dropdowns. | none |

---

## FacilityServicesController
**Base route:** `/api/v{version}/FacilityServices`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreateFacilityService` | Retrieves facility services data. | Query: `typename` |
| GET | `GetFacilityServicesDropDown` | Returns a lightweight list of facility services suitable for dropdowns. | none |

---

## FacilityTypesController
**Base route:** `/api/v{version}/FacilityTypes`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreateFacilityType` | Retrieves facility types data. | Query: `typename` |
| GET | `GetFacilityTypesDropDown` | Returns a lightweight list of facility types suitable for dropdowns. | none |

---

## NewFacilityController
**Base route:** `/api/v{version}/NewFacility`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| POST | `AddFacilityAsync` | Creates or updates configuration to match the provided payload (sync operation). | Form: `AddFacilityDto` |  |
| POST | `AddFacilityOperatingHours` | Creates operating hours configuration for the selected new facilitie. | Body: `FacilityOperatingHoursDto` |  |
| GET | `GetFaciityCardList` | Returns new facilities as card/list items (typically for UI lists). | Query: `pageNumber`, `branchID` |  |
| GET | `GetAllFacilities` | Retrieves all new facilities. | none |  |
| GET | `GetFacilityDetailsByIdAsync/{facilityID}` | Creates or updates configuration to match the provided payload (sync operation). | Route: `facilityID` |  |
| DELETE | `RemoveFacilityAsync/{facilityID}` | Creates or updates configuration to match the provided payload (sync operation). | Route: `facilityID` |  |
| DELETE | `RemoveFacilityMediaAsync` | Creates or updates configuration to match the provided payload (sync operation). | Query: `facilityID`, `mediaId` |  |
| POST | `EditFacilityAsync` | Creates or updates configuration to match the provided payload (sync operation). | Form: `EditFacilityDto` |  |
| GET | `GetFacilityEditDetailsByIdAsync/{facilityID}` | Retrieves data needed to populate the edit view for the selected new facilitie. | Route: `facilityID` |  |
| POST | `AddFacilityNewMedia` | Uploads and attaches new media files to the selected new facilitie. | Form: `List<IFormFile> newMedia`, Query: `facilityID` |  |
| PUT | `UpdateFacilityOperatingHoursAsync` | Updates operating hours configuration for the selected new facilitie. | Body: `EditFacilityOperatingHoursDto` |  |
| GET | `GetMobileFacilityVenues/{facilityID}` | Retrieves a mobile-optimized view related to new facilities. | Route: `facilityID` |  |
| GET | `GetMobileFacilityDetailsAsync/{facilityID}` | Retrieves a mobile-optimized view related to new facilities. | Route: `facilityID` |  |
| GET | `GetFacilitiesByBranchID/{branchID}` | Retrieves new facilities filtered by the specified parameter(s). | Route: `branchID` |  |

---

## SportsController
**Base route:** `/api/v{version}/Sports`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreateSport` | Retrieves sports data. | Query: `sportName` |
| GET | `GetSportsDropDown` | Returns a lightweight list of sports suitable for dropdowns. | none |

---

## FederationController
**Base route:** `/api/v{version}/Federation`  
**Auth:** Authorize

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| POST | `CreateFederation` | Creates a new federation. | Form: `AddFederationDto` |  |
| POST | `CreateFederation` (CreateFederationEntity) | Creates a new federation. | Form: `AddFederationEntityDto` | Route is `CreateFederation` in code; also has `Create-Federation` |
| POST | `Create-Federation` | Creates a new federation. | Form: `AddFederationDto` |  |
| GET | `GetAll` | Retrieves all federations. | Query: `tenentId`, `pageNumber?`, `pageSize?` | Uses `User.GetTenantId()` if present |
| GET | `GetFederationEntityById` | Retrieves a single federation by its identifier. | Query: `tenentId` |  |
| PUT | `Edit` | Updates an existing federation. | Form: `UpdateFederationDto` |  |
| DELETE | `Delete/{id}` | Deletes (or soft-deletes) the specified federation. | Route: `id` |  |

---

## AccessibilityOptionsController
**Base route:** `/api/v{version}/AccessibilityOptions`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreateAccessibilityOption` | Retrieves accessibility options data. | Query: `typename` |
| GET | `GetAccessibilityOptionsDropDown` | Returns a lightweight list of accessibility options suitable for dropdowns. | none |

---

## CityController
**Base route:** `/api/v{version}/City`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreateCity` | Retrieves cities data. | Query: `stateID`, `cityName` |
| GET | `GetAllCities` | Retrieves all cities. | Query: `stateID` |

---

## CountryCodesController
**Base route:** `/api/v{version}/CountryCodes`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `CreateCountryCode` | Creates a new country code. | Form: `code`, `flag (IFormFile)` |
| GET | `GetCountryCodesDropDown` | Returns a lightweight list of country codes suitable for dropdowns. | none |

---

## CountryController
**Base route:** `/api/v{version}/Country`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreateCountry` | Retrieves countries data. | Query: `countryName` |
| GET | `GetAllCountries` | Retrieves all countries. | none |

---

## FedrationBranchController
**Base route:** `/api/v{version}/FedrationBranch`  
**Auth:** Authorize

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| POST | `CreateFedrationBranch` | Creates a new fedration branche. | Query: `enName`, `arName`, `TenantId` |  |
| GET | `GetBranchDropDown` | Returns a lightweight list of fedration branches suitable for dropdowns. | Query: `languageType` | Calls service with hard-coded `2` for tenantId in code |

---

## FedrationEntityTypesController
**Base route:** `/api/v{version}/FedrationEntityTypes`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `CreateFedrationEntityType` | Creates a new fedration entity type. | Query: `name` |
| GET | `GetFedEntityTypes` | Retrieves fedration entity types data. | none |

---

## StateController
**Base route:** `/api/v{version}/State`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreateState` | Retrieves states data. | Query: `countryID`, `stateName` |
| GET | `GetAllStates` | Retrieves all states. | Query: `countryID` |

---

## TeamsController
**Base route:** `/api/v{version}/Teams`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `CreateNewTeam` | Creates a new team. | Form: `AddNewTeamDto` |
| POST | `GetTeamsList` | Retrieves teams data. | Body: `TeamsFilterDto?` |
| GET | `GetEditTeamDetails` | Retrieves teams data. | Query: `teamID` |
| PUT | `EditTeam` | Updates an existing team. | Form: `EditTeamDto` |
| DELETE | `DeleteTeamAsync/{teamId}` | Creates or updates configuration to match the provided payload (sync operation). | Route: `teamId` |
| GET | `GetTeamBasicDetails/{teamID}` | Retrieves teams data. | Route: `teamID` |
| GET | `GetTeamMembers` | Retrieves teams data. | Query: `teamID`, `pageNumber` |

---

## CoreUtilitiesController
**Base route:** `/api/v{version}/CoreUtilities`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreateCoreUtilitie` | Retrieves core utilities data. | Query: `typename` |
| GET | `GetCoreUtilitiesDropDown` | Returns a lightweight list of core utilities suitable for dropdowns. | none |

---

## TurfTypesController
**Base route:** `/api/v{version}/TurfTypes`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreatTurfType` | Retrieves turf types data. | Query: `typename` |
| GET | `GetTurfTypesDropDown` | Returns a lightweight list of turf types suitable for dropdowns. | none |

---

## VenueBookingTypesController
**Base route:** `/api/v{version}/VenueBookingTypes`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `CreateVenueBookingType` | Creates a booking record based on the provided booking payload. | Form: `name`, `description`, `Icon (IFormFile)` |
| GET | `GetVenueBookingTypesAsync` | Creates or updates configuration to match the provided payload (sync operation). | none |

---

## VenueConfigurationController
**Base route:** `/api/v{version}/VenueConfiguration`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `CreateVenueConfiguration` | Creates a new venue configuration. | Body: `AddVenueConfigurationDto` |
| POST | `GetVenueConfiguration` | Retrieves venue configurations data. | Query: `venueID` |

---

## VenueController
**Base route:** `/api/v{version}/Venue`  
**Auth:** Authorize

This controller contains both:
- direct EF Core actions returning `IActionResult`
- service-backed actions returning `IResult`

### Direct EF Core endpoints (IActionResult)

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| GET | `Get-All` | Retrieves all venues. | none | Loads venues with many includes; resolves image pre-signed URL |
| GET | `Get-AllAccessibilityFeatures` | Retrieves all venues. | none | Reads `_Context.AccessibilityFeatures` |
| GET | `Get-AllFacilities` | Retrieves all venues. | none | Reads `_Context.StadFacilities` |
| GET | `Get-AllSportsAccommodated` | Retrieves all venues. | none | Reads `_Context.SportsAccommodated` |
| GET | `Get-AllFacilityType` | Retrieves all venues. | none | Reads `_Context.FacilityType` |
| GET | `GetByPagination` | Retrieves venues filtered by the specified parameter(s). | Query: `pageNo=1`, `pageSize=10` | Returns `GetVenuePaginationDto` |
| GET | `GetOldVenueById/{id}` | Retrieves a single venue by its identifier. | Route: `id` | Reads old `Venues` set |
| GET | `GetById/{id}` | Retrieves a single venue by its identifier. | Route: `id` | Reads `NewVenue` set |
| POST | `CreateVenue` | Creates a new venue. | Body: `AddVenueDto` | Uploads image; creates mapping entities |
| PUT | `UpdateVenue` | Updates an existing venue. | Body: `UpdateVenueDto` | Updates fields + remaps feature/facility links |
| DELETE | `delete/{id}` | Deletes (or soft-deletes) the specified venue. | Route: `id` | Deletes venue + mapping rows |
| POST | `SetOpreatingHours` | Performs the requested operation. | Body: `SetVenueOperatingDto` | Adds `VenueOperatingHours` |
| PUT | `UpdateOpreatingHours` | Updates an existing venue. | Body: `UpdateVenueOperatingHoursDto` | Updates `VenueOperatingHours` |

### Service-backed endpoints (IResult)

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `IsVenuesIdForSpecificTenantValid/{tenantId}` | Performs the requested operation. | Route: `tenantId`, Body: `List<int> venusId` |
| POST | `GetVenuesByIds` | Retrieves a single venue by its identifier. | Body: `List<int> venusId` |
| POST | `GetVenuesInfoByListOfIds` | Retrieves venues filtered by the specified parameter(s). | Body: `List<int> venusId` |
| POST | `GetVenuesInfoByListOfIds` | Retrieves venues filtered by the specified parameter(s). | Body: `List<int> venusId` |
| GET | `GetVenuesByName/{venueName}` | Retrieves venues filtered by the specified parameter(s). | Route: `venueName` |
| GET | `GetVenueListByFacilityID` | Retrieves venues filtered by the specified parameter(s). | Query: `pageNumber`, `facilityID` |
| GET | `GetMobileVenueListByFacilityID` | Retrieves a mobile-optimized view related to venues. | Query: `pageNumber`, `facilityID` |
| GET | `GetVenueDetailsByIdAsync/{venueID}` | Creates or updates configuration to match the provided payload (sync operation). | Route: `venueID` |
| POST | `CreateNewVenue` | Creates a new venue. | Form: `AddNewVenueDto` |
| POST | `AddVenueOperatingHoursAsync` | Creates operating hours configuration for the selected venue. | Form: `VenueOperatingHoursDto` |
| DELETE | `RemoveVenueAsync/{venueID}` | Creates or updates configuration to match the provided payload (sync operation). | Route: `venueID` |
| DELETE | `RemoveVenueMediaAsync` | Creates or updates configuration to match the provided payload (sync operation). | Query: `venueID`, `mediaId` |
| POST | `EditVenueAsync` | Creates or updates configuration to match the provided payload (sync operation). | Form: `EditVenueDto` |
| GET | `GetVenueEditDetailsByIdAsync/{venueID}` | Retrieves data needed to populate the edit view for the selected venue. | Route: `venueID` |
| GET | `GetMobileVenueDetailsAsync/{venueID}` | Retrieves a mobile-optimized view related to venues. | Route: `venueID` |
| POST | `AddVenueNewMedia` | Uploads and attaches new media files to the selected venue. | Form: `List<IFormFile> newMedia`, Query: `venueID` |
| PUT | `UpdateVenueOperatingHoursAsync` | Updates operating hours configuration for the selected venue. | Body: `EditVenueOperatingHoursDto` |
| GET | `GetVenueBookingHeader/{venueId}` | Retrieves venue booking information based on query parameters. | Route: `venueId` |
| GET | `GetVenueOperatingHours/{venueId}` | Retrieves operating hours information for the selected venue. | Route: `venueId` |
| POST | `GetAvailableSlots` | Retrieves venues data. | Body: `GetAvailableSlotsRequestDto` |
| POST | `BookVenueForEvent` | Creates a booking record based on the provided booking payload. | Body: `EventBookingDto` |
| POST | `BookVenueOutOfService` | Creates a booking record based on the provided booking payload. | Body: `BaseBookingDto` |
| POST | `BookVenueForMember` | Creates a booking record based on the provided booking payload. | Body: `EventMemberBookingDto` |
| POST | `GetVenuesBookings` | Creates a booking record based on the provided booking payload. | Body: `GetVenueBookingsDto` |
| GET | `GetVenuesBookingsMobile/{venueID}` | Retrieves a mobile-optimized view related to venues. | Route: `venueID` |
| GET | `GetCancellationPolicyMessage/{venueID}` | Retrieves venues data. | Route: `venueID` |
| DELETE | `DeleteVenueBookingWithSlots/{id:int}` | Creates a booking record based on the provided booking payload. | Route: `id` |
| GET | `GetRescheduleMemberBookingDetails/{bookingID}` | Retrieves venue booking information based on query parameters. | Route: `bookingID` |
| GET | `GetRescheduleOutOfServiceBookingDetails/{bookingID}` | Retrieves venue booking information based on query parameters. | Route: `bookingID` |
| GET | `GetRescheduleEventBookingDetails/{bookingID}` | Retrieves venue booking information based on query parameters. | Route: `bookingID` |
| PUT | `EditBookingSlotsAsync` | Creates or updates configuration to match the provided payload (sync operation). | Body: `GeneralEditDto` |
| POST | `GetVenuesUsage` | Returns usage analytics based on the provided parameters. | Body: `VenueUsageRequestModel` |
| POST | `ValidateVenueOperatingHours` | Creates operating hours configuration for the selected venue. | Body: `List<RecurringEventDto>` |
| POST | `ValidateVenueEventOperatingHours` | Creates operating hours configuration for the selected venue. | Form: `AddEventWithVenueDto` |
| GET | `booking` | Retrieves venue booking information based on query parameters. | Query: `VenueBookingQueryParametersDto` |

---

## VenueReviewsController
**Base route:** `/api/v{version}/VenueReviews`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `CreateReview` | Creates a new venue review. | Body: `AddVenueReviewDto` |
| GET | `GetVenueReviews/{venueID}` | Retrieves venue reviews data. | Route: `venueID` |
| GET | `GetVenueReviewsDashBoard` | Retrieves venue reviews data. | Query: `venueID`, `pageNumber` |

---

## VenueTypesController
**Base route:** `/api/v{version}/VenueTypes`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreatVenueType` | Retrieves venue types data. | Query: `typename` |
| GET | `GetVenueTypesDropDown` | Returns a lightweight list of venue types suitable for dropdowns. | none |

---

## AtheletesController
**Base route:** `/api/v{version}/Atheletes`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `athelete` | Performs the requested operation. | Form: `AddAtheleteDto` |
| GET | `athelete/{tenantId}` | Retrieves atheletes data. | Route: `tenantId` |

---

## BranchController
**Base route:** `/api/v{version}/Branch`  
**Auth:** Authorize

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| GET | `GetAllBranches` | Retrieves all branches. | none |  |
| GET | `GetAllBranchesByTenant` | Retrieves branches filtered by the specified parameter(s). | none | Uses `User.GetTenantId()` |
| GET | `GetBranchById/{branchId}` | Retrieves a single branche by its identifier. | Route: `branchId` |  |
| GET | `GetAllBranchesByTenantId/{tenantId}` | Retrieves branches filtered by the specified parameter(s). | Route: `tenantId`, Query: `searchTerm?`, `pageIndex?`, `pageSize?` | Overrides `tenantId` if token tenant > 0 |
| GET | `GetBranchDetailsByIdAsync/{branchID}` | Creates or updates configuration to match the provided payload (sync operation). | Route: `branchID` |  |
| GET | `GetBranchCardList/{pageNumber}/{tenantID}` | Returns branches as card/list items (typically for UI lists). | Route shows tenantID but method uses only `pageNumber` | Uses token tenantId internally |
| POST | `AddBranch` | Adds a new branche or related record. | Form: `AddBranchDto` | Forces `TenantId` from token if > 0 |
| POST | `AddBranchOperatingHours` | Creates operating hours configuration for the selected branche. | Body: `BranchOperatingHoursDto?` |  |
| PUT | `UpdateBranch` | Updates an existing branche. | Body: `BranchDto` | Forces `TenantId` from token if > 0 |
| PUT | `EditBranch` | Updates an existing branche. | Form: `EditBranchDto` |  |
| DELETE | `RemoveBranchAsync` | Creates or updates configuration to match the provided payload (sync operation). | Query: `branchID` |  |
| DELETE | `RemoveBranchMediaAsync` | Creates or updates configuration to match the provided payload (sync operation). | Query: `branchId`, `mediaId` |  |
| GET | `GetBranchEditDetailsByIdAsync/{branchId}` | Retrieves data needed to populate the edit view for the selected branche. | Route: `branchId` |  |
| GET | `GetBranchCardsByTenantID/{pageNumber}/{tenantID}` | Returns branches as card/list items (typically for UI lists). | Route: `pageNumber`, `tenantID` | Mobile only |
| GET | `GetMobileBranchDetailsByID/{branchId}` | Retrieves a mobile-optimized view related to branches. | Route: `branchId` | Mobile only |
| GET | `GetMobileBranchFacilities/{branchId}` | Retrieves a mobile-optimized view related to branches. | Route: `branchId` | Mobile only |
| POST | `AddBranchNewMedia` | Uploads and attaches new media files to the selected branche. | Form: `List<IFormFile> newMedia`, Query: `branchID` |  |
| PUT | `UpdateBranchOperatingHoursAsync` | Updates operating hours configuration for the selected branche. | Body: `EditBranchOperatingHoursDto` |  |

---

## CommentController
**Base route:** `/api/v{version}/Comment`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `AddComment` | Adds a new comment or related record. | Body: `AddCommentDto` |
| PUT | `EditComment` | Updates an existing comment. | Query: `commentId`, `userId`, `text` |
| PUT | `DeleteComment` | Updates an existing comment. | Query: `commentId`, `userId` |
| GET | `GetCommentsOfPosts` | Query: `postId` | Returns `Response<List<CommentDto>>` directly |

---

## CoreServicesController
**Base route:** `/api/v{version}/CoreServices`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `CreateCoreService` | Retrieves core services data. | Query: `serviceName` |
| GET | `GetCoreServicesDropDown` | Returns a lightweight list of core services suitable for dropdowns. | none |

---

## DashboardController
**Base route:** `/api/v{version}/Dashboard`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetDashboardTenatSummary/{tenantId}` | Returns analytics/summary data for reporting dashboards. | Route: `tenantId` |

---

## EventController
**Base route:** `/api/v{version}/Event`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `AddEvent` | Adds a new event or related record. | Body: `EventDto` |
| PUT | `UpdateEvent/{eventId}` | Updates an existing event. | Route: `eventId`, Body: `UpdateEventDto` |
| DELETE | `DeleteEvent/{eventId}` | Deletes (or soft-deletes) the specified event. | Route: `eventId` |
| GET | `GetEvent` | Retrieves events data. | Query: `eventId` |
| GET | `GetAllEvents` | Retrieves all events. | none |
| GET | `GetAllEventsWithPagination` | Retrieves all events. | Query: `seriesId`, `pageNumber`, `pageSize` |
| POST | `FilterEventsWithPagination/{seriesId}` | Filters results based on the provided criteria. | Route: `seriesId`, Body: `FilterEventDto`, Query: `pageNumber`, `pageSize` |
| GET | `SortEventsWithPagination/{seriesId}` | Returns results sorted according to the provided sort parameters. | Route: `seriesId`, Query: `sort`, `pageNumber`, `pageSize` |

---

## EventDetailController
**Base route:** `/api/v{version}/EventDetail`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `AddEventDetail` | Adds a new event detail or related record. | Form: `EventDetailDto` |
| PUT | `UpdateEventDetail/{eventDetailsId}` | Updates an existing event detail. | Route: `eventDetailsId`, Form: `UpdateEventDetailDto` |
| DELETE | `DeleteEventDetail/{eventDetailsId}` | Deletes (or soft-deletes) the specified event detail. | Route: `eventDetailsId` |
| GET | `GetEventDetail/{eventDetailsId}` | Retrieves event details data. | Route: `eventDetailsId` |
| GET | `GetAllEventDetails` | Retrieves all event details. | none |

---

## FacilityController (Legacy / EF)
**Base route:** `/api/v{version}/Facility`  
**Auth:** Authorize  
**Response:** `IActionResult`

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `Get-All` | Retrieves all event details. | none |
| GET | `GetByPagination` | Retrieves event details filtered by the specified parameter(s). | Query: `pageNo=1`, `pageSize=10` |
| GET | `GetById/{id}` | Retrieves a single event detail by its identifier. | Route: `id` |
| PUT | `UpdateFacility` | Updates an existing event detail. | Form: `UpdateFacilityDto` |
| DELETE | `delete/{id}` | Deletes (or soft-deletes) the specified event detail. | Route: `id` |
| POST | `SetOpreatingHours` | Performs the requested operation. | Body: `SetFacilityOperatingDto` |
| PUT | `UpdateOpreatingHours` | Updates an existing event detail. | Body: `UpdateFacilityOperatingHoursDto` |

---

## OrganizationController (EF)
**Base route:** `/api/v{version}/Organization`  
**Auth:** Authorize  
**Response:** `IActionResult`

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `Get-All` | Retrieves all event details. | none |
| GET | `GetByPagination` | Retrieves event details filtered by the specified parameter(s). | Query: `pageNo=1`, `pageSize=10` |
| GET | `GetById/{id}` | Retrieves a single event detail by its identifier. | Route: `id` |
| POST | `CreateOrganization` | Creates a new event detail. | Form: `AddOrganizationDto` |
| PUT | `UpdateOrganization` | Updates an existing event detail. | Form: `UpdateOrganizationDto` |
| DELETE | `delete/{id}` | Deletes (or soft-deletes) the specified event detail. | Route: `id` |

---

## PackageController
**Base route:** `/api/v{version}/Package`  
**Auth:** Authorize

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| POST | `AddPackage` | Adds a new package or related record. | Form: `AddPackageDto` + file via `AddPackageModelBinder` | Binder returns dynamic `{ PackageDto, File }` |
| GET | `GetAllPackages` | Retrieves all packages. | Query: `tenatId` |  |
| GET | `GetPackageById/{packageId}` | Retrieves a single package by its identifier. | Route: `packageId` |  |
| GET | `GetPackageByIdAsync/{packageId}` | Creates or updates configuration to match the provided payload (sync operation). | Route: `packageId` |  |
| GET | `GetPackagesByTenantId/{tenantId}` | Retrieves packages filtered by the specified parameter(s). | Route: `tenantId` |  |
| GET | `GetAllPackagesByTenantId/{tenantId}` | Retrieves packages filtered by the specified parameter(s). | Route exists but method has no params | Uses token tenantId |
| POST | `GetPackagesByTenantIdAndListOfIds/{tenantId}` | Retrieves packages filtered by the specified parameter(s). | Route contains tenantId but method uses token tenantId, Body: `List<int>` |  |
| DELETE | `SoftDeletePackage/{packageId}` | Deletes (or soft-deletes) the specified package. | Route: `packageId` |  |
| GET | `GetSuperPackages` | Retrieves packages data. | none | Returns `Results.Ok(result)` |
| GET | `SupPackage/{superPackageId}` | Retrieves packages data. | Route: `superPackageId` |  |
| GET | `GetPackageBranchesAsync/{packageId}` | Creates or updates configuration to match the provided payload (sync operation). | Query: `packageId`, `PackageBranchListDto` |  |
| GET | `GetAccessedFacilitiesWithVenuesAsync/{packageId}` | Creates or updates configuration to match the provided payload (sync operation). | Query: `packageId`, `branchIds (List<int>)` |  |

---

## PostController
**Base route:** `/api/v{version}/Post`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `CreateAnnouncement` | Creates a new post. | Form: `AnnouncementDto` |
| POST | `CreateNewsFeed` | Creates a new post. | Form: `NewsFeedDto` |
| POST | `CreatePoll` | Creates a new post. | Form: `PollDto` |
| POST | `CreateHighlight` | Creates a new post. | Form: `HighlightDto` |
| GET | `GetAllPosts` | Retrieves all posts. | Query: `GetAllPostsWebDto` |
| GET | `GetAllMobilePosts` | Retrieves a mobile-optimized view related to posts. | Query: `GetAllMobilePostsDto` |
| POST | `LikeToggle/{postId}/{userId}` | Toggles like/unlike for the specified post and user. | Route: `postId`, `userId` |
| GET | `GetPostsOfOtherEntities/{tenantId}` | Retrieves posts data. | Route: `tenantId` |
| GET | `GetPostById/{postId}` | Retrieves a single post by its identifier. | Route: `postId` |
| POST | `SharePost/{postId}/{userId}` | Records a share action for the specified post and user. | Route: `postId`, `userId` |
| POST | `VoteOnAPoll` | Submits a vote for a poll. | Body: `VoteDto` |

---

## RequestAuthorizationController
**Base route:** `/api/v{version}/RequestAuthorization`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetAllRequestAuthorizations` | Retrieves all request authorizations. | none |
| POST | `AddRequestAuthorization` | Adds a new request authorization or related record. | Body: `RequestAuthorizationDto` |

---

## SeriesController
**Base route:** `/api/v{version}/Series`  
**Auth:** ClientIdPolicy

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `AddSeries` | Adds a new serie or related record. | Form: `SeriesDto` |
| PUT | `UpdateSeries/{seriesId}` | Updates an existing serie. | Route: `seriesId`, Form: `UpdateSeriesDto` |
| DELETE | `DeleteSeries/{seriesId}` | Deletes (or soft-deletes) the specified serie. | Route: `seriesId` |
| GET | `GetSeries/{seriesId}` | Retrieves series data. | Route: `seriesId` |
| GET | `GetAllSeries` | Retrieves all series. | Query: `search?` |
| POST | `FilterSeries` | Filters results based on the provided criteria. | Body: `FilterSeriesDto` |
| GET | `SortSeries` | Returns results sorted according to the provided sort parameters. | Query: `sort` |

---

## TenantController
**Base route:** `/api/v{version}/Tenant`  
**Auth:** Authorize

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| POST | `AddTenant` | Adds a new tenant or related record. | Body: `AddTenantDto` |  |
| GET | `GetAllTenants` | Retrieves all tenants. | Query: `searchName?` |  |
| GET | `GetAllFederations` | Retrieves all tenants. | Query: `searchName?` |  |
| POST | `GetTenantsByIds` | Retrieves a single tenant by its identifier. | Body: `List<int> tenantIds` |  |
| POST | `GetFedByIdAsync` | Creates or updates configuration to match the provided payload (sync operation). | Query: `fedId` |  |
| GET | `GetTenantById/{tenantId}` | Retrieves a single tenant by its identifier. | Route: `tenantId` |  |
| PUT | `UpdateTenant` | Updates an existing tenant. | Form: `UpdateTenantDto` |  |
| GET | `CheckTanantTypeById` | Retrieves tenants data. | Query: `tanantId` |  |
| GET | `GetFederationEntities` | Retrieves tenants data. | none | Uses `tenantId` claim |
| GET | `GetTenantDetails` | Retrieves tenants data. | Query: `tanantId` |  |
| GET | `GetEntitiesByArea` | Retrieves tenants filtered by the specified parameter(s). | Query: `GetEntitiesByAreaParametersDto` | Defaults `FederationId` to claim |
| GET | `GetAreaStatistics` | Retrieves tenants data. | Query: `AreaId` |  |

---

# Membership APIs

## AdditionalConfigurationController
**Base route:** `/api/v{version}/AdditionalConfiguration`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetAllAdditionalConfigurations` | Retrieves all additional configurations. | none |
| GET | `GetAdditionalConfigurationById/{id}` | Retrieves a single additional configuration by its identifier. | Route: `id` |
| GET | `GetAdditionalConfigurationByPackageId/{packageId}` | Retrieves additional configurations filtered by the specified parameter(s). | Route: `packageId` |
| POST | `SyncAdditionalConfiguration` | Creates or updates configuration to match the provided payload (sync operation). | Query: `packageId`, Body: `AdditionalConfigurationDto` |
| PUT | `UpdateAdditionalConfiguration` | Updates an existing additional configuration. | Body: `AdditionalConfigurationDto` |
| DELETE | `DeleteAdditionalConfiguration/{id}` | Deletes (or soft-deletes) the specified additional configuration. | Route: `id` |

---

## AreaController
**Base route:** `/api/v{version}/Area`  
**Auth:** Authorize

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| POST | `AddArea` | Adds a new area or related record. | Body: `AddAreaDto` | Defaults `FederationId` from claim |
| GET | `GetAllAreas` | Retrieves all areas. | Query: `PageIndex`, `PageSize`, `SearchBy?` | Uses token `tenantId` as FederationId |
| GET | `GetAreaWithBankAccounts` | Retrieves areas data. | Query: `AreaId` |  |
| PUT | `UpdateArea` | Updates an existing area. | Body: `AreaWithAccountsDto` |  |
| GET | `GetAllAreaPricing` | Retrieves all areas. | Query: `GetAllAreaPricingParametersDto` |  |
| PUT | `UpdateAreaPricing` | Updates an existing area. | Body: `UpdateAreaPricingDto` |  |
| GET | `GetAreaConfigurations` | Retrieves areas data. | Query: `AreaId` |  |
| POST | `AddAreaConfiguration` | Adds a new area or related record. | Body: `AddAreaConfigurationDto` |  |
| PUT | `UpdateAreaConfiguration` | Updates an existing area. | Body: `UpdateAreaConfigurationDto` |  |
| DELETE | `DeleteAreaConfiguration` | Deletes (or soft-deletes) the specified area. | Query: `Id` |  |
| POST | `AddAreaBankAccount` | Adds a new area or related record. | Body: `AddAreaBankAccountDto` |  |
| GET | `GetAllFederationAreas` | Retrieves all areas. | Query: `FederationId?` | Defaults FederationId from claim; uses `User.GetUserId()` |
| GET | `GetAllFederationAreasByEntityId` | Retrieves areas filtered by the specified parameter(s). | none | Uses claim tenantId as entityId |
| GET | `GetEntitiesByAreaWithoutPagination` | Retrieves areas filtered by the specified parameter(s). | Query: `areaId` | Method name duplicated in code; route is `GetEntitiesByAreaWithoutPagination` |
| GET | `GetEntitiesByArea` | Retrieves areas filtered by the specified parameter(s). | Query: `GetEntitiesByAreaParametersDto` | Defaults FederationId from claim |

---

## CancellationConfigurationController
**Base route:** `/api/v{version}/CancellationConfiguration`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetCancellationConfigurationByPackageId/{packageId:int}` | Retrieves cancellation configurations filtered by the specified parameter(s). | Route: `packageId` |
| POST | `SyncCancellationConfiguration` | Creates or updates configuration to match the provided payload (sync operation). | Body: `CancellationConfigurationSyncDto` |
| DELETE | `DeleteCancellationConfiguration` | Deletes (or soft-deletes) the specified cancellation configuration. | Query: `id` |

---

## ConfigrationFieldResponseController
**Base route:** `/api/v{version}/ConfigrationFieldResponse`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `AddConfigrationFieldResponse` | Adds a new configration field response or related record. | Body: `List<ConfigrationFieldResponseDto>` |
| PUT | `UpdateConfigrationFieldResponse` | Updates an existing configration field response. | Body: `ConfigrationFieldResponseDto` |
| DELETE | `DeleteConfigrationFieldResponse/{id}` | Deletes (or soft-deletes) the specified configration field response. | Route: `id` |
| GET | `GetAllConfigrationFieldResponses` | Retrieves all configration field responses. | none |
| GET | `GetConfigrationFieldResponseById/{id}` | Retrieves a single configration field response by its identifier. | Route: `id` |
| PUT | `sync/{packageId}` | Creates or updates configuration to match the provided payload (sync operation). | Route: `packageId`, Body: `List<ConfigrationFieldResponseDto>` |
| GET | `GetConfigurationFieldResponses` | Retrieves configration field responses data. | Query: `requestTypeId`, `packageID` |

---

## FederationEntityController
**Base route:** `/api/v{version}/FederationEntity`  
**Auth:** Authorize

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| POST | `createfederationeentity` | Creates a new federation entitie. | Body: `EntityDto` |  |
| GET | `getallentities` | Retrieves all federation entities. | Query: `GetFederationEntitiesParameterDto` | Fills `userId` and overrides `tenentId` from token |
| GET | `getfederationentityById` | Retrieves a single federation entitie by its identifier. | Query: `tenentId` |  |
| GET | `GetEntityCounts` | Retrieves federation entities data. | none | Uses token tenantId |
| PUT | `editentity/{id}` | Updates an existing federation entitie. | Form: `UpdateEntityDto` | Route contains `{id}` but action param is only dto |
| DELETE | `delete/{id}` | Deletes (or soft-deletes) the specified federation entitie. | Route: `id` |  |

---

## FederationRequestsController
**Base route:** `/api/v{version}/FederationRequests`  
**Auth:** Authorize

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| POST | `AddAtheleteFormResponse` | Adds a new federation request or related record. | Form: `AddAtheleteFormResponseDto` |  |
| POST | `AddTransferFormResponse` | Adds a new federation request or related record. | Form: `AddAtheleteFormResponseDto` | Uses requestTypeId `19` internally |
| POST | `AddLoanferFormResponse` | Adds a new federation request or related record. | Form: `AddAtheleteFormResponseDto` | Uses requestTypeId `20` internally |
| POST | `AddAthletesFromExcel` | Adds a new federation request or related record. | Form: `ExcelAtheleteRowDto` |  |
| POST | `AddFormSubmission` | Adds a new federation request or related record. | Form: `FormSubmissionDto` |  |
| GET | `ValidateAthleteMembership` | Validates the provided payload against service rules and constraints. | Query: `areaId`, `entityId`, `fileNumber`, `nationalId` |  |
| PUT | `update-form-response/{requestId}` | Updates an existing federation request. | Route: `requestId`, Form: `List<AtheleteFieldResponseDto>`, `PackageMembershipRequestAccessDto?` |  |

---

## FieldsController
**Base route:** `/api/v{version}/Fields`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetAllFields` | Retrieves all fields. | none |

---

## FieldTypeController
**Base route:** `/api/v{version}/FieldType`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetAllTypes` | Retrieves all field types. | none |
| POST | `AddFieldType` | Adds a new field type or related record. | Body: `FieldTypeDto` |

---

## FormController
**Base route:** `/api/v{version}/Form`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetRequestTypeFields` | Retrieves forms data. | Query: `requestTypeId` |
| POST | `CreateUpdateForm` | Creates a new form. | Body: `CreateUpdateFormDto` |
| GET | `GetForm` | Retrieves forms data. | Query: `id`, `packageId?`, `memberTypeId?` |
| GET | `GetDraftForm` | Retrieves forms data. | Query: `id`, `packageId?` |
| GET | `GetFormsCreatedBy` | Retrieves forms filtered by the specified parameter(s). | Query: `userId`, `requestTypeId`, `title?` |
| GET | `GetRequestTypesGroupedByCategories` | Retrieves forms filtered by the specified parameter(s). | Query: `userId` |
| DELETE | `DeleteForm` | Deletes (or soft-deletes) the specified form. | Query: `id` |
| POST | `AddFormUserResponse` | Adds a new form or related record. | Body: `FormUserResponseDto` |
| GET | `GetFormByRequestTypeMemberTypeId` | Retrieves forms filtered by the specified parameter(s). | Query: `memberTypeId`, `requestTypeId` |
| POST | `UploadFile` | Performs the requested operation. | Form: `UploadFileDto` (`file`) |
| GET | `GetFormByPackgeAndRequestTypeAndMemberTypeId` | Retrieves forms filtered by the specified parameter(s). | Query: `packageId`, `requestTypeId` |
| GET | `GetConfiguration` | Retrieves forms data. | Query: `packageId`, `requestTypeId` |
| GET | `GetNewRegistrationForms` | Retrieves forms data. | none |
| GET | `GetNewRegistrationFormById/{Id}` | Retrieves a single form by its identifier. | Route: `Id` |
| GET | `CheckCanSelectPackage` | Retrieves forms data. | Query: `userId`, `packageId` |
| GET | `GetFreezCard` | Retrieves forms data. | Query: `membershipId` |
| POST | `CheckExceedWithAdditionalPayment` | Adds a new form or related record. | Body: `FormUserResponseDto` |

---

## FreezeConfigurationController
**Base route:** `/api/v{version}/FreezeConfiguration`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetFreezeConfigurationByPackageId/{packageId:int}` | Retrieves freeze configurations filtered by the specified parameter(s). | Route: `packageId` |
| POST | `SyncFreezeConfiguration` | Creates or updates configuration to match the provided payload (sync operation). | Body: `FreezeConfigurationSyncDto` |
| DELETE | `DeleteFreezeConfiguration` | Deletes (or soft-deletes) the specified freeze configuration. | Query: `id` |

---

## GenerateTempleteController
**Base route:** `/api/v{version}/GenerateTemplete`  
**Auth:** Authorize  
**Notes:** Uses `IExcelTemplateService` via `[FromServices]`. Several endpoints upload/download Excel files.

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `Download-Entity-Template-Excel` | Downloads an Excel template file used for bulk upload/import. | none |
| POST | `validate-entity-template` | Validates the uploaded Excel template file before import. | Form: `file (IFormFile)` |
| POST | `upload-entity-template` | Uploads an Excel template file and imports its contents. | Form: `file (IFormFile)` |
| GET | `Download-Branch-Entity-Template-Excel` | Downloads an Excel template file used for bulk upload/import. | none |
| POST | `validate-branch-entity-template` | Validates the uploaded Excel template file before import. | Form: `file (IFormFile)` |
| POST | `upload-branch-entities` | Performs the requested operation. | Form: `file (IFormFile)`, Query: `fedrationBranchId` |
| GET | `Download-New-Memebers-Template-Excel` | Downloads an Excel template file used for bulk upload/import. | none |
| POST | `validate-New-Members-template` | Validates the uploaded Excel template file before import. | Form: `file (IFormFile)` |
| POST | `validate-Members-template` | Validates the uploaded Excel template file before import. | Form: `file (IFormFile)` |
| POST | `upload-Memberships` | Performs the requested operation. | Form: `file (IFormFile)` |

---

## GetConfigurationController
**Base route:** `/api/v{version}/GetConfiguration`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetConfigurationByPackageId/{packageId:int}/{requestTypesId:int}` | Retrieves get configurations filtered by the specified parameter(s). | Route: `packageId`, `requestTypesId`, Query: `subMemberId?` |

---

## GetPackageMemberTypesController
**Base route:** `/api/v{version}/GetPackageMemberTypes`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetPackageMemberTypesIdsByPackageId/{packageId:int}` | Retrieves get package member types filtered by the specified parameter(s). | Route: `packageId` |
| GET | `GetMemberTypeNamesByPackageId/{packageId:int}` | Retrieves get package member types filtered by the specified parameter(s). | Route: `packageId` |

---

## HistoricalTournamentController
**Base route:** `/api/v{version}/HistoricalTournament`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `Create` | Creates a new historical tournament. | Body: `CreateHistoricalTournamentDto` |
| GET | *(root)* | Retrieves historical tournaments data. | Query: `GetHistoricalTournamentParams` |
| GET | `GetAllWithoutPagination` | Retrieves all historical tournaments. | none |
| GET | `{id}` | Retrieves historical tournaments data. | Route: `id` |
| PUT | `{id}` | Updates an existing historical tournament. | Route: `id`, Body: `HistoricalTournamentUpdateDto` |
| GET | `TournamentResults/{id}` | Retrieves historical tournaments data. | Route: `id`, Query: `GetAllTournamentResultsParams` |
| DELETE | `TournamentResult/{id}` | Deletes (or soft-deletes) the specified historical tournament. | Route: `id` |
| GET | `GetAllHistoricalTournamentResults` | Retrieves all historical tournaments. | Query: `GetAllHistoricalTournamentResultsParams` |
| GET | `GetHistoricalResultsByMembership` | Retrieves historical tournaments filtered by the specified parameter(s). | Query: `HistoricalTournamentResultByMemberParams` |
| POST | `AddResult` | Adds a new historical tournament or related record. | Body: `AddHistoricalTournamentResultDto` |
| GET | `GetResultById/{id}` | Retrieves a single historical tournament by its identifier. | Route: `id` |
| DELETE | `{id}` | Deletes (or soft-deletes) the specified historical tournament. | Route: `id` (soft delete) |

---

## MembershipController
**Base route:** `/api/v{version}/Membership`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `CreateMembership` | Creates a new membership. | Body: `MembershipDto` |
| POST | `CreateSumMemberMembership` | Creates a new membership. | Body: `subMemberMembershipDto` |
| GET | `GeMembershipsByPackageId` | Retrieves memberships data. | Query: `packageId` |
| GET | `GeMembershipsByTenantId/{tenantId}` | Retrieves memberships data. | Route: `tenantId` |
| GET | `GetMembershipsWithPackage/{year}/{tenantId}` | Retrieves memberships data. | Route: `year`, `tenantId` |
| GET | `GetMembershipGrowthRateSummary/{year}/{tenantId}` | Returns analytics/summary data for reporting dashboards. | Route: `year`, `tenantId` |
| GET | `GetMembershipsSegmentationByPackage/{year}/{tenantId}` | Returns analytics/summary data for reporting dashboards. | Route: `year`, `tenantId` |
| GET | `GetTenantMembers` | Retrieves memberships data. | Query: `GetTenantMembersParamDto` |
| GET | `GetFederationMembers` | Retrieves memberships data. | Query: `GetFederationMembersParamDto` |
| GET | `GetBelts` | Retrieves memberships data. | none |
| GET | `GetSwimmingMembers` | Retrieves memberships data. | Query: `GetSwimmingMembersParamDto` |
| GET | `GetMembershipById` | Retrieves a single membership by its identifier. | Query: `membershipId` |
| GET | `SearchMemberships` | Searches based on the provided criteria. | Query: `searchTerm?` |
| GET | `GetAllUserMemberships` | Retrieves all memberships. | Query: `ChildrenIds (List<string>)` |
| POST | `CreateSwimmingMembership` | Creates a new membership. | Form: `CreateMembershipDto` |
| PUT | `UpdateSwimmingMembership` | Updates an existing membership. | Form: `UpdateMembershipDto` |
| DELETE | `DeleteSwimmingMembership` | Deletes (or soft-deletes) the specified membership. | Query: `membershipId` |
| DELETE | `UnSoftDeleteSwimmingMembership` | Deletes (or soft-deletes) the specified membership. | Query: `membershipId` |

---

## MembershipRequestController
**Base route:** `/api/v{version}/MembershipRequest`  
**Auth:** Authorize

This controller is large; below is the endpoint list as provided.

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| GET | `GetAllTenantRequestsGrouped` | Retrieves all membership requests. | Query: `tenantId` |  |
| POST | `CreateMembershipRequest` | Creates a new membership request. | Form: `CreateMembershipRequestDto` |  |
| POST | `AddSubMember` | Adds a new membership request or related record. | Form: `SubMemberDto` |  |
| POST | `AddMembershipRequest` | Adds a new membership request or related record. | Body: `AddMembershipRequestDto` |  |
| POST | `AddMembershipWaiverRequest` | Adds a new membership request or related record. | Body: `AddMembershipWaiverRequestDto` |  |
| POST | `AddMembershipSeparationRequest` | Adds a new membership request or related record. | Body: `AddMembershipSeparationRequestDto` |  |
| POST | `AddMembershipModificationRequest` | Adds a new membership request or related record. | Body: `AddMembershipModificationRequestDto` |  |
| POST | `GetAllMembershipRequestsByTenantId` | Retrieves membership requests filtered by the specified parameter(s). | Body: `NewMembershipRequestsDto?`, Query: `tenantId`, `pageNumber`, `pageSize` |  |
| POST | `GetAllPendingPaymentRequests` | Retrieves all membership requests. | Body: `PendingPaymentRequestDto?`, Query: `tenantId`, `pageNumber`, `pageSize` |  |
| GET | `GetCurrentUserMemberships` | Retrieves membership requests data. | Query: `userId`, `name?` |  |
| GET | `GetAuthourizationDataByPackage` | Retrieves membership requests filtered by the specified parameter(s). | Query: `packageId` |  |
| GET | `GetCurrentUserMembershipRequests` | Retrieves membership requests data. | Query: `userId`, `name?` |  |
| GET | `GetMembershipRequestById` | Retrieves a single membership request by its identifier. | Query: `tenantId`, `requestId` |  |
| PUT | `Aprove-update-status-PendingPayment` | Approves the request and performs the related state transition(s). | Query: `tenantId`, `requestId` | Sets status to `PendingPayment` |
| PUT | `Aprove-update-status-RequestResolved` | Approves the request and performs the related state transition(s). | Query: `tenantId`, `requestId` | Sets status to `RequestResolved` and updates membership |
| PUT | `update-status-Reject-Request` | Rejects the request and records the rejection reason/details. | Query: `tenantId`, `requestId` | Sets status to `Rejected` |
| PUT | `rejectrequest` | Rejects the request and records the rejection reason/details. | Query: `requestId`, `rejectionReason` | Uses enum `RejectionReason` |
| PUT | `Aprove-update-status-PaymentComplete` | Approves the request and performs the related state transition(s). | Body: `UpdateRequestStatusWithComment` | Sets status to `PaymentComplete` |
| PUT | `update-status-Reject` | Rejects the request and records the rejection reason/details. | Body: `UpdateRequestStatusWithComment` | Sets status to `Rejected` |
| PUT | `submit-comments-And-justfications` | Updates an existing membership request. | Body: `SubmitCommentsAndJustficationsDto` |  |
| GET | `sort-membership-requests` | Returns results sorted according to the provided sort parameters. | Query: `sortBy`, `order`, `pageNumber`, `pageSize` |  |
| GET | `FilterByRequestSatus` | Filters results based on the provided criteria. | Query: `tenantId`, `requestStatus`, `pageNumber`, `pageSize` |  |
| GET | `search-membership-requests` | Searches based on the provided criteria. | Query: `searchTerm`, `pageNumber`, `pageSize` |  |
| GET | `GetMembershipRequest` | Retrieves membership requests data. | Query: `Id` |  |
| GET | `GetMembership` | Retrieves membership requests data. | Query: `Id` |  |
| GET | `GetMembershipRequestActivity` | Retrieves membership requests data. | Query: `Id` |  |
| PUT | `SetSubmittedStatus` | Updates an existing membership request. | Query: `Id` | Sets pending status |
| PUT | `UpdateParentWithSubmembersRequestStatus` | Updates an existing membership request. | Query: `tenantId`, `requestId` |  |
| GET | `GeMembershipRequestssByPackageId` | Retrieves membership requests data. | Query: `packageId` |  |
| GET | `GetRevenuesPerMembershipsPackage/{tenantId}/{month}/{year}` | Returns analytics/summary data for reporting dashboards. | Route: `tenantId`, `month`, `year` |  |
| POST | `GetFedEntityRequests/{tenantID}/{typeId}` | Retrieves membership requests data. | Route: `tenantID`, `typeId`, Body: `FedEntityRequestFilterDto` |  |
| POST | `GetFedRequests/{tenantID}/{typeId}` | Retrieves membership requests data. | Route: `tenantID`, `typeId`, Body: `FedRequestFilterDto` |  |
| POST | `GetFedEntityRequestsByBatchID/{batchID}/{typeId}` | Retrieves membership requests filtered by the specified parameter(s). | Route: `batchID`, `typeId`, Body: `FedEntityRequestByBatchFilterDto` |  |
| POST | `GetFedEntityBatchList/{tenantID}/{typeId}` | Retrieves membership requests data. | Route: `tenantID`, `typeId`, Body: `FedEntityBatchFilterDto` |  |
| POST | `GetFedEntityDraftBatchList/{tenantID}/{typeId}` | Retrieves membership requests data. | Route: `tenantID`, `typeId`, Body: `FedEntityBatchFilterDto` |  |
| POST | `GetFedBatchList/{tenantID}/{typeId}` | Retrieves membership requests data. | Route: `tenantID`, `typeId`, Body: `FedBatchFilterDto` |  |
| GET | `GetMembershipRequestDetails/{requestId}` | Retrieves membership requests data. | Route: `requestId` |  |
| GET | `GetRequestDetailsByBatchID/{batchId}` | Retrieves membership requests filtered by the specified parameter(s). | Route: `batchId` |  |
| GET | `GetBatchDetailedView/{batchId}` | Retrieves membership requests data. | Route: `batchId` |  |
| GET | `GetFedEntityMembershipAnalytics/{tenantId}/{typeId}` | Returns analytics/summary data for reporting dashboards. | Route: `tenantId`, `typeId` |  |
| GET | `GetFedMembershipAnalytics/{tenantId}/{typeId}` | Returns analytics/summary data for reporting dashboards. | Route: `tenantId`, `typeId` |  |
| GET | `GetBatchAnalytics/{batchId}/{typeId}` | Returns analytics/summary data for reporting dashboards. | Route: `batchId`, `typeId` |  |
| PUT | `ApproveRequestStatus` | Approves the request and performs the related state transition(s). | Query: `requestId` | Approve & transfer/loan membership |
| PUT | `ApproveRequestAndCreateMembership` | Approves the request and performs the related state transition(s). | Query: `requestId` |  |
| PUT | `ApproveRequestAndCreateMembershipWithDates` | Approves the request and performs the related state transition(s). | Body: `ApproveMembershipRequestDto` |  |
| GET | `GetConfirmedMemberships/{entityId}` | Retrieves membership requests data. | Route: `entityId`, Query: `pageIndex=1`, `pageSize=10` |  |
| GET | `GetEntityMembershipAnalytics/{tenantId}` | Returns analytics/summary data for reporting dashboards. | Route: `tenantId` |  |
| DELETE | `DeleteRequest/{requestId}` | Deletes (or soft-deletes) the specified membership request. | Route: `requestId` | Soft delete |
| GET | `GetMembershipRequestById/{requestId}` | Retrieves a single membership request by its identifier. | Route: `requestId` |  |
| GET | `GetSwimmingCurrentUserMembershipRequests` | Retrieves membership requests data. | none | Uses internal current user context |
| GET | `GetCurrentUserMembershipRequestsWithChildren` | Retrieves membership requests data. | Query: `requestId`, `childRequestId?` |  |
| GET | `GetCurrentUserMembershipRequestsWithChildrenByBatch` | Retrieves membership requests filtered by the specified parameter(s). | Query: `requestId` |  |
| GET | `GetAllBatches` | Retrieves all membership requests. | Query: `GetAllBatchesParamDto` |  |
| GET | `GetAllChildRequests` | Retrieves all membership requests. | Query: `GetAllChildRequestParametersDto` |  |
| GET | `GetAllChildRequestsByBatch` | Retrieves membership requests filtered by the specified parameter(s). | Query: `GetAllChildRequestParametersDto`, `batchId` |  |
| PUT | `SetBatchRequestsToPending` | Updates an existing membership request. | Query: `batchId` |  |

---

## MemberTypeController
**Base route:** `/api/v{version}/MemberType`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetAllTypes` | Retrieves all member types. | Query: `Name?`, `requestTypeId?` |
| POST | `AddMemberType` | Adds a new member type or related record. | Body: `MemberTypeDto` |
| GET | `GetMemberTypeById` | Retrieves a single member type by its identifier. | Query: `id` |
| GET | `GetAllTypeExceptMain` | Retrieves all member types. | none |

---

## NewMembershipConfigurationController
**Base route:** `/api/v{version}/NewMembershipConfiguration`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetAllConfigurations` | Retrieves all new membership configurations. | none |
| GET | `GetConfigurationById/{id}` | Retrieves a single new membership configuration by its identifier. | Route: `id` |
| GET | `GetConfigurationByPackageId/{packageId}` | Retrieves new membership configurations filtered by the specified parameter(s). | Route: `packageId` |
| POST | `CreateConfiguration` | Creates a new new membership configuration. | Body: `NewMembershipConfigurationDto` |
| PUT | `UpdateConfiguration/{id}` | Updates an existing new membership configuration. | Route: `id`, Body: `NewMembershipConfigurationDto` |
| DELETE | `DeleteConfiguration/{id}` | Deletes (or soft-deletes) the specified new membership configuration. | Route: `id` |
| POST | `SyncConfiguration/{packageId}` | Creates or updates configuration to match the provided payload (sync operation). | Route: `packageId`, Body: `NewMembershipConfigurationDto` |

---

## PaakageMemberFormController
**Base route:** `/api/v{version}/PaakageMemberForm`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `AddPaakageMemberForm` | Adds a new paakage member form or related record. | Body: `List<PaakageMemberFormDto>` |
| PUT | `UpdatePaakageMemberForm` | Updates an existing paakage member form. | Body: `PaakageMemberFormDto` |
| DELETE | `DeletePaakageMemberForm/{id}` | Deletes (or soft-deletes) the specified paakage member form. | Route: `id` |
| GET | `GetAllPaakageMemberForms` | Retrieves all paakage member forms. | none |
| GET | `GetPaakageMemberFormById/{id}` | Retrieves a single paakage member form by its identifier. | Route: `id` |
| PUT | `SyncPaakageMemberForms/{packageId}` | Creates or updates configuration to match the provided payload (sync operation). | Route: `packageId`, Body: `List<PaakageMemberFormDto>` |

---

## PackageRequestAuthorizationController
**Base route:** `/api/v{version}/PackageRequestAuthorization`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| POST | `AddPackageRequestAuthorization` | Adds a new package request authorization or related record. | Body: `PackageRequestAuthorizationDto` |
| POST | `AddPackageRequestAuthorizations` | Adds a new package request authorization or related record. | Body: `List<PackageRequestAuthorizationDto>` |
| GET | `GetAllPackageRequestAuthorizations` | Retrieves all package request authorizations. | none |
| GET | `GetPackageRequestAuthorizationById` | Retrieves a single package request authorization by its identifier. | Query: `id` |
| PUT | `UpdatePackageRequestAuthorization` | Updates an existing package request authorization. | Body: `PackageRequestAuthorizationDto` |
| DELETE | `DeletePackageRequestAuthorization` | Deletes (or soft-deletes) the specified package request authorization. | Query: `id` |

---

## PreRegisteredUsersController
**Base route:** `/api/v{version}/PreRegisteredUsers`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `pre-registered-users` | Retrieves pre registered users data. | Query: `fileNumber` |

---

## RemovalConfigurationController
**Base route:** `/api/v{version}/RemovalConfiguration`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetRemovalConfigurationByPackageId/{packageId:int}` | Retrieves removal configurations filtered by the specified parameter(s). | Route: `packageId` |
| POST | `SyncRemovalConfiguration` | Creates or updates configuration to match the provided payload (sync operation). | Body: `RemovalConfigurationSyncDto` |
| DELETE | `DeleteRemovalConfiguration` | Deletes (or soft-deletes) the specified removal configuration. | Query: `id` |

---

## RenewalConfigurationController
**Base route:** `/api/v{version}/RenewalConfiguration`  
**Auth:** Authorize

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| GET | `GetAllRenewalConfigurations` | Retrieves all renewal configurations. | none |  |
| GET | `GetRenewalConfigurationById/{id}` | Retrieves a single renewal configuration by its identifier. | Route: `id` |  |
| GET | `GetRenewalConfigurationByPackageId/{packageId}` | Retrieves renewal configurations filtered by the specified parameter(s). | Route: `packageId` |  |
| POST | `SyncRenewalConfiguration` | Creates or updates configuration to match the provided payload (sync operation). | Query: `packageId`, Body: `RenewalConfigurationDto` |  |
| PUT | `UpdateRenewalConfiguration` | Updates an existing renewal configuration. | Query: `id`, Body: `RenewalConfigurationDto` |  |
| DELETE | `DeleteRenewalConfiguration/{id}` | Deletes (or soft-deletes) the specified renewal configuration. | Route: `id` |  |

---

## RequestTypeController
**Base route:** `/api/v{version}/RequestType`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetAllRequestTypes` | Retrieves all request types. | none |
| GET | `GetRequestTypeById` | Retrieves a single request type by its identifier. | Query: `Id` |
| PUT | `UpdateRequestType` | Updates an existing request type. | Body: `RequestTypeDto` |
| POST | `AddRequestType` | Adds a new request type or related record. | Body: `RequestTypeDto` |
| DELETE | `DeleteRequestType` | Deletes (or soft-deletes) the specified request type. | Query: `Id` |
| GET | `GetAllTypeExceptNew` | Retrieves all request types. | none |

---

## UnitController
**Base route:** `/api/v{version}/Unit`  
**Auth:** Authorize

| Method | Route | Description | Request |
| --- | --- | --- | --- |
| GET | `GetAllUnits` | Retrieves all units. | none |
| GET | `GetUnitById/{id:int}` | Retrieves a single unit by its identifier. | Route: `id` |
| GET | `GetUnitByName/{name:alpha}` | Retrieves units filtered by the specified parameter(s). | Route: `name` |
| GET | `GetByUnitTypeId/{unitTypeId:int}` | Retrieves units filtered by the specified parameter(s). | Route: `unitTypeId` |
| POST | `AddUnit` | Adds a new unit or related record. | Body: `AddUnitDto` |
| DELETE | `RemoveUnit/{id:int}` | Deletes (or soft-deletes) the specified unit. | Route: `id` |

---

## SearchController
**Base route:** `/api/v{version}/Search`  
**Auth:** **Anonymous** (the `[Authorize]` attribute is commented out in the provided code)

| Method | Route | Description | Request | Notes |
| --- | --- | --- | --- | --- |
| POST | `MembershipModuleSearch` | Searches based on the provided criteria. | Body: `GenericSearchDto` | Uses `User.GetUserId()` internally (may fail if truly anonymous) |
| POST | `EntityModuleSearch` | Searches based on the provided criteria. | Body: `GenericSearchDto` | Uses `User.GetUserId()` internally |
| POST | `FederationModuleSearch` | Searches based on the provided criteria. | Body: `GenericSearchDto` |  |
| POST | `SportsClubsModuleSearch` | Searches based on the provided criteria. | Body: `GenericSearchDto` |  |

---

## Change notes

Generated from provided files/snippets on **2025-12-15** (Africa/Cairo), including:

- Controllers (Entity + Membership)
- `appsettings.json`
- `Properties/launchSettings.json`
- `AddAppAuthetication` extension
- `TenantMiddleware`
- `Response<T>` and `GlobalResponseExtension.GetResponse()`
- MassTransit consumers for `InjuryStatusUpdatedEvent` and `UserLevelUpdatedEvent`