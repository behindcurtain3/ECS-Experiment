using System.Collections.Generic;
using IsoECS.Entities;

namespace IsoECS.Systems
{
    public interface ISystem
    {
        void Update(EntityManager em, int dt);
        void Init(EntityManager em);
        void Shutdown(EntityManager em);
    }
}
