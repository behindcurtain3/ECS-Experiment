using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoECS.Components;

namespace IsoECS.DataRenderers
{
    public class ComponentRenderer : DataRenderer<Component>
    {
        public new Component Data { get; set; }

        public ComponentRenderer(Component data)
            : base(data)
        {
            Data = data;
        }
    }
}
