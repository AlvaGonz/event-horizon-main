using System;
using System.Linq;
using Constructor.Satellites;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using UnityEngine;

namespace Game.Exploration
{
    public class EnemyShipBuilder : IEnemyShipBuilder
    {
        private readonly ShipBuildParameters _parameters;
        private readonly IDatabase _database;
        private readonly IShipBuildGenerator _generator;

        // Backward compatibility constructor
        public EnemyShipBuilder(ItemId<ShipBuild> id, IDatabase database, int level, int seed, bool randomizeColor = false, bool allowSatellites = true)
            : this(
                ShipBuildParameters.Default()
                    .WithLevel(level)
                    .WithSeed(seed)
                    .WithSpecificShipId(id.Value)
                    .WithRandomizeColor(randomizeColor)
                    .WithAllowSatellites(allowSatellites),
                database,
                new ParameterizedShipBuildGenerator())
        {
        }

        // New constructor for injection
        public EnemyShipBuilder(ShipBuildParameters parameters, IDatabase database, IShipBuildGenerator generator = null)
        {
            _parameters = parameters;
            _database = database;
            _generator = generator ?? new ParameterizedShipBuildGenerator();
        }

        public Combat.Component.Ship.Ship Build(Combat.Factory.ShipFactory shipFactory, Combat.Factory.SpaceObjectFactory objectFactory, Vector2 position, float rotation)
        {
            var model = CreateShip();
            var spec = model.CreateBuilder().Build(_database.ShipSettings);
            var ship = shipFactory.CreateEnemyShip(spec, position, rotation, Maths.Distance.AiLevel(_parameters.Level));
            return ship;
        }

        private IShip CreateShip()
        {
            var random = new System.Random(_parameters.Seed);

            // Use generator to get the build
            var build = _generator.Generate(_parameters, _database);
            
            var ship = new EnemyShip(build, _database);

            var shipLevel = _database.GalaxySettings.EnemyLevel(_parameters.Level);
            shipLevel -= random.Next(shipLevel/3);
            ship.Experience = Maths.Experience.FromLevel(shipLevel);

            var satelliteClass = Maths.Distance.MaxShipClass(_parameters.Level);
            // Check implicit parameter about common ships and satellites
            if (_parameters.AllowSatellites && ship.Model.ShipType == ShipType.Common && satelliteClass != DifficultyClass.Default)
            {
                var satellites = _database.SatelliteBuildList.LimitClass(satelliteClass).SuitableFor(build.Ship);
                if (satellites.Any())
                {
                    if (random.Next(3) != 0)
                        ship.FirstSatellite = new CommonSatellite(satellites.RandomElement(random));
                    if (random.Next(3) != 0)
                        ship.SecondSatellite = new CommonSatellite(satellites.RandomElement(random));
                }
            }

            if (_parameters.RandomizeColor)
            {
                ship.ColorScheme.Type = ShipColorScheme.SchemeType.Hsv;
                ship.ColorScheme.Hue = random.NextFloat();
            }

            return ship;
        }
    }
}
