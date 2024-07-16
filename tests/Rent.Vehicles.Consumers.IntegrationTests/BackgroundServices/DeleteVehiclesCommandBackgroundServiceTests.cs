using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

using AutoFixture;

using FluentAssertions;

using Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices.ClassDatas;
using Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;
using Rent.Vehicles.Consumers.IntegrationTests.Extensions;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Services.Extensions;
using Rent.Vehicles.Services.Responses;

using CommandResponse = Rent.Vehicles.Api.Responses.CommandResponse;

namespace Rent.Vehicles.Consumers.IntegrationTests.BackgroundServices;

[Collection(nameof(IntegrationTestWebAppFactoryFixture))]
public class DeleteVehiclesCommandBackgroundServiceTests : IAsyncLifetime
{
    private readonly Fixture _fixture;

    private readonly HttpClient _httpClient;

    private readonly IntegrationTestWebAppFactory _integrationTestWebAppFactory;

    private readonly JsonSerializerOptions _options = new()
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }, PropertyNameCaseInsensitive = true
    };

    public DeleteVehiclesCommandBackgroundServiceTests(IntegrationTestWebAppFactory integrationTestWebAppFactory)
    {
        _fixture = new Fixture();

        _integrationTestWebAppFactory = integrationTestWebAppFactory;

        _httpClient = _integrationTestWebAppFactory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _integrationTestWebAppFactory.ResetDatabaseAsync();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    [Theory(DisplayName =
        $"{nameof(DeleteVehiclesCommandBackgroundServiceTests)}.{nameof(SendDeleteVehiclesCommandVerifyEventStatusAndStatusCode)}")]
    [ClassData(typeof(DeleteVehiclesCommandBackgroundServiceTestData))]
    public async Task SendDeleteVehiclesCommandVerifyEventStatusAndStatusCode(Tuple<string, StatusType>[] tuples,
        HttpStatusCode statusCode,
        IEnumerable<dynamic> entities,
        string endpointAction,
        string endpointGet)
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        foreach (var entity in entities)
        {
            _ = await _integrationTestWebAppFactory.SaveAsync(entity, cancellationTokenSource.Token);

            if (entity.GetType().Name == "Vehicle")
            {
                _ = await _integrationTestWebAppFactory.SaveAsync(ToExtension.ToProjection<VehicleProjection>(entity),
                    cancellationTokenSource.Token);
            }
        }

        var response = await _httpClient.DeleteAsync(endpointAction, cancellationTokenSource.Token);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationTokenSource.Token);

        var commandResponse = JsonSerializer.Deserialize<CommandResponse>(responseBody, _options);

        var location = response?.Headers?.Location?.ToString();

        var found = false;

        do
        {
            var locationResponse = await _httpClient.GetAsync(location, cancellationTokenSource.Token);

            IList<EventResponse> events = [];

            if (locationResponse.StatusCode == HttpStatusCode.OK)
            {
                var locationResponseBody =
                    await locationResponse.Content.ReadAsStringAsync(cancellationTokenSource.Token);
                events = JsonSerializer.Deserialize<IList<EventResponse>>(locationResponseBody, _options) ?? [];
            }

            var entityResponse = await _httpClient.GetAsync(endpointGet, cancellationTokenSource.Token);

            found = events.GroupBy(v => v.SagaId)
                        .Where(g => g.Count() == tuples.Length)
                        .SelectMany(x => x.ToArray())
                        .AllOrFalseIfEmpty(x => tuples.Any(y => y.Item1 == x.Name && y.Item2 == x.StatusType)) &&
                    entityResponse.StatusCode == statusCode;

            await periodicTimer.WaitForNextTickAsync(cancellationTokenSource.Token);
        } while (!found && !cancellationTokenSource.IsCancellationRequested);

        // Assert
        found.Should().BeTrue();

        response?.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
