using System.Collections.Generic;
using IsoECS.Entities;

namespace IsoECS.Systems
{
    public interface ISystem
    {
        void Update(int dt);
        void Init();
        void Shutdown();
    }
}
