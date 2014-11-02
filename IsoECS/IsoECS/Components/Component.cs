using System;
using IsoECS.Entities;

namespace IsoECS.Components
{
    [Serializable]
    public abstract class Component
    {
        public Entity BelongsTo { get; set; }
    }
}
