using Common.RabbitMq.Users;
using MassTransit;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using Serilog;

namespace MyEntityMembershipService.Extensions
{
    public class UserSportDeletedConsumer : IConsumer<UserSportDeletedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserSportDeletedConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<UserSportDeletedEvent> context)
        {
            var message = context.Message;

            Log.Information("[RabbitMQ] Received UserSportDeletedEvent for User {UserId}, Sport {SportId}",
                            message.UserId, message.SportId);

            var records = await _unitOfWork.Repository<MemberSportLevel>()
                .GetAllAsync(x =>
                    x.UserId == message.UserId &&
                    x.SportId == message.SportId &&
                    x.TenantId == message.TenantId
                );

            if (records == null || !records.Any())
            {
                Log.Warning("[RabbitMQ] No MemberSportLevel records found for User {UserId}, Sport {SportId}",
                            message.UserId, message.SportId);
                return;
            }

            _unitOfWork.Repository<MemberSportLevel>().SoftDeleteRange(records);
            await _unitOfWork.Save();

            Log.Information("[RabbitMQ] Soft deleted {Count} MemberSportLevel records for User {UserId}, Sport {SportId}",
                            records.Count, message.UserId, message.SportId);
        }
    }

    public class UserSportDeletedConsumerDefinition : ConsumerDefinition<UserSportDeletedConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                                                  IConsumerConfigurator<UserSportDeletedConsumer> consumerConfigurator)
        {
            // Retry policy: 3 attempts, 5 seconds apart
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }
    }
}