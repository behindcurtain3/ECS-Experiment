using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoECS.GamePlay
{
    public class Recipe
    {
        public List<Item> Input { get; private set; }
        public Item Output { get; set; }

        public Recipe()
        {
            Input = new List<Item>();
        }
    }
}
