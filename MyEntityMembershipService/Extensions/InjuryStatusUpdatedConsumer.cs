using MassTransit;
using System.Threading.Tasks;
using Common.RabbitMq.Users;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.DTOs.Membership.Swimming;
using Serilog;
using Microsoft.AspNetCore.Http;
using MyEntityMembershipService.Extensions;

namespace MyEntityMembershipService.Consumers
{
    public class InjuryStatusUpdatedConsumer : IConsumer<InjuryStatusUpdatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        public InjuryStatusUpdatedConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<InjuryStatusUpdatedEvent> context)
        {
            Log.Information("Starting Add injury status By RabbitMQ");
           

            var message = context.Message;

            // Find the membership by UserId
            var memberships = await _unitOfWork.Repository<Membership>()
                .GetAllAsync(m => m.UserId == message.UserId && m.TenantId == message.TenantId);

            if (memberships == null)
            {
                Log.Error($"[RabbitMQ] Membership not found for user {message.UserId}");
                return;
            }

            // Update the InjuryStatus based on event
            foreach (var membership in memberships)
            {
                membership.InjuryStatus = message.ActiveInjury
               ? InjuryStatus.Injured
               : InjuryStatus.Uninjured;

                _unitOfWork.Repository<Membership>().Update(membership);


            }

            await _unitOfWork.Save();

            Log.Information($"[RabbitMQ] Updated injury status for User {message.UserId} to {message.ActiveInjury}");
        }
    }

    public class InjuryStatusUpdatedConsumerDefinition : ConsumerDefinition<InjuryStatusUpdatedConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<InjuryStatusUpdatedConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }
    }
}
