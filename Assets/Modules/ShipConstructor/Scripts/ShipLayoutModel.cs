using System.Collections.Generic;
using GameDatabase.DataModel;
using Constructor.Model;
using Constructor;

namespace Constructor.Model
{
    public class ShipLayoutModel
    {
        public ShipLayoutModel(ShipElementType type, ShipLayoutAdapter layout, object barrels, ComponentTracker tracker) { }
        public bool IsSuitableLocation(int x, int y, GameDatabase.DataModel.Component component) => true;
        public void InstallComponent(int x, int y, ComponentInfo component, ComponentSettings settings) { }
    }

    public class ComponentSettings
    {
        public ComponentSettings(string keyBinding, int behaviour, bool locked) { }
    }
    
    public enum ShipElementType
    {
        Ship,
        SatelliteL
    }
}
