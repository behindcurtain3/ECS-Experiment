using IsoECS.Components.GamePlay;
using IsoECS.Entities;

namespace IsoECS.Systems.GamePlay
{
    public class DateTimeSystem : ISystem
    {
        // an update rate of 50ms mean for each real life second 20 in game seconds pass
        private int _updateRate = 50;
        private int _updateCountdown;
        private Entity _dateEntity;

        public void Update(int dt)
        {
            _updateCountdown -= dt;

            if (_updateCountdown <= 0)
            {
                _updateCountdown += _updateRate;

                if (_dateEntity != null)
                {
                    GameDateComponent date = _dateEntity.Get<GameDateComponent>();
                    date.Time++;
                }
            }
        }

        public void Init()
        {
            _updateCountdown = _updateRate;
            _dateEntity = EntityManager.Instance.Entities.Find(delegate(Entity e) { return e.HasComponent<GameDateComponent>(); });
        }

        public void Shutdown()
        {
        }
    }
}
