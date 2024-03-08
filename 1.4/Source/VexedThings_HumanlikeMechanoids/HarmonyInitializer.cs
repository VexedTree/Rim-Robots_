using HarmonyLib;
using System.Reflection;
using Verse;

namespace VexedThings.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var Harmony = new Harmony("vexedtrees.RimRobots");
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}