using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Exceptions;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class VehicleDataService : DataService<Vehicle>, IVehicleDataService
{
    public VehicleDataService(ILogger<VehicleDataService> logger,
        IVehicleValidator validator,
        IRepository<Vehicle> repository) : base(logger, validator, repository)
    {
    }

    public async Task<Result<Vehicle>> RentItAsync(CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => !x.IsRented, cancellationToken);

        if (!entity.IsSuccess)
        {
            return new NoVehicleToRentException("Não existe veiculo disponível para aluguel");
        }

        entity.Value!.IsRented = true;
        entity.Value!.RentCount++;

        return await UpdateAsync(entity.Value!, cancellationToken);
    }

    public async Task<Result<Vehicle>> ReturnItAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        entity.Value!.IsRented = false;

        return await UpdateAsync(entity.Value!, cancellationToken);
    }

    public async Task<Result<Vehicle>> UpdateAsync(Guid id,
        string licensePlate,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        entity.Value!.LicensePlate = licensePlate;

        return await UpdateAsync(entity.Value, cancellationToken);
    }
}
