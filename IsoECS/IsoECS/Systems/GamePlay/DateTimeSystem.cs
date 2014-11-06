using IsoECS.Components.GamePlay;
using TecsDotNet;

namespace IsoECS.Systems.GamePlay
{
    public class DateTimeSystem : GameSystem
    {
        // an update rate of 50ms mean for each real life second 20 in game seconds pass
        private double _updateRate = 0.05;
        private double _updateCountdown;
        private Entity _dateEntity;

        public override void Update(double dt)
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

        public override void Init()
        {
            base.Init();

            _updateCountdown = _updateRate;
            _dateEntity = World.Entities.Find(delegate(Entity e) { return e.HasComponent<GameDateComponent>(); });
        }
    }
}
