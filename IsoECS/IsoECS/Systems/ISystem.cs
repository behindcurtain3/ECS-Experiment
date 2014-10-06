using System.Collections.Generic;
using IsoECS.Entities;

namespace IsoECS.Systems
{
    public interface ISystem
    {
        // Update the system
        void Update(List<Entity> entities, int dt);
    }
}
