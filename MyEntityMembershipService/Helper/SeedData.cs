using Microsoft.EntityFrameworkCore;
using MyEntityMembershipService.Data;

using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Extensions;
using Serilog;

namespace MyEntityMembershipService.Helper
{
    public static class SeedData
    {

        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            var tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
            tenantProvider.IgnoreTenantFilter = true;

            using var context = new EntityMembershipDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<EntityMembershipDbContext>>(),
                serviceProvider.GetRequiredService<IHttpContextAccessor>(),
                tenantProvider);
            tenantProvider.IgnoreTenantFilter = true;


            #region Membership

            if (!context.MemberType.IgnoreQueryFilters().Any())
            {
                context.MemberType.AddRange(
                    new MemberType() { Id = 1, Name = "Wife", IsDeleted = false, ArabicName = "زوجة" },
                    new MemberType() { Id = 2, Name = "Daughter", IsDeleted = false, ArabicName = "ابنه" },
                    new MemberType() { Id = 3, Name = "Son", IsDeleted = false, ArabicName = "ابن" },
                    new MemberType() { Id = 4, Name = "Brother", IsDeleted = false, ArabicName = "أخ" },
                    new MemberType() { Id = 5, Name = "Main Member", IsDeleted = false, ArabicName = "العضو الرئيسي" },
                    new MemberType() { Id = 7, Name = "Step-Son", IsDeleted = false, ArabicName = "ابن الزوج" },
                    new MemberType() { Id = 8, Name = "Step-Daughter", IsDeleted = false, ArabicName = "ابنة الزوج" },
                    new MemberType() { Id = 9, Name = "Mother-in-Law", IsDeleted = false, ArabicName = "أم الزوج" },
                    new MemberType() { Id = 10, Name = "Father-in-Law", IsDeleted = false, ArabicName = "أب الزوج" },
                    new MemberType() { Id = 11, Name = "Sister", IsDeleted = false, ArabicName = "أخت" },
                    new MemberType() { Id = 12, Name = "Adopted Son", IsDeleted = false, ArabicName = "ابن بالتبني" },
                    new MemberType() { Id = 13, Name = "Adopted Daughter", IsDeleted = false, ArabicName = "ابنة بالتبني" },
                    new MemberType() { Id = 14, Name = "Nurse", IsDeleted = false, ArabicName = "ممرضة" },
                    new MemberType() { Id = 15, Name = "Nanny", IsDeleted = false, ArabicName = "مربية" },
                    new MemberType() { Id = 16, Name = "Driver", IsDeleted = false, ArabicName = "سائق" }
                    

                );
                context.SaveChanges();
            }
            if (!context.MemberType.Any(x => x.Id == 17))
            {
                context.MemberType.Add(new MemberType() { Id = 17, Name = "Child", IsDeleted = false, ArabicName = "ابن/ابنه" });
                context.SaveChanges();
            }


            if (!context.RequestTypeCategory.IgnoreQueryFilters().Any())
            {
                context.RequestTypeCategory.AddRange(
                    new RequestTypeCategory() { Id = 1, Name = "Creation", IsDeleted = false },
                    new RequestTypeCategory() { Id = 2, Name = "Modification", IsDeleted = false },
                    new RequestTypeCategory() { Id = 3, Name = "Termination", IsDeleted = false });
                context.SaveChanges();
            }
            if (!context.PaymentInvoiceTypes.IgnoreQueryFilters().Any())
            {
                context.PaymentInvoiceTypes.AddRange(
                    new PaymentInvoiceType() { Id = 1, Name = "Package", IsDeleted = false },

                    new PaymentInvoiceType() { Id = 2, Name = "Program", IsDeleted = false },

                    new PaymentInvoiceType() { Id = 3, Name = "Membership Form", IsDeleted = false },

                    new PaymentInvoiceType() { Id = 4, Name = "Operation Fees", IsDeleted = false },
                    new PaymentInvoiceType() { Id = 5, Name = "Venue Booking", IsDeleted = false });


                context.SaveChanges();
            }
            if (!context.UnitTypes.IgnoreQueryFilters().Any())
            {
                context.AddRange(
                    new UnitType() { Id = 1, Name = "Duration Unit" },
                    new UnitType() { Id = 2, Name = "Price Unit" });
                context.SaveChanges();
            }
            if (!context.Units.IgnoreQueryFilters().Any())
            {
                context.AddRange(
                    new Unit() { Id = 1, Name = "Day", UnitTypeId = 1 },
                    new Unit() { Id = 2, Name = "Month", UnitTypeId = 1 },
                    new Unit() { Id = 3, Name = "Year", UnitTypeId = 1 },
                    new Unit() { Id = 4, Name = "EGP", UnitTypeId = 2 });
                context.SaveChanges();
            }

            if (!context.RequestTypes.IgnoreQueryFilters().Any())
            {
                context.RequestTypes.AddRange(
                    new RequestType() { Id = 6, Name = "New Membership Request", NameAr = "طلب عضوية جديدة", IsDeleted = false, RequestTypeCategoryId = 1, Description = "View and manage forms for new membership applications." },
                    new RequestType() { Id = 7, Name = "Renewal Request", NameAr = "طلب تجديد", IsDeleted = false, RequestTypeCategoryId = 1, Description = "Access forms for processing membership renewals." },
                    new RequestType() { Id = 8, Name = "Transfer Ownership Request", NameAr = "طلب نقل ملكية", IsDeleted = false, RequestTypeCategoryId = 2, Description = "Browse forms for membership transfers between members." },
                    new RequestType() { Id = 9, Name = "Separation Request", NameAr = "طلب فصل", IsDeleted = false, RequestTypeCategoryId = 2, Description = "Examine forms for separating sub-members from main memberships." },
                    new RequestType() { Id = 10, Name = "Alteration Request", NameAr = "طلب تعديل", IsDeleted = false, RequestTypeCategoryId = 2, Description = "Review forms for membership upgrade or downgrade." },
                    new RequestType() { Id = 12, Name = "Freeze Request", NameAr = "طلب تجميد", IsDeleted = false, RequestTypeCategoryId = 3, Description = "Look through forms for freezing or temporarily suspending memberships." },
                    new RequestType() { Id = 15, Name = "Transfer Location Request", NameAr = "طلب نقل فرع", IsDeleted = false, RequestTypeCategoryId = 2, Description = "Browse forms for membership transfers between locations." },
                    new RequestType() { Id = 16, Name = "Addition Request", NameAr = "طلب إضافة", IsDeleted = false, RequestTypeCategoryId = 2, Description = "Check forms for adding new sub-members." },
                    new RequestType() { Id = 17, Name = "Cancellation Request", NameAr = "طلب إلغاء", IsDeleted = false, RequestTypeCategoryId = 3, Description = "Inspect forms for membership cancellation requests." },
                    new RequestType() { Id = 18, Name = "Removal Request", NameAr = "طلب إزالة", IsDeleted = false, RequestTypeCategoryId = 2, Description = "Examine forms for separating sub-members from main memberships." },
                    new RequestType() { Id = 19, Name = "Transfer", NameAr = "تحويل", IsDeleted = false, RequestTypeCategoryId = 2, Description = "" },
                    new RequestType() { Id = 20, Name = "Loan", NameAr = "إعارة", IsDeleted = false, RequestTypeCategoryId = 2, Description = "" });

                context.SaveChanges();
            }
            if (!context.RequestTypes.Any(x => x.Id == 21))
            {
                context.RequestTypes.Add(new RequestType() { Id = 21, Name = "ParentApplication", NameAr = "ب ولي أمر اضافه طفل", IsDeleted = false, RequestTypeCategoryId = 1, Description = "" });
                context.SaveChanges();
            }

            if (!context.FieldTypes.IgnoreQueryFilters().Any())
            {
                context.FieldTypes.AddRange(
                    new FieldType() { Id = 1, Name = "text", IsDeleted = false },
                    new FieldType() { Id = 2, Name = "dropdown", IsDeleted = false },
                    new FieldType() { Id = 3, Name = "date", IsDeleted = false },
                    new FieldType() { Id = 4, Name = "number", IsDeleted = false },
                    new FieldType() { Id = 5, Name = "toggle", IsDeleted = false },
                    new FieldType() { Id = 6, Name = "file", IsDeleted = false },
                    new FieldType() { Id = 7, Name = "checkbox", IsDeleted = false },
                    new FieldType() { Id = 8, Name = "view", IsDeleted = false },
                    new FieldType() { Id = 9, Name = "radio", IsDeleted = false },
                    new FieldType() { Id = 10, Name = "FieldWithUnit", IsDeleted = false });
                context.SaveChanges();
            }

            if (!context.Fields.IgnoreQueryFilters().Any())
            {
                context.Fields.AddRange(
                new Field() { Id = 22, Title = "Start Date", FieldTypeId = 3, IsDeleted = false, MapField = "" },
                new Field() { Id = 23, Title = "End Date", FieldTypeId = 3, IsDeleted = false, MapField = "" },
                new Field() { Id = 24, Title = "Duration", FieldTypeId = 10, UnitTypeId = 1, IsDeleted = false, MapField = "" },
                new Field() { Id = 49, Title = "Removal Date", FieldTypeId = 3, IsDeleted = false, MapField = "" },
                new Field() { Id = 51, Title = "First Name", FieldTypeId = 1, IsDeleted = false, MapField = "FirstName" },
                new Field() { Id = 52, Title = "Last Name", FieldTypeId = 1, IsDeleted = false, MapField = "LastName" },
                new Field() { Id = 53, Title = "Date of Birth", FieldTypeId = 3, IsDeleted = false, MapField = "BirthDate" },
                new Field() { Id = 54, Title = "Gender", FieldTypeId = 2, IsDeleted = false, MapField = "Gender" },
                new Field() { Id = 55, Title = "Nationality", FieldTypeId = 2, IsDeleted = false, MapField = "Nationality" },
                new Field() { Id = 56, Title = "Email Address", FieldTypeId = 1, IsDeleted = false, MapField = "Email" },
                new Field() { Id = 57, Title = "Phone Number", FieldTypeId = 1, IsDeleted = false, MapField = "PhoneNumber" },
                new Field() { Id = 58, Title = "National ID (Number)", FieldTypeId = 1, IsDeleted = false, MapField = "NationalId" },
                new Field() { Id = 59, Title = "Address", FieldTypeId = 1, IsDeleted = false, MapField = "Address" },
                new Field() { Id = 60, Title = "Second Name", FieldTypeId = 1, IsDeleted = false, MapField = "" },
                new Field() { Id = 61, Title = "Middle Name", FieldTypeId = 1, IsDeleted = false, MapField = "" },
                new Field() { Id = 62, Title = "Birth Certificate", FieldTypeId = 6, IsDeleted = false, MapField = "" },
                new Field() { Id = 63, Title = "National ID/Passport", FieldTypeId = 6, IsDeleted = false, MapField = "" },
                new Field() { Id = 64, Title = "Reason for Removal", FieldTypeId = 2, IsDeleted = false, MapField = "" },
                new Field() { Id = 65, Title = "Reason for Separation", FieldTypeId = 2, IsDeleted = false, MapField = "" },
                new Field() { Id = 66, Title = "Additional Comments", FieldTypeId = 1, IsDeleted = false, MapField = "" },
                new Field() { Id = 67, Title = "ID Proof Upload", FieldTypeId = 6, IsDeleted = false, MapField = "" },
                new Field() { Id = 69, Title = "Reason for Cancellation", FieldTypeId = 2, IsDeleted = false, MapField = "" },
                new Field() { Id = 70, Title = "Preferred Cancellation Date", FieldTypeId = 3, IsDeleted = false, MapField = "" },
                new Field() { Id = 71, Title = "Feedback and Additional Comments", FieldTypeId = 1, IsDeleted = false, MapField = "" },
                new Field() { Id = 77, Title = "Reason for Freeze", FieldTypeId = 2, IsDeleted = false, MapField = "" },
                new Field() { Id = 75, Title = "Medical Document", FieldTypeId = 6, IsDeleted = false, MapField = "" },
                new Field() { Id = 76, Title = "Personal Photo", FieldTypeId = 6, IsDeleted = false, MapField = "ProfileImage" },

                //Karata Federation Form Fnew Fields
                new Field() { Id = 78, Title = "Date of Acquisition", MapField = "DateOfAcquisition", FieldTypeId = 3, IsDeleted = false },
                new Field() { Id = 79, Title = "School/Work Entity", MapField = "SchoolWorkEntity", FieldTypeId = 1, IsDeleted = false },
                new Field() { Id = 80, Title = "Date of Expiry", MapField = "DateOfExpiry", FieldTypeId = 3, IsDeleted = false },
                new Field() { Id = 81, Title = "Date of Issuance", MapField = "DateOfIssuance", FieldTypeId = 3, IsDeleted = false },
                new Field() { Id = 82, Title = "Home Number", MapField = "HomeNumber", FieldTypeId = 1, IsDeleted = false },
                new Field() { Id = 83, Title = "Curren Belt Level", MapField = "BeltName", FieldTypeId = 2, IsDeleted = false },
                new Field() { Id = 84, Title = "Passport Number", MapField = "PassportNumber", FieldTypeId = 1, IsDeleted = false },
                new Field() { Id = 85, Title = "File Number", MapField = "FileNumber", FieldTypeId = 1, IsDeleted = false },
                new Field { Id = 86, Title = "Belt Certificate", MapField = "BeltCertificate", FieldTypeId = 6, IsDeleted = false },
                new Field { Id = 87, Title = "Birth Certificate Or NationalID", MapField = "BirthCertificateOrNationalID", FieldTypeId = 6, IsDeleted = false },
                new Field { Id = 88, Title = "Military Status", MapField = "MilitaryStatus", FieldTypeId = 2, IsDeleted = false },
                new Field { Id = 89, Title = "Profile Image", MapField = "UserPhoto", FieldTypeId = 6, IsDeleted = false },

                //Karata Federation Transfer Form Fnew Fields

                new Field() { Id = 90, Title = "Branch", MapField = "BranchId", FieldTypeId = 1, IsDeleted = false },
                new Field() { Id = 91, Title = "Entity", MapField = "FromTenantId", FieldTypeId = 1, IsDeleted = false },
                new Field() { Id = 92, Title = "Transfer Request Document", FieldTypeId = 6, IsDeleted = false, MapField = "" },
                new Field() { Id = 93, Title = "Loan Request Document", FieldTypeId = 6, IsDeleted = false, MapField = "" }





            );


                context.SaveChanges();
            }

            if (context.Fields.FirstOrDefault(f => f.Id == 83) is Field level)
            {
                level.Title = "Level";

                context.Update(level);

                context.SaveChanges();

            }

            if (!context.Fields.Any(f => f.Id >= 94 && f.Id <= 97))
            {
                context.Fields.AddRange(
                    new Field() { Id = 94, Title = "Is Parent", MapField = "IsParent", FieldTypeId = 1, IsDeleted = false },
                    new Field() { Id = 95, Title = "Is City Club Member", MapField = "IsCityClubMember", FieldTypeId = 1, IsDeleted = false },
                    new Field() { Id = 96, Title = "SelectedPackage", MapField = "", FieldTypeId = 1, IsDeleted = false },
                    new Field() { Id = 97, Title = "LevelId", MapField = "", FieldTypeId = 1, IsDeleted = false }
                );

                context.SaveChanges();
            }

           



            // Replace the existing block that adds fields 98..103 and the dropdown blocks
            // with the following. This adds only missing Field rows (so a partial existing
            // range won't prevent missing items from being created) and ensures Fields
            // exist before inserting DropdownValue rows.

            if (!context.Fields.IgnoreQueryFilters().Any(f => f.Id >= 98 && f.Id <= 103))
            {
                // Build the desired set and only add missing entries (handles partial seeding)
                var desiredFields = new List<Field>
                {
                    new Field()
                    {
                        Id = 98,
                        Title = "Player Position",
                        MapField = "PlayerPosition",
                        FieldTypeId = 2, // Dropdown
                        IsDeleted = false
                    },
                    new Field()
                    {
                        Id = 99,
                        Title = "Has Club Experience",
                        MapField = "HasClubExperience",
                        FieldTypeId = 1, // Boolean
                        IsDeleted = false
                    },
                    new Field()
                    {
                        Id = 100,
                        Title = "Club Name",
                        MapField = "ClubName",
                        FieldTypeId = 1, // Text
                        IsDeleted = false
                    },
                    new Field()
                    {
                        Id = 101,
                        Title = "سن اللاعب",
                        MapField = "PlayerAge",
                        FieldTypeId = 2, // Dropdown
                        IsDeleted = false
                    },
                    new Field()
                    {
                        Id = 102,
                        Title = "Payment Method",
                        MapField = "PaymentMethod",
                        FieldTypeId = 2, // Dropdown
                        IsDeleted = false
                    },
                    new Field()
                    {
                        Id = 103,
                        Title = "Has Passport",
                        MapField = "HasPassport",
                        FieldTypeId = 1, // Boolean
                        IsDeleted = false
                    }
                };

                foreach (var f in desiredFields)
                {
                    if (!context.Fields.IgnoreQueryFilters().Any(e => e.Id == f.Id))
                        context.Fields.Add(f);
                }

                context.SaveChanges();
            }
            else
            {
                // If the range check returned true it means at least one exists,
                // but some specific ids might be missing. Ensure each id is present.
                var ensureFields = new List<Field>
            {
                new Field { Id = 98, Title = "Player Position", MapField = "PlayerPosition", FieldTypeId = 2, IsDeleted = false },
                new Field { Id = 99, Title = "Has Club Experience", MapField = "HasClubExperience", FieldTypeId = 1, IsDeleted = false },
                new Field { Id = 100, Title = "Club Name", MapField = "ClubName", FieldTypeId = 1, IsDeleted = false },
                new Field { Id = 101, Title = "سن اللاعب", MapField = "PlayerAge", FieldTypeId = 2, IsDeleted = false },
                new Field { Id = 102, Title = "Payment Method", MapField = "PaymentMethod", FieldTypeId = 2, IsDeleted = false },
                new Field { Id = 103, Title = "Has Passport", MapField = "HasPassport", FieldTypeId = 1, IsDeleted = false }
            };

                foreach (var f in ensureFields)
                {
                    if (!context.Fields.IgnoreQueryFilters().Any(e => e.Id == f.Id))
                        context.Fields.Add(f);
                }

                context.SaveChanges();
            }
            var fields = new List<Field>
            {
                new Field { Id = 105, Title = "Player Code", MapField = "PlayerCode", FieldTypeId = 1, IsDeleted = false },
                new Field { Id = 106, Title = "MedicalFileData", MapField = "MedicalFileData", FieldTypeId = 3, IsDeleted = false },
                new Field { Id = 107, Title = "Frist Acadimy Membership Date", MapField = "FristAcadimyMembershipDate", FieldTypeId = 3, IsDeleted = false }
            };

            foreach (var field in fields)
            {
                if (!context.Fields.IgnoreQueryFilters().Any(f => f.Id == field.Id))
                {
                    context.Fields.Add(field);
                }
            }

            context.SaveChanges();
            // Now seed dropdown values, but only if the referenced Field exists.
            // This prevents FK constraint failures when a dropdown is inserted while its Field is missing.

            if (context.Fields.IgnoreQueryFilters().Any(f => f.Id == 98)
                && !context.DropdownValue.IgnoreQueryFilters().Any(d => d.FieldId == 98))
            {
                context.DropdownValue.AddRange(
                    new DropdownValue { Id = 74, Value = "GK", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 75, Value = "CB", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 76, Value = "RB", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 77, Value = "LB", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 78, Value = "CM", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 79, Value = "RW", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 80, Value = "LW", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 81, Value = "ST", FieldId = 98, IsDeleted = false }
                );
                context.SaveChanges();
            }

            if (context.Fields.IgnoreQueryFilters().Any(f => f.Id == 101)
      && !context.DropdownValue.IgnoreQueryFilters().Any(d => d.FieldId == 101))
            {
                context.DropdownValue.AddRange(
                    new DropdownValue { Id = 88, Value = "Under 8", FieldId = 101, IsDeleted = false },
                    new DropdownValue { Id = 89, Value = "Under 10", FieldId = 101, IsDeleted = false },
                    new DropdownValue { Id = 90, Value = "Under 12", FieldId = 101, IsDeleted = false },
                    new DropdownValue { Id = 91, Value = "Under 14", FieldId = 101, IsDeleted = false },
                    new DropdownValue { Id = 92, Value = "Under 16", FieldId = 101, IsDeleted = false }
                // Add whatever values match your use case
                );
                context.SaveChanges();
            }
            if (context.Fields.IgnoreQueryFilters().Any(f => f.Id == 102)
                && !context.DropdownValue.IgnoreQueryFilters().Any(d => d.FieldId == 102))
            {
                context.DropdownValue.AddRange(
                    new DropdownValue { Id = 86, Value = "Vodafone Cash", FieldId = 102, IsDeleted = false },
                    new DropdownValue { Id = 87, Value = "Instapay", FieldId = 102, IsDeleted = false }
                );
                context.SaveChanges();
            }
            if (!context.FieldCategory.IgnoreQueryFilters().Any())
            {
                context.FieldCategory.AddRange(
                    new FieldCategory() { Id = 1, Name = "Core Attributes", IsDeleted = false },
                    new FieldCategory() { Id = 2, Name = "Optional Data", IsDeleted = false },
                    new FieldCategory() { Id = 3, Name = "Documents", IsDeleted = false });
                context.SaveChanges();
            }
            if (!context.RequestTypeFields.IgnoreQueryFilters().Any())
            {
                context.RequestTypeFields.AddRange(
                new RequestTypeFields() { RequestTypeId = 12, FieldId = 22, FieldCategoryId = 1, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 12, FieldId = 24, FieldCategoryId = 1, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 18, FieldId = 49, FieldCategoryId = 1, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 51, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 51, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 52, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 52, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 53, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 53, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 54, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 54, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 55, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 55, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 56, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 56, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 57, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 57, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 58, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 58, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 59, FieldCategoryId = 2, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 59, FieldCategoryId = 2, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 60, FieldCategoryId = 1, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 60, FieldCategoryId = 1, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 61, FieldCategoryId = 1, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 61, FieldCategoryId = 1, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 62, FieldCategoryId = 3, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 62, FieldCategoryId = 3, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 63, FieldCategoryId = 3, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 63, FieldCategoryId = 3, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 18, FieldId = 64, FieldCategoryId = 2, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 12, FieldId = 77, FieldCategoryId = 2, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 12, FieldId = 71, FieldCategoryId = 2, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 12, FieldId = 75, FieldCategoryId = 3, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 16, FieldId = 76, FieldCategoryId = 3, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 18, FieldId = 71, FieldCategoryId = 1, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 18, FieldId = 75, FieldCategoryId = 3, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 17, FieldId = 70, FieldCategoryId = 1, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 17, FieldId = 69, FieldCategoryId = 2, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 17, FieldId = 71, FieldCategoryId = 2, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 17, FieldId = 75, FieldCategoryId = 3, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                new RequestTypeFields() { RequestTypeId = 6, FieldId = 76, FieldCategoryId = 3, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false }
            







                );
                context.SaveChanges();
            }


            if (!context.RequestTypeFields.IgnoreQueryFilters().Any(rtf => rtf.RequestTypeId == 21))
            {
                context.RequestTypeFields.AddRange(
                    new RequestTypeFields() { RequestTypeId = 21, FieldId = 51, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                    new RequestTypeFields() { RequestTypeId = 21, FieldId = 52, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                    new RequestTypeFields() { RequestTypeId = 21, FieldId = 53, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                    new RequestTypeFields() { RequestTypeId = 21, FieldId = 55, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                    new RequestTypeFields() { RequestTypeId = 21, FieldId = 56, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                    new RequestTypeFields() { RequestTypeId = 21, FieldId = 57, FieldCategoryId = 1, IsRequired = true, IsEditable = null, IsChecked = null, IsDeleted = false },
                    new RequestTypeFields() { RequestTypeId = 21, FieldId = 59, FieldCategoryId = 2, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                    new RequestTypeFields() { RequestTypeId = 21, FieldId = 62, FieldCategoryId = 3, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                    new RequestTypeFields() { RequestTypeId = 21, FieldId = 63, FieldCategoryId = 3, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false },
                    new RequestTypeFields() { RequestTypeId = 21, FieldId = 76, FieldCategoryId = 2, IsRequired = null, IsEditable = null, IsChecked = null, IsDeleted = false }
                );
                context.SaveChanges();
            }

            if (!context.Configurations.IgnoreQueryFilters().Any())
            {
                context.AddRange(
                    new Configuration() { Id = 1, Name = "Freeze Configuration", IsDeleted = false, RequestTypeId = 12 },
                    new Configuration() { Id = 2, Name = "Cancellation Configuration", IsDeleted = false, RequestTypeId = 17 },
                    new Configuration() { Id = 3, Name = "Remove configuaration", IsDeleted = false, RequestTypeId = 18 },
                    new Configuration() { Id = 4, Name = "Addition Configuration", IsDeleted = false, RequestTypeId = 16 },
                    new Configuration() { Id = 5, Name = "New Membership Configuration", IsDeleted = false, RequestTypeId = 6 },
                    new Configuration() { Id = 6, Name = "Renewal Configuration", IsDeleted = false, RequestTypeId = 7 },
                    new Configuration() { Id = 7, Name = "Transfer Ownership Configuration", IsDeleted = false, RequestTypeId = 8 },
                    new Configuration() { Id = 8, Name = "Separaion Configuration", IsDeleted = false, RequestTypeId = 9 },
                    new Configuration() { Id = 9, Name = "Altration Confiuratioins", IsDeleted = false, RequestTypeId = 10 },
                    new Configuration() { Id = 10, Name = "Transfer Location Configuration", IsDeleted = false, RequestTypeId = 15 });
                context.SaveChanges();
            }

            if (!context.Configurations.Any(c => c.Name == "ParentApplication"))
            {
                context.Configurations.Add(new Configuration() { Id = 11, Name = "ParentApplication", IsDeleted = false, RequestTypeId = 21 });
                context.SaveChanges();
            }

            if (!context.PackageConfigurationSections.IgnoreQueryFilters().Any())
            {
                context.AddRange(
                    new PackageConfigurationSection() { Id = 1, ConfigurationId = 1, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 2, Name = "Cancellation Fees", ConfigurationId = 2, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 3, Name = "List of potential reasons for cancellation", ConfigurationId = 2, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 4, Name = "Removal Fees", ConfigurationId = 3, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 5, Name = "List of potential reasons for Removal", ConfigurationId = 3, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 6, Name = "Section add Config", ConfigurationId = 4, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 7, ConfigurationId = 5, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 8, Name = "Separation Fees", ConfigurationId = 8, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 9, Name = "List of potential reasons for Separation", ConfigurationId = 8, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 10, Name = "List of potential reasons for Transfer Ownership", ConfigurationId = 7, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 11, ConfigurationId = 6, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 12, ConfigurationId = 6, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 13, ConfigurationId = 6, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 14, ConfigurationId = 6, IsDeleted = false },
                    new PackageConfigurationSection() { Id = 15, ConfigurationId = 6, IsDeleted = false });
                context.SaveChanges();
            }
            if (!context.ConfigurationFields.IgnoreQueryFilters().Any())
            {
                context.AddRange(
                    new ConfigurationField() { Id = 1, Name = "Freeze Limit", FieldTypeId = 4, IsDeleted = false, PackageConfigurationSectionId = 1 },
                    new ConfigurationField() { Id = 2, Name = "Can Exceed Freeze Limit", FieldTypeId = 5, IsDeleted = false, PackageConfigurationSectionId = 1 },
                    new ConfigurationField() { Id = 3, Name = "Price Per Additional Day", FieldTypeId = 4, IsDeleted = false, PackageConfigurationSectionId = 1 },
                    new ConfigurationField() { Id = 4, Name = "Realocation", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 3 },
                    new ConfigurationField() { Id = 5, Name = "Financial Reasons", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 3 },
                    new ConfigurationField() { Id = 6, Name = "Personal Reasons", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 3 },
                    new ConfigurationField() { Id = 7, Name = "Lack of use", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 3 },
                    new ConfigurationField() { Id = 8, Name = "Technical Issues", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 3 },
                    new ConfigurationField() { Id = 9, Name = "Wife", FieldTypeId = 2, IsDeleted = false, PackageConfigurationSectionId = 6 },
                    new ConfigurationField() { Id = 10, Name = "Son", FieldTypeId = 2, IsDeleted = false, PackageConfigurationSectionId = 6 },
                    new ConfigurationField() { Id = 11, Name = "Realocation", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 5 },
                    new ConfigurationField() { Id = 12, Name = "", FieldTypeId = 4, IsDeleted = false, PackageConfigurationSectionId = 2 },
                    new ConfigurationField() { Id = 13, Name = "Daughter", FieldTypeId = 2, IsDeleted = false, PackageConfigurationSectionId = 6 },
                    new ConfigurationField() { Id = 14, Name = "Main Member", FieldTypeId = 2, IsDeleted = false, PackageConfigurationSectionId = 7 },
                    //new ConfigurationField() { Id = 15, Name = "Wife", FieldTypeId = 2, IsDeleted = false, PackageConfigurationSectionId = 7 },
                    //new ConfigurationField() { Id = 17, Name = "Son", FieldTypeId = 2, IsDeleted = false, PackageConfigurationSectionId = 7 },
                    //new ConfigurationField() { Id = 18, Name = "Daughter", FieldTypeId = 2, IsDeleted = false, PackageConfigurationSectionId = 7 },
                    //new ConfigurationField() { Id = 19, Name = "Brother", FieldTypeId = 2, IsDeleted = false, PackageConfigurationSectionId = 7 },
                    new ConfigurationField() { Id = 20, Name = "Dissatisfaction with Services", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 3 },
                    new ConfigurationField() { Id = 21, Name = "Change in Schedule Program or Class Discontinuation", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 3 },
                    new ConfigurationField() { Id = 22, Name = "Family Commitments", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 3 },
                    new ConfigurationField() { Id = 23, Name = "Travel", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 3 },
                    new ConfigurationField() { Id = 24, Name = "Membership Benefits Contractual Issues", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 3 },
                    new ConfigurationField() { Id = 25, Name = "Service Relocation", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 3 },
                    new ConfigurationField() { Id = 26, Name = "Brother", FieldTypeId = 2, IsDeleted = false, PackageConfigurationSectionId = 6 },
                    new ConfigurationField() { Id = 27, Name = "", FieldTypeId = 4, IsDeleted = false, PackageConfigurationSectionId = 8 },
                    new ConfigurationField() { Id = 28, Name = "Realocation", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 9 },
                    new ConfigurationField() { Id = 29, Name = "Financial Reasons", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 9 },
                    new ConfigurationField() { Id = 30, Name = "Personal Reasons", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 9 },
                    new ConfigurationField() { Id = 31, Name = "Lack of use", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 9 },
                    new ConfigurationField() { Id = 32, Name = "Technical Issues", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 9 },
                    new ConfigurationField() { Id = 33, Name = "Travel", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 9 },
                    new ConfigurationField() { Id = 34, Name = "Service Relocation", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 9 },
                    new ConfigurationField() { Id = 35, Name = "Membership Benefits Contractual Issues", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 9 },
                    new ConfigurationField() { Id = 36, Name = "Personal Reasons", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 10 },
                    new ConfigurationField() { Id = 37, Name = "Financial Reasons", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 10 },
                    new ConfigurationField() { Id = 38, Name = "Auto Renwal", FieldTypeId = 5, IsDeleted = false, PackageConfigurationSectionId = 11 },
                    new ConfigurationField() { Id = 39, Name = "Duration", FieldTypeId = 4, IsDeleted = false, PackageConfigurationSectionId = 12 },
                    new ConfigurationField() { Id = 40, Name = "Price", FieldTypeId = 4, IsDeleted = false, PackageConfigurationSectionId = 12 },
                    new ConfigurationField() { Id = 41, Name = "Early Renwal", FieldTypeId = 5, IsDeleted = false, PackageConfigurationSectionId = 13 },
                    new ConfigurationField() { Id = 42, Name = "Duration", FieldTypeId = 4, IsDeleted = false, PackageConfigurationSectionId = 13 },
                    new ConfigurationField() { Id = 43, Name = "Form Resubmission", FieldTypeId = 5, IsDeleted = false, PackageConfigurationSectionId = 14 },
                    new ConfigurationField() { Id = 44, Name = "Early Renwal", FieldTypeId = 5, IsDeleted = false, PackageConfigurationSectionId = 15 },
                    new ConfigurationField() { Id = 45, Name = "Specific number", FieldTypeId = 9, IsDeleted = false, PackageConfigurationSectionId = 15 },
                    new ConfigurationField() { Id = 46, Name = "", FieldTypeId = 4, IsDeleted = false, PackageConfigurationSectionId = 15 },
                    new ConfigurationField() { Id = 47, Name = "unlimited", FieldTypeId = 9, IsDeleted = false, PackageConfigurationSectionId = 15 },
                    new ConfigurationField() { Id = 48, Name = "  ", FieldTypeId = 4, IsDeleted = false, PackageConfigurationSectionId = 4 },
                    new ConfigurationField() { Id = 52, Name = "Financial Reasons", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 5 },
                    new ConfigurationField() { Id = 53, Name = "Personal Reasons", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 5 },
                    new ConfigurationField() { Id = 54, Name = "Lack of use", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 5 },
                    new ConfigurationField() { Id = 55, Name = "Technical Issues", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 5 },
                    new ConfigurationField() { Id = 56, Name = "Dissatisfaction with Services", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 5 },
                    new ConfigurationField() { Id = 57, Name = "Family Commitments", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 5 },
                    new ConfigurationField() { Id = 58, Name = "Travel", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 5 },
                    new ConfigurationField() { Id = 59, Name = "Membership Benefits Contractual Issues", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 5 },
                    new ConfigurationField() { Id = 60, Name = "Service Relocation", FieldTypeId = 7, IsDeleted = false, PackageConfigurationSectionId = 5 });
                context.SaveChanges();
            }
            if (!context.DropdownValue.IgnoreQueryFilters().Any())
            {
                context.AddRange(
                    new DropdownValue() { Id = 1, Value = "Male", FieldId = 54, IsDeleted = false },
                    new DropdownValue() { Id = 2, Value = "Female", FieldId = 54, IsDeleted = false },
                    new DropdownValue() { Id = 3, Value = "Egyptian", FieldId = 55, IsDeleted = false },
                    //Freeze
                    new DropdownValue() { Id = 4, Value = "Relocation", FieldId = 77, IsDeleted = false },
                    new DropdownValue() { Id = 5, Value = "Financial Reasons", FieldId = 77, IsDeleted = false },
                    new DropdownValue() { Id = 6, Value = "Personal Reasons", FieldId = 77, IsDeleted = false },
                    new DropdownValue() { Id = 7, Value = "Lack of Use", FieldId = 77, IsDeleted = false },
                    new DropdownValue() { Id = 8, Value = "Dissatisfaction with Services", FieldId = 77, IsDeleted = false },
                    new DropdownValue() { Id = 9, Value = "Change in Schedule Program or Class Discontinuation", FieldId = 77, IsDeleted = false },
                    new DropdownValue() { Id = 10, Value = "Technical Issues", FieldId = 77, IsDeleted = false },
                    new DropdownValue() { Id = 11, Value = "Familly Commitments", FieldId = 77, IsDeleted = false },
                    new DropdownValue() { Id = 12, Value = "Travel", FieldId = 77, IsDeleted = false },
                    new DropdownValue() { Id = 13, Value = "Membership Benefits Contractual Issues", FieldId = 77, IsDeleted = false },
                    new DropdownValue() { Id = 14, Value = "Service Relocation", FieldId = 77, IsDeleted = false },
                    //Cancellation
                    new DropdownValue() { Id = 15, Value = "Relocation", FieldId = 69, IsDeleted = false },
                    new DropdownValue() { Id = 16, Value = "Financial Reasons", FieldId = 69, IsDeleted = false },
                    new DropdownValue() { Id = 17, Value = "Personal Reasons", FieldId = 69, IsDeleted = false },
                    new DropdownValue() { Id = 18, Value = "Lack of Use", FieldId = 69, IsDeleted = false },
                    new DropdownValue() { Id = 19, Value = "Dissatisfaction with Services", FieldId = 69, IsDeleted = false },
                    new DropdownValue() { Id = 20, Value = "Change in Schedule Program or Class Discontinuation", FieldId = 69, IsDeleted = false },
                    new DropdownValue() { Id = 21, Value = "Technical Issues", FieldId = 69, IsDeleted = false },
                    new DropdownValue() { Id = 22, Value = "Familly Commitments", FieldId = 69, IsDeleted = false },
                    new DropdownValue() { Id = 23, Value = "Travel", FieldId = 69, IsDeleted = false },
                    new DropdownValue() { Id = 24, Value = "Membership Benefits Contractual Issues", FieldId = 69, IsDeleted = false },
                    new DropdownValue() { Id = 25, Value = "Service Relocation", FieldId = 69, IsDeleted = false },
                    //Separation
                    new DropdownValue() { Id = 26, Value = "Relocation", FieldId = 65, IsDeleted = false },
                    new DropdownValue() { Id = 27, Value = "Financial Reasons", FieldId = 65, IsDeleted = false },
                    new DropdownValue() { Id = 28, Value = "Personal Reasons", FieldId = 65, IsDeleted = false },
                    new DropdownValue() { Id = 29, Value = "Lack of Use", FieldId = 65, IsDeleted = false },
                    new DropdownValue() { Id = 30, Value = "Dissatisfaction with Services", FieldId = 65, IsDeleted = false },
                    new DropdownValue() { Id = 31, Value = "Change in Schedule Program or Class Discontinuation", FieldId = 65, IsDeleted = false },
                    new DropdownValue() { Id = 32, Value = "Technical Issues", FieldId = 65, IsDeleted = false },
                    new DropdownValue() { Id = 33, Value = "Familly Commitments", FieldId = 65, IsDeleted = false },
                    new DropdownValue() { Id = 34, Value = "Travel", FieldId = 65, IsDeleted = false },
                    new DropdownValue() { Id = 35, Value = "Membership Benefits Contractual Issues", FieldId = 65, IsDeleted = false },
                    new DropdownValue() { Id = 36, Value = "Service Relocation", FieldId = 65, IsDeleted = false },
                    //Rmoval
                    new DropdownValue() { Id = 37, Value = "Relocation", FieldId = 64, IsDeleted = false },
                    new DropdownValue() { Id = 38, Value = "Financial Reasons", FieldId = 64, IsDeleted = false },
                    new DropdownValue() { Id = 39, Value = "Personal Reasons", FieldId = 64, IsDeleted = false },
                    new DropdownValue() { Id = 40, Value = "Lack of Use", FieldId = 64, IsDeleted = false },
                    new DropdownValue() { Id = 41, Value = "Dissatisfaction with Services", FieldId = 64, IsDeleted = false },
                    new DropdownValue() { Id = 42, Value = "Change in Schedule Program or Class Discontinuation", FieldId = 64, IsDeleted = false },
                    new DropdownValue() { Id = 43, Value = "Technical Issues", FieldId = 64, IsDeleted = false },
                    new DropdownValue() { Id = 44, Value = "Familly Commitments", FieldId = 64, IsDeleted = false },
                    new DropdownValue() { Id = 45, Value = "Travel", FieldId = 64, IsDeleted = false },
                    new DropdownValue() { Id = 46, Value = "Membership Benefits Contractual Issues", FieldId = 64, IsDeleted = false },
                    new DropdownValue() { Id = 47, Value = "Service Relocation", FieldId = 64, IsDeleted = false },
                  //Karata Federation Form Fnew Drobdowan values
                  new DropdownValue { Id = 48, Value = "أبيض", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 49, Value = "أصفر 1", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 50, Value = "أصفر 2", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 51, Value = "برتقالي 1", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 52, Value = "برتقالي 2", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 53, Value = "أخضر 1", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 54, Value = "أخضر 2", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 55, Value = "أزرق 1", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 56, Value = "أزرق 2", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 57, Value = "بني 1", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 58, Value = "بني 2", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 59, Value = "دان ناشئين", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 60, Value = "دان 1", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 61, Value = "دان 2", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 62, Value = "دان 3", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 63, Value = "دان 4", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 64, Value = "دان 5", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 65, Value = "دان 6", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 66, Value = "دان 7", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 67, Value = "دان 8", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 68, Value = "دان 9", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 69, Value = "دان 10", FieldId = 83, IsDeleted = false },
                  new DropdownValue { Id = 70, Value = "دان 11", FieldId = 83, IsDeleted = false },
                  //milaturtStatus
                  new DropdownValue() { Id = 71, Value = "أدي الخدمة", FieldId = 88, IsDeleted = false },
                  new DropdownValue() { Id = 72, Value = "لم يصبه الدور", FieldId = 88, IsDeleted = false },
                  new DropdownValue() { Id = 73, Value = "معفي", FieldId = 88, IsDeleted = false }

                    );
                if (!context.Operations.Any())
                {
                    context.AddRange(
                        new Operation { Id = 1, Name = "New Membership", IsDeleted = false, },
                        new Operation { Id = 2, Name = "Transfer", IsDeleted = false, },
                        new Operation { Id = 3, Name = "Loan", IsDeleted = false, }
                        );
                }

                context.SaveChanges();
            }
         
            if (!context.FedrationEntityTypes.IgnoreQueryFilters().Any())
            {
                context.FedrationEntityTypes.AddRange(
                    new FedrationEntityType { Id = 1, Name = "أكاديمية", IsDeleted = false },
                    new FedrationEntityType { Id = 2, Name = "نادي", IsDeleted = false },
                    new FedrationEntityType { Id = 3, Name = "Private Academy", IsDeleted = true },
                    new FedrationEntityType { Id = 4, Name = "Private Club", IsDeleted = true },
                    new FedrationEntityType { Id = 5, Name = "مركز تدريب", IsDeleted = false },
                    new FedrationEntityType { Id = 6, Name = "مركز شباب", IsDeleted = false }
                );
                context.SaveChanges();
            }

            #region Karata forms and FormFields
            if (!context.Forms.IgnoreQueryFilters().Any())
            {
                context.Forms.Add(new Form
                {
                    Id = 159,
                    Title = "نموذح اشتراك جديد كاراتيه",
                    Alias = "Karate Fedration New Form",
                    Fees = 1000,
                    WorkflowSelection = "Approval Workflow",
                    RequestTypeId = 6,
                    IsDeleted = false,
                    IsDraft = false
                });
                context.Forms.Add(new Form
                {
                    Id = 160,
                    Title = "نموذج نقل",
                    Alias = "Karate Transfer new Form",
                    Fees = 1000,
                    WorkflowSelection = "Approval Workflow",
                    RequestTypeId = 19,
                    IsDeleted = false,
                    IsDraft = false
                });
                context.Forms.Add(new Form
                {
                    Id = 161,
                    Title = "نموذج إعاره",
                    Alias = "Karate Loan Form",
                    Fees = 1000,
                    WorkflowSelection = "Approval Workflow",
                    RequestTypeId = 20,
                    IsDeleted = false,
                    IsDraft = false
                });
                context.Forms.Add(new Form
                {
                    Id = 162,
                    Title = "Parent & Child Membership Form",
                    Alias = "ParentChildForm",
                    Fees = 1000,
                    WorkflowSelection = "Approval Workflow",
                    RequestTypeId = 21,
                    IsDeleted = false,
                    IsDraft = false
                });
                context.SaveChanges();
            }

            if (!context.Forms.Any(x => x.Id == 162))
            {
                context.Forms.Add(new Form
                {
                    Id = 162,
                    Title = "Parent & Child Membership Form",
                    Alias = "ParentChildForm",
                    Fees = 1000,
                    WorkflowSelection = "Approval Workflow",
                    RequestTypeId = 21,
                    IsDeleted = false,
                    IsDraft = false
                });
                context.SaveChanges();
            }

          

            if (!context.FormFields.IgnoreQueryFilters().Any(ff => ff.FormId == 159))
            {
                context.FormFields.AddRange(
                   new FormFields { FormId = 159, FieldId = 51, IsRequired = true, IsEditable = true, Index = 1, IsDeleted = false }, // First Name
                    new FormFields { FormId = 159, FieldId = 52, IsRequired = true, IsEditable = true, Index = 2, IsDeleted = false }, // Last Name
                    new FormFields { FormId = 159, FieldId = 53, IsRequired = true, IsEditable = true, Index = 3, IsDeleted = false }, // Birth Date
                    new FormFields { FormId = 159, FieldId = 54, IsRequired = true, IsEditable = true, Index = 4, IsDeleted = false }, // Gender

                    new FormFields { FormId = 159, FieldId = 56, IsRequired = false, IsEditable = true, Index = 5, IsDeleted = false }, // Email
                    new FormFields { FormId = 159, FieldId = 57, IsRequired = true, IsEditable = true, Index = 6, IsDeleted = false }, // Phone Number
                    new FormFields { FormId = 159, FieldId = 58, IsRequired = true, IsEditable = true, Index = 7, IsDeleted = false }, // National ID
                    new FormFields { FormId = 159, FieldId = 59, IsRequired = false, IsEditable = false, Index = 8, IsDeleted = false }, // Address

                    new FormFields { FormId = 159, FieldId = 60, IsRequired = true, IsEditable = true, Index = 9, IsDeleted = false }, // Second Name
                    new FormFields { FormId = 159, FieldId = 61, IsRequired = true, IsEditable = true, Index = 10, IsDeleted = false }, // Middle Name

                    new FormFields { FormId = 159, FieldId = 62, IsRequired = false, IsEditable = true, Index = 11, IsDeleted = false }, // Birth Certificate
                    new FormFields { FormId = 159, FieldId = 63, IsRequired = false, IsEditable = false, Index = 12, IsDeleted = false }, // National ID/Passport

                    new FormFields { FormId = 159, FieldId = 76, IsRequired = false, IsEditable = false, Index = 13, IsDeleted = false }, // Personal Photo

                    new FormFields { FormId = 159, FieldId = 78, IsRequired = false, IsEditable = true, Index = 14, IsDeleted = false },
                    new FormFields { FormId = 159, FieldId = 79, IsRequired = false, IsEditable = true, Index = 15, IsDeleted = false },
                    new FormFields { FormId = 159, FieldId = 80, IsRequired = false, IsEditable = true, Index = 16, IsDeleted = false },
                    new FormFields { FormId = 159, FieldId = 81, IsRequired = false, IsEditable = true, Index = 17, IsDeleted = false },
                    new FormFields { FormId = 159, FieldId = 82, IsRequired = false, IsEditable = true, Index = 18, IsDeleted = false },

                    new FormFields { FormId = 159, FieldId = 83, IsRequired = false, IsEditable = true, Index = 19, IsDeleted = false }, // Level

                    new FormFields { FormId = 159, FieldId = 84, IsRequired = false, IsEditable = true, Index = 20, IsDeleted = false }, // Passport Number
                    new FormFields { FormId = 159, FieldId = 85, IsRequired = false, IsEditable = true, Index = 21, IsDeleted = false }, // File Number
                    new FormFields { FormId = 159, FieldId = 86, IsRequired = false, IsEditable = true, Index = 22, IsDeleted = false }, // Belt Certificate
                    new FormFields { FormId = 159, FieldId = 87, IsRequired = false, IsEditable = true, Index = 23, IsDeleted = false }, // Birth Certificate/National ID
                    new FormFields { FormId = 159, FieldId = 88, IsRequired = false, IsEditable = true, Index = 24, IsDeleted = false }, // Military Status

                    new FormFields { FormId = 159, FieldId = 89, IsRequired = true, IsEditable = true, Index = 25, IsDeleted = false }  // Profile Image
                );

                context.SaveChanges();
            }
            if (!context.FormFields.IgnoreQueryFilters().Any(ff => ff.FormId == 160))
            {
                context.FormFields.AddRange(
                    new FormFields { FormId = 160, FieldId = 51, IsRequired = false, IsEditable = true, Index = 1, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 52, IsRequired = false, IsEditable = true, Index = 2, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 53, IsRequired = false, IsEditable = true, Index = 3, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 54, IsRequired = false, IsEditable = true, Index = 4, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 56, IsRequired = false, IsEditable = true, Index = 5, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 57, IsRequired = false, IsEditable = true, Index = 6, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 58, IsRequired = false, IsEditable = true, Index = 7, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 59, IsRequired = false, IsEditable = false, Index = 8, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 60, IsRequired = false, IsEditable = true, Index = 9, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 61, IsRequired = false, IsEditable = true, Index = 10, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 62, IsRequired = false, IsEditable = true, Index = 11, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 63, IsRequired = false, IsEditable = false, Index = 12, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 76, IsRequired = false, IsEditable = false, Index = 13, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 78, IsRequired = false, IsEditable = true, Index = 14, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 79, IsRequired = false, IsEditable = true, Index = 15, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 80, IsRequired = false, IsEditable = true, Index = 16, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 81, IsRequired = false, IsEditable = true, Index = 17, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 82, IsRequired = false, IsEditable = true, Index = 18, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 83, IsRequired = false, IsEditable = true, Index = 19, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 84, IsRequired = false, IsEditable = true, Index = 20, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 85, IsRequired = false, IsEditable = true, Index = 21, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 87, IsRequired = false, IsEditable = true, Index = 23, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 88, IsRequired = false, IsEditable = true, Index = 24, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 89, IsRequired = false, IsEditable = true, Index = 25, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 90, IsRequired = false, IsEditable = true, Index = 26, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 91, IsRequired = false, IsEditable = true, Index = 27, IsDeleted = false },
                    new FormFields { FormId = 160, FieldId = 92, IsRequired = false, IsEditable = true, Index = 28, IsDeleted = false }

                );

                context.SaveChanges();
            }

            if (!context.FormFields.IgnoreQueryFilters().Any(ff => ff.FormId == 161))
            {
                context.FormFields.AddRange(
                    new FormFields { FormId = 161, FieldId = 85, IsRequired = false, IsEditable = true, Index = 1, IsDeleted = false },
                    new FormFields { FormId = 161, FieldId = 86, IsRequired = false, IsEditable = true, Index = 1, IsDeleted = false },
                    new FormFields { FormId = 161, FieldId = 90, IsRequired = false, IsEditable = true, Index = 2, IsDeleted = false },
                    new FormFields { FormId = 161, FieldId = 91, IsRequired = false, IsEditable = true, Index = 3, IsDeleted = false },
                    new FormFields { FormId = 161, FieldId = 92, IsRequired = false, IsEditable = true, Index = 4, IsDeleted = false }
                );

                context.SaveChanges();
            }
            //Add fields for the merged form
            if (!context.FormFields.IgnoreQueryFilters().Any(ff => ff.FormId == 162))
            {
                context.FormFields.AddRange(
                    // Start / End Date
                    new FormFields { FormId = 162, FieldId = 22, IsRequired = true, IsEditable = true, Index = 1, IsDeleted = false, IsHidden = true }, // Start Date
                    new FormFields { FormId = 162, FieldId = 23, IsRequired = true, IsEditable = true, Index = 2, IsDeleted = false, IsHidden = true }, // End Date

                    // ---------------- Parent & child fields ----------------
                    new FormFields { FormId = 162, FieldId = 51, IsRequired = true, IsEditable = true, Index = 3, IsDeleted = false, IsHidden = false },
                    new FormFields { FormId = 162, FieldId = 60, IsRequired = true, IsEditable = true, Index = 4, IsDeleted = false, IsHidden = false },
                    new FormFields { FormId = 162, FieldId = 61, IsRequired = true, IsEditable = true, Index = 5, IsDeleted = false, IsHidden = false },
                    new FormFields { FormId = 162, FieldId = 52, IsRequired = true, IsEditable = true, Index = 6, IsDeleted = false, IsHidden = false },
                    new FormFields { FormId = 162, FieldId = 56, IsRequired = false, IsEditable = true, Index = 7, IsDeleted = false, IsHidden = false },
                    new FormFields { FormId = 162, FieldId = 57, IsRequired = false, IsEditable = true, Index = 8, IsDeleted = false, IsHidden = false },
                    new FormFields { FormId = 162, FieldId = 53, IsRequired = false, IsEditable = true, Index = 9, IsDeleted = false, IsHidden = false },
                    new FormFields { FormId = 162, FieldId = 54, IsRequired = false, IsEditable = true, Index = 10, IsDeleted = false, IsHidden = false },
                    new FormFields { FormId = 162, FieldId = 58, IsRequired = false, IsEditable = true, Index = 11, IsDeleted = false, IsHidden = false },
                    new FormFields { FormId = 162, FieldId = 62, IsRequired = false, IsEditable = true, Index = 12, IsDeleted = false, IsHidden = true },
                    new FormFields { FormId = 162, FieldId = 89, IsRequired = false, IsEditable = true, Index = 13, IsDeleted = false, IsHidden = true },
                    new FormFields { FormId = 162, FieldId = 94, IsRequired = false, IsEditable = true, Index = 14, IsDeleted = false, IsHidden = true },
                    new FormFields { FormId = 162, FieldId = 83, IsRequired = false, IsEditable = true, Index = 15, IsDeleted = false, IsHidden = true },//level
                    new FormFields { FormId = 162, FieldId = 85, IsRequired = false, IsEditable = true, Index = 16, IsDeleted = false, IsHidden = true },
                    new FormFields { FormId = 162, FieldId = 95, IsRequired = false, IsEditable = true, Index = 17, IsDeleted = false, IsHidden = true },
                    new FormFields { FormId = 162, FieldId = 24, IsRequired = true, IsEditable = true, Index = 18, IsDeleted = false, IsHidden = false },
                    new FormFields { FormId = 162, FieldId = 96, IsRequired = true, IsEditable = true, Index = 19, IsDeleted = false, IsHidden = true },//selrcted package
                    new FormFields { FormId = 162, FieldId = 97, IsRequired = false, IsEditable = true, Index = 20, IsDeleted = false, IsHidden = true }//levelid







                );

                context.SaveChanges();
            }

            int parentFormId = 162;

            var newFieldIds = new[] { 105, 106, 107 };

            var existing = context.FormFields
                .IgnoreQueryFilters()
                .Any(f => f.FormId == parentFormId && newFieldIds.Contains(f.FieldId));

            if (!existing)
            {
                int nextIndex = context.FormFields
                    .Where(f => f.FormId == parentFormId)
                    .Select(f => (int?)f.Index)
                    .Max() ?? 0;

                context.FormFields.AddRange(
                    new FormFields { FormId = parentFormId, FieldId = 105, IsRequired = false, IsEditable = true, IsHidden = false, Index = ++nextIndex, IsDeleted = false },
                    new FormFields { FormId = parentFormId, FieldId = 106, IsRequired = false, IsEditable = true, IsHidden = false, Index = ++nextIndex, IsDeleted = false },
                    new FormFields { FormId = parentFormId, FieldId = 107, IsRequired = false, IsEditable = true, IsHidden = false, Index = ++nextIndex, IsDeleted = false }
                );

                context.SaveChanges();
            }


            if (!context.MemberRequestTypes.IgnoreQueryFilters().Any())
            {
                context.MemberRequestTypes.AddRange(
                    // RequestType 6 - New Membership Request (Main Member only)
                    new Data.Membership.MemberRequestType { Id = 1, RequestTypeId = 6, MemberTypeId = 5, IsDeleted = false },

                    // RequestType 17 - Cancellation Request (Main Member only)
                    new Data.Membership.MemberRequestType { Id = 2, RequestTypeId = 17, MemberTypeId = 5, IsDeleted = false },

                    // RequestType 18 - Removal Request (all except Main Member)
                    new Data.Membership.MemberRequestType { Id = 3, RequestTypeId = 18, MemberTypeId = 1, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 4, RequestTypeId = 18, MemberTypeId = 2, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 5, RequestTypeId = 18, MemberTypeId = 3, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 6, RequestTypeId = 18, MemberTypeId = 4, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 7, RequestTypeId = 18, MemberTypeId = 7, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 8, RequestTypeId = 18, MemberTypeId = 8, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 9, RequestTypeId = 18, MemberTypeId = 9, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 10, RequestTypeId = 18, MemberTypeId = 10, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 11, RequestTypeId = 18, MemberTypeId = 11, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 12, RequestTypeId = 18, MemberTypeId = 12, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 13, RequestTypeId = 18, MemberTypeId = 13, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 14, RequestTypeId = 18, MemberTypeId = 14, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 15, RequestTypeId = 18, MemberTypeId = 15, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 16, RequestTypeId = 18, MemberTypeId = 16, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 17, RequestTypeId = 18, MemberTypeId = 17, IsDeleted = false },

                    // RequestType 9 - Separation Request (all except Main Member)
                    new Data.Membership.MemberRequestType { Id = 18, RequestTypeId = 9, MemberTypeId = 1, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 19, RequestTypeId = 9, MemberTypeId = 2, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 20, RequestTypeId = 9, MemberTypeId = 3, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 21, RequestTypeId = 9, MemberTypeId = 4, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 22, RequestTypeId = 9, MemberTypeId = 7, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 23, RequestTypeId = 9, MemberTypeId = 8, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 24, RequestTypeId = 9, MemberTypeId = 9, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 25, RequestTypeId = 9, MemberTypeId = 10, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 26, RequestTypeId = 9, MemberTypeId = 11, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 27, RequestTypeId = 9, MemberTypeId = 12, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 28, RequestTypeId = 9, MemberTypeId = 13, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 29, RequestTypeId = 9, MemberTypeId = 14, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 30, RequestTypeId = 9, MemberTypeId = 15, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 31, RequestTypeId = 9, MemberTypeId = 16, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 32, RequestTypeId = 9, MemberTypeId = 17, IsDeleted = false },

                    // RequestType 16 - Addition Request (all except Main Member)
                    new Data.Membership.MemberRequestType { Id = 33, RequestTypeId = 16, MemberTypeId = 1, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 34, RequestTypeId = 16, MemberTypeId = 2, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 35, RequestTypeId = 16, MemberTypeId = 3, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 36, RequestTypeId = 16, MemberTypeId = 4, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 37, RequestTypeId = 16, MemberTypeId = 7, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 38, RequestTypeId = 16, MemberTypeId = 8, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 39, RequestTypeId = 16, MemberTypeId = 9, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 40, RequestTypeId = 16, MemberTypeId = 10, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 41, RequestTypeId = 16, MemberTypeId = 11, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 42, RequestTypeId = 16, MemberTypeId = 12, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 43, RequestTypeId = 16, MemberTypeId = 13, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 44, RequestTypeId = 16, MemberTypeId = 14, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 45, RequestTypeId = 16, MemberTypeId = 15, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 46, RequestTypeId = 16, MemberTypeId = 16, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 47, RequestTypeId = 16, MemberTypeId = 17, IsDeleted = false },

                    // RequestType 21 - ParentApplication (Child + Main Member)
                    new Data.Membership.MemberRequestType { Id = 48, RequestTypeId = 21, MemberTypeId = 17, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 49, RequestTypeId = 21, MemberTypeId = 5, IsDeleted = false },

                    // RequestType 7 - Renewal Request (all member types)
                    new Data.Membership.MemberRequestType { Id = 50, RequestTypeId = 7, MemberTypeId = 1, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 51, RequestTypeId = 7, MemberTypeId = 2, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 52, RequestTypeId = 7, MemberTypeId = 3, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 53, RequestTypeId = 7, MemberTypeId = 4, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 54, RequestTypeId = 7, MemberTypeId = 5, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 55, RequestTypeId = 7, MemberTypeId = 7, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 56, RequestTypeId = 7, MemberTypeId = 8, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 57, RequestTypeId = 7, MemberTypeId = 9, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 58, RequestTypeId = 7, MemberTypeId = 10, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 59, RequestTypeId = 7, MemberTypeId = 11, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 60, RequestTypeId = 7, MemberTypeId = 12, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 61, RequestTypeId = 7, MemberTypeId = 13, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 62, RequestTypeId = 7, MemberTypeId = 14, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 63, RequestTypeId = 7, MemberTypeId = 15, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 64, RequestTypeId = 7, MemberTypeId = 16, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 65, RequestTypeId = 7, MemberTypeId = 17, IsDeleted = false },

                    // RequestType 8 - Transfer Ownership Request (all member types)
                    new Data.Membership.MemberRequestType { Id = 66, RequestTypeId = 8, MemberTypeId = 1, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 67, RequestTypeId = 8, MemberTypeId = 2, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 68, RequestTypeId = 8, MemberTypeId = 3, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 69, RequestTypeId = 8, MemberTypeId = 4, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 70, RequestTypeId = 8, MemberTypeId = 5, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 71, RequestTypeId = 8, MemberTypeId = 7, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 72, RequestTypeId = 8, MemberTypeId = 8, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 73, RequestTypeId = 8, MemberTypeId = 9, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 74, RequestTypeId = 8, MemberTypeId = 10, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 75, RequestTypeId = 8, MemberTypeId = 11, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 76, RequestTypeId = 8, MemberTypeId = 12, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 77, RequestTypeId = 8, MemberTypeId = 13, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 78, RequestTypeId = 8, MemberTypeId = 14, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 79, RequestTypeId = 8, MemberTypeId = 15, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 80, RequestTypeId = 8, MemberTypeId = 16, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 81, RequestTypeId = 8, MemberTypeId = 17, IsDeleted = false },

                    // RequestType 10 - Alteration Request (all member types)
                    new Data.Membership.MemberRequestType { Id = 82, RequestTypeId = 10, MemberTypeId = 1, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 83, RequestTypeId = 10, MemberTypeId = 2, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 84, RequestTypeId = 10, MemberTypeId = 3, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 85, RequestTypeId = 10, MemberTypeId = 4, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 86, RequestTypeId = 10, MemberTypeId = 5, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 87, RequestTypeId = 10, MemberTypeId = 7, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 88, RequestTypeId = 10, MemberTypeId = 8, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 89, RequestTypeId = 10, MemberTypeId = 9, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 90, RequestTypeId = 10, MemberTypeId = 10, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 91, RequestTypeId = 10, MemberTypeId = 11, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 92, RequestTypeId = 10, MemberTypeId = 12, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 93, RequestTypeId = 10, MemberTypeId = 13, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 94, RequestTypeId = 10, MemberTypeId = 14, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 95, RequestTypeId = 10, MemberTypeId = 15, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 96, RequestTypeId = 10, MemberTypeId = 16, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 97, RequestTypeId = 10, MemberTypeId = 17, IsDeleted = false },

                    // RequestType 12 - Freeze Request (all member types)
                    new Data.Membership.MemberRequestType { Id = 98, RequestTypeId = 12, MemberTypeId = 1, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 99, RequestTypeId = 12, MemberTypeId = 2, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 100, RequestTypeId = 12, MemberTypeId = 3, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 101, RequestTypeId = 12, MemberTypeId = 4, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 102, RequestTypeId = 12, MemberTypeId = 5, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 103, RequestTypeId = 12, MemberTypeId = 7, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 104, RequestTypeId = 12, MemberTypeId = 8, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 105, RequestTypeId = 12, MemberTypeId = 9, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 106, RequestTypeId = 12, MemberTypeId = 10, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 107, RequestTypeId = 12, MemberTypeId = 11, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 108, RequestTypeId = 12, MemberTypeId = 12, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 109, RequestTypeId = 12, MemberTypeId = 13, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 110, RequestTypeId = 12, MemberTypeId = 14, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 111, RequestTypeId = 12, MemberTypeId = 15, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 112, RequestTypeId = 12, MemberTypeId = 16, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 113, RequestTypeId = 12, MemberTypeId = 17, IsDeleted = false },

                    // RequestType 15 - Transfer Location Request (all member types)
                    new Data.Membership.MemberRequestType { Id = 114, RequestTypeId = 15, MemberTypeId = 1, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 115, RequestTypeId = 15, MemberTypeId = 2, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 116, RequestTypeId = 15, MemberTypeId = 3, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 117, RequestTypeId = 15, MemberTypeId = 4, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 118, RequestTypeId = 15, MemberTypeId = 5, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 119, RequestTypeId = 15, MemberTypeId = 7, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 120, RequestTypeId = 15, MemberTypeId = 8, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 121, RequestTypeId = 15, MemberTypeId = 9, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 122, RequestTypeId = 15, MemberTypeId = 10, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 123, RequestTypeId = 15, MemberTypeId = 11, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 124, RequestTypeId = 15, MemberTypeId = 12, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 125, RequestTypeId = 15, MemberTypeId = 13, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 126, RequestTypeId = 15, MemberTypeId = 14, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 127, RequestTypeId = 15, MemberTypeId = 15, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 128, RequestTypeId = 15, MemberTypeId = 16, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 129, RequestTypeId = 15, MemberTypeId = 17, IsDeleted = false },

                    // RequestType 19 - Transfer (all member types)
                    new Data.Membership.MemberRequestType { Id = 130, RequestTypeId = 19, MemberTypeId = 1, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 131, RequestTypeId = 19, MemberTypeId = 2, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 132, RequestTypeId = 19, MemberTypeId = 3, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 133, RequestTypeId = 19, MemberTypeId = 4, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 134, RequestTypeId = 19, MemberTypeId = 5, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 135, RequestTypeId = 19, MemberTypeId = 7, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 136, RequestTypeId = 19, MemberTypeId = 8, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 137, RequestTypeId = 19, MemberTypeId = 9, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 138, RequestTypeId = 19, MemberTypeId = 10, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 139, RequestTypeId = 19, MemberTypeId = 11, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 140, RequestTypeId = 19, MemberTypeId = 12, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 141, RequestTypeId = 19, MemberTypeId = 13, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 142, RequestTypeId = 19, MemberTypeId = 14, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 143, RequestTypeId = 19, MemberTypeId = 15, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 144, RequestTypeId = 19, MemberTypeId = 16, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 145, RequestTypeId = 19, MemberTypeId = 17, IsDeleted = false },

                    // RequestType 20 - Loan (all member types)
                    new Data.Membership.MemberRequestType { Id = 146, RequestTypeId = 20, MemberTypeId = 1, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 147, RequestTypeId = 20, MemberTypeId = 2, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 148, RequestTypeId = 20, MemberTypeId = 3, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 149, RequestTypeId = 20, MemberTypeId = 4, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 150, RequestTypeId = 20, MemberTypeId = 5, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 151, RequestTypeId = 20, MemberTypeId = 7, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 152, RequestTypeId = 20, MemberTypeId = 8, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 153, RequestTypeId = 20, MemberTypeId = 9, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 154, RequestTypeId = 20, MemberTypeId = 10, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 155, RequestTypeId = 20, MemberTypeId = 11, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 156, RequestTypeId = 20, MemberTypeId = 12, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 157, RequestTypeId = 20, MemberTypeId = 13, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 158, RequestTypeId = 20, MemberTypeId = 14, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 159, RequestTypeId = 20, MemberTypeId = 15, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 160, RequestTypeId = 20, MemberTypeId = 16, IsDeleted = false },
                    new Data.Membership.MemberRequestType { Id = 161, RequestTypeId = 20, MemberTypeId = 17, IsDeleted = false }
                );

                context.SaveChanges();
            }
            #endregion
            #region Tenet clash


            //======Fields======
            // Replace the existing block that adds fields 98..103 and the dropdown blocks
            // with the following. This adds only missing Field rows (so a partial existing
            // range won't prevent missing items from being created) and ensures Fields
            // exist before inserting DropdownValue rows.

            if (!context.Fields.IgnoreQueryFilters().Any(f => f.Id >= 98 && f.Id <= 103))
            {
                // Build the desired set and only add missing entries (handles partial seeding)
                var desiredFields = new List<Field>
                {
                    new Field()
                    {
                        Id = 98,
                        Title = "Player Position",
                        MapField = "PlayerPosition",
                        FieldTypeId = 2, // Dropdown
                        IsDeleted = false
                    },
                    new Field()
                    {
                        Id = 99,
                        Title = "Has Club Experience",
                        MapField = "HasClubExperience",
                        FieldTypeId = 1, // Boolean
                        IsDeleted = false
                    },
                    new Field()
                    {
                        Id = 100,
                        Title = "Club Name",
                        MapField = "ClubName",
                        FieldTypeId = 1, // Text
                        IsDeleted = false
                    },
                    new Field()
                    {
                        Id = 101,
                        Title = "سن اللاعب",
                        MapField = "PlayerAge",
                        FieldTypeId = 2, // Dropdown
                        IsDeleted = false
                    },
                    new Field()
                    {
                        Id = 102,
                        Title = "Payment Method",
                        MapField = "PaymentMethod",
                        FieldTypeId = 2, // Dropdown
                        IsDeleted = false
                    },
                    new Field()
                    {
                        Id = 103,
                        Title = "Has Passport",
                        MapField = "HasPassport",
                        FieldTypeId = 1, // Boolean
                        IsDeleted = false
                    }
                };

                foreach (var f in desiredFields)
                {
                    if (!context.Fields.IgnoreQueryFilters().Any(e => e.Id == f.Id))
                        context.Fields.Add(f);
                }

                context.SaveChanges();
            }
            else
            {
                // If the range check returned true it means at least one exists,
                // but some specific ids might be missing. Ensure each id is present.
                var ensureFields = new List<Field>
                {
                    new Field { Id = 98, Title = "Player Position", MapField = "PlayerPosition", FieldTypeId = 2, IsDeleted = false },
                    new Field { Id = 99, Title = "Has Club Experience", MapField = "HasClubExperience", FieldTypeId = 1, IsDeleted = false },
                    new Field { Id = 100, Title = "Club Name", MapField = "ClubName", FieldTypeId = 1, IsDeleted = false },
                    new Field { Id = 101, Title = "سن اللاعب", MapField = "PlayerAge", FieldTypeId = 2, IsDeleted = false },
                    new Field { Id = 102, Title = "Payment Method", MapField = "PaymentMethod", FieldTypeId = 2, IsDeleted = false },
                    new Field { Id = 103, Title = "Has Passport", MapField = "HasPassport", FieldTypeId = 1, IsDeleted = false }
                };

                foreach (var f in ensureFields)
                {
                    if (!context.Fields.IgnoreQueryFilters().Any(e => e.Id == f.Id))
                        context.Fields.Add(f);
                }

                context.SaveChanges();
            }

            // Now seed dropdown values, but only if the referenced Field exists.
            // This prevents FK constraint failures when a dropdown is inserted while its Field is missing.

            if (context.Fields.IgnoreQueryFilters().Any(f => f.Id == 98)
                && !context.DropdownValue.IgnoreQueryFilters().Any(d => d.FieldId == 98))
            {
                context.DropdownValue.AddRange(
                    new DropdownValue { Id = 74, Value = "GK", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 75, Value = "CB", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 76, Value = "RB", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 77, Value = "LB", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 78, Value = "CM", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 79, Value = "RW", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 80, Value = "LW", FieldId = 98, IsDeleted = false },
                    new DropdownValue { Id = 81, Value = "ST", FieldId = 98, IsDeleted = false }
                );
                context.SaveChanges();
            }

            if (context.Fields.IgnoreQueryFilters().Any(f => f.Id == 101)
                && !context.DropdownValue.IgnoreQueryFilters().Any(d => d.FieldId == 101))
            {
                context.DropdownValue.AddRange(
                    new DropdownValue { Id = 88, Value = "Under 8", FieldId = 101, IsDeleted = false },
                    new DropdownValue { Id = 89, Value = "Under 10", FieldId = 101, IsDeleted = false },
                    new DropdownValue { Id = 90, Value = "Under 12", FieldId = 101, IsDeleted = false },
                    new DropdownValue { Id = 91, Value = "Under 14", FieldId = 101, IsDeleted = false },
                    new DropdownValue { Id = 92, Value = "Under 16", FieldId = 101, IsDeleted = false }
                // Add whatever values match your use case
                );
                context.SaveChanges();
            }
            if (context.Fields.IgnoreQueryFilters().Any(f => f.Id == 102)
                && !context.DropdownValue.IgnoreQueryFilters().Any(d => d.FieldId == 102))
            {
                context.DropdownValue.AddRange(
                    new DropdownValue { Id = 86, Value = "Vodafone Cash", FieldId = 102, IsDeleted = false },
                    new DropdownValue { Id = 87, Value = "Instapay", FieldId = 102, IsDeleted = false }
                );
                context.SaveChanges();
            }

            // ==== Forms ====
            if (!context.Forms.Any(f => f.Id == 163))
            {
                context.Forms.Add(new Form
                {
                    Id = 163,
                    Title = "Talent Clash Event - Pre Register(Friday Event)",
                    Alias = "TalentClashForm",
                    Fees = 0,
                    WorkflowSelection = "Approval Workflow",
                    RequestTypeId = 21,
                    IsDeleted = false,
                    IsDraft = false
                });
                context.SaveChanges();

            }
            if (!context.Forms.Any(f => f.Id == 164))
            {
                context.Forms.Add(new Form
                {
                    Id = 164,
                    Title = "Talent Clash Event - Pre Register(Saturday Event)",
                    Alias = "TalentClashForm",
                    Fees = 0,
                    WorkflowSelection = "Approval Workflow",
                    RequestTypeId = 21,
                    IsDeleted = false,
                    IsDraft = false
                });
                context.SaveChanges();

            }
            if (!context.Forms.Any(f => f.Id == 165))
            {
                context.Forms.Add(new Form
                {
                    Id = 165,
                    Title = "Talent Clash Event - Pre Register(Sunday Event)",
                    Alias = "TalentClashForm",
                    Fees = 0,
                    WorkflowSelection = "Approval Workflow",
                    RequestTypeId = 21,
                    IsDeleted = false,
                    IsDraft = false
                });
                context.SaveChanges();

            }

            if (!context.Forms.Any(f => f.Id == 166))
            {
                context.Forms.Add(new Form
                {
                    Id = 166,
                    Title = "Talent Clash Event - Pre Register (Event Three Days)",
                    Alias = "TalentClashFormEvent Three Days",
                    Fees = 0,
                    WorkflowSelection = "Approval Workflow",
                    RequestTypeId = 21,
                    IsDeleted = false,
                    IsDraft = false
                });
                context.SaveChanges();

            }



            // List of FormIds
            var formIds = new List<int> { 163, 164, 165, 166 };

            // Common fields for all forms
            var commonFields = new List<FormFields>
                {
                    new FormFields { FieldId = 51, Index = 1 },
                    new FormFields { FieldId = 60, Index = 2 },
                    new FormFields { FieldId = 61, Index = 3 },
                    new FormFields { FieldId = 52, Index = 4 },
                    new FormFields { FieldId = 101, Index = 5 }, // سن اللاعب
                    new FormFields { FieldId = 99, Index = 6 },
                    new FormFields { FieldId = 100, Index = 7 },
                    new FormFields { FieldId = 79, Index = 8 },
                    new FormFields { FieldId = 98, Index = 9 },
                    new FormFields { FieldId = 57, Index = 10 },
                    new FormFields { FieldId = 103, Index = 11}
                };

            // Add the common fields to each form
            foreach (var formId in formIds)
            {
                foreach (var field in commonFields)
                {
                    bool exists = context.FormFields
                        .IgnoreQueryFilters()
                        .AsNoTracking()
                        .Any(ff => ff.FormId == formId && ff.FieldId == field.FieldId);

                    if (!exists)
                    {
                        context.FormFields.Add(new FormFields
                        {
                            FormId = formId,
                            FieldId = field.FieldId,
                            IsRequired = field.IsRequired,
                            IsEditable = field.IsEditable,
                            Index = field.Index,
                            IsDeleted = field.IsDeleted
                        });
                    }
                }

                context.SaveChanges();
                context.ChangeTracker.Clear(); 
            }

            #endregion
            #endregion


            #region Entity

            if (!await context.EntityType.IgnoreQueryFilters().AnyAsync())
            {
                await context.EntityType.AddRangeAsync(
                    new EntityType { Id = 1, Name = "Entity" },
                    new EntityType { Id = 2, Name = "Federation" }
                    );
                await context.SaveChangesAsync();


            }
            if (!await context.AttainmentCategories.IgnoreQueryFilters().AnyAsync())
            {
                await context.AttainmentCategories.AddRangeAsync(
                    new AttainmentCategory { Id = 1, Name = "Professional Certifications", ArabicName = "الشهادات المهنية" },
                    new AttainmentCategory { Id = 2, Name = "Recognition & License", ArabicName = "الاعتراف والترخيص" },
                    new AttainmentCategory { Id = 3, Name = "Saftey Accreditation", ArabicName = "اعتماد السلامة" }
                    );
                await context.SaveChangesAsync();


            }
            if (!await context.CountryCodes.IgnoreQueryFilters().AnyAsync())
            {
                await context.CountryCodes.AddRangeAsync(
                    new CountryCodes() { Id = 1, Code = "+93", },
                    new CountryCodes() { Id = 2, Code = "+355" },
                    new CountryCodes() { Id = 3, Code = "+213" },
                    new CountryCodes() { Id = 4, Code = "+376" },
                    new CountryCodes() { Id = 5, Code = "+244" },
                    new CountryCodes() { Id = 6, Code = "+268" },
                    new CountryCodes() { Id = 7, Code = "+54" },
                    new CountryCodes() { Id = 8, Code = "+374" },
                    new CountryCodes() { Id = 9, Code = "+61" },
                    new CountryCodes() { Id = 10, Code = "+43" },
                    new CountryCodes() { Id = 11, Code = "+994" },
                    new CountryCodes() { Id = 12, Code = "+242" },
                    new CountryCodes() { Id = 13, Code = "+973" },
                    new CountryCodes() { Id = 14, Code = "+880" },
                    new CountryCodes() { Id = 15, Code = "+246" },
                    new CountryCodes() { Id = 16, Code = "+375" },
                    new CountryCodes() { Id = 17, Code = "+32" },
                    new CountryCodes() { Id = 18, Code = "+501" },
                    new CountryCodes() { Id = 19, Code = "+229" },
                    new CountryCodes() { Id = 20, Code = "+975" },
                    new CountryCodes() { Id = 21, Code = "+591" },
                    new CountryCodes() { Id = 22, Code = "+387" },
                    new CountryCodes() { Id = 23, Code = "+267" },
                    new CountryCodes() { Id = 24, Code = "+55" },
                    new CountryCodes() { Id = 25, Code = "+673" },
                    new CountryCodes() { Id = 26, Code = "+359" },
                    new CountryCodes() { Id = 27, Code = "+226" },
                    new CountryCodes() { Id = 28, Code = "+257" },
                    new CountryCodes() { Id = 29, Code = "+238" },
                    new CountryCodes() { Id = 30, Code = "+855" },
                    new CountryCodes() { Id = 31, Code = "+237" },
                    new CountryCodes() { Id = 32, Code = "+1" },
                    new CountryCodes() { Id = 33, Code = "+236" },
                    new CountryCodes() { Id = 34, Code = "+235" },
                    new CountryCodes() { Id = 35, Code = "+56" },
                    new CountryCodes() { Id = 36, Code = "+86" },
                    new CountryCodes() { Id = 37, Code = "+57" },
                    new CountryCodes() { Id = 38, Code = "+269" },
                    new CountryCodes() { Id = 39, Code = "+243" },
                    new CountryCodes() { Id = 40, Code = "+242" },
                    new CountryCodes() { Id = 41, Code = "+506" },
                    new CountryCodes() { Id = 42, Code = "+385" },
                    new CountryCodes() { Id = 43, Code = "+53" },
                    new CountryCodes() { Id = 44, Code = "+357" },
                    new CountryCodes() { Id = 45, Code = "+420" },
                    new CountryCodes() { Id = 46, Code = "+45" },
                    new CountryCodes() { Id = 47, Code = "+253" },
                    new CountryCodes() { Id = 48, Code = "+767" },
                    new CountryCodes() { Id = 49, Code = "+1-809" },
                    new CountryCodes() { Id = 50, Code = "+593" },
                    new CountryCodes() { Id = 51, Code = "+20" },
                    new CountryCodes() { Id = 52, Code = "+503" },
                    new CountryCodes() { Id = 53, Code = "+240" },
                    new CountryCodes() { Id = 54, Code = "+291" },
                    new CountryCodes() { Id = 55, Code = "+372" },
                    new CountryCodes() { Id = 56, Code = "+268" },
                    new CountryCodes() { Id = 57, Code = "+251" },
                    new CountryCodes() { Id = 58, Code = "+679" },
                    new CountryCodes() { Id = 59, Code = "+358" },
                    new CountryCodes() { Id = 60, Code = "+33" },
                    new CountryCodes() { Id = 61, Code = "+241" },
                    new CountryCodes() { Id = 62, Code = "+220" },
                    new CountryCodes() { Id = 63, Code = "+995" },
                    new CountryCodes() { Id = 64, Code = "+49" },
                    new CountryCodes() { Id = 65, Code = "+233" },
                    new CountryCodes() { Id = 66, Code = "+30" },
                    new CountryCodes() { Id = 67, Code = "+473" },
                    new CountryCodes() { Id = 68, Code = "+502" },
                    new CountryCodes() { Id = 69, Code = "+224" },
                    new CountryCodes() { Id = 70, Code = "+245" },
                    new CountryCodes() { Id = 71, Code = "+592" },
                    new CountryCodes() { Id = 72, Code = "+509" },
                    new CountryCodes() { Id = 73, Code = "+504" },
                    new CountryCodes() { Id = 74, Code = "+36" },
                    new CountryCodes() { Id = 75, Code = "+354" },
                    new CountryCodes() { Id = 76, Code = "+91" },
                    new CountryCodes() { Id = 77, Code = "+62" },
                    new CountryCodes() { Id = 78, Code = "+98" },
                    new CountryCodes() { Id = 79, Code = "+964" },
                    new CountryCodes() { Id = 80, Code = "+353" },
                    new CountryCodes() { Id = 81, Code = "+972" },
                    new CountryCodes() { Id = 82, Code = "+39" },
                    new CountryCodes() { Id = 83, Code = "+876" },
                    new CountryCodes() { Id = 84, Code = "+81" },
                    new CountryCodes() { Id = 85, Code = "+962" },
                    new CountryCodes() { Id = 86, Code = "+7" },
                    new CountryCodes() { Id = 87, Code = "+254" },
                    new CountryCodes() { Id = 88, Code = "+686" },
                    new CountryCodes() { Id = 89, Code = "+965" },
                    new CountryCodes() { Id = 90, Code = "+996" },
                    new CountryCodes() { Id = 91, Code = "+856" },
                    new CountryCodes() { Id = 92, Code = "+371" },
                    new CountryCodes() { Id = 93, Code = "+961" },
                    new CountryCodes() { Id = 94, Code = "+266" },
                    new CountryCodes() { Id = 95, Code = "+231" },
                    new CountryCodes() { Id = 96, Code = "+218" },
                    new CountryCodes() { Id = 97, Code = "+423" },
                    new CountryCodes() { Id = 98, Code = "+370" },
                    new CountryCodes() { Id = 99, Code = "+352" },
                    new CountryCodes() { Id = 100, Code = "+261" },
                    new CountryCodes() { Id = 101, Code = "+265" },
                    new CountryCodes() { Id = 102, Code = "+60" },
                    new CountryCodes() { Id = 103, Code = "+960" },
                    new CountryCodes() { Id = 104, Code = "+223" },
                    new CountryCodes() { Id = 105, Code = "+356" },
                    new CountryCodes() { Id = 106, Code = "+692" },
                    new CountryCodes() { Id = 107, Code = "+222" },
                    new CountryCodes() { Id = 108, Code = "+230" },
                    new CountryCodes() { Id = 109, Code = "+52" },
                    new CountryCodes() { Id = 110, Code = "+691" },
                    new CountryCodes() { Id = 111, Code = "+373" },
                    new CountryCodes() { Id = 112, Code = "+377" },
                    new CountryCodes() { Id = 113, Code = "+976" },
                    new CountryCodes() { Id = 114, Code = "+382" },
                    new CountryCodes() { Id = 115, Code = "+212" },
                    new CountryCodes() { Id = 116, Code = "+258" },
                    new CountryCodes() { Id = 117, Code = "+264" },
                    new CountryCodes() { Id = 118, Code = "+674" },
                    new CountryCodes() { Id = 119, Code = "+977" },
                    new CountryCodes() { Id = 120, Code = "+31" },
                    new CountryCodes() { Id = 121, Code = "+64" },
                    new CountryCodes() { Id = 122, Code = "+505" },
                    new CountryCodes() { Id = 123, Code = "+227" },
                    new CountryCodes() { Id = 124, Code = "+234" },
                    new CountryCodes() { Id = 125, Code = "+389" },
                    new CountryCodes() { Id = 126, Code = "+47" },
                    new CountryCodes() { Id = 127, Code = "+968" },
                    new CountryCodes() { Id = 128, Code = "+92" },
                    new CountryCodes() { Id = 129, Code = "+680" },
                    new CountryCodes() { Id = 130, Code = "+507" },
                    new CountryCodes() { Id = 131, Code = "+675" },
                    new CountryCodes() { Id = 132, Code = "+595" },
                    new CountryCodes() { Id = 133, Code = "+51" },
                    new CountryCodes() { Id = 134, Code = "+63" },
                    new CountryCodes() { Id = 135, Code = "+48" },
                    new CountryCodes() { Id = 136, Code = "+351" },
                    new CountryCodes() { Id = 137, Code = "+974" },
                    new CountryCodes() { Id = 138, Code = "+40" },
                    new CountryCodes() { Id = 139, Code = "+7" },
                    new CountryCodes() { Id = 140, Code = "+250" },
                    new CountryCodes() { Id = 141, Code = "+869" },
                    new CountryCodes() { Id = 142, Code = "+758" },
                    new CountryCodes() { Id = 143, Code = "+784" },
                    new CountryCodes() { Id = 144, Code = "+685" },
                    new CountryCodes() { Id = 145, Code = "+378" },
                    new CountryCodes() { Id = 146, Code = "+239" },
                    new CountryCodes() { Id = 147, Code = "+966" },
                    new CountryCodes() { Id = 148, Code = "+221" },
                    new CountryCodes() { Id = 149, Code = "+381" },
                    new CountryCodes() { Id = 150, Code = "+248" },
                    new CountryCodes() { Id = 151, Code = "+232" },
                    new CountryCodes() { Id = 152, Code = "+65" },
                    new CountryCodes() { Id = 153, Code = "+421" },
                    new CountryCodes() { Id = 154, Code = "+386" },
                    new CountryCodes() { Id = 155, Code = "+677" },
                    new CountryCodes() { Id = 156, Code = "+252" },
                    new CountryCodes() { Id = 157, Code = "+27" },
                    new CountryCodes() { Id = 158, Code = "+82" },
                    new CountryCodes() { Id = 159, Code = "+211" },
                    new CountryCodes() { Id = 160, Code = "+34" },
                    new CountryCodes() { Id = 161, Code = "+94" },
                    new CountryCodes() { Id = 162, Code = "+249" },
                    new CountryCodes() { Id = 163, Code = "+597" },
                    new CountryCodes() { Id = 164, Code = "+46" },
                    new CountryCodes() { Id = 165, Code = "+41" },
                    new CountryCodes() { Id = 166, Code = "+963" },
                    new CountryCodes() { Id = 167, Code = "+886" },
                    new CountryCodes() { Id = 168, Code = "+992" },
                    new CountryCodes() { Id = 169, Code = "+255" },
                    new CountryCodes() { Id = 170, Code = "+66" },
                    new CountryCodes() { Id = 171, Code = "+228" },
                    new CountryCodes() { Id = 172, Code = "+676" },
                    new CountryCodes() { Id = 173, Code = "+868" },
                    new CountryCodes() { Id = 174, Code = "+216" },
                    new CountryCodes() { Id = 175, Code = "+90" },
                    new CountryCodes() { Id = 176, Code = "+993" },
                    new CountryCodes() { Id = 177, Code = "+688" },
                    new CountryCodes() { Id = 178, Code = "+256" },
                    new CountryCodes() { Id = 179, Code = "+380" },
                    new CountryCodes() { Id = 180, Code = "+971" },
                    new CountryCodes() { Id = 181, Code = "+44" },
                    new CountryCodes() { Id = 182, Code = "+1" },
                    new CountryCodes() { Id = 183, Code = "+598" },
                    new CountryCodes() { Id = 184, Code = "+998" },
                    new CountryCodes() { Id = 185, Code = "+678" },
                    new CountryCodes() { Id = 186, Code = "+39" },
                    new CountryCodes() { Id = 187, Code = "+58" },
                    new CountryCodes() { Id = 188, Code = "+84" },
                    new CountryCodes() { Id = 189, Code = "+967" },
                    new CountryCodes() { Id = 190, Code = "+260" },
                    new CountryCodes() { Id = 191, Code = "+263" },
                    new CountryCodes() { Id = 192, Code = "+1-829" },
                    new CountryCodes() { Id = 193, Code = "+1-849" }
                );

                await context.SaveChangesAsync();
            }

            if (!await context.VenueTypes.IgnoreQueryFilters().AnyAsync())
            {
                await context.VenueTypes.AddRangeAsync(
                    new VenueTypes { Id = 1, TypeName = "Indoor Basketball Court" },
                    new VenueTypes { Id = 2, TypeName = "Indoor Volleyball Court" },
                    new VenueTypes { Id = 3, TypeName = "Indoor Badminton Courts" },
                    new VenueTypes { Id = 4, TypeName = "Indoor Table Tennis Courts" },
                    new VenueTypes { Id = 5, TypeName = "Olympic-Sized Swimming Pool" },
                    new VenueTypes { Id = 6, TypeName = "Standard Water Polo Pool" },
                    new VenueTypes { Id = 7, TypeName = "Diving Pool" },
                    new VenueTypes { Id = 8, TypeName = "Outdoor Tennis Courts" },
                    new VenueTypes { Id = 9, TypeName = "Indoor Covered Tennis Courts" },
                    new VenueTypes { Id = 10, TypeName = "Weightlifting Area" },
                    new VenueTypes { Id = 11, TypeName = "Cardio Area" },
                    new VenueTypes { Id = 12, TypeName = "Martial Arts Dojo" },
                    new VenueTypes { Id = 13, TypeName = "Boxing Ring" },
                    new VenueTypes { Id = 14, TypeName = "Full-Size Soccer Field" },
                    new VenueTypes { Id = 15, TypeName = "Full-Size Rugby Field" },
                    new VenueTypes { Id = 16, TypeName = "Full-Size American Football Field" },
                    new VenueTypes { Id = 17, TypeName = "Cricket Pitch" },
                    new VenueTypes { Id = 18, TypeName = "Running Track" },
                    new VenueTypes { Id = 19, TypeName = "Long Jump Pit" },
                    new VenueTypes { Id = 20, TypeName = "High Jump Area" },
                    new VenueTypes { Id = 21, TypeName = "Indoor Velodrome" },
                    new VenueTypes { Id = 22, TypeName = "Outdoor Cycling Track" },
                    new VenueTypes { Id = 23, TypeName = "Outdoor BMX Track" },
                    new VenueTypes { Id = 24, TypeName = "Indoor Squash Courts" },
                    new VenueTypes { Id = 25, TypeName = "18-Hole Golf Course" },
                    new VenueTypes { Id = 26, TypeName = "Indoor Climbing Wall" },
                    new VenueTypes { Id = 27, TypeName = "Dance Studio with Wood Flooring" },
                    new VenueTypes { Id = 28, TypeName = "Yoga Room" },
                    new VenueTypes { Id = 29, TypeName = "Pilates Room" }
                );

                await context.SaveChangesAsync();
            }
            Log.Information("Venues Seeding");


            //// Seed Venues
            //if (!context.Venues.AnyAsync())
            //{
            //    context.Venues.AddRangeAsync(
            //        new Venue
            //        {
            //            Id = 1,
            //            name = "Shrouk Venue1",
            //            Image = "https://imagesdocs.blob.core.windows.net/buildupimagedocs/player/WhatsApp Image 2024-08-15 at 11.46.29 AM.jpeg",
            //            FacilityId = 1 // Tying the venue to the facility
            //        }
            //    );
            //    context.SaveChangesAsync();
            //}
            Log.Information("Venues Seeded");


            Log.Information("CoreFacilityServices Seeding");


            if (!await context.CoreFacilityServices.IgnoreQueryFilters().AnyAsync())
            {
                context.CoreFacilityServices.AddRangeAsync(
                    new CoreFacilityServices() { Id = 1, Name = "Equipment rental" },
                    new CoreFacilityServices() { Id = 2, Name = "Game scheduling" },
                    new CoreFacilityServices() { Id = 3, Name = "Event hosting" },
                    new CoreFacilityServices() { Id = 4, Name = "Personal training" },
                    new CoreFacilityServices() { Id = 5, Name = "Court booking" },
                    new CoreFacilityServices() { Id = 6, Name = "Group classes" },
                    new CoreFacilityServices() { Id = 7, Name = "Coaching" },
                    new CoreFacilityServices() { Id = 8, Name = "Lifeguard services" },
                    new CoreFacilityServices() { Id = 9, Name = "Swimming lessons" },
                    new CoreFacilityServices() { Id = 10, Name = "Pool maintenance" },
                    new CoreFacilityServices() { Id = 11, Name = "Event scheduling" },
                    new CoreFacilityServices() { Id = 12, Name = "Safety briefings" },
                    new CoreFacilityServices() { Id = 13, Name = "Diving lessons" },
                    new CoreFacilityServices() { Id = 14, Name = "Tournament organization" },
                    new CoreFacilityServices() { Id = 15, Name = "Fitness assessments" },
                    new CoreFacilityServices() { Id = 16, Name = "Class scheduling" },
                    new CoreFacilityServices() { Id = 17, Name = "Belt testing" },
                    new CoreFacilityServices() { Id = 18, Name = "Safety equipment rental" },
                    new CoreFacilityServices() { Id = 19, Name = "Field maintenance" },
                    new CoreFacilityServices() { Id = 20, Name = "Event timing services" },
                    new CoreFacilityServices() { Id = 21, Name = "Bike maintenance" },
                    new CoreFacilityServices() { Id = 22, Name = "Training sessions" },
                    new CoreFacilityServices() { Id = 23, Name = "Bicycle rentals" },
                    new CoreFacilityServices() { Id = 24, Name = "Tee-time booking" },
                    new CoreFacilityServices() { Id = 25, Name = "Caddy services" },
                    new CoreFacilityServices() { Id = 26, Name = "Dance classes" },
                    new CoreFacilityServices() { Id = 27, Name = "Dance rehearsals" }
                );

                await context.SaveChangesAsync();
            }

            Log.Information("CoreFacilityServices Seeded");

            Log.Information("Sports Seeding");

            if (!await context.Sports.IgnoreQueryFilters().AnyAsync())
            {
                await context.Sports.AddRangeAsync(
                    new Sports() { Id = 1, SportName = "Basketball" },
                    new Sports() { Id = 2, SportName = "Volleyball" },
                    new Sports() { Id = 3, SportName = "Badminton" },
                    new Sports() { Id = 4, SportName = "Table Tennis" },
                    new Sports() { Id = 5, SportName = "Swimming" },
                    new Sports() { Id = 6, SportName = "Water Polo" },
                    new Sports() { Id = 7, SportName = "Diving" },
                    new Sports() { Id = 8, SportName = "Aqua Aerobics" },
                    new Sports() { Id = 9, SportName = "Tennis" },
                    new Sports() { Id = 10, SportName = "Paddle Tennis" },
                    new Sports() { Id = 11, SportName = "Weightlifting" },
                    new Sports() { Id = 12, SportName = "Cardio" },
                    new Sports() { Id = 13, SportName = "Group Fitness Classes" },
                    new Sports() { Id = 14, SportName = "Karate" },
                    new Sports() { Id = 15, SportName = "Judo" },
                    new Sports() { Id = 16, SportName = "Taekwondo" },
                    new Sports() { Id = 17, SportName = "Boxing" },
                    new Sports() { Id = 18, SportName = "Soccer" },
                    new Sports() { Id = 19, SportName = "Rugby" },
                    new Sports() { Id = 20, SportName = "American Football" },
                    new Sports() { Id = 21, SportName = "Cricket" },
                    new Sports() { Id = 22, SportName = "Running" },
                    new Sports() { Id = 23, SportName = "Long Jump" },
                    new Sports() { Id = 24, SportName = "High Jump" },
                    new Sports() { Id = 25, SportName = "Discus Throw" },
                    new Sports() { Id = 26, SportName = "Cycling" },
                    new Sports() { Id = 27, SportName = "BMX" },
                    new Sports() { Id = 28, SportName = "Squash" },
                    new Sports() { Id = 29, SportName = "Golf" },
                    new Sports() { Id = 30, SportName = "Rock Climbing" },
                    new Sports() { Id = 31, SportName = "Ballet" },
                    new Sports() { Id = 32, SportName = "Contemporary Dance" },
                    new Sports() { Id = 33, SportName = "Hip-Hop" },
                    new Sports() { Id = 34, SportName = "Yoga" },
                    new Sports() { Id = 35, SportName = "Pilates" },
                    new Sports() { Id = 36, SportName = "Aerobics" }
                );

                await context.SaveChangesAsync();
            }

            Log.Information("Sports Seeded");
            Log.Information("FacilityType Seeding");

            if (!await context.FacilityType.IgnoreQueryFilters().AnyAsync())
            {
                await context.FacilityType.AddRangeAsync(
                    new FacilityType() { Id = 1, name = "Indoor Sports Hall" },
                    new FacilityType() { Id = 2, name = "Swimming Pool" },
                    new FacilityType() { Id = 3, name = "Tennis Courts" },
                    new FacilityType() { Id = 4, name = "Fitness Center" },
                    new FacilityType() { Id = 5, name = "Martial Arts Dojo" },
                    new FacilityType() { Id = 6, name = "Outdoor Fields" },
                    new FacilityType() { Id = 7, name = "Track and Field Area" },
                    new FacilityType() { Id = 8, name = "Cycling Track" },
                    new FacilityType() { Id = 9, name = "Squash Courts" },
                    new FacilityType() { Id = 10, name = "Golf Course" },
                    new FacilityType() { Id = 11, name = "Climbing Wall" },
                    new FacilityType() { Id = 12, name = "Dance Studio" },
                    new FacilityType() { Id = 13, name = "Multi-purpose Room" }
                );

                await context.SaveChangesAsync();
            }
            Log.Information("FacilityType Seeded");
            Log.Information("Amenities Seeding");


            if (!await context.Amenities.IgnoreQueryFilters().AnyAsync())
            {
                await context.Amenities.AddRangeAsync(
                    new Amenities() { Id = 1, name = "Changing rooms" },
                    new Amenities() { Id = 2, name = "Lockers" },
                    new Amenities() { Id = 3, name = "Rest areas" },
                    new Amenities() { Id = 4, name = "Showers" },
                    new Amenities() { Id = 5, name = "Water fountains" },
                    new Amenities() { Id = 6, name = "Seating area" },
                    new Amenities() { Id = 7, name = "Benches" },
                    new Amenities() { Id = 8, name = "Water dispensers" },
                    new Amenities() { Id = 9, name = "Poolside seating" },
                    new Amenities() { Id = 10, name = "Towels" },
                    new Amenities() { Id = 11, name = "Diving boards" },
                    new Amenities() { Id = 12, name = "Shade structures" },
                    new Amenities() { Id = 13, name = "Free weights" },
                    new Amenities() { Id = 14, name = "Machines" },
                    new Amenities() { Id = 15, name = "Mats" },
                    new Amenities() { Id = 16, name = "Sound system" },
                    new Amenities() { Id = 17, name = "Exercise equipment" },
                    new Amenities() { Id = 18, name = "Mat area" },
                    new Amenities() { Id = 19, name = "Punching bags" },
                    new Amenities() { Id = 20, name = "Water stations" },
                    new Amenities() { Id = 21, name = "Restrooms" },
                    new Amenities() { Id = 22, name = "Sand pits" },
                    new Amenities() { Id = 23, name = "Bike racks" },
                    new Amenities() { Id = 24, name = "Clubhouse" },
                    new Amenities() { Id = 25, name = "Pro shop" },
                    new Amenities() { Id = 26, name = "Crash mats" },
                    new Amenities() { Id = 27, name = "Full-length mirrors" },
                    new Amenities() { Id = 28, name = "Barres" },
                    new Amenities() { Id = 29, name = "Exercise balls" },
                    new Amenities() { Id = 30, name = "Mirrors" }
                );

                await context.SaveChangesAsync();
            }

            if (!await context.AccessibilityOptions.IgnoreQueryFilters().AnyAsync())
            {
                await context.AccessibilityOptions.AddRangeAsync(
                    new AccessibilityOptions() { Id = 1, OptionName = "Wheelchair ramps" },
                    new AccessibilityOptions() { Id = 2, OptionName = "Accessible restrooms" },
                    new AccessibilityOptions() { Id = 3, OptionName = "Accessible entrances" },
                    new AccessibilityOptions() { Id = 4, OptionName = "Elevators" },
                    new AccessibilityOptions() { Id = 5, OptionName = "Ramped access" },
                    new AccessibilityOptions() { Id = 6, OptionName = "Accessible viewing area" },
                    new AccessibilityOptions() { Id = 7, OptionName = "Pool lifts" },
                    new AccessibilityOptions() { Id = 8, OptionName = "Accessible changing rooms" },
                    new AccessibilityOptions() { Id = 9, OptionName = "Accessible entry" },
                    new AccessibilityOptions() { Id = 10, OptionName = "Ramped seating" },
                    new AccessibilityOptions() { Id = 11, OptionName = "Accessible class registration" },
                    new AccessibilityOptions() { Id = 12, OptionName = "Wheelchair-accessible courts" },
                    new AccessibilityOptions() { Id = 13, OptionName = "Accessible pathways" },
                    new AccessibilityOptions() { Id = 14, OptionName = "Accessible gym equipment" },
                    new AccessibilityOptions() { Id = 15, OptionName = "Accessible cardio machines" },
                    new AccessibilityOptions() { Id = 16, OptionName = "Accessible class areas" },
                    new AccessibilityOptions() { Id = 17, OptionName = "Adapted changing rooms" },
                    new AccessibilityOptions() { Id = 18, OptionName = "Wheelchair-accessible rooms" },
                    new AccessibilityOptions() { Id = 19, OptionName = "Accessible paths" },
                    new AccessibilityOptions() { Id = 20, OptionName = "Accessible seating" },
                    new AccessibilityOptions() { Id = 21, OptionName = "Wheelchair-accessible seating" },
                    new AccessibilityOptions() { Id = 22, OptionName = "Wheelchair-accessible track" },
                    new AccessibilityOptions() { Id = 23, OptionName = "Accessible areas for spectators" },
                    new AccessibilityOptions() { Id = 24, OptionName = "Accessible bike access" },
                    new AccessibilityOptions() { Id = 25, OptionName = "Wheelchair seating" },
                    new AccessibilityOptions() { Id = 26, OptionName = "Accessible court entry" },
                    new AccessibilityOptions() { Id = 27, OptionName = "Accessible golf carts" },
                    new AccessibilityOptions() { Id = 28, OptionName = "Wheelchair-accessible clubhouse" },
                    new AccessibilityOptions() { Id = 29, OptionName = "Adapted climbing routes" },
                    new AccessibilityOptions() { Id = 30, OptionName = "Wheelchair-accessible space" }
                );

                await context.SaveChangesAsync();
            }
            if (!await context.NewAccessibilityOptions.IgnoreQueryFilters().AnyAsync())
            {
                await context.NewAccessibilityOptions.AddRangeAsync(
                    new NewAccessibilityOptions() { Id = 1, OptionName = "Wheelchair ramps" },
                    new NewAccessibilityOptions() { Id = 2, OptionName = "Accessible restrooms" },
                    new NewAccessibilityOptions() { Id = 3, OptionName = "Accessible entrances" },
                    new NewAccessibilityOptions() { Id = 4, OptionName = "Elevators" },
                    new NewAccessibilityOptions() { Id = 5, OptionName = "Ramped access" },
                    new NewAccessibilityOptions() { Id = 6, OptionName = "Accessible viewing area" },
                    new NewAccessibilityOptions() { Id = 7, OptionName = "Pool lifts" },
                    new NewAccessibilityOptions() { Id = 8, OptionName = "Accessible changing rooms" },
                    new NewAccessibilityOptions() { Id = 9, OptionName = "Accessible entry" },
                    new NewAccessibilityOptions() { Id = 10, OptionName = "Ramped seating" },
                    new NewAccessibilityOptions() { Id = 11, OptionName = "Accessible class registration" },
                    new NewAccessibilityOptions() { Id = 12, OptionName = "Wheelchair-accessible courts" },
                    new NewAccessibilityOptions() { Id = 13, OptionName = "Accessible pathways" },
                    new NewAccessibilityOptions() { Id = 14, OptionName = "Accessible gym equipment" },
                    new NewAccessibilityOptions() { Id = 15, OptionName = "Accessible cardio machines" },
                    new NewAccessibilityOptions() { Id = 16, OptionName = "Accessible class areas" },
                    new NewAccessibilityOptions() { Id = 17, OptionName = "Adapted changing rooms" },
                    new NewAccessibilityOptions() { Id = 18, OptionName = "Wheelchair-accessible rooms" },
                    new NewAccessibilityOptions() { Id = 19, OptionName = "Accessible paths" },
                    new NewAccessibilityOptions() { Id = 20, OptionName = "Accessible seating" },
                    new NewAccessibilityOptions() { Id = 21, OptionName = "Wheelchair-accessible seating" },
                    new NewAccessibilityOptions() { Id = 22, OptionName = "Wheelchair-accessible track" },
                    new NewAccessibilityOptions() { Id = 23, OptionName = "Accessible areas for spectators" },
                    new NewAccessibilityOptions() { Id = 24, OptionName = "Accessible bike access" },
                    new NewAccessibilityOptions() { Id = 25, OptionName = "Wheelchair seating" },
                    new NewAccessibilityOptions() { Id = 26, OptionName = "Accessible court entry" },
                    new NewAccessibilityOptions() { Id = 27, OptionName = "Accessible golf carts" },
                    new NewAccessibilityOptions() { Id = 28, OptionName = "Wheelchair-accessible clubhouse" },
                    new NewAccessibilityOptions() { Id = 29, OptionName = "Adapted climbing routes" },
                    new NewAccessibilityOptions() { Id = 30, OptionName = "Wheelchair-accessible space" }
                );

                await context.SaveChangesAsync();
            }

            if (!await context.Country.IgnoreQueryFilters().AnyAsync())
            {
                await context.Country.AddRangeAsync(
                    new Country() { Id = 1, Name = "Egypt", IsDeleted = false },
                    new Country() { Id = 2, Name = "United States", IsDeleted = false },
                    new Country() { Id = 3, Name = "Germany", IsDeleted = false }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.State.IgnoreQueryFilters().AnyAsync())
            {
                var states = new List<State>
                    {
                        new State() { Id = 1, Name = "Cairo", CountryID = 1, IsDeleted = false },
                        new State() { Id = 2, Name = "Giza", CountryID = 1, IsDeleted = false },
                        new State() { Id = 3, Name = "Alexandria", CountryID = 1, IsDeleted = false },
                        new State() { Id = 4, Name = "Aswan", CountryID = 1, IsDeleted = false },
                        new State() { Id = 5, Name = "Asyut", CountryID = 1, IsDeleted = false },
                        new State() { Id = 6, Name = "Beheira", CountryID = 1, IsDeleted = false },
                        new State() { Id = 7, Name = "Beni Suef", CountryID = 1, IsDeleted = false },
                        new State() { Id = 8, Name = "Dakahlia", CountryID = 1, IsDeleted = false },
                        new State() { Id = 9, Name = "Damietta", CountryID = 1, IsDeleted = false },
                        new State() { Id = 10, Name = "Faiyum", CountryID = 1, IsDeleted = false },
                        new State() { Id = 11, Name = "Gharbia", CountryID = 1, IsDeleted = false },
                        new State() { Id = 12, Name = "Ismailia", CountryID = 1, IsDeleted = false },
                        new State() { Id = 13, Name = "Kafr El Sheikh", CountryID = 1, IsDeleted = false },
                        new State() { Id = 14, Name = "Luxor", CountryID = 1, IsDeleted = false },
                        new State() { Id = 15, Name = "Matrouh", CountryID = 1, IsDeleted = false },
                        new State() { Id = 16, Name = "Minya", CountryID = 1, IsDeleted = false },
                        new State() { Id = 17, Name = "Port Said", CountryID = 1, IsDeleted = false },
                        new State() { Id = 18, Name = "Qalyubia", CountryID = 1, IsDeleted = false },
                        new State() { Id = 19, Name = "Qena", CountryID = 1, IsDeleted = false },
                        new State() { Id = 20, Name = "Red Sea", CountryID = 1, IsDeleted = false },
                        new State() { Id = 21, Name = "Sharqia", CountryID = 1, IsDeleted = false },
                        new State() { Id = 22, Name = "Sohag", CountryID = 1, IsDeleted = false },
                        new State() { Id = 23, Name = "South Sinai", CountryID = 1, IsDeleted = false },
                        new State() { Id = 24, Name = "Suez", CountryID = 1, IsDeleted = false },

                    };

                await context.State.AddRangeAsync(states);
                await context.SaveChangesAsync();
            }

            if (!await context.City.IgnoreQueryFilters().AnyAsync())
            {
                var cities = new List<City>
                    {
                        // Cairo cities
                        new City() { Id = 1, Name = "Cairo", StateID = 1, IsDeleted = false },
                        new City() { Id = 2, Name = "El Marg", StateID = 1, IsDeleted = false },
                        new City() { Id = 3, Name = "El Mataria", StateID = 1, IsDeleted = false },
                        new City() { Id = 4, Name = "Heliopolis", StateID = 1, IsDeleted = false },
                        new City() { Id = 5, Name = "Ain Shams", StateID = 1, IsDeleted = false },
                        new City() { Id = 6, Name = "Maadi", StateID = 1, IsDeleted = false },
                        new City() { Id = 7, Name = "Nasr City", StateID = 1, IsDeleted = false },
                        new City() { Id = 8, Name = "Helwan", StateID = 1, IsDeleted = false },
                        new City() { Id = 9, Name = "El Shorouk", StateID = 1, IsDeleted = false },
                        new City() { Id = 10, Name = "El Katameya", StateID = 1, IsDeleted = false },
                        new City() { Id = 11, Name = "Mokattam", StateID = 1, IsDeleted = false },

                        // Giza cities
                        new City() { Id = 12, Name = "Giza", StateID = 2, IsDeleted = false },
                        new City() { Id = 13, Name = "6th of October City", StateID = 2, IsDeleted = false },
                        new City() { Id = 14, Name = "Sheikh Zayed", StateID = 2, IsDeleted = false },
                        new City() { Id = 15, Name = "Hawamdeya", StateID = 2, IsDeleted = false },
                        new City() { Id = 16, Name = "Al-Badrashein", StateID = 2, IsDeleted = false },
                        new City() { Id = 17, Name = "Abu Rawash", StateID = 2, IsDeleted = false },
                        new City() { Id = 18, Name = "Kerdasa", StateID = 2, IsDeleted = false },
                        new City() { Id = 19, Name = "Atfih", StateID = 2, IsDeleted = false },
                        new City() { Id = 20, Name = "El Ayat", StateID = 2, IsDeleted = false },
                        new City() { Id = 21, Name = "Al Saff", StateID = 2, IsDeleted = false },
                        new City() { Id = 22, Name = "Monshaat El Qanater", StateID = 2, IsDeleted = false },
                        new City() { Id = 23, Name = "Nahya", StateID = 2, IsDeleted = false },
                        new City() { Id = 24, Name = "Mit Oqba", StateID = 2, IsDeleted = false },
                        new City() { Id = 25, Name = "El Omraniya", StateID = 2, IsDeleted = false },
                        new City() { Id = 26, Name = "Giza Plateau", StateID = 2, IsDeleted = false },
                        new City() { Id = 27, Name = "Kafr Hakim", StateID = 2, IsDeleted = false },
                        new City() { Id = 28, Name = "Sakkara", StateID = 2, IsDeleted = false },

                        // Alexandria cities
                        new City() { Id = 29, Name = "Alexandria", StateID = 3, IsDeleted = false },
                        new City() { Id = 30, Name = "Borg El Arab", StateID = 3, IsDeleted = false },
                        new City() { Id = 31, Name = "New Borg El Arab", StateID = 3, IsDeleted = false },
                        new City() { Id = 32, Name = "Al Mandara", StateID = 3, IsDeleted = false },
                        new City() { Id = 33, Name = "Abu Qir", StateID = 3, IsDeleted = false },
                        new City() { Id = 34, Name = "Agami", StateID = 3, IsDeleted = false },
                        new City() { Id = 35, Name = "Sidi Gaber", StateID = 3, IsDeleted = false },
                        new City() { Id = 36, Name = "El Max", StateID = 3, IsDeleted = false },
                        new City() { Id = 37, Name = "El Montaza", StateID = 3, IsDeleted = false },
                        new City() { Id = 38, Name = "El Mamoura", StateID = 3, IsDeleted = false },

                        // Aswan cities
                        new City() { Id = 39, Name = "Aswan", StateID = 4, IsDeleted = false },
                        new City() { Id = 40, Name = "Kom Ombo", StateID = 4, IsDeleted = false },
                        new City() { Id = 41, Name = "Edfu", StateID = 4, IsDeleted = false },
                        new City() { Id = 42, Name = "Daraw", StateID = 4, IsDeleted = false },
                        new City() { Id = 43, Name = "Sebaiya", StateID = 4, IsDeleted = false },
                        new City() { Id = 44, Name = "El-Radisia", StateID = 4, IsDeleted = false },
                        new City() { Id = 45, Name = "Ballana", StateID = 4, IsDeleted = false },
                        new City() { Id = 46, Name = "Kalabsha", StateID = 4, IsDeleted = false },
                        new City() { Id = 47, Name = "Nasr City", StateID = 4, IsDeleted = false },
                        new City() { Id = 48, Name = "Abu Simbel", StateID = 4, IsDeleted = false },
                        new City() { Id = 49, Name = "Al Rassolya", StateID = 4, IsDeleted = false },
                        new City() { Id = 50, Name = "Nagaa Hamada", StateID = 4, IsDeleted = false },
                        new City() { Id = 51, Name = "New Toshka", StateID = 4, IsDeleted = false },

                        // Asyut cities
                        new City() { Id = 52, Name = "Asyut", StateID = 5, IsDeleted = false },
                        new City() { Id = 53, Name = "Dairut", StateID = 5, IsDeleted = false },
                        new City() { Id = 54, Name = "Manfalut", StateID = 5, IsDeleted = false },
                        new City() { Id = 55, Name = "Abnub", StateID = 5, IsDeleted = false },
                        new City() { Id = 56, Name = "Sahel Selim", StateID = 5, IsDeleted = false },
                        new City() { Id = 57, Name = "Al Badari", StateID = 5, IsDeleted = false },
                        new City() { Id = 58, Name = "El Qusiya", StateID = 5, IsDeleted = false },
                        new City() { Id = 59, Name = "Al Fath", StateID = 5, IsDeleted = false },
                        new City() { Id = 60, Name = "El Hammamiya", StateID = 5, IsDeleted = false },
                        new City() { Id = 61, Name = "Abu Tig", StateID = 5, IsDeleted = false },
                        new City() { Id = 62, Name = "Ghanayem", StateID = 5, IsDeleted = false },
                        new City() { Id = 63, Name = "El Zarabi", StateID = 5, IsDeleted = false },
                        new City() { Id = 64, Name = "Qesem Al Ayah", StateID = 5, IsDeleted = false },
                        new City() { Id = 65, Name = "Sheikh Mahran", StateID = 5, IsDeleted = false },
                        new City() { Id = 66, Name = "Manqabad", StateID = 5, IsDeleted = false },
                        new City() { Id = 67, Name = "El Matmar", StateID = 5, IsDeleted = false },
                        new City() { Id = 68, Name = "Nazlet Abdel Rahman", StateID = 5, IsDeleted = false },
                        new City() { Id = 69, Name = "Arab El Awamer", StateID = 5, IsDeleted = false },
                        new City() { Id = 70, Name = "Beni Mur", StateID = 5, IsDeleted = false },

                        // Beheira cities
                        new City() { Id = 71, Name = "Damanhur", StateID = 6, IsDeleted = false },
                        new City() { Id = 72, Name = "Kafr El Dawwar", StateID = 6, IsDeleted = false },
                        new City() { Id = 73, Name = "Rashid (Rosetta)", StateID = 6, IsDeleted = false },
                        new City() { Id = 74, Name = "Edku", StateID = 6, IsDeleted = false },
                        new City() { Id = 75, Name = "Kom Hamada", StateID = 6, IsDeleted = false },
                        new City() { Id = 76, Name = "El Rahmaniya", StateID = 6, IsDeleted = false },
                        new City() { Id = 77, Name = "Shubrakhit", StateID = 6, IsDeleted = false },
                        new City() { Id = 78, Name = "Abu Hummus", StateID = 6, IsDeleted = false },
                        new City() { Id = 79, Name = "Hosh Essa", StateID = 6, IsDeleted = false },
                        new City() { Id = 80, Name = "Itay El Barud", StateID = 6, IsDeleted = false },
                        new City() { Id = 81, Name = "Delingat", StateID = 6, IsDeleted = false },
                        new City() { Id = 82, Name = "Mahmoudiyah", StateID = 6, IsDeleted = false },
                        new City() { Id = 83, Name = "Wadi El Natrun", StateID = 6, IsDeleted = false },
                        new City() { Id = 84, Name = "El Nubariya", StateID = 6, IsDeleted = false },
                        new City() { Id = 85, Name = "Sidi Ghazi", StateID = 6, IsDeleted = false },
                        new City() { Id = 86, Name = "Menyat Elnasr", StateID = 6, IsDeleted = false },
                        new City() { Id = 87, Name = "Metoubas", StateID = 6, IsDeleted = false },
                        new City() { Id = 88, Name = "Halfa", StateID = 6, IsDeleted = false },
                        new City() { Id = 89, Name = "Sandala", StateID = 6, IsDeleted = false },
                        new City() { Id = 90, Name = "Sakha", StateID = 6, IsDeleted = false },

                        // Beni Suef cities
                        new City() { Id = 91, Name = "Beni Suef", StateID = 7, IsDeleted = false },
                        new City() { Id = 92, Name = "Al Fashn", StateID = 7, IsDeleted = false },
                        new City() { Id = 93, Name = "Al Wasta", StateID = 7, IsDeleted = false },
                        new City() { Id = 94, Name = "Ehnasia", StateID = 7, IsDeleted = false },
                        new City() { Id = 95, Name = "Nasser", StateID = 7, IsDeleted = false },
                        new City() { Id = 96, Name = "Samasta", StateID = 7, IsDeleted = false },
                        new City() { Id = 97, Name = "Beba", StateID = 7, IsDeleted = false },
                        new City() { Id = 98, Name = "Beni Haroun", StateID = 7, IsDeleted = false },
                        new City() { Id = 99, Name = "Al-Qaleh", StateID = 7, IsDeleted = false },
                        new City() { Id = 100, Name = "Meidum", StateID = 7, IsDeleted = false },
                        new City() { Id = 101, Name = "Al Hiba", StateID = 7, IsDeleted = false },
                        new City() { Id = 102, Name = "Ashmant", StateID = 7, IsDeleted = false },
                        new City() { Id = 103, Name = "Shosha", StateID = 7, IsDeleted = false },
                        new City() { Id = 104, Name = "Al Kom", StateID = 7, IsDeleted = false },
                        new City() { Id = 105, Name = "Abou Saleh", StateID = 7, IsDeleted = false },
                        new City() { Id = 106, Name = "El Shorfa", StateID = 7, IsDeleted = false },
                        new City() { Id = 107, Name = "Al Mashaala", StateID = 7, IsDeleted = false },
                        new City() { Id = 108, Name = "Ibsheway", StateID = 7, IsDeleted = false },

                        // Dakahlia cities
                        new City() { Id = 109, Name = "Mansoura", StateID = 8, IsDeleted = false },
                        new City() { Id = 110, Name = "Talkha", StateID = 8, IsDeleted = false },
                        new City() { Id = 111, Name = "Mit Ghamr", StateID = 8, IsDeleted = false },
                        new City() { Id = 112, Name = "Belqas", StateID = 8, IsDeleted = false },
                        new City() { Id = 113, Name = "Al-Senbellawein", StateID = 8, IsDeleted = false },
                        new City() { Id = 114, Name = "Gamasa", StateID = 8, IsDeleted = false },
                        new City() { Id = 115, Name = "Sherbin", StateID = 8, IsDeleted = false },
                        new City() { Id = 116, Name = "Aga", StateID = 8, IsDeleted = false },
                        new City() { Id = 117, Name = "Manzala", StateID = 8, IsDeleted = false },
                        new City() { Id = 118, Name = "Mataria", StateID = 8, IsDeleted = false },
                        new City() { Id = 119, Name = "Nabroh", StateID = 8, IsDeleted = false },
                        new City() { Id = 120, Name = "Temay El Amdeed", StateID = 8, IsDeleted = false },
                        new City() { Id = 121, Name = "Minyet El-Nasr", StateID = 8, IsDeleted = false },
                        new City() { Id = 122, Name = "Beni Ebeid", StateID = 8, IsDeleted = false },
                        new City() { Id = 123, Name = "Meet Elkorama", StateID = 8, IsDeleted = false },
                        new City() { Id = 124, Name = "Dikirnis", StateID = 8, IsDeleted = false },
                        new City() { Id = 125, Name = "Mahalat Damana", StateID = 8, IsDeleted = false },
                        new City() { Id = 126, Name = "Al Gamaliya", StateID = 8, IsDeleted = false },
                        new City() { Id = 127, Name = "El Kurdi", StateID = 8, IsDeleted = false },
                        new City() { Id = 128, Name = "Al Azaziya", StateID = 8, IsDeleted = false },
                        new City() { Id = 129, Name = "Karawan", StateID = 8, IsDeleted = false },
                        new City() { Id = 130, Name = "Kafr Al Badamas", StateID = 8, IsDeleted = false },

                        // Damietta cities
                        new City() { Id = 131, Name = "Damietta", StateID = 9, IsDeleted = false },
                        new City() { Id = 132, Name = "New Damietta", StateID = 9, IsDeleted = false },
                        new City() { Id = 133, Name = "Ras El Bar", StateID = 9, IsDeleted = false },
                        new City() { Id = 134, Name = "Faraskur", StateID = 9, IsDeleted = false },
                        new City() { Id = 135, Name = "El Zarqa", StateID = 9, IsDeleted = false },
                        new City() { Id = 136, Name = "Kafr Saad", StateID = 9, IsDeleted = false },
                        new City() { Id = 137, Name = "Kafr El Battikh", StateID = 9, IsDeleted = false },
                        new City() { Id = 138, Name = "Sherbim", StateID = 9, IsDeleted = false },
                        new City() { Id = 139, Name = "Basarta", StateID = 9, IsDeleted = false },
                        new City() { Id = 140, Name = "Ezbet El Borg", StateID = 9, IsDeleted = false },
                        new City() { Id = 141, Name = "Al Gamaliya", StateID = 9, IsDeleted = false },
                        new City() { Id = 142, Name = "Mit Abu Ghaleb", StateID = 9, IsDeleted = false },
                        new City() { Id = 143, Name = "Al Aziziya", StateID = 9, IsDeleted = false },
                        new City() { Id = 144, Name = "Al Safaniyah", StateID = 9, IsDeleted = false },
                        new City() { Id = 145, Name = "Al Ramlah", StateID = 9, IsDeleted = false },
                        new City() { Id = 146, Name = "Al Zawya", StateID = 9, IsDeleted = false },
                        new City() { Id = 147, Name = "Al Makhlouf", StateID = 9, IsDeleted = false },
                        new City() { Id = 148, Name = "El Kafr", StateID = 9, IsDeleted = false },

                        // Faiyum cities
                        new City() { Id = 149, Name = "Faiyum", StateID = 10, IsDeleted = false },
                        new City() { Id = 150, Name = "Sinnuris", StateID = 10, IsDeleted = false },
                        new City() { Id = 151, Name = "Ibshaway", StateID = 10, IsDeleted = false },
                        new City() { Id = 152, Name = "Tamia", StateID = 10, IsDeleted = false },
                        new City() { Id = 153, Name = "Yousef El Seddik", StateID = 10, IsDeleted = false },
                        new City() { Id = 154, Name = "Etsa", StateID = 10, IsDeleted = false },
                        new City() { Id = 155, Name = "Qarun", StateID = 10, IsDeleted = false },
                        new City() { Id = 156, Name = "Qasr Qarun", StateID = 10, IsDeleted = false },
                        new City() { Id = 157, Name = "Al Masraf", StateID = 10, IsDeleted = false },
                        new City() { Id = 158, Name = "Atsa", StateID = 10, IsDeleted = false },
                        new City() { Id = 159, Name = "El Azab", StateID = 10, IsDeleted = false },
                        new City() { Id = 160, Name = "Zawyet al-Karadsa", StateID = 10, IsDeleted = false },
                        new City() { Id = 161, Name = "Yebsa", StateID = 10, IsDeleted = false },
                        new City() { Id = 162, Name = "Kom Oshim", StateID = 10, IsDeleted = false },
                        new City() { Id = 163, Name = "El Rayan", StateID = 10, IsDeleted = false },
                        new City() { Id = 164, Name = "Tunis Village", StateID = 10, IsDeleted = false },
                        new City() { Id = 165, Name = "Nazlet El Saman", StateID = 10, IsDeleted = false },


                        // Gharbia cities
                        new City() { Id = 166, Name = "Tanta", StateID = 11, IsDeleted = false },
                        new City() { Id = 167, Name = "Kafr El Zayat", StateID = 11, IsDeleted = false },
                        new City() { Id = 168, Name = "El-Mahalla El-Kubra", StateID = 11, IsDeleted = false },
                        new City() { Id = 169, Name = "Samanoud", StateID = 11, IsDeleted = false },
                        new City() { Id = 170, Name = "Qutour", StateID = 11, IsDeleted = false },
                        new City() { Id = 171, Name = "Zifta", StateID = 11, IsDeleted = false },
                        new City() { Id = 172, Name = "Basyoun", StateID = 11, IsDeleted = false },
                        new City() { Id = 173, Name = "Santa", StateID = 11, IsDeleted = false },
                        new City() { Id = 174, Name = "Beba", StateID = 11, IsDeleted = false },
                        new City() { Id = 175, Name = "Mit Asim", StateID = 11, IsDeleted = false },
                        new City() { Id = 176, Name = "Harbata", StateID = 11, IsDeleted = false },
                        new City() { Id = 177, Name = "Qantarah", StateID = 11, IsDeleted = false },
                        new City() { Id = 178, Name = "Al Mahalla Al Kubra", StateID = 11, IsDeleted = false },
                        new City() { Id = 179, Name = "Al Qasr", StateID = 11, IsDeleted = false },
                        new City() { Id = 180, Name = "Shubra Bab", StateID = 11, IsDeleted = false },
                        new City() { Id = 181, Name = "Gharbeya", StateID = 11, IsDeleted = false },
                        new City() { Id = 182, Name = "Sahel Al Gharbia", StateID = 11, IsDeleted = false },
                        new City() { Id = 183, Name = "El Tarah", StateID = 11, IsDeleted = false },
                        new City() { Id = 184, Name = "Sirsina", StateID = 11, IsDeleted = false },

                        // Ismailia cities
                        new City() { Id = 185, Name = "Ismailia", StateID = 12, IsDeleted = false },
                        new City() { Id = 186, Name = "Al Qantara Sharq", StateID = 12, IsDeleted = false },
                        new City() { Id = 187, Name = "Al Qantara Gharb", StateID = 12, IsDeleted = false },
                        new City() { Id = 188, Name = "Tell El Kebir", StateID = 12, IsDeleted = false },
                        new City() { Id = 189, Name = "Fayed", StateID = 12, IsDeleted = false },
                        new City() { Id = 190, Name = "Abu Sweir", StateID = 12, IsDeleted = false },
                        new City() { Id = 191, Name = "Abu Sultan", StateID = 12, IsDeleted = false },
                        new City() { Id = 192, Name = "Al Geneina", StateID = 12, IsDeleted = false },
                        new City() { Id = 193, Name = "Al Shuhada", StateID = 12, IsDeleted = false },
                        new City() { Id = 194, Name = "Sarabiyum", StateID = 12, IsDeleted = false },
                        new City() { Id = 195, Name = "El Taliaa", StateID = 12, IsDeleted = false },
                        new City() { Id = 196, Name = "Dehibat", StateID = 12, IsDeleted = false },
                        new City() { Id = 197, Name = "El Kassasin", StateID = 12, IsDeleted = false },

                        // Kafr El Sheikh cities
                        new City() { Id = 198, Name = "Kafr El Sheikh", StateID = 13, IsDeleted = false },
                        new City() { Id = 199, Name = "Desouk", StateID = 13, IsDeleted = false },
                        new City() { Id = 200, Name = "Fuwah", StateID = 13, IsDeleted = false },
                        new City() { Id = 201, Name = "Baltim", StateID = 13, IsDeleted = false },
                        new City() { Id = 202, Name = "Sidi Salem", StateID = 13, IsDeleted = false },
                        new City() { Id = 203, Name = "Qallin", StateID = 13, IsDeleted = false },
                        new City() { Id = 204, Name = "Metoubes", StateID = 13, IsDeleted = false },
                        new City() { Id = 205, Name = "Riyadh", StateID = 13, IsDeleted = false },
                        new City() { Id = 206, Name = "Borollos", StateID = 13, IsDeleted = false },
                        new City() { Id = 207, Name = "Al Mahalla", StateID = 13, IsDeleted = false },
                        new City() { Id = 208, Name = "Kafr Shukr", StateID = 13, IsDeleted = false },
                        new City() { Id = 209, Name = "Al Hamul", StateID = 13, IsDeleted = false },
                        new City() { Id = 210, Name = "Shubra Khit", StateID = 13, IsDeleted = false },
                        new City() { Id = 211, Name = "Abou Matamir", StateID = 13, IsDeleted = false },
                        new City() { Id = 212, Name = "Ezbat Shuhayb", StateID = 13, IsDeleted = false },
                        new City() { Id = 213, Name = "Al Rahmaniya", StateID = 13, IsDeleted = false },
                        new City() { Id = 214, Name = "Al Nouriya", StateID = 13, IsDeleted = false },
                        new City() { Id = 215, Name = "Al Borlos", StateID = 13, IsDeleted = false },

                        // Luxor cities
                        new City() { Id = 216, Name = "Luxor", StateID = 14, IsDeleted = false },
                        new City() { Id = 217, Name = "Armant", StateID = 14, IsDeleted = false },
                        new City() { Id = 218, Name = "Esna", StateID = 14, IsDeleted = false },
                        new City() { Id = 219, Name = "El Toud", StateID = 14, IsDeleted = false },
                        new City() { Id = 220, Name = "New Tiba", StateID = 14, IsDeleted = false },
                        new City() { Id = 221, Name = "Karnak", StateID = 14, IsDeleted = false },
                        new City() { Id = 222, Name = "Nagaa Al Moqattam", StateID = 14, IsDeleted = false },
                        new City() { Id = 223, Name = "Al Qorna", StateID = 14, IsDeleted = false },
                        new City() { Id = 224, Name = "Al Bayadeyah", StateID = 14, IsDeleted = false },
                        new City() { Id = 225, Name = "Al Ba'irat", StateID = 14, IsDeleted = false },
                        new City() { Id = 226, Name = "Al Gizaqat", StateID = 14, IsDeleted = false },
                        new City() { Id = 227, Name = "Al Hagz", StateID = 14, IsDeleted = false },
                        new City() { Id = 228, Name = "Shaghag", StateID = 14, IsDeleted = false },

                        // Matrouh cities
                        new City() { Id = 229, Name = "Marsa Matrouh", StateID = 15, IsDeleted = false },
                        new City() { Id = 230, Name = "Siwa", StateID = 15, IsDeleted = false },
                        new City() { Id = 231, Name = "El Alamein", StateID = 15, IsDeleted = false },
                        new City() { Id = 232, Name = "Dabaa", StateID = 15, IsDeleted = false },
                        new City() { Id = 233, Name = "Sallum", StateID = 15, IsDeleted = false },
                        new City() { Id = 234, Name = "Al Naghila", StateID = 15, IsDeleted = false },
                        new City() { Id = 235, Name = "Al Qusour", StateID = 15, IsDeleted = false },
                        new City() { Id = 236, Name = "El Negaila", StateID = 15, IsDeleted = false },
                        new City() { Id = 237, Name = "Al Daba'a", StateID = 15, IsDeleted = false },
                        new City() { Id = 238, Name = "Al Qrira", StateID = 15, IsDeleted = false },
                        new City() { Id = 239, Name = "Al-Mazlag", StateID = 15, IsDeleted = false },
                        new City() { Id = 240, Name = "El Himara", StateID = 15, IsDeleted = false },

                        // Sohag cities
                        new City() { Id = 241, Name = "Sohag", StateID = 22, IsDeleted = false },
                        new City() { Id = 242, Name = "Akhmim", StateID = 22, IsDeleted = false },
                        new City() { Id = 243, Name = "Tahta", StateID = 22, IsDeleted = false },
                        new City() { Id = 244, Name = "Juhayna", StateID = 22, IsDeleted = false },
                        new City() { Id = 245, Name = "Maragha", StateID = 22, IsDeleted = false },
                        new City() { Id = 246, Name = "Dar El Salam", StateID = 22, IsDeleted = false },
                        new City() { Id = 247, Name = "El Balyana", StateID = 22, IsDeleted = false },
                        new City() { Id = 248, Name = "Girga", StateID = 22, IsDeleted = false },
                        new City() { Id = 249, Name = "Tama", StateID = 22, IsDeleted = false },
                        new City() { Id = 250, Name = "Naga Hammadi", StateID = 22, IsDeleted = false },
                        new City() { Id = 251, Name = "Shandaweel El Bahri", StateID = 22, IsDeleted = false },
                        new City() { Id = 252, Name = "Saqulta", StateID = 22, IsDeleted = false },
                        new City() { Id = 253, Name = "El Mena", StateID = 22, IsDeleted = false },

                        // South Sinai cities
                        new City() { Id = 254, Name = "Sharm El Sheikh", StateID = 23, IsDeleted = false },
                        new City() { Id = 255, Name = "Dahab", StateID = 23, IsDeleted = false },
                        new City() { Id = 256, Name = "El Tor", StateID = 23, IsDeleted = false },
                        new City() { Id = 257, Name = "Ras Sidr", StateID = 23, IsDeleted = false },
                        new City() { Id = 258, Name = "Nuweiba", StateID = 23, IsDeleted = false },
                        new City() { Id = 259, Name = "Taba", StateID = 23, IsDeleted = false },
                        new City() { Id = 260, Name = "Saint Catherine", StateID = 23, IsDeleted = false },
                        new City() { Id = 261, Name = "Abu Zenima", StateID = 23, IsDeleted = false },
                        new City() { Id = 262, Name = "Abu Rudeis", StateID = 23, IsDeleted = false },
                        new City() { Id = 263, Name = "Wadi Feiran", StateID = 23, IsDeleted = false },
                        new City() { Id = 264, Name = "Sheikh Al Amer", StateID = 23, IsDeleted = false },
                        new City() { Id = 265, Name = "Al Mazra’a", StateID = 23, IsDeleted = false },
                        new City() { Id = 266, Name = "Wadi Kibran", StateID = 23, IsDeleted = false },
                        new City() { Id = 267, Name = "Wadi El Raha", StateID = 23, IsDeleted = false },
                        new City() { Id = 268, Name = "Zaranik", StateID = 23, IsDeleted = false },

                        // Suez cities
                        new City() { Id = 269, Name = "Suez", StateID = 24, IsDeleted = false },
                        new City() { Id = 270, Name = "Ain Sokhna", StateID = 24, IsDeleted = false },
                        new City() { Id = 271, Name = "El Arbaeen", StateID = 24, IsDeleted = false },
                        new City() { Id = 272, Name = "Ataka", StateID = 24, IsDeleted = false },
                        new City() { Id = 273, Name = "Ganayen", StateID = 24, IsDeleted = false },
                        new City() { Id = 274, Name = "Port Tawfiq", StateID = 24, IsDeleted = false },
                        new City() { Id = 275, Name = "Ras El Sudr", StateID = 24, IsDeleted = false },
                        new City() { Id = 276, Name = "Ras Ghareb", StateID = 24, IsDeleted = false },
                        new City() { Id = 277, Name = "Al Fayrouz", StateID = 24, IsDeleted = false },
                        new City() { Id = 278, Name = "Hurghada North", StateID = 24, IsDeleted = false },
                        new City() { Id = 279, Name = "El Galala", StateID = 24, IsDeleted = false },
                        new City() { Id = 280, Name = "Wadi El Dom", StateID = 24, IsDeleted = false },
                        new City() { Id = 281, Name = "Wadi Arbaeen", StateID = 24, IsDeleted = false },

                        // Minya cities
                        new City() { Id = 282, Name = "Minya", StateID = 16, IsDeleted = false },
                        new City() { Id = 283, Name = "Maghagha", StateID = 16, IsDeleted = false },
                        new City() { Id = 284, Name = "Bani Mazar", StateID = 16, IsDeleted = false },
                        new City() { Id = 285, Name = "Samalout", StateID = 16, IsDeleted = false },
                        new City() { Id = 286, Name = "Abu Qirqas", StateID = 16, IsDeleted = false },
                        new City() { Id = 287, Name = "Deir Mawas", StateID = 16, IsDeleted = false },
                        new City() { Id = 288, Name = "El Minya", StateID = 16, IsDeleted = false },
                        new City() { Id = 289, Name = "Mallawi", StateID = 16, IsDeleted = false },
                        new City() { Id = 290, Name = "Adwa", StateID = 16, IsDeleted = false },
                        new City() { Id = 291, Name = "El-Hawsh", StateID = 16, IsDeleted = false },
                        new City() { Id = 292, Name = "Al-Mahalla", StateID = 16, IsDeleted = false },
                        
                        // Port Said cities
                        new City() { Id = 293, Name = "Port Said", StateID = 17, IsDeleted = false },
                        new City() { Id = 294, Name = "Ad-Duqqi", StateID = 17, IsDeleted = false },
                        new City() { Id = 295, Name = "Al Manakh", StateID = 17, IsDeleted = false },
                        new City() { Id = 296, Name = "Al Sharq", StateID = 17, IsDeleted = false },
                        new City() { Id = 297, Name = "Al Aman", StateID = 17, IsDeleted = false },
                        new City() { Id = 298, Name = "Al Damsi", StateID = 17, IsDeleted = false },
                        
                        // Qalyubia cities
                        new City() { Id = 299, Name = "Benha", StateID = 18, IsDeleted = false },
                        new City() { Id = 300, Name = "Qalyub", StateID = 18, IsDeleted = false },
                        new City() { Id = 301, Name = "Shibin El Qanater", StateID = 18, IsDeleted = false },
                        new City() { Id = 302, Name = "Kafr Shukr", StateID = 18, IsDeleted = false },
                        new City() { Id = 303, Name = "Banha", StateID = 18, IsDeleted = false },
                        new City() { Id = 304, Name = "Toukh", StateID = 18, IsDeleted = false },
                        new City() { Id = 305, Name = "Qaha", StateID = 18, IsDeleted = false },
                        new City() { Id = 306, Name = "Al-Khanka", StateID = 18, IsDeleted = false },
                        new City() { Id = 307, Name = "El Shuhada", StateID = 18, IsDeleted = false },
                        new City() { Id = 308, Name = "Kafr El-Hosseini", StateID = 18, IsDeleted = false },
                        
                        // Qena cities
                        new City() { Id = 309, Name = "Qena", StateID = 19, IsDeleted = false },
                        new City() { Id = 310, Name = "Nag Hammadi", StateID = 19, IsDeleted = false },
                        new City() { Id = 311, Name = "Dishna", StateID = 19, IsDeleted = false },
                        new City() { Id = 312, Name = "Qift", StateID = 19, IsDeleted = false },
                        new City() { Id = 313, Name = "Abu Tesht", StateID = 19, IsDeleted = false },
                        new City() { Id = 314, Name = "Farshout", StateID = 19, IsDeleted = false },
                        new City() { Id = 315, Name = "Naqada", StateID = 19, IsDeleted = false },
                        new City() { Id = 316, Name = "New Qena City", StateID = 19, IsDeleted = false },
                        new City() { Id = 317, Name = "El Waqf", StateID = 19, IsDeleted = false },
                        new City() { Id = 318, Name = "Hegaza", StateID = 19, IsDeleted = false },
                        new City() { Id = 319, Name = "El Tawd", StateID = 19, IsDeleted = false },
                        new City() { Id = 320, Name = "Koom Baloq", StateID = 19, IsDeleted = false },
                        new City() { Id = 321, Name = "Al Rizeqa", StateID = 19, IsDeleted = false },
                        new City() { Id = 322, Name = "Nag Al Ma’abda", StateID = 19, IsDeleted = false },
                        new City() { Id = 323, Name = "Abu Dawoud", StateID = 19, IsDeleted = false },
                        new City() { Id = 324, Name = "Hamra Dome", StateID = 19, IsDeleted = false },
                        new City() { Id = 325, Name = "Zawyet El Maragha", StateID = 19, IsDeleted = false },
                        new City() { Id = 326, Name = "Shaybiya", StateID = 19, IsDeleted = false },
                        
                        // Red Sea cities
                        new City() { Id = 327, Name = "Hurghada", StateID = 20, IsDeleted = false },
                        new City() { Id = 328, Name = "Safaga", StateID = 20, IsDeleted = false },
                        new City() { Id = 329, Name = "Marsa Alam", StateID = 20, IsDeleted = false },
                        new City() { Id = 330, Name = "El Qoseir", StateID = 20, IsDeleted = false },
                        new City() { Id = 331, Name = "Ras Ghareb", StateID = 20, IsDeleted = false },
                        new City() { Id = 332, Name = "Shalateen", StateID = 20, IsDeleted = false },
                        new City() { Id = 333, Name = "Halayeb", StateID = 20, IsDeleted = false },
                        new City() { Id = 334, Name = "Abu Ramada", StateID = 20, IsDeleted = false },
                        new City() { Id = 335, Name = "Berenice", StateID = 20, IsDeleted = false },
                        new City() { Id = 336, Name = "Baranis", StateID = 20, IsDeleted = false },
                        new City() { Id = 337, Name = "El Shalateen", StateID = 20, IsDeleted = false },
                        new City() { Id = 338, Name = "Sheikh Al Shazly", StateID = 20, IsDeleted = false },
                        new City() { Id = 339, Name = "Wadi Lahmy", StateID = 20, IsDeleted = false },
                        new City() { Id = 340, Name = "Soma Bay", StateID = 20, IsDeleted = false },
                        new City() { Id = 341, Name = "Makadi Bay", StateID = 20, IsDeleted = false },
                        
                        // Sharqia cities
                        new City() { Id = 342, Name = "Zagazig", StateID = 21, IsDeleted = false },
                        new City() { Id = 343, Name = "10th of Ramadan City", StateID = 21, IsDeleted = false },
                        new City() { Id = 344, Name = "Belbeis", StateID = 21, IsDeleted = false },
                        new City() { Id = 345, Name = "Abu Hammad", StateID = 21, IsDeleted = false },
                        new City() { Id = 346, Name = "Minya El Qamh", StateID = 21, IsDeleted = false },
                        new City() { Id = 347, Name = "Faqous", StateID = 21, IsDeleted = false },
                        new City() { Id = 348, Name = "Kafr Saqr", StateID = 21, IsDeleted = false },
                        new City() { Id = 349, Name = "Husseiniya", StateID = 21, IsDeleted = false },
                        new City() { Id = 350, Name = "Awlad Saqr", StateID = 21, IsDeleted = false },
                        new City() { Id = 351, Name = "Deyarb Negm", StateID = 21, IsDeleted = false },
                        new City() { Id = 352, Name = "Mashtool El Souq", StateID = 21, IsDeleted = false },
                        new City() { Id = 353, Name = "Hehia", StateID = 21, IsDeleted = false },
                        new City() { Id = 354, Name = "Monshaat Radwan", StateID = 21, IsDeleted = false },
                        new City() { Id = 355, Name = "Al Obour", StateID = 21, IsDeleted = false },
                        new City() { Id = 356, Name = "Al Tall Al Kebeer", StateID = 21, IsDeleted = false },
                        new City() { Id = 357, Name = "Maydan Al Masala", StateID = 21, IsDeleted = false },


                    };

                await context.City.AddRangeAsync(cities);
                await context.SaveChangesAsync();
            }

            if (!await context.CoreServices.IgnoreQueryFilters().AnyAsync())
            {
                await context.CoreServices.AddRangeAsync(
                    new CoreService() { Id = 1, Name = "Membership Management" },
                    new CoreService() { Id = 2, Name = "Facility Booking" },
                    new CoreService() { Id = 3, Name = "Personalized Training" },
                    new CoreService() { Id = 4, Name = "Expert Coaching" },
                    new CoreService() { Id = 5, Name = "Event Participation" },
                    new CoreService() { Id = 6, Name = "Health & Wellness Services" },
                    new CoreService() { Id = 7, Name = "Community Engagement" },
                    new CoreService() { Id = 8, Name = "Performance Tracking" },
                    new CoreService() { Id = 9, Name = "Sponsorship Opportunities" },
                    new CoreService() { Id = 10, Name = "Equipment & Gear Services" },
                    new CoreService() { Id = 11, Name = "Travel & Accommodation" },
                    new CoreService() { Id = 12, Name = "Digital Access" },
                    new CoreService() { Id = 13, Name = "Feedback & Support" }
                );
                await context.SaveChangesAsync();


            }

            if (!await context.NewCoreUtilities.IgnoreQueryFilters().AnyAsync())
            {
                await context.NewCoreUtilities.AddRangeAsync(
                [
                    new NewCoreUtilities { UtilityName = "Restrooms",                    IsDeleted = false },
                        new NewCoreUtilities { UtilityName = "Changing Rooms",               IsDeleted = false },
                        new NewCoreUtilities { UtilityName = "Lockers",                      IsDeleted = false },
                        new NewCoreUtilities { UtilityName = "Showers",                      IsDeleted = false },
                        new NewCoreUtilities { UtilityName = "Drinking Water Station",       IsDeleted = false },
                        new NewCoreUtilities { UtilityName = "First Aid / Medical Room",     IsDeleted = false },
                        new NewCoreUtilities { UtilityName = "Reception / Check-in Desk",    IsDeleted = false },
                        new NewCoreUtilities { UtilityName = "Equipment Storage Room",       IsDeleted = false },
                        new NewCoreUtilities { UtilityName = "Security (CCTV & Access)",     IsDeleted = false },
                        new NewCoreUtilities { UtilityName = "Parking / Drop-off Area",      IsDeleted = false },
                    ]);

                await context.SaveChangesAsync();
            }
            if (!await context.TurfTypes.IgnoreQueryFilters().AnyAsync())
            {
                await context.TurfTypes.AddRangeAsync(
                [
                    new TurfTypes { TypeName = "Natural Grass (Bermuda)",                    IsDeleted = false },
                        new TurfTypes { TypeName = "Natural Grass (Paspalum)",                   IsDeleted = false },
                        new TurfTypes { TypeName = "Hybrid Grass (Natural + Synthetic Fibers)",  IsDeleted = false },
                        new TurfTypes { TypeName = "Artificial Turf 3G (Rubber + Sand Infill)",  IsDeleted = false },
                        new TurfTypes { TypeName = "Artificial Turf 4G",                         IsDeleted = false },
                        new TurfTypes { TypeName = "Artificial Turf with Shockpad (5G System)",  IsDeleted = false },
                        new TurfTypes { TypeName = "Artificial Turf (Sand-Filled)",              IsDeleted = false },
                        new TurfTypes { TypeName = "Artificial Turf (Non-Infill)",               IsDeleted = false },
                        new TurfTypes { TypeName = "Multi-Sport Synthetic Turf",                 IsDeleted = false },
                        new TurfTypes { TypeName = "Indoor Turf (Short Pile)",                   IsDeleted = false },
                    ]);

                await context.SaveChangesAsync();
            }

            if (! await context.PackageScopes.IgnoreQueryFilters().AnyAsync())
            {
                await context.PackageScopes.AddRangeAsync(
                [
                    new PackageScope{ScopeType =Data.Membership.Entities.Enums.PackageScopeType.Access },
                    new PackageScope{ScopeType =Data.Membership.Entities.Enums.PackageScopeType.Eligibility},
                ]);
                await context.SaveChangesAsync();

            }
            #endregion

            #region add parent regitration form 
            if (!context.Forms.IgnoreQueryFilters().Any(x => x.Id == 167))
            {
                context.Forms.Add(new Form
                {
                    Id = 167,
                    Title = "Parent Data Form",
                    Alias = "ParentDataForm",
                    Fees = 0,
                    WorkflowSelection = "Approval Workflow",
                    RequestTypeId = 21, // update if needed
                    IsDeleted = false,
                    IsDraft = false
                });

                context.SaveChanges();
            }

            if (!context.FormFields.IgnoreQueryFilters().Any(ff => ff.FormId == 167))
            {
                context.FormFields.AddRange(

                    // First Name
                    new FormFields
                    {
                        FormId = 167,
                        FieldId = 51,
                        IsRequired = true,
                        IsEditable = true,
                        Index = 1,
                        IsDeleted = false,
                        IsHidden = false
                    },

                    // Second Name
                    new FormFields
                    {
                        FormId = 167,
                        FieldId = 60,
                        IsRequired = true,
                        IsEditable = true,
                        Index = 2,
                        IsDeleted = false,
                        IsHidden = false
                    },

                    // Middle Name
                    new FormFields
                    {
                        FormId = 167,
                        FieldId = 61,
                        IsRequired = false,
                        IsEditable = true,
                        Index = 3,
                        IsDeleted = false,
                        IsHidden = false
                    },

                    // Last Name
                    new FormFields
                    {
                        FormId = 167,
                        FieldId = 52,
                        IsRequired = true,
                        IsEditable = true,
                        Index = 4,
                        IsDeleted = false,
                        IsHidden = false
                    },

                    // Email
                    new FormFields
                    {
                        FormId = 167,
                        FieldId = 56,
                        IsRequired = true,
                        IsEditable = true,
                        Index = 5,
                        IsDeleted = false,
                        IsHidden = false
                    },

                    // Phone Number
                    new FormFields
                    {
                        FormId = 167,
                        FieldId = 57,
                        IsRequired = true,
                        IsEditable = true,
                        Index = 6,
                        IsDeleted = false,
                        IsHidden = false
                    },

                    // National ID
                    new FormFields
                    {
                        FormId = 167,
                        FieldId = 58,
                        IsRequired = true,
                        IsEditable = true,
                        Index = 7,
                        IsDeleted = false,
                        IsHidden = false
                    },

                    // Profile Image
                    new FormFields
                    {
                        FormId = 167,
                        FieldId = 89,
                        IsRequired = false,
                        IsEditable = true,
                        Index = 8,
                        IsDeleted = false,
                        IsHidden = false
                    },

                    // Is Parent (Hidden)
                    new FormFields
                    {
                        FormId = 167,
                        FieldId = 94,
                        IsRequired = false,
                        IsEditable = true,
                        IsHidden = true,
                        Index = 9,
                        IsDeleted = false
                    }

                );

                context.SaveChanges();
            }
            #endregion

            tenantProvider.IgnoreTenantFilter = false;


        }
    }
}

