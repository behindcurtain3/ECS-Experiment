using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoECS.Util
{
    public class StringHelper
    {
        public static string Ordinal(int num)
        {
            switch (num)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }
    }
}
