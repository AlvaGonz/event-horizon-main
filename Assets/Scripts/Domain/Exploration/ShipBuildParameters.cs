using System;
using System.Collections.Generic;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;

namespace Game.Exploration
{
    public class ShipBuildParameters
    {
        public int Level { get; private set; }
        public int Seed { get; private set; }
        public bool RandomizeColor { get; private set; }
        public bool AllowSatellites { get; private set; }
        public int? SpecificShipId { get; private set; }
        
        // Filters
        public Faction FactionFilter { get; private set; } = Faction.Empty;
        public DifficultyClass? MinDifficultyClass { get; private set; }
        public DifficultyClass? MaxDifficultyClass { get; private set; }
        public HashSet<SizeClass> SizeClasses { get; private set; }
        public HashSet<ShipRarity> Rarities { get; private set; }
        public Predicate<ShipBuild> CustomFilter { get; private set; }

        private ShipBuildParameters() { }

        public static ShipBuildParameters Default()
        {
            return new ShipBuildParameters
            {
                Level = 1,
                Seed = Environment.TickCount,
                AllowSatellites = true,
                RandomizeColor = false
            };
        }

        public ShipBuildParameters WithLevel(int level)
        {
            Level = level;
            return this;
        }

        public ShipBuildParameters WithSeed(int seed)
        {
            Seed = seed;
            return this;
        }

        public ShipBuildParameters WithRandomizeColor(bool randomize)
        {
            RandomizeColor = randomize;
            return this;
        }

        public ShipBuildParameters WithAllowSatellites(bool allow)
        {
            AllowSatellites = allow;
            return this;
        }

        public ShipBuildParameters WithSpecificShipId(int? id)
        {
            SpecificShipId = id;
            return this;
        }

        public ShipBuildParameters WithFactionFilter(Faction faction)
        {
            FactionFilter = faction;
            return this;
        }

        public ShipBuildParameters WithDifficultyClassRange(DifficultyClass min, DifficultyClass max)
        {
            MinDifficultyClass = min;
            MaxDifficultyClass = max;
            return this;
        }
        
        public ShipBuildParameters WithMinDifficultyClass(DifficultyClass min)
        {
            MinDifficultyClass = min;
            return this;
        }

        public ShipBuildParameters WithMaxDifficultyClass(DifficultyClass max)
        {
            MaxDifficultyClass = max;
            return this;
        }

        public ShipBuildParameters WithSizeClasses(params SizeClass[] sizes)
        {
            if (sizes != null && sizes.Length > 0)
            {
                if (SizeClasses == null) SizeClasses = new HashSet<SizeClass>();
                else SizeClasses.Clear();
                
                foreach (var s in sizes) SizeClasses.Add(s);
            }
            else
            {
                SizeClasses = null;
            }
            return this;
        }

        public ShipBuildParameters WithRarities(params ShipRarity[] rarities)
        {
             if (rarities != null && rarities.Length > 0)
            {
                if (Rarities == null) Rarities = new HashSet<ShipRarity>();
                else Rarities.Clear();
                
                foreach (var r in rarities) Rarities.Add(r);
            }
            else
            {
                Rarities = null;
            }
            return this;
        }

        public ShipBuildParameters WithCustomFilter(Predicate<ShipBuild> filter)
        {
            CustomFilter = filter;
            return this;
        }
    }
}
