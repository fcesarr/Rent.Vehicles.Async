using FluentValidation;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.Validators;

public class VehicleValidator : Validator<Vehicle>, IVehicleValidator
{
    public VehicleValidator(IRepository<Vehicle> repository)
    {
        RuleFor(x => x.LicensePlate)
            .MustAsync(async (e, licensePlate, cancellationToken) =>
            {
                var entity = await repository.GetAsync(x => x.Id != e.Id &&
                                                            x.LicensePlate == licensePlate,
                    cancellationToken: cancellationToken);

                return entity is null;
            }).WithMessage("Veiculo com placa já cadastrada");

        RuleFor(x => x.IsRented)
            .MustAsync(async (e, isRented, cancellationToken) =>
            {
                var entity = await repository.GetAsync(x => x.Id == e.Id,
                    cancellationToken: cancellationToken);

                if (entity is null)
                {
                    return true;
                }

                if (entity.IsRented && isRented)
                {
                    return false;
                }

                return true;
            }).WithMessage("Veiculo esta alugado");
        
        RuleFor(x => x.RentCount)
            .MustAsync(async (e, rentCount, cancellationToken) => {

                var entity = await repository.GetAsync(x => x.Id == e.Id,
                    cancellationToken: cancellationToken);
                
                if(entity is null)
                    return true;

                if((entity.IsRented != e.IsRented || entity.LicensePlate != e.LicensePlate) &&
                    entity.RentCount <= rentCount)
                    return true;

                if(entity.RentCount > 0)
                    return false;

                return true;
            })
            .WithMessage("Veiculo possui alugueis cadastrados");
    }
}
