using System.Collections.Generic;
using IsoECS.Entities;

namespace IsoECS.Systems
{
    public interface ISystem
    {
        void Update(List<Entity> entities, int dt);
        void Init(List<Entity> entities);
        void Shutdown(List<Entity> entities);
    }
}
