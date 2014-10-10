﻿using System;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class BuildableComponent : Component
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ConstructSpriteSheetName { get; set; }
        public string ConstructSourceID { get; set; }

        public bool DragBuildEnabled { get; set; }
        public bool Destructable { get; set; }

        public BuildableComponent()
        {
            DragBuildEnabled = false;
            Destructable = true;
        }
    }
}