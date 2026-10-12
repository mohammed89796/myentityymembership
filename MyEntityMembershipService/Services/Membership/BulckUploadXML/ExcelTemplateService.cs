using AutoMapper;
using ClosedXML.Excel;
using Common.RabbitMq.Tenant;
using Common.RabbitMq.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyEntityMembershipService.Data;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Entity;
using MyEntityMembershipService.DTOs.Entity.FederationDtos;
using MyEntityMembershipService.DTOs.Membership.BulckUploadDtos;
using MyEntityMembershipService.DTOs.Membership.Swimming;
using MyEntityMembershipService.DTOs.Membership.TempRegisterResponseDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.AreaServ;
using MyEntityMembershipService.Services.Membership.FetchService;
using MyEntityMembershipService.Services.Membership.FormServ;
using MyEntityMembershipService.Services.Membership.MembershipServ;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;
using System.Globalization;
using System.Linq.Expressions;
using Common.RabbitMq.Membership;

namespace MyEntityMembershipService.Services.Membership.BulckUploadXML
{
    public class ExcelTemplateService : IExcelTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAreaService _areaService;
        private readonly IMapper _mapper;
        private readonly IFormService _formService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFetchService _fetchService;
        private readonly IMembershipService _membershipService;
        private readonly MassTransit.IPublishEndpoint _publishEndpoint;
        public ExcelTemplateService(IUnitOfWork unitOfWork, IAreaService areaService,IMembershipService membershipService, IMapper mapper, IFormService formService, IHttpContextAccessor httpContextAccessor, IFetchService fetchService, MassTransit.IPublishEndpoint publishEndpoint)
        {
            _unitOfWork = unitOfWork;
            _areaService = areaService;
            _mapper = mapper;
            _formService = formService;
            _membershipService = membershipService;
            _httpContextAccessor = httpContextAccessor;
            _fetchService = fetchService;
            _publishEndpoint = publishEndpoint;
        }





        #region  BulkUpload entity template
        public async Task<byte[]> GenerateEntityTemplateExcelAsync(int federationId, string userId)
        {
            var areas = (await _areaService.GetAllFederationAreas(federationId, userId)).Data;
            var fedrationTypes = await _unitOfWork.Repository<FedrationEntityType>().GetAllAsync();

            var headers = new List<string> { "Name", "FedrationEntityTypeId", "AreaId" };
            var dropdowns = new List<ExcelDropdownDataDto>
            {
                new ExcelDropdownDataDto
                {
                    RangeName = "EntityType",
                    SourceStartColumn = 1,
                    SourceHeader = "FedrationEntityTypeId",
                    SourceData = fedrationTypes.Select(f => new KeyValuePair<int, string>(f.Id, f.Name)).ToList(),
                    TargetColumnInMainSheet = 2
                },
                new ExcelDropdownDataDto
                {
                    RangeName = "AreaList",
                    SourceStartColumn = 4,
                    SourceHeader = "AreaId",
                    SourceData = areas.Select(a => new KeyValuePair<int, string>(a.Id, a.Name)).ToList(),
                    TargetColumnInMainSheet = 3
                }
            };

            return GenerateExcelTemplateExtension.GenerateExcelTemplate(headers, dropdowns);
        }

        public async Task<Response<List<ExcelValidationError>>> ValidateExcelTemplateAsync(IFormFile file, int federationId, string userId)
        {
            var headers = new List<string> { "Name", "FedrationEntityTypeId", "AreaId" };

            var areas = (await _areaService.GetAllFederationAreas(federationId, userId)).Data;
            var fedrationTypes = await _unitOfWork.Repository<FedrationEntityType>().GetAllAsync();

            var dropdowns = new List<ExcelDropdownDataDto>
            {
                new ExcelDropdownDataDto
                {
                    RangeName = "Entity Type",
                    SourceStartColumn = 4,
                    SourceHeader = "FedrationEntityTypeId",
                    SourceData = fedrationTypes.Select(f => new KeyValuePair<int, string>(f.Id, f.Name)).ToList(),
                    TargetColumnInMainSheet = 2
                },
                new ExcelDropdownDataDto
                {
                    RangeName = "AreaList",
                    SourceStartColumn = 1,
                    SourceHeader = "AreaId",
                    SourceData = areas.Select(a => new KeyValuePair<int, string>(a.Id, a.Name)).ToList(),
                    TargetColumnInMainSheet = 3
                }
            };

            var errors = GenerateExcelTemplateExtension.ValidateTemplateAsync(file, headers, dropdowns);

            if (errors.Any())
            {
                var errorMessages = errors
                    .Select(e => $"{(string.IsNullOrEmpty(e.Cell) ? "" : $"Cell {e.Cell}, ")}{e.Message}")
                    .ToList();

                var fullErrorMessage = string.Join(" | ", errorMessages);

                return new Response<List<ExcelValidationError>>
                {
                    Succeeded = false,
                    Message = fullErrorMessage,
                    Data = null
                };
            }

            return new ResponseHandler().Success(errors, "Validation passed.");
        }
        public async Task<Response<string>> AddEntitiesFromExcelAsync(IFormFile file, int federationId)
        {
            try
            {
                var tenants = await _unitOfWork.Repository<Tenant>()
                    .GetAllAsync(t => t.SuperEntityId == federationId && !t.IsDeleted);

                var existingNames = tenants.Select(t => t.Name.Trim().ToLower()).ToList();

                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var sheet = workbook.Worksheet("Template");

                var rowIndex = 2;
                var namesToAdd = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var errors = new List<ExcelValidationError>();

                foreach (var row in sheet.RowsUsed().Skip(1))
                {
                    var name = row.Cell(1).GetString()?.Trim();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        errors.Add(new ExcelValidationError { Cell = $"A{rowIndex}", Message = "Name is required." });
                        rowIndex++;
                        continue;
                    }

                    if (existingNames.Contains(name.ToLower()) || namesToAdd.Contains(name))
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = $"A{rowIndex}",
                            Message = $"Duplicate name '{name}' in this federation."
                        });
                        rowIndex++;
                        continue;
                    }

                    namesToAdd.Add(name);

                    var fedrationEntityTypeId = int.TryParse(row.Cell(2).GetString(), out var fedId) ? fedId : 0;
                    var areaId = int.TryParse(row.Cell(3).GetString(), out var area) ? area : 0;

                    var dto = new AddBulkEntityDto
                    {
                        Name = name,
                        FedrationEntityTypeId = fedrationEntityTypeId,
                        FedrationBranchId = areaId,
                        EntityTypeId = 1,
                        SuperEntityId = federationId
                    };

                    var entity = _mapper.Map<Tenant>(dto);
                    entity.CreationDate = DateTime.UtcNow;

                    await _unitOfWork.Repository<Tenant>().Add(entity);
                    rowIndex++;
                }

                if (errors.Any())
                {
                    var firstError = errors.First();
                    var errorMessage = $"Cell {firstError.Cell}: {firstError.Message}";
                    return new ResponseHandler().BadRequest<string>(errorMessage);
                }

                await _unitOfWork.Save();
                return new ResponseHandler().Success("Data uploaded successfully");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>($"Upload failed: {ex.Message}");
            }
        }



        #endregion

        #region BulkUpload Branch Entity Template
        public async Task<byte[]> GenerateBranchEntityTemplateExcelAsync()
        {
            var fedrationTypes = await _unitOfWork.Repository<FedrationEntityType>().GetAllAsync();

            var headers = new List<string> { "Name", "FedrationEntityTypeId" };
            var dropdowns = new List<ExcelDropdownDataDto>
            {
                new ExcelDropdownDataDto
                {
                    RangeName = "EntityType",
                    SourceStartColumn = 1,
                    SourceHeader = "FedrationEntityTypeId",
                    SourceData = fedrationTypes.Select(f => new KeyValuePair<int, string>(f.Id, f.Name)).ToList(),
                    TargetColumnInMainSheet = 2
                }
            };

            return GenerateExcelTemplateExtension.GenerateExcelTemplate(headers, dropdowns);
        }

        public async Task<Response<List<ExcelValidationError>>> ValidateBranchEntityTemplateAsync(IFormFile file, int federationId)
        {
            var headers = new List<string> { "Name", "FedrationEntityTypeId" };

            var fedrationTypes = await _unitOfWork.Repository<FedrationEntityType>().GetAllAsync();

            var dropdowns = new List<ExcelDropdownDataDto>
            {
                new ExcelDropdownDataDto
                {
                    RangeName = "EntityType",
                    SourceStartColumn = 4,
                    SourceHeader = "FedrationEntityTypeId",
                    SourceData = fedrationTypes.Select(f => new KeyValuePair<int, string>(f.Id, f.Name)).ToList(),
                    TargetColumnInMainSheet = 2
                }
            };

            var errors = GenerateExcelTemplateExtension.ValidateTemplateAsync(file, headers, dropdowns);

            if (errors.Any())
            {
                var errorMessages = errors
                    .Select(e => $"{(string.IsNullOrEmpty(e.Cell) ? "" : $"Cell {e.Cell}, ")}{e.Message}")
                    .ToList();

                var fullErrorMessage = string.Join(" | ", errorMessages);

                return new Response<List<ExcelValidationError>>
                {
                    Succeeded = false,
                    Message = fullErrorMessage,
                    Data = null
                };
            }

            return new ResponseHandler().Success(errors, "Validation passed.");
        }
        public async Task<Response<string>> UploadBranchEntitiesFromExcelAsync(IFormFile file, int federationId, int fedrationBranchId)
        {
            var errors = new List<ExcelValidationError>();
            try
            {
                var tenants = await _unitOfWork.Repository<Tenant>()
                    .GetAllAsync(t => t.SuperEntityId == federationId && !t.IsDeleted);

                var existingNames = tenants
                    .Select(t => t.Name.Trim().ToLower())
                    .ToList();

                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var sheet = workbook.Worksheet("Template");

                var rowIndex = 2;
                var namesToAdd = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var row in sheet.RowsUsed().Skip(1))
                {
                    var name = row.Cell(1).GetString()?.Trim();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        errors.Add(new ExcelValidationError { Cell = $"A{rowIndex}", Message = "Name is required." });
                        rowIndex++;
                        continue;
                    }

                    if (existingNames.Contains(name.ToLower()) || namesToAdd.Contains(name))
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = $"A{rowIndex}",
                            Message = $"Duplicate name '{name}' in this federation."
                        });
                        rowIndex++;
                        continue;
                    }

                    namesToAdd.Add(name);

                    var fedrationEntityTypeId = int.TryParse(row.Cell(2).GetString(), out var fedId) ? fedId : 0;

                    var dto = new AddBulkEntityDto
                    {
                        Name = name,
                        FedrationEntityTypeId = fedrationEntityTypeId,
                        FedrationBranchId = fedrationBranchId,
                        EntityTypeId = 1,
                        SuperEntityId = federationId
                    };

                    var entity = _mapper.Map<Tenant>(dto);
                    entity.CreationDate = DateTime.UtcNow;

                    await _unitOfWork.Repository<Tenant>().Add(entity);
                    rowIndex++;
                }

                if (errors.Any())
                {
                    var firstError = errors.First();
                    var errorMessage = $"Cell {firstError.Cell}: {firstError.Message}";
                    return new ResponseHandler().BadRequest<string>(errorMessage);
                }

                await _unitOfWork.Save();
                return new ResponseHandler().Success("Data uploaded successfully");
            }
            catch (Exception ex)
            {
                return new ResponseHandler().BadRequest<string>($"Upload failed: {ex.Message}");
            }
        }

        #endregion

        #region BulkUpload New Karate Members

        public async Task<byte[]> GenerateNewMembersTemplateExcelAsync()
        {
            var form = (await _formService.GetForm(159, null, null)).Data;

            var headers = new List<string>();
            var dropdowns = new List<ExcelDropdownDataDto>();
            var textFormatColumns = new List<int>();

            int dropdownStartColumn = 1;
            int dropdownRangeIndex = 1;

            for (int i = 0; i < form.Fields.Count; i++)
            {
                var field = form.Fields[i];

                // Skip file fields
                if (field.FieldTypeName?.ToLower() == "file")
                    continue;

                headers.Add(field.Title);

                bool isDropdown = field.FieldTypeName?.ToLower() == "dropdown";

                if (!isDropdown)
                    textFormatColumns.Add(headers.Count);

                if (isDropdown)
                {
                    List<string> dropdownValues = new();

                    if (field.DropdownValuesDto != null && field.DropdownValuesDto.Any())
                    {
                        dropdownValues = field.DropdownValuesDto;
                    }

                    if (dropdownValues.Any())
                    {
                        dropdowns.Add(new ExcelDropdownDataDto
                        {
                            SourceHeader = field.Title,
                            SourceStartColumn = dropdownStartColumn,
                            RangeName = $"DropdownRange{dropdownRangeIndex++}",
                            SourceData = dropdownValues
                                .Select((v, index) => new KeyValuePair<int, string>(index + 1, v))
                                .ToList(),
                            TargetColumnInMainSheet = headers.Count
                        });

                        dropdownStartColumn += 2;
                    }
                }
            }

            return GenerateExcelTemplateExtension.GenerateExcelTemplateWithdropdownNameReferance(headers, dropdowns, textFormatColumns);
        }

        public async Task<Response<List<ExcelValidationError>>> ValidateNewMembersTemplateAsync(IFormFile file)
        {
            var form = (await _formService.GetForm(159, null, null)).Data;

            var headers = new List<string>();
            var dropdowns = new List<ExcelDropdownDataDto>();

            int dropdownStartColumn = 1;
            int dropdownRangeIndex = 1;

            for (int i = 0; i < form.Fields.Count; i++)
            {
                var field = form.Fields[i];

                // Skip file fields
                if (field.FieldTypeName?.ToLower() == "file")
                    continue;

                headers.Add(field.Title);

                if (field.FieldTypeName?.ToLower() == "dropdown")
                {
                    List<string> dropdownValues = new();

                    if (field.DropdownValuesDto != null && field.DropdownValuesDto.Any())
                    {
                        dropdownValues = field.DropdownValuesDto;
                    }


                    if (dropdownValues.Any())
                    {
                        dropdowns.Add(new ExcelDropdownDataDto
                        {
                            RangeName = $"DropdownRange{dropdownRangeIndex++}",
                            SourceHeader = field.Title,
                            SourceStartColumn = dropdownStartColumn,
                            SourceData = dropdownValues
                            .Select((v, index) => new KeyValuePair<int, string>(index + 1, v))
                            .ToList(),
                            TargetColumnInMainSheet = headers.Count // because header was just added
                        });

                        dropdownStartColumn += 2;
                    }
                }
            }

            // Perform validation
            var errors = GenerateExcelTemplateExtension.ValidateTemplateByValueAsync(file, headers, dropdowns);

            if (errors.Any())
            {
                var errorMessages = errors
                    .Select(e => $"{(string.IsNullOrEmpty(e.Cell) ? "" : $"Cell {e.Cell}, ")}{e.Message}")
                    .ToList();

                var fullErrorMessage = string.Join(" | ", errorMessages);

                return new Response<List<ExcelValidationError>>
                {
                    Succeeded = false,
                    Message = fullErrorMessage,
                    Data = null
                };
            }

            return new ResponseHandler().Success<List<ExcelValidationError>>(null, "Validation passed.");
        }

        #endregion



        public async Task<Response<List<ExcelValidationError>>> ValidateMembershipTemplateAsync(IFormFile file)
        {
            var headers = new List<string>
    {
        "ID",
        "الاسم الأول للرياضي",
        "الاسم الثانى للرياضي",
        "الاسم الثالث للرياضي",
        "الاسم الأخير للرياضي",
        "نوع الرياضى",
        "الرقم القومي للرياضى من واقع شهادة الميلاد",
        "تاريخ ميلاد الرياضي",
        "البريد الالكترونى للرياضى",
        "رقم التليفون المحمول للرياضى",
        "عنوان الرياضى",
        "عضو أندية سيتى كلوب",
        "رقم عضوية نادى سيتى كلوب",
        "فرع سيتى كلوب",
        "الفترة المفضل الاشتراك بها",
        "التاريخ المفضل لبدء الإشتراك",
        "Package ID",
        "Branch ID",
        "Gender ID",
        "Level ID",
        "Star ID",
        "المستوي"
    };

            var errors = GenerateExcelTemplateExtension.ValidateTemplateAsync(file, headers, new List<ExcelDropdownDataDto>());

            if (errors.Any())
                return BulkUploadValidationHelper.BuildErrorResponse(errors);

            // ── Pre-load valid IDs from DB once (outside loop for performance) ──
            var validPackageIds = (await _unitOfWork.Repository<Package>().GetAllAsync())
                .Select(p => p.Id)
                .ToHashSet();

            var validBranchIds = (await _unitOfWork.Repository<Branch>().GetAllAsync())
                .Select(b => b.Id)
                .ToHashSet();

            var validGenderIds = Enum.GetValues<Gender>()
                .Select(g => (int)g)
                .ToHashSet();

            var seenEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var seenPhones = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var seenNationalIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var sheet = workbook.Worksheets.FirstOrDefault() ?? workbook.Worksheet("Template");

            int rowIndex = 2;

            foreach (var row in sheet.RowsUsed().Skip(1))
            {
                var nationalId = row.Cell(7).GetString()?.Trim();
                var birthDateStr = row.Cell(8).GetString()?.Trim();
                var email = row.Cell(9).GetString()?.Trim();
                var phone = row.Cell(10).GetString()?.Trim();
                var packageRaw = row.Cell(17).GetString()?.Trim();
                var branchRaw = row.Cell(18).GetString()?.Trim();
                var genderRaw = row.Cell(19).GetString()?.Trim();

                // =====================================================
                // National ID Validation
                // =====================================================
                if (!string.IsNullOrWhiteSpace(nationalId))
                {
                    //if (!BulkUploadValidationHelper.IsValidEgyptianNationalId(nationalId))
                    //{
                    //    errors.Add(new ExcelValidationError
                    //    {
                    //        Cell = $"G{rowIndex}",
                    //        Message = $"الرقم القومي '{nationalId}' غير صحيح."
                    //    });
                    //}
                    if (!seenNationalIds.Add(nationalId))
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = $"G{rowIndex}",
                            Message = $"الرقم القومي '{nationalId}' مكرر داخل الشيت."
                        });
                    }
                }

                // =====================================================
                // Email Validation
                // =====================================================
                if (!string.IsNullOrWhiteSpace(email))
                {
                    //if (!BulkUploadValidationHelper.IsValidEmail(email))
                    //{
                    //    errors.Add(new ExcelValidationError
                    //    {
                    //        Cell = $"I{rowIndex}",
                    //        Message = $"البريد الالكتروني '{email}' غير صالح."
                    //    });
                    //}
                     if (!seenEmails.Add(email))
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = $"I{rowIndex}",
                            Message = $"البريد الالكتروني '{email}' مكرر داخل الشيت."
                        });
                    }
                }

                // =====================================================
                // Phone Validation
                // =====================================================
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    phone = BulkUploadValidationHelper.NormalizeEgyptianPhone(phone);

                    //if (!BulkUploadValidationHelper.IsValidEgyptianPhone(phone))
                    //{
                    //    errors.Add(new ExcelValidationError
                    //    {
                    //        Cell = $"J{rowIndex}",
                    //        Message = $"رقم الهاتف '{phone}' غير صحيح."
                    //    });
                    //}
                     if (!seenPhones.Add(phone))
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = $"J{rowIndex}",
                            Message = $"رقم الهاتف '{phone}' مكرر داخل الشيت."
                        });
                    }
                }

                // =====================================================
                // Birthdate Validation
                // =====================================================
                if (string.IsNullOrWhiteSpace(birthDateStr))
                {
                    errors.Add(new ExcelValidationError
                    {
                        Cell = $"H{rowIndex}",
                        Message = "تاريخ الميلاد مطلوب"
                    });
                    rowIndex++;
                    continue;
                }

                DateTime birthDate;
                bool isValidDate = false;

                var formats = new[]
                {
            "dd/MM/yyyy",
            "d/M/yyyy",
            "MM/dd/yyyy",
            "M/d/yyyy",
            "yyyy-MM-dd",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss.ffffff",
            "MM/dd/yyyy HH:mm:ss",
            "M/d/yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm:ss"
        };

                if (double.TryParse(birthDateStr, out var oaDate))
                {
                    birthDate = DateTime.FromOADate(oaDate);
                    isValidDate = true;
                }
                else if (DateTime.TryParseExact(birthDateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
                {
                    isValidDate = true;
                }
                else if (DateTime.TryParse(birthDateStr, out birthDate))
                {
                    isValidDate = true;
                }

                if (!isValidDate)
                {
                    errors.Add(new ExcelValidationError
                    {
                        Cell = $"H{rowIndex}",
                        Message = $"صيغة تاريخ الميلاد غير صحيحة. القيمة: {birthDateStr}، الرجاء استخدام الصيغة: dd/MM/yyyy مثل 14/09/2015"
                    });
                    rowIndex++;
                    continue;
                }

                //var age = DateTime.Today.Year - birthDate.Year;
                //if (birthDate.Date > DateTime.Today.AddYears(-age)) age--;

                //if (age < 10)
                //{
                //    errors.Add(new ExcelValidationError
                //    {
                //        Cell = $"H{rowIndex}",
                //        Message = $"عمر الرياضي يجب أن يكون أكبر من 10 سنوات. العمر الحالي: {age}"
                //    });
                //}

                // =====================================================
                // Package ID Validation (col 17 → Q)
                // =====================================================
                if (!string.IsNullOrWhiteSpace(packageRaw))
                {
                    var packageId = ParseLookupId(packageRaw);
                    if (packageId == null)
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = $"Q{rowIndex}",
                            Message = $"قيمة Package ID '{packageRaw}' غير صالحة. يرجى الاختيار من القائمة."
                        });
                    }
                    else if (!validPackageIds.Contains(packageId.Value))
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = $"Q{rowIndex}",
                            Message = $"Package ID '{packageId}' غير موجود في النظام."
                        });
                    }
                }

                // =====================================================
                // Branch ID Validation (col 18 → R)
                // =====================================================
                if (!string.IsNullOrWhiteSpace(branchRaw))
                {
                    var branchId = ParseLookupId(branchRaw);
                    if (branchId == null)
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = $"R{rowIndex}",
                            Message = $"قيمة Branch ID '{branchRaw}' غير صالحة. يرجى الاختيار من القائمة."
                        });
                    }
                    else if (!validBranchIds.Contains(branchId.Value))
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = $"R{rowIndex}",
                            Message = $"Branch ID '{branchId}' غير موجود في النظام."
                        });
                    }
                }

                // =====================================================
                // Gender ID Validation (col 19 → S)
                // =====================================================
                if (!string.IsNullOrWhiteSpace(genderRaw))
                {
                    var genderId = ParseLookupId(genderRaw);
                    if (genderId == null || !validGenderIds.Contains(genderId.Value))
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = $"S{rowIndex}",
                            Message = $"قيمة Gender ID '{genderRaw}' غير صحيحة. القيم المقبولة: 1 (Male), 2 (Female)."
                        });
                    }
                }

                rowIndex++;
            }

            if (errors.Any())
                return BulkUploadValidationHelper.BuildErrorResponse(errors);

            return new ResponseHandler().Success(errors, "تمت عملية التحقق بنجاح.");
        }

        /// <summary>
        /// Extracts the integer ID from a lookup display value.
        /// Handles both plain integers ("5") and dropdown-style values ("5 – Package Name" or "5 - Package Name").
        /// Returns null if the value cannot be parsed.
        /// </summary>
        private static int? ParseLookupId(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            // Support "5 – Label" (em dash) and "5 - Label" (hyphen)
            var dashIndex = raw.IndexOfAny(new[] { '–', '-' });
            var idPart = dashIndex > 0 ? raw[..dashIndex].Trim() : raw.Trim();

            return int.TryParse(idPart, out var id) ? id : null;
        }
        public async Task<Response<string>> UploadMembershipsFromExcelAsync(IFormFile file)
        {
            var tenantId = int.Parse(_httpContextAccessor.HttpContext.User.GetTenantId());

            IDbContextTransaction? transaction = null;
            var registeredUserIds = new List<string>();
            var pendingEvents = new List<object>();

            try
            {
                transaction = await _unitOfWork.BeginTransactionAsync();

                // STEP 1: Validate sheet
                var validationResult = await ValidateMembershipTemplateAsync(file);
                if (!validationResult.Succeeded)
                    return new ResponseHandler().BadRequest<string>("Validation failed. " + validationResult.Message);

                // STEP 2: Extract temp users
                var tempUsers = new List<TempRegisterRequestDto>();
                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);

                var sheet = workbook.Worksheets.FirstOrDefault() ?? workbook.Worksheet("Template");

                foreach (var row in sheet.RowsUsed().Skip(1))
                {
                    var email = row.Cell(9).GetString()?.Trim();
                    var phone = row.Cell(10).GetString()?.Trim();
                    var nationalId = row.Cell(7).GetString()?.Trim();

                    tempUsers.Add(new TempRegisterRequestDto
                    {
                        UserName = email,
                        NationalityId = nationalId,
                        TenantId = tenantId,
                        Email = email,
                        PhoneNumber = phone
                    });
                }

                // STEP 3: Bulk register in identity
                var registerResult = await _fetchService.BulkRegisterUsersAsync(tempUsers);
                if (!registerResult.Succeeded || registerResult.Data == null)
                {
                    await transaction.RollbackAsync();
                    return new ResponseHandler().BadRequest<string>("User registration failed: " + registerResult.Message);
                }

                var registeredUsers = registerResult.Data;
                registeredUserIds = registeredUsers
                   .Where(u => !string.IsNullOrEmpty(u.UserId))
                   .Select(u => u.UserId)
                   .ToList();

                // STEP 4: Insert Membership + MembershipAccess
                int rowIndex = 2;
                foreach (var row in sheet.RowsUsed().Skip(1))
                {
                    var user = registeredUsers.ElementAt(rowIndex - 2);
                    if (string.IsNullOrEmpty(user.UserId))
                    {
                        await transaction.RollbackAsync();
                        await SafeDeleteUsersAsync(registeredUserIds, rowIndex);
                        return new ResponseHandler().BadRequest<string>($"Failed to register user at row {rowIndex}");
                    }

                    // Parse Gender (col 19)
                    Gender? gender = null;
                    var genderStr = row.Cell(19).GetString()?.Trim();
                    if (!string.IsNullOrEmpty(genderStr))
                    {
                        var genderId = ParseLookupId(genderStr);
                        if (genderId.HasValue && Enum.IsDefined(typeof(Gender), genderId.Value))
                        {
                            gender = (Gender)genderId.Value;
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            await SafeDeleteUsersAsync(registeredUserIds, rowIndex);
                            return new ResponseHandler().BadRequest<string>($"Invalid Gender value at row {rowIndex}: '{genderStr}'");
                        }
                    }

                    var membership = new Data.Membership.Entities.Membership
                    {
                        FirstName = row.Cell(2).GetString()?.Trim(),
                        SecondName = row.Cell(3).GetString()?.Trim(),
                        MiddleName = row.Cell(4).GetString()?.Trim(),
                        LastName = row.Cell(5).GetString()?.Trim(),
                        NationalId = row.Cell(7).GetString()?.Trim(),
                        BirthDate = ParseBirthDate(row.Cell(8)),
                        Email = row.Cell(9).GetString()?.Trim(),
                        PhoneNumber = row.Cell(10).GetString()?.Trim(),
                        Address = row.Cell(11).GetString()?.Trim(),
                        PackageId = ParseLookupId(row.Cell(17).GetString()),
                        Gender = gender,
                        TenantId = tenantId,
                        UserId = user.UserId,
                        CreationDate = DateTime.UtcNow,
                        MembershipStatus = MembershipStatus.Active,
                        MemberShipId = GenerateMembershipId(),
                        BeltName = row.Cell(20).GetString()?.Trim() ?? "Championship Division"
                    };

                    await _unitOfWork.Repository<Data.Membership.Entities.Membership>().Add(membership);
                    await _unitOfWork.Save();


                    // STEP 4 (inside foreach row after saving Membership)
                    var branchId = ParseLookupId(row.Cell(18).GetString());
                    if (branchId.HasValue && branchId.Value > 0)
                    {
                        if (membership.PackageId.HasValue)
                        {
                            var packageId = membership.PackageId.Value;

                            // Venue-level access
                            var branchFacilitiesVenues = await _unitOfWork
                                .Repository<AccessedFacilitiesVenues>()
                                .GetAllAsync(fv => fv.PackageId == packageId && fv.BranchId == branchId.Value);

                            foreach (var fv in branchFacilitiesVenues)
                            {
                                await _unitOfWork.Repository<MembershipAccess>().Add(new MembershipAccess
                                {
                                    Membership = membership,
                                    BranchId = branchId.Value,
                                    FacilityId = fv.FacilityId,
                                    VenueId = fv.VenueId
                                });
                            }

                            // Facility-level access
                            var branchFacilities = await _unitOfWork
                                .Repository<AccessedBranchesFacilities>()
                                .GetAllAsync(bf => bf.PackageId == packageId && bf.BranchId == branchId.Value);

                            foreach (var bf in branchFacilities)
                            {
                                await _unitOfWork.Repository<MembershipAccess>().Add(new MembershipAccess
                                {
                                    Membership = membership,
                                    BranchId = branchId.Value,
                                    FacilityId = bf.FacilityId,
                                    VenueId = null
                                });
                            }

                            // Branch-level access
                            var accessedBranch = await _unitOfWork
                                .Repository<AccessedBranches>()
                                .GetAllAsync(b => b.PackageId == packageId && b.BranchId == branchId.Value);

                            if (accessedBranch != null)
                            {
                                await _unitOfWork.Repository<MembershipAccess>().Add(new MembershipAccess
                                {
                                    Membership = membership,
                                    BranchId = branchId.Value,
                                    FacilityId = null,
                                    VenueId = null
                                });
                            }

                            await _unitOfWork.Save();
                        }
                    }
                    // For Star — buffered, published only after commit
                    if (!string.IsNullOrWhiteSpace(row.Cell(21).GetString()))
                    {
                        var starOptionId = ParseLookupId(row.Cell(21).GetString());
                        if (starOptionId.HasValue)
                        {
                            pendingEvents.Add(new AssignUserLevelCommand
                            {
                                UserId = membership.UserId,
                                MembershipId = membership.Id,
                                TenantId = membership.TenantId,
                                OptionId = starOptionId.Value,
                                ChangedBy = _httpContextAccessor.HttpContext.User.GetUserId()
                            });
                        }
                    }

                    var levelOptionId = ParseLookupId(row.Cell(20).GetString());
                    pendingEvents.Add(new AssignUserLevelCommand
                    {
                        UserId = membership.UserId,
                        MembershipId = membership.Id,
                        TenantId = membership.TenantId,
                        OptionId = levelOptionId ?? 76,
                        ChangedBy = _httpContextAccessor.HttpContext.User.GetUserId()
                    });
                    Log.Information($"Prepared UserLevelUpdatedEvent for UserId={membership.UserId}, LevelOption={row.Cell(20).GetString() ?? "76"}");

                    // Always publish tenant access event — buffered, published only after commit
                    pendingEvents.Add(new UserTeanatAccessCreatedDto
                    {
                        UserId = membership.UserId,
                        TenantId = membership.TenantId
                    });

                    rowIndex++;
                }

                // STEP 5: Save and commit
                await transaction.CommitAsync();

                // STEP 6: Only publish events AFTER the transaction is durably committed
                foreach (var evt in pendingEvents)
                {
                    switch (evt)
                    {
                        case AssignUserLevelCommand levelEvt:
                            await _publishEndpoint.Publish(levelEvt);
                            break;
                        case UserTeanatAccessCreatedDto tenantEvt:
                            await _publishEndpoint.Publish(tenantEvt);
                            break;
                    }
                }

                return new ResponseHandler().Success("تم رفع بيانات الأعضاء وإنشاء الصلاحيات للفروع بنجاح.");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try
                    {
                        await transaction.RollbackAsync();
                    }
                    catch (Exception rollbackEx)
                    {
                        Log.Warning(rollbackEx, "Rollback was already completed or failed during exception handling.");
                    }
                }

                await SafeDeleteUsersAsync(registeredUserIds, null);

                return new ResponseHandler().BadRequest<string>($"Upload failed: {ex.Message}");
            }
            finally
            {
                if (transaction != null)
                    await transaction.DisposeAsync();
            }
        }

        /// <summary>
        /// Compensating delete for identity users created via BulkRegisterUsersAsync.
        /// DeleteUsers never throws (it wraps its own try/catch and returns a failure
        /// Response instead) — so the important part here is checking Succeeded, not
        /// catching exceptions. The try/catch is kept as defensive insurance in case
        /// that implementation ever changes.
        /// </summary>
        private async Task SafeDeleteUsersAsync(List<string> userIds, int? failedRow)
        {
            if (userIds == null || !userIds.Any()) return;

            try
            {
                var deleteResult = await _fetchService.DeleteUsers(userIds);
                if (deleteResult == null || !deleteResult.Succeeded)
                {
                    Log.Error(
                        "Failed to clean up {Count} identity users after membership upload error (row {Row}). Message: {Message}. UserIds: {UserIds}",
                        userIds.Count, failedRow, deleteResult?.Message, string.Join(",", userIds));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                    "Exception while cleaning up {Count} identity users after membership upload error (row {Row}). UserIds: {UserIds}",
                    userIds.Count, failedRow, string.Join(",", userIds));
            }
        }
        private string GenerateMembershipId()
        {

            Guid guid = Guid.NewGuid();


            int uniqueNumber = Math.Abs(BitConverter.ToInt32(guid.ToByteArray(), 0));


            string timestamp = DateTime.Now.Ticks.ToString();


            var uniqueId = uniqueNumber.ToString() + timestamp;

            return uniqueId;
        }
        private static DateTime? ParseBirthDate(IXLCell cell)
        {
            if (cell.IsEmpty()) return null;

            if (cell.DataType == XLDataType.DateTime)
                return cell.GetDateTime();

            if (cell.DataType == XLDataType.Number)
                return DateTime.FromOADate(cell.GetDouble());

            var text = cell.GetString()?.Trim();
            if (string.IsNullOrEmpty(text)) return null;

            // Convert Arabic-Indic digits (٠-٩) to Latin digits
            text = new string(text.Select(c => c >= '٠' && c <= '٩' ? (char)('0' + (c - '٠')) : c).ToArray());

            string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy", "yyyy-MM-dd", "dd/MM/yyyy HH:mm:ss" };
            return DateTime.TryParseExact(text, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
                ? d
                : null;
        }
        public async Task<byte[]> DownloadMembersTemplateExcelAsync(int? tenantId)
        {
            if (!tenantId.HasValue)
            {
                Log.Error("Tenant ID is null. Cannot generate members template.");
                throw new ArgumentNullException(nameof(tenantId), "Tenant ID cannot be null.");
            }

            try
            {
                // ── Fetch lookup data ──────────────────────────────────────────────
                var packages = await _unitOfWork.Repository<Package>()
                    .GetAllAsync(p => p.TenantId == tenantId.Value && p.IsActive);

                var branches = await _unitOfWork.Repository<Branch>()
                    .GetAllAsync(b => b.TenantId == tenantId.Value);

                var headers = new List<string>
        {
            "ID",
            "الاسم الأول للرياضي",
            "الاسم الثانى للرياضي",
            "الاسم الثالث للرياضي",
            "الاسم الأخير للرياضي",
            "نوع الرياضى",
            "الرقم القومي للرياضى من واقع شهادة الميلاد",
            "تاريخ ميلاد الرياضي",
            "البريد الالكترونى للرياضى",
            "رقم التليفون المحمول للرياضى",
            "عنوان الرياضى",
            "عضو أندية سيتى كلوب",
            "رقم عضوية نادى سيتى كلوب",
            "فرع سيتى كلوب",
            "الفترة المفضل الاشتراك بها",
            "التاريخ المفضل لبدء الإشتراك",
            "Package ID",   // col 17
            "Branch ID",    // col 18
            "Gender ID",    // col 19
            "Level ID",
            "Star ID",
            "المستوي"
        };

                var lookups = new List<LookupSheetDto>
        {
            new LookupSheetDto
            {
                RangeName = "PackageList",
                DisplayName = "Package",
                TargetColumnInMainSheet = 17,          // "Package ID" column
                Items = packages
                    .Select(p => new LookupItem { Id = p.Id, Label = p.Name })
                    .ToList()
            },
            new LookupSheetDto
            {
                RangeName = "BranchList",
                DisplayName = "Branch",
                TargetColumnInMainSheet = 18,          // "Branch ID" column
                Items = branches
                    .Select(b => new LookupItem { Id = b.Id, Label = b.Name })
                    .ToList()
            },
            new LookupSheetDto
            {
                RangeName = "GenderList",
                DisplayName = "Gender",
                TargetColumnInMainSheet = 19,          // "Gender ID" column
                Items = new List<LookupItem>
                {
                    new LookupItem { Id = (int)Gender.Male,   Label = "Male" },
                    new LookupItem { Id = (int)Gender.Female, Label = "Female" }
                }
            }
            };

                return GenerateExcelTemplateExtension.GenerateExcelWithLookupsTemplate(headers, lookups);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating members template Excel.");
                return Array.Empty<byte>();
            }
        }

        public async Task<byte[]> DownloadSwimmingMembersExcelAsync()
        {
            var membersResult = await  _membershipService.GetAllSwimmingMembers();
            if (!membersResult.Succeeded || membersResult.Data == null)
                throw new Exception("Failed to retrieve members data.");

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Swimming Members");

            // Headers
            var headers = new[]
            {
                "ID", "User ID", "Full Name", "First Name", "Last Name",
                "Phone Number", "Email", "National ID", "Birth Date",
                "Gender", "Level", "Injury Status", "Date of Acquisition", "Branches"
            };

            for (int i = 0; i < headers.Length; i++)
                sheet.Cell(1, i + 1).Value = headers[i];

            // Data
            var members = membersResult.Data;
            for (int i = 0; i < members.Count; i++)
            {
                var m = members[i];
                int row = i + 2;

                sheet.Cell(row, 1).Value = m.Id;
                sheet.Cell(row, 2).Value = m.UserId;
                sheet.Cell(row, 3).Value = m.FullName;
                sheet.Cell(row, 4).Value = m.FirstName;
                sheet.Cell(row, 5).Value = m.LastName;
                sheet.Cell(row, 6).Value = m.PhoneNumber;
                sheet.Cell(row, 7).Value = m.Email;
                sheet.Cell(row, 8).Value = m.NationalId;
                sheet.Cell(row, 9).Value = m.BirthDate == DateTime.MinValue ? "" : m.BirthDate.ToString("yyyy-MM-dd");
                sheet.Cell(row, 10).Value = m.Gender.ToString();
                sheet.Cell(row, 11).Value = m.Levels?.Count > 0 ? string.Join(", ", m.Levels.Select(l => l.LevelName)) : "";
                sheet.Cell(row, 12).Value = m.InjuryStatus.ToString();
                sheet.Cell(row, 13).Value = m.DateOfAcquisition == DateTime.MinValue ? "" : m.DateOfAcquisition.ToString();
                sheet.Cell(row, 14).Value = m.BranchNames?.Count > 0 ? string.Join(", ", m.BranchNames) : "";
            }

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> ExportMembersSportLevelsExcelAsync()
        {
            var tenantId = int.Parse(_httpContextAccessor.HttpContext.User.GetTenantId());

            Expression<Func<MemberSportLevel, bool>> criteria = m =>
                m.TenantId == tenantId &&
                m.Membership.MembershipStatus == MembershipStatus.Active;

            var spec = new GeneralSpecifications<MemberSportLevel>(criteria);

            spec.AddInclude(x => x.Include(x => x.Membership)
                                  .ThenInclude(m => m.MembershipAccesses)
                                  .ThenInclude(a => a.Branch));

            spec.SetOrderByDescending(x => x.CreationDate);

            var data = await _unitOfWork.Repository<MemberSportLevel>()
                .GetAllWithSpecifications(spec);

            if (data == null || !data.Any())
                throw new Exception("No data found");

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Members Sport Levels");

            var headers = new[]
            {
                "الاسم",
                "تاريخ الميلاد",
                "النوع",
                "الرياضة",
                "المستوى",
                "الفرع"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
                sheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            int row = 2;

            foreach (var x in data)
            {
                var m = x.Membership;

                // الاسم
                sheet.Cell(row, 1).Value =
                    $"{m.FirstName} {m.SecondName} {m.MiddleName} {m.LastName}";

                // تاريخ الميلاد
                sheet.Cell(row, 2).Value =
                    m.BirthDate.HasValue
                    ? m.BirthDate.Value.ToString("yyyy-MM-dd")
                    : "";

                // النوع
                sheet.Cell(row, 3).Value =
                    m.Gender.HasValue ? m.Gender.ToString() : "";

                // الرياضة
                sheet.Cell(row, 4).Value =
                    x.SportNameAr ?? x.SportName;

                // المستوى
                sheet.Cell(row, 5).Value =
                    x.OptionName;

                // الفروع
                var branches = m.MembershipAccesses
                    .Where(a => a.Branch != null)
                    .Select(a => a.Branch.Name)
                    .Distinct()
                    .ToList();

                sheet.Cell(row, 6).Value =
                    branches.Any() ? string.Join(", ", branches) : "N/A";

                row++;
            }

            // تحسين الشكل
            sheet.Columns().AdjustToContents();
            sheet.RightToLeft = true;

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);

            return ms.ToArray();
        }
    }

}
