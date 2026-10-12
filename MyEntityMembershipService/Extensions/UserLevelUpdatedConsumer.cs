using Common.RabbitMq.Users;
using MassTransit;
using MyEntityMembershipService.Consumers;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using Serilog;

namespace MyEntityMembershipService.Extensions
{
public class UserLevelUpdatedConsumer : IConsumer<UserLevelUpdatedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    public UserLevelUpdatedConsumer(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<UserLevelUpdatedEvent> context)
    {
        Log.Information("Starting Add level Option By RabbitMQ");

        var message = context.Message;

        var membership = await _unitOfWork.Repository<Membership>()
            .GetByIdAsync(message.MembershipId);

        if (membership == null)
        {
            Log.Error($"[RabbitMQ] Membership {message.MembershipId} not found for user {message.UserId}");
            return;
        }

        membership.BeltName = message.OptionName;
        var memberlevel = new MemberSportLevel
        {
            MembershipId = message.MembershipId,
            UserId = message.UserId,
            TenantId = message.TenantId,
            LevelId = message.LevelId,
            OptionId = message.OptionId,
            LevelName = message.LevelName,
            LevelNameAr = message.LevelNameAr,
            OptionName = message.OptionName,
            OptionNameAr = message.OptionNameAr,
            SportId = message.SportId,
            SportName = message.SportName,
            SportNameAr = message.SportNameAr,
            CreatedBy = message.ChangedBy,
            CategoryName = message.CategoryName,
            CategoryId = message.CategoryId,
            SportImage = message.SportImage,
            CategoryNameAr = message.CategoryNameAr
        };
        membership.MemberSportLevels.Add(memberlevel);

        _unitOfWork.Repository<Membership>().Update(membership);
        await _unitOfWork.Save();

        Log.Information($"[RabbitMQ] Updated Level for User {message.UserId} to {message.OptionName}");
    }
}

public class UserLevelUpdatedConsumerDefinition : ConsumerDefinition<UserLevelUpdatedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UserLevelUpdatedConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
    }
}}