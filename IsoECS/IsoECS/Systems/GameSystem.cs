using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.GamePlay;

namespace IsoECS.Systems
{
    public class GameSystem : TecsDotNet.System
    {
        public new GameWorld World
        {
            get
            {
                return (GameWorld)base.World;
            }
            set
            {
                base.World = value;
            }
        }
    }
}
