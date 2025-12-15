using GameDatabase;
using GameDatabase.DataModel;

namespace Game.Exploration
{
    public interface IShipBuildGenerator
    {
        ShipBuild Generate(ShipBuildParameters parameters, IDatabase database);
    }
}
