using System;
using System.Collections.Generic;
using IsoECS.Entities;
using IsoECS.Components.GamePlay;

namespace IsoECS.Systems.GamePlay
{
    public class DateTimeSystem : ISystem
    {
        // an update rate of 50ms mean for each real life second 20 in game seconds pass
        private int _updateRate = 50;
        private int _updateCountdown;

        public void Update(List<Entity> entities, int dt)
        {
            _updateCountdown -= dt;

            if (_updateCountdown <= 0)
            {
                _updateCountdown += _updateRate;

                Entity dateEntity = entities.Find(delegate(Entity e) { return e.HasComponent<GameDateComponent>(); });

                if (dateEntity != null)
                {
                    GameDateComponent date = dateEntity.Get<GameDateComponent>();
                    date.Time++;
                }
            }
        }

        public void Init(List<Entity> entities)
        {
            _updateCountdown = _updateRate;
        }

        public void Shutdown(List<Entity> entities)
        {
            throw new NotImplementedException();
        }
    }
}
