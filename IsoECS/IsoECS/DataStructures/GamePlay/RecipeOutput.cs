using System;

namespace IsoECS.DataStructures.GamePlay
{
    [Serializable]
    public class RecipeOutput
    {
        // the name of the item that is output
        public string Item { get; set; }

        // the amount that is produced each time a worker completes a "block" of work
        // block: think of it as a abstract way to track the production of output. A block could
        // represent a worker gathering the needed materials, performing some work on them and then
        // producing a good from the work.
        public double AmountProduced { get; set; }

        // how hard each "block" of work is to accomplish
        public double EffortRequired { get; set; }
    }
}
