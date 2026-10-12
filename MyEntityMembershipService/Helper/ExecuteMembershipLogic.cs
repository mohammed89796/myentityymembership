using AutoMapper;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.DTOs.Membership.FormDtos;
using MyEntityMembershipService.DTOs.Membership.MembershipDtos;
using MyEntityMembershipService.Repositories.Common;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.Services.Membership.FormServ;
namespace MyEntityMembershipService.Helper
{
    public static class ExecuteMembershipLogic
    {
        public static async Task<Response<MembershipDto>> ExecuteTransferAndLoanMembershipLogic(IUnitOfWork unitOfWork, IMapper mapper, MembershipRequest request)
        {
            var oldmembership = await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Membership>().GetByIdAsync(request.MembershipId.Value);

            if (oldmembership.MembershipStatus == MembershipStatus.Transfered || oldmembership.MembershipStatus == MembershipStatus.Removed)
            {
                return new ResponseHandler().BadRequest<MembershipDto>("Membership is Transfered or Removed allready.");
            }
            else
            {
                if (request.RequestTypeId == 19)
                {
                    oldmembership.MembershipStatus = MembershipStatus.Transfered;

                }
                else if (request.RequestTypeId == 20)
                {
                    oldmembership.MembershipStatus = MembershipStatus.Loaned;

                }

                unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Membership>().Update(oldmembership);

            }

            var newMembership = new MyEntityMembershipService.Data.Membership.Entities.Membership
            {
                FirstName = oldmembership.FirstName,
                LastName = oldmembership.LastName,
                StartDate = oldmembership.StartDate,
                EndDate = oldmembership.EndDate,
                MembershipType = oldmembership.MembershipType,
                NationalId = oldmembership.NationalId,
                Nationality = oldmembership.Nationality,
                NationalIDPhoto = oldmembership.NationalIDPhoto,
                UserPhoto = oldmembership.UserPhoto,
                BirthDate = oldmembership.BirthDate,
                PhoneNumber = oldmembership.PhoneNumber,
                Gender = oldmembership.Gender,
                Address = oldmembership.Address,
                Country = oldmembership.Country,
                City = oldmembership.City,
                Governate = oldmembership.Governate,
                PackageId = oldmembership.PackageId,
                FileNumber = oldmembership.FileNumber,
                Email = oldmembership.Email,
                UserId = oldmembership.UserId,
                MemberTypeId = oldmembership.MemberTypeId,
                ParentMembershipId = oldmembership.ParentMembershipId,
                MemberShipId = Guid.NewGuid().ToString(),
                BeltName = oldmembership.BeltName,

                // Overwritten fields for new membership
                MembershipStatus = MembershipStatus.Active,
                TenantId = request.TenantId,
                CreatedBy = request.CreatedBy,
                CreationDate = DateTime.Now
            };

            await unitOfWork.Repository<MyEntityMembershipService.Data.Membership.Entities.Membership>().Add(newMembership);
            var newMembershipDto = mapper.Map<MembershipDto>(newMembership);
            return new ResponseHandler().Success(newMembershipDto);

        }


        public async static Task<Response<object>> CheckOutMembership(IFormService formService, IUnitOfWork unitOfWork, MembershipRequest request, int requestId)
        {
            // Step 4: Create Membership
            var executeLogicResult = await formService.ExecuteLogicForRequestType(new ExecuteLogicForRequestTypeDto
            {
                RequestTypeId = 6,
                FormUserResponseId = request.FormUserResponse.Id,
            });

            if (!executeLogicResult.Succeeded)
            {
                return new ResponseHandler().BadRequest<object>("Failed to execute logic for the request type.");
            }
            //step:5 Update Requsrt status History

            var existingHistory = request.RequestStatusHistories
          .FirstOrDefault(h => h.MembershipRequestId == requestId && h.RequestStatusId == RequestStatus.RequestResolved);

            if (existingHistory == null)
            {
                request.RequestStatusHistories.Add(new RequestStatusHistory
                {
                    MembershipRequestId = requestId,
                    RequestStatusId = RequestStatus.RequestResolved,
                    ModificationDate = DateTime.Now,
                });
            }
            else
            {
                existingHistory.ModificationDate = DateTime.Now;

                unitOfWork.Repository<MembershipRequest>().Update(request);
            }
            return new ResponseHandler().Success<object>("Done");

        }
    }

}
