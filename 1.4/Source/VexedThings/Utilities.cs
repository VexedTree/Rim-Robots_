using RimWorld;
using AlienRace;
using Verse;
using System.Linq;
using UnityEngine;

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

        public static void EmitMote(ThingDef moteDef, Pawn pawn)
        {
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef, null);
            moteThrown.Scale = 0.6f;
            moteThrown.rotationRate = Rand.Range(-1, 1);
            moteThrown.exactPosition = pawn.Position.ToVector3();
            moteThrown.exactPosition += new Vector3(0.85f, 0f, 0.85f);
            moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value) * 0.1f;
            moteThrown.SetVelocity(Rand.Range(30f, 60f), Rand.Range(0.35f, 0.55f));
            GenSpawn.Spawn(moteThrown, pawn.Position, pawn.Map, WipeMode.Vanish);
        }
    }

    [StaticConstructorOnStartup]
    public static class Methods
    {

        public static bool IsMechanoidlikeThingdef(ThingDef thingDef)
        {
            return thingDef.HasModExtension<HumanlikeMechanoidsExtension>();
        }

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
            return pawn.def.HasModExtension<HumanlikeMechanoidsExtension>();
        }

        public static bool ShouldBeRepairedNowByPlayer(Pawn pawn)
        {
            return pawn.playerSettings != null && HealthAIUtility.ShouldEverReceiveMedicalCareFromPlayer(pawn) && pawn.health.HasHediffsNeedingTendByPlayer(false);
        }

        public static bool HediffControlTag(HediffDef hediffDef)
        {
            HumanlikeMechanoidsExtension modExtension = hediffDef.GetModExtension<HumanlikeMechanoidsExtension>();
            if (modExtension != null)
            {
                return modExtension.areSyntheticPawnsVictims;
            }
            return (hediffDef.tags == null || hediffDef.CompProps<HediffCompProperties_Immunizable>() == null && !hediffDef.makesSickThought && !hediffDef.chronic && !typeof(Hediff_Addiction).IsAssignableFrom(hediffDef.hediffClass) && !DefDatabase<ChemicalDef>.AllDefs.Any((ChemicalDef x) => x.toleranceHediff == hediffDef) && !typeof(Hediff_High).IsAssignableFrom(hediffDef.hediffClass) && !typeof(Hediff_Hangover).IsAssignableFrom(hediffDef.hediffClass));
        }

        public static bool IsPersonaDrug(this ThingDef drug)
        {
            VexedThingsItemExtension modExtension = drug.GetModExtension<VexedThingsItemExtension>();
            if (modExtension != null)
            {
                return modExtension.isIngestibleByPersonaeOnly;
            }
            return (drug.HasModExtension<VexedThingsItemExtension>() && drug.GetModExtension<VexedThingsItemExtension>().isIngestibleByPersonaeOnly);
        }
    }
}