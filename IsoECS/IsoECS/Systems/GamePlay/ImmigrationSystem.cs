using System.Collections.Generic;
using System.Linq;
using IsoECS.Components.GamePlay;
using IsoECS.Entities;
using IsoECS.DataStructures;
using IsoECS.Components;
using IsoECS.Util;

namespace IsoECS.Systems.GamePlay
{
    public class ImmigrationSystem : ISystem
    {
        private List<Entity> _spawners;
        private int _spawnCountdown;

        public void Update(EntityManager em, int dt)
        {
            _spawnCountdown -= dt;
            if (_spawnCountdown <= 0)
            {
                ResetCountdown();

                int spawnIndex = Game1.Random.Next(0, _spawners.Count);
                SpawnerComponent spawner = _spawners[spawnIndex].Get<SpawnerComponent>();
                PositionComponent position = _spawners[spawnIndex].Get<PositionComponent>();

                string spawnID = spawner.Spawns[Game1.Random.Next(0, spawner.Spawns.Count)];

                Entity spawned = EntityLibrary.Instance.Get(spawnID).DeepCopy();

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

                em.AddEntity(spawned);
            }
        }

        public void Init(EntityManager em)
        {
            _spawners = em.Entities.FindAll(delegate(Entity e) { return e.HasComponent<SpawnerComponent>(); }).ToList();
            ResetCountdown();
        }

        public void Shutdown(EntityManager em)
        {
            _spawners.Clear();
        }

        private void ResetCountdown()
        {
            _spawnCountdown = Game1.Random.Next(5, 30) * 1000;
        }
    }
}
