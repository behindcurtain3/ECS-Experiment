using System;

namespace IsoECS.Components.GamePlay
{
    [Serializable]
    public class BuildableComponent : Component
    {
        public string Category;
        public string Name;
        public string Description;
        public string SpriteSheetName;
        public string SourceID;

        public string ConstructSpriteSheetName;
        public string ConstructSourceID;

        public bool DragBuildEnabled;
    }
}
