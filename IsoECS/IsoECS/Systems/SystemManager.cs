using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoECS.Systems
{
    public sealed class SystemManager
    {
        private static readonly SystemManager _instance = new SystemManager();

        public static SystemManager Instance
        {
            get { return _instance; }
        }

        public List<IRenderSystem> CustomRenderers { get; set; }
        public IRenderSystem Renderer { get; set; }

        private SystemManager()
        {
            CustomRenderers = new List<IRenderSystem>();
        }
    }
}
