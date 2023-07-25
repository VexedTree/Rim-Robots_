using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VexedThings
{
    [DefOf]
    public static class RR_DefOf
    {
        static RR_DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RR_DefOf));
        }
        public static ThingDef FuelNode_SynthDiet;
    }
}
