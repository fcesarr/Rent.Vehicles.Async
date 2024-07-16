using System.ComponentModel.DataAnnotations.Schema;

using Rent.Vehicles.Entities.Types;

namespace Rent.Vehicles.Entities;

[Table("vehicles", Schema = "vehicles")]
public class Vehicle : Entity
{
    public int Year
    {
        get;
        init;
    }

    public required string Model
    {
        get;
        init;
    }

    public required string LicensePlate
    {
        get;
        set;
    }

    public VehicleType Type
    {
        get;
        init;
    }

    public bool IsRented
    {
        get;
        set;
    } = false;

    public int RentCount { get; set; }
}
