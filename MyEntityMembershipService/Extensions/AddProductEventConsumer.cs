using Common.RabbitMq.Membership;
using DocumentFormat.OpenXml.Spreadsheet;
using MassTransit;
using MyEntityMembershipService.Consumers;
using MyEntityMembershipService.Data.Membership.Entities.Enums;
using MyEntityMembershipService.Repositories.UnitOfWorkRepo;
using MyEntityMembershipService.SpecificationPattern;
using Serilog;

namespace MyEntityMembershipService.Extensions
{
    public class AddProductEventConsumer : IConsumer<ProductCreatedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddProductEventConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
        {
            try
            {
                var message = context.Message;

                Log.Information($"Received ProductCreatedEvent: SourceId={message.SourceId}, Name={message.Name}, ProductType={message.ProductType}");
                if(message is null)
                {
                    Log.Error("[RabbitMQ] Received null ProductCreatedEvent");
                    return;
                }

                if(message.SourceId <= 0)
                {
                    Log.Error($"[RabbitMQ] Invalid SourceId in ProductCreatedEvent: {message.SourceId}");
                    return;
                }


                var existingProductSpec = new GeneralSpecifications<Data.Membership.Entities.Product>(p => p.ProductId == message.SourceId);
                var existingProduct = await _unitOfWork.Repository<Data.Membership.Entities.Product>().GetWithSpecifications(existingProductSpec);

                if (existingProduct is not null)
                {
                    Log.Warning($"[RabbitMQ] Product with SourceId {message.SourceId} already exists. Skipping creation.");
                    return;
                }

                var product = new Data.Membership.Entities.Product
                {
                    ProductId = message.SourceId,
                    Name = message.Name,
                    Image = message.ImageUrl,
                    StartDate = message.StartDate,
                    EndDate = message.EndDate,
                    GlobalTenantId = message.TenantId,
                    ProductType = (ProductType) message.ProductType
                };

                await _unitOfWork.Repository<Data.Membership.Entities.Product>().Add(product);
                await _unitOfWork.Save();

                Log.Information($"[RabbitMQ] Successfully processed ProductCreatedEvent for ProductId {message.SourceId}");

            }
            catch(Exception ex)
            {
                Log.Error($"[RabbitMQ] Error processing ProductCreatedEvent: {ex.Message}");
            }
        }
    }


    public class AddProductEventConsumerDefinition : ConsumerDefinition<AddProductEventConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AddProductEventConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }
    }
}
