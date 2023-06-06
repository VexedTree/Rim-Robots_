using AlienRace;
using Verse;


namespace VexedThings
{
    public class Utilities
    {

        public static bool BioVsMechConflict(ThingDef one, ThingDef two)
        {
            var bothSynthetic = one == two;
            var bothHumanlike = (one?.race?.Humanlike ?? false) && (two?.race?.Humanlike ?? false);
            ThingDef_AlienRace pawn1 = one as ThingDef_AlienRace;
            ThingDef_AlienRace pawn2 = two as ThingDef_AlienRace;
            var isFleshValue = pawn1?.alienRace?.compatibility?.IsFlesh != pawn2?.alienRace?.compatibility?.IsFlesh;
            return !bothSynthetic && bothHumanlike && isFleshValue;
        }
    }

    [StaticConstructorOnStartup]
    public static class Methods
    {
        public static bool IsHumanlikeMechanoid(this Pawn pawn)
        {
            if (pawn == null)
            {
                Log.Error("Check for null.");
                return false;
            }
            if (!pawn.RaceProps.Humanlike)
            {
                return false;
            }
            return pawn.HasSyntheticPawnExtention();
        }

        public static bool HasSyntheticPawnExtention(this Pawn pawn)
        {
            if (pawn == null)
            {
                Log.Error("Check for null.");
                return false;
            }
            return pawn.def.HasModExtension<HumanlikeMechanoidsExtension>();
        }
    }
}