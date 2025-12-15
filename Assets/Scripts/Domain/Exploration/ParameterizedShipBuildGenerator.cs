using System.Linq;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Query;
using UnityEngine;

namespace Game.Exploration
{
    public class ParameterizedShipBuildGenerator : IShipBuildGenerator
    {
        public ShipBuild Generate(ShipBuildParameters parameters, IDatabase database)
        {
            if (parameters.SpecificShipId.HasValue)
            {
                return database.GetShipBuild(new ItemId<ShipBuild>(parameters.SpecificShipId.Value));
            }

            // Using deterministic random based on seed
            var random = new System.Random(parameters.Seed);

            var query = ShipBuildQuery.EnemyShips(database);

            // Apply filters
            if (parameters.CustomFilter != null)
            {
                query = query.Where(build => parameters.CustomFilter(build));
            }

            query = query.TryApplyFilter(build =>
            {
                // Faction Filter
                if (parameters.FactionFilter != Faction.Empty && build.Ship.Faction != parameters.FactionFilter)
                    return false;

                // Difficulty Class Filter
                if (parameters.MinDifficultyClass.HasValue && build.DifficultyClass < parameters.MinDifficultyClass.Value)
                    return false;
                if (parameters.MaxDifficultyClass.HasValue && build.DifficultyClass > parameters.MaxDifficultyClass.Value)
                    return false;

                // Size Class Filter
                if (parameters.SizeClasses != null && !parameters.SizeClasses.Contains(build.Ship.SizeClass))
                    return false;

                // Rarity Filter
                if (parameters.Rarities != null && !parameters.Rarities.Contains(build.Ship.ShipRarity))
                    return false;

                return true;
            });
            
            var candidates = query.All.ToList();

            if (candidates.Count == 0)
            {
                Debug.LogWarning($"[ParameterizedShipBuildGenerator] No ships found for parameters: Level={parameters.Level}, Faction={parameters.FactionFilter}");
                // Fallback to random ship or default? 
                // For now, let's try to return something at least, or maybe the first one from unfiltered if it's too strict.
                // But better to return null or throw? The original code assumes a result usually.
                // Let's fallback to a simpler query if empty
                 return database.ShipBuildList.RandomElement(random);
            }

            return candidates.RandomElement(random);
        }
    }
}
