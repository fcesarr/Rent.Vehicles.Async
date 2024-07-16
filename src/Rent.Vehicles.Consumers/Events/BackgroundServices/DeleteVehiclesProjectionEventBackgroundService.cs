using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class DeleteVehiclesProjectionEventBackgroundService : HandlerEventServicePublishBackgroundService<
    DeleteVehiclesProjectionEvent>
{
    public DeleteVehiclesProjectionEventBackgroundService(
        ILogger<DeleteVehiclesProjectionEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IOptions<ConsumerSetting> consumerSetting,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        consumerSetting, serviceScopeFactory)
    {
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(DeleteVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        using var serviceScope = _serviceScopeFactory.CreateScope();

        var serviceProvider = serviceScope.ServiceProvider;

        var service = serviceProvider.GetRequiredService<IVehicleProjectionFacade>();

        var entity = await service.DeleteAsync(@event, cancellationToken);

        if (!entity.IsSuccess)
        {
            _logger.LogError(entity.Exception, "Aqui Projection");
            return entity.Exception!;
        }


        _logger.LogInformation("Aqui Projection");
        return Task.CompletedTask;
    }
}
