using HarmonyLib;
using System.Reflection;
using Verse;

namespace VexedThings.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class HarmonyInitializer
    {
        static HarmonyInitializer()
        {
            var Harmony = new Harmony("vexedtrees.RimRobots");
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}