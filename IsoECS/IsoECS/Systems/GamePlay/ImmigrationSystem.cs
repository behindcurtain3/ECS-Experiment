using System.Collections.Generic;
using System.Linq;
using IsoECS.Components;
using IsoECS.Components.GamePlay;
using IsoECS.DataStructures;
using IsoECS.Entities;
using IsoECS.Util;

namespace IsoECS.Systems.GamePlay
{
    public class ImmigrationSystem : ISystem
    {
        private List<Entity> _spawners;
        private int _spawnCountdown;

        public void Update(int dt)
        {
            _spawnCountdown -= dt;
            if (_spawnCountdown <= 0)
            {
                ResetCountdown();

                // get the total number of citizens
                int vacanies = CountVacanies(EntityManager.Instance.Entities);
                int population = EntityManager.Instance.CityInformation.Population;
                int maxPopulation = MaxPopulation(EntityManager.Instance.Entities);

                // TODO: don't just fill vacancies, check to see if the city is "good" enough for immigrants
                if (population < maxPopulation && vacanies > 0)
                {
                    int spawnIndex = EntityManager.Random.Next(0, _spawners.Count);
                    SpawnerComponent spawner = _spawners[spawnIndex].Get<SpawnerComponent>();
                    PositionComponent position = _spawners[spawnIndex].Get<PositionComponent>();

                    string spawnID = spawner.Spawns[EntityManager.Random.Next(0, spawner.Spawns.Count)];

                    Entity spawned = Serialization.DeepCopy<Entity>(EntityLibrary.Instance.Get(spawnID));
                    spawned.ResetID();

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

                    EntityManager.Instance.AddEntity(spawned);
                }
            }
        }

        public void Init()
        {
            _spawners = EntityManager.Instance.Entities.FindAll(delegate(Entity e) { return e.HasComponent<SpawnerComponent>(); }).ToList();
            ResetCountdown();
        }

        public void Shutdown()
        {
            _spawners.Clear();
        }

        private void ResetCountdown()
        {
            _spawnCountdown = EntityManager.Random.Next(2, 6) * 100;
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
