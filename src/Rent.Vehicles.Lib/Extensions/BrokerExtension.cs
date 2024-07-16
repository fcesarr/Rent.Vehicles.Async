using System.Diagnostics.CodeAnalysis;

using Amqp;
using Amqp.Framing;
using Amqp.Types;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

using ConnectionFactory = RabbitMQ.Client.ConnectionFactory;
using IConnection = RabbitMQ.Client.IConnection;

namespace Rent.Vehicles.Lib.Extensions;

[ExcludeFromCodeCoverage]
public static class BrokerExtension
{
    public static IServiceCollection AddARabbitMqBroker(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddSingleton<IConnection>(service =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetConnectionString("Broker") ?? string.Empty;

                var factory = new ConnectionFactory
                {
                    Uri = new Uri(connectionString),
                    DispatchConsumersAsync = true,
                    ConsumerDispatchConcurrency = 100
                };

                return factory.CreateConnection();
            })
            .AddTransient<IConsumer>(service =>
            {
                var connection = service.GetRequiredService<IConnection>();
                return new RabbitMQConsumer(connection.CreateModel());
            })
            .AddSingleton<IPublisher>(service =>
            {
                var connection = service.GetRequiredService<IConnection>();
                var serializer = service.GetRequiredService<ISerializer>();
                return new RabbitMQPublisher(connection.CreateModel(), serializer);
            });
    }

    public static IServiceCollection AddAmqpLiteBroker(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddSingleton<ISession>(services =>
            {
                var brokerConnectionString = configuration.GetConnectionString("Broker") ?? string.Empty;

                Uri uri = new Uri(brokerConnectionString);

                var address = new Address(uri.ToString());

                string virtualHost = Uri.UnescapeDataString(uri.AbsolutePath.Substring(1));

                var open = new Open
                {
                    HostName = $"vhost:{virtualHost}",
                };

                var connection = Connection.Factory.CreateAsync(address, open: open)
                    .GetAwaiter()
                    .GetResult();

                return new Session(connection);
            })
            .AddTransient<IConsumer>(services =>
            {
                var logger = services.GetRequiredService<ILogger<AmqpConsumer>>();

                var session = services.GetRequiredService<ISession>();

                return new AmqpConsumer(logger, session);
            })
            .AddSingleton<IPublisher>(services =>
            {
                var serializer = services.GetRequiredService<ISerializer>();

                var session = services.GetRequiredService<ISession>();

                return new AmqpPublisher(session, serializer);
            });
    }
}
