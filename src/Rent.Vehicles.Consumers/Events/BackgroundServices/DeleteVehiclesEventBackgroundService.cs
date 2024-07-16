using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;

using Event = Rent.Vehicles.Lib.Event;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class DeleteVehiclesEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    DeleteVehiclesEvent>
{
    public DeleteVehiclesEventBackgroundService(ILogger<DeleteVehiclesEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IOptions<ConsumerSetting> consumerSetting,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        consumerSetting, serviceScopeFactory)
    {
    }

    protected override IEnumerable<Event> CreateEventToPublish(DeleteVehiclesEvent @event)
    {
        return
        [
            new DeleteVehiclesProjectionEvent { Id = @event.Id, SagaId = @event.SagaId }
        ];
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(DeleteVehiclesEvent @event,
        CancellationToken cancellationToken = default)
    {
        using var serviceScope = _serviceScopeFactory.CreateScope();

        var serviceProvider = serviceScope.ServiceProvider;

        var service = serviceProvider.GetRequiredService<IVehicleFacade>();

        var entity = await service.DeleteAsync(@event, cancellationToken);

        if (!entity.IsSuccess)
        {
            _logger.LogError(entity.Exception, "Aqui Event");
            return entity.Exception!;
        }

        _logger.LogInformation("Aqui Event");
        return Task.CompletedTask;
    }
}
