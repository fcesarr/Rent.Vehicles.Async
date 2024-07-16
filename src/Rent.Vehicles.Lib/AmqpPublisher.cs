using Amqp;
using Amqp.Framing;

using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Lib;

public class AmqpPublisher : IPublisher
{
    private readonly ISerializer _serializer;

    private readonly ISession _session;

    private readonly IDictionary<string, ISenderLink> _senderLinks = new Dictionary<string, ISenderLink>();

    public AmqpPublisher(ISession session, ISerializer serializer)
    {
        _session = session;
        _serializer = serializer;
    }

    public async Task PublishCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : Command
    {
        ISenderLink? sender;

        if(!_senderLinks.TryGetValue(command.GetType().Name, out sender))
        {
            sender = _session.CreateSender(Guid.NewGuid().ToString(), command.GetType().Name);

            _senderLinks.Add(command.GetType().Name, sender);
        }

        var data = await _serializer.SerializeAsync(command, command.GetType(), cancellationToken);

        var message = new Amqp.Message { BodySection = new Data { Binary = data } };

        sender.Send(message);
    }

    public async Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : Event
    {
        ISenderLink? sender;

        if(!_senderLinks.TryGetValue(@event.GetType().Name, out sender))
        {
            sender = _session.CreateSender(Guid.NewGuid().ToString(), @event.GetType().Name);

            _senderLinks.Add(@event.GetType().Name, sender);
        }

        var data = await _serializer.SerializeAsync(@event, @event.GetType(), cancellationToken);

        var message = new Amqp.Message { BodySection = new Data { Binary = data } };

        sender.Send(message);
    }

    public async Task PublishSingleEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : Event
    {
        await PublishEventAsync(@event, cancellationToken);
    }
}
