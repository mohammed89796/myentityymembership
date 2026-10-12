

using AutoMapper;
using Microsoft.EntityFrameworkCore;

using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Entity.AtheleteDtos;
using MyEntityMembershipService.Helper;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.SpecificationPattern;
using System.Globalization;

namespace MyEntityMembershipService.Services.Entity.AtheleteService
{
    public class AtheleteService : IAtheleteService
    {
        private readonly Repositories.UnitOfWorkRepo.IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AtheleteService(Repositories.UnitOfWorkRepo.IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Web
        public async Task<Response<int>> CreateAtheleteAsync(AddAtheleteDto atheleteDto)
        {

            try
            {

                var validationResult = await ValidateOnAddAthelete(atheleteDto);

                if (validationResult.Length > 0)
                    return new ResponseHandler().BadRequest<int>(validationResult);

                var entityMember = _mapper.Map<AddAtheleteDto, EntityMember>(atheleteDto);

                if (atheleteDto.PersonalPhoto != null)
                {
                    entityMember.PersonalPhotoUrl = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString"), "buildupimagedocs", atheleteDto.PersonalPhoto);
                }

                if (atheleteDto.BeltCertificate != null)
                {
                    entityMember.BeltCertificateUrl = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString"), "buildupimagedocs", atheleteDto.BeltCertificate);
                }
                if (atheleteDto.NationalIdPhoto != null)
                {
                    entityMember.NationalIdUrl = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString"), "buildupimagedocs", atheleteDto.NationalIdPhoto);
                }
                if (atheleteDto.PersonalPhotoWithStamp != null)
                {
                    entityMember.PersonalPhotoWithStampUrl = await DocumentSetting.AddToContainerAndGetUrl(Environment.GetEnvironmentVariable("StorageConnectionString"), "buildupimagedocs", atheleteDto.PersonalPhotoWithStamp);
                }
                await _unitOfWork.Repository<EntityMember>().Add(entityMember);

                await _unitOfWork.Save();


                return new ResponseHandler().Created(entityMember.Id);

            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<int>(ex.Message);
            }
        }

        public async Task<Response<List<AtheleteToReturnDto>>> GetDraftAtheleteListAsync(int tenantId)
        {
            try
            {


                var entityMemberSpecifications = new GeneralSpecifications<EntityMember>(em => em.TenantId == tenantId && em.BatchId == null) { AsNoTracking = true };

                entityMemberSpecifications.AddInclude(follow => follow.Include(em => em.Batch));
                entityMemberSpecifications.AddInclude(follow => follow.Include(em => em.Belt));
                var entityMembers = await _unitOfWork.Repository<EntityMember>().GetAllWithSpecifications(entityMemberSpecifications);

                var atheleteList = _mapper.Map<List<AtheleteToReturnDto>>(entityMembers);

                var tasks= atheleteList.Select(async athelete =>
                {
                    if (!string.IsNullOrWhiteSpace(athelete.PersonalPhotoUrl))
                    {
                        athelete.PersonalPhotoUrl = await DocumentSetting.GetPreSignedUrlAsync( athelete.PersonalPhotoUrl);
                    }
                    if (!string.IsNullOrWhiteSpace(athelete.BeltCertificateUrl))
                    {
                        athelete.BeltCertificateUrl = await DocumentSetting.GetPreSignedUrlAsync( athelete.BeltCertificateUrl);
                    }
                    if (!string.IsNullOrWhiteSpace(athelete.NationalIdUrl))
                    {
                        athelete.NationalIdUrl = await DocumentSetting.GetPreSignedUrlAsync( athelete.NationalIdUrl);
                    }
                    if (!string.IsNullOrWhiteSpace(athelete.PersonalPhotoWithStampUrl))
                    {
                        athelete.PersonalPhotoWithStampUrl = await DocumentSetting.GetPreSignedUrlAsync( athelete.PersonalPhotoWithStampUrl);
                    }
                });
                return new ResponseHandler().Success(atheleteList);
            }
            catch (Exception ex)
            {

                return new ResponseHandler().BadRequest<List<AtheleteToReturnDto>>("Something went wrong!");
            }

        }

        #endregion

        #region Validations
        private async Task<string> ValidateOnAddAthelete(AddAtheleteDto atheleteDto)
        {


            if (atheleteDto == null)
            {
                return "Athelete data is null";
            }
            if (atheleteDto.NationalId.Length > 0)
            {

                string dobPart = atheleteDto.NationalId.Substring(1, 6);

                if (!DateOnly.TryParseExact(dobPart, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsedDOF))
                    return "National ID contains invalid date of birth";

                if (parsedDOF != atheleteDto.DateOfBirth)
                    return "Date of birth does not match with National ID";


                int.TryParse(atheleteDto.NationalId.ElementAt(12).ToString(), out int checkDigit);

                if (checkDigit % 2 == 0 && atheleteDto.Gender == Gender.Male)
                    return "Gender does not match with National ID";

                if (checkDigit % 2 != 0 && atheleteDto.Gender == Gender.Female)
                    return "Gender does not match with National ID";

            }

            if (atheleteDto.DateOfBirth >= DateOnly.FromDateTime(DateTime.Now))
                return "Date of birth cannot be in the future";

            if (atheleteDto.DateOfAcquisition >= DateOnly.FromDateTime(DateTime.Now))
                return "Date of acquisition cannot be in the future";

            if (atheleteDto.DateOfAcquisition <= atheleteDto.DateOfBirth)
                return "Date of acquisition must be after date of birth";

            if (atheleteDto.DateOfExpiry <= DateOnly.FromDateTime(DateTime.Now))
                return "Date of expiry cannot be in the past";

            if (atheleteDto.DateOfIssuance.HasValue && atheleteDto.DateOfIssuance.Value >= DateOnly.FromDateTime(DateTime.Now))
                return "Date of issuance cannot be in the future";

            if (atheleteDto.DateOfIssuance.HasValue && atheleteDto.DateOfExpiry <= atheleteDto.DateOfIssuance)
                return "Date of expiry must be after date of issuance";

            var validationResult = await ValidateOnAtheleteContactData(atheleteDto);

            if (validationResult.Length > 0)
                return validationResult;

            var mediaValidation = ValidateOnAtheleteMedia(atheleteDto);

            if (mediaValidation.Length > 0)
                return mediaValidation;
            return string.Empty;

        }

        private async Task<string> ValidateOnAtheleteContactData(ICommonData commonData)
        {
            if (!string.IsNullOrWhiteSpace(commonData.Email))
            {
                bool isExsists = await _unitOfWork.Repository<EntityMember>().IsExistsAsync(x => x.Email == commonData.Email);

                if (isExsists)
                    return "Email already exists";
            }

            if (!string.IsNullOrWhiteSpace(commonData.PassportNumber))
            {
                bool isExsists = await _unitOfWork.Repository<EntityMember>().IsExistsAsync(x => x.PassportNumber == commonData.PassportNumber);

                if (isExsists)
                    return "Passport Number already exists";
            }

            if (!string.IsNullOrWhiteSpace(commonData.NationalId))
            {
                bool isExsists = await _unitOfWork.Repository<EntityMember>().IsExistsAsync(x => x.NationalId == commonData.NationalId);

                if (isExsists)
                    return "National ID already exists";
            }

            if (!string.IsNullOrWhiteSpace(commonData.MobileNumber))
            {
                bool isExsists = await _unitOfWork.Repository<EntityMember>().IsExistsAsync(x => x.MobileNumber == commonData.MobileNumber);
                if (isExsists)
                    return "Mobile number already exists";
            }

            if (!string.IsNullOrWhiteSpace(commonData.HomeNumber))
            {
                bool isExsists = await _unitOfWork.Repository<EntityMember>().IsExistsAsync(x => x.HomeNumber == commonData.HomeNumber);
                if (isExsists)
                    return "Home number already exists";
            }
            return string.Empty;
        }

        private string ValidateOnAtheleteMedia(ICommonData commonData)
        {
            if (commonData.PersonalPhoto != null)
            {
                if (!DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(commonData.PersonalPhoto,allowedExtensions: new List<string> { ".jpg", ".jpeg", ".png", ".webp" }))
                    return "Personal photo is not a valid format or exceeds max size";
            }
            if (commonData.NationalIdPhoto != null)
            {
                if (!DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(commonData.NationalIdPhoto))
                    return "National ID photo is not a valid format or exceeds max size";
            }
            if (commonData.BeltCertificate != null)
            {
                if (!DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(commonData.BeltCertificate))
                    return "Belt certificate is not a valid format or exceeds max size";
            }
            if (commonData.PersonalPhotoWithStamp != null)
            {
                if (!DocumentSetting.IsFileValidFormatAndDoesnotExceedMaxSize(commonData.PersonalPhotoWithStamp))
                    return "Personal photo with stamp is not a valid format or exceeds max size";
            }
            return string.Empty;
        }

        #endregion
    }
}

