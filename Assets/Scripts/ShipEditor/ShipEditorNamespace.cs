using System;
using System.Collections.Generic;
using CommonComponents.Signals;
using Constructor;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;

namespace ShipEditor
{
    public class CloseEditorSignal : SmartWeakSignal<CloseEditorSignal>
    {
    }
}

namespace ShipEditor.Context
{
    public interface IShipEditorContext
    {
         IShip Ship { get; }
         IInventoryProvider Inventory { get; }
         IShipPresetStorage ShipPresetStorage { get; }
         IComponentUpgradesProvider UpgradesProvider { get; }
    }

    public interface IInventoryProvider
    {
         // Add methods if needed based on errors
    }

    public interface IShipPresetStorage
    {
        // Add methods if needed
    }

    public interface IShipDataProvider
    {
    }
}
