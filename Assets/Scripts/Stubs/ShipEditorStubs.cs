using System;
using System.Collections.Generic;
using ShipEditor.Context;
using GameDatabase.DataModel;

namespace ShipEditor.Context
{
    public interface IComponentUpgradesProvider { }
}

namespace ShipEditor.Context
{
    // Needs to interact with Constructor types which might be in Scripts or ShipConstructor module
    // This file is in Assets/Scripts, so it should see everything.

    public class DatabaseEditorContext : IShipEditorContext 
    {
        public DatabaseEditorContext(Constructor.Ships.IShip ship) { }
        
        public Constructor.Ships.IShip Ship => null;
        public IInventoryProvider Inventory => null;
        public IShipDataProvider ShipDataProvider => null;
        public bool IsShipNameEditable => false;
        public IShipPresetStorage ShipPresetStorage => null;
        public IComponentUpgradesProvider UpgradesProvider => null;
        public bool CanBeUnlocked(Component component) => true;
    }
    
    public class EmptyDataProvider : IShipDataProvider 
    {
         // Stubs
         public float GetStat(string id) => 0f;
    }
}
