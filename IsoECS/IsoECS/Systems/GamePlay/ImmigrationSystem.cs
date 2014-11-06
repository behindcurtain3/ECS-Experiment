using System.Collections.Generic;
using System.Linq;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using TecsDotNet;

namespace IsoECS.Systems.GamePlay
{
    public class ImmigrationSystem : GameSystem
    {
        private List<Entity> _spawners;
        private double _spawnCountdown;

        public override void Update(double dt)
        {
            _spawnCountdown -= dt;
            if (_spawnCountdown <= 0)
            {
                ResetCountdown();

                // get the total number of citizens
                int vacanies = CountVacanies(World.Entities);
                int population = World.City.Population;
                int maxPopulation = MaxPopulation(World.Entities);

                // TODO: don't just fill vacancies, check to see if the city is "good" enough for immigrants
                if (population < maxPopulation && vacanies > 0)
                {
                    int spawnIndex = World.Random.Next(0, _spawners.Count);
                    SpawnerComponent spawner = _spawners[spawnIndex].Get<SpawnerComponent>();
                    PositionComponent position = _spawners[spawnIndex].Get<PositionComponent>();

                    string spawnID = spawner.Spawns[World.Random.Next(0, spawner.Spawns.Count)];

                    Entity spawned = (Entity)World.Prototypes[spawnID];

                    if (spawned.HasComponent<PositionComponent>())
                    {
                        PositionComponent aPosition = spawned.Get<PositionComponent>();
                        aPosition.X = position.X;
                        aPosition.Y = position.Y;
                        aPosition.Index = position.Index;
                    }
                    else
                    {
                        PositionComponent bPosition = new PositionComponent(position.Position);
                        bPosition.Index = position.Index;
                        spawned.AddComponent(bPosition);
                    }

                    World.Entities.Add(spawned);
                }
            }
        }

        public override void Init()
        {
            base.Init();
            _spawners = World.Entities.FindAll(delegate(Entity e) { return e.HasComponent<SpawnerComponent>(); }).ToList();
            ResetCountdown();
        }

        public override void Shutdown()
        {
            _spawners.Clear();
        }

        private void ResetCountdown()
        {
            _spawnCountdown = World.Random.NextDouble();
        }

        private int CountVacanies(List<Entity> entities)
        {
            int count = 0;
            
            foreach(Entity e in entities)
            {
                if (!e.HasComponent<HousingComponent>())
                    continue;

                HousingComponent house = e.Get<HousingComponent>();

                if (house.SpaceAvailable > 0)
                    count += house.SpaceAvailable;
            }

            return count;
        }

        private int MaxPopulation(List<Entity> entities)
        {
            int count = 0;

            foreach(Entity e in entities)
            {
                if (!e.HasComponent<HousingComponent>())
                    continue;

                count += e.Get<HousingComponent>().MaxOccupants;
            }

            return count;
        }
    }
}
