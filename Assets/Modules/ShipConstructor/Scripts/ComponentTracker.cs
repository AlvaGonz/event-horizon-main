using GameDatabase.DataModel;
using Constructor.Ships;

namespace Constructor
{
    public class ComponentTracker
    {
        public ComponentTracker(IShip ship) { }
        public bool IsCompatible(GameDatabase.DataModel.Component component) => true;
    }
}
