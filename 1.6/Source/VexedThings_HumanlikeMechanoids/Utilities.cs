using RimWorld;
using AlienRace;
using Verse;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.Diagnostics;

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
        public static Dictionary<Pawn, bool> cachedPawn = new Dictionary<Pawn, bool>();
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
            return pawn.def.HasModExtension<HumanlikeMechanoidsExtension>();
        }

        public static bool IsFleshylike(this Pawn __instance)
        {
            if (__instance == null)
            {
                Log.Error("Check for null.");
                return false;
            }
            var p = __instance.def as ThingDef_AlienRace;
            var isFleshValue = p?.alienRace?.compatibility?.IsFlesh;
            return (bool)isFleshValue;
        }

        public static bool IsPersonaOnlyDrug(this ThingDef drug)
        {
            VexedThingsItemExtension modExtension = drug.GetModExtension<VexedThingsItemExtension>();
            if (modExtension != null)
            {
                return modExtension.isIngestibleByPersonaeOnly;
            }
            return (drug.HasModExtension<VexedThingsItemExtension>() && drug.GetModExtension<VexedThingsItemExtension>().isIngestibleByPersonaeOnly);
        }

        public static bool IsPersonaOnlyHediff(this HediffDef hediff)
        {
            VexedThingsHediffExtension modExtension = hediff.GetModExtension<VexedThingsHediffExtension>();
            if (modExtension != null)
            {
                return modExtension.onlyAffectsPersonae;
            }
            return (hediff.HasModExtension<VexedThingsHediffExtension>() && hediff.GetModExtension<VexedThingsHediffExtension>().onlyAffectsPersonae);
        }

        public static Pawn RRGetCreator(this Pawn pawn)
        {
            Pawn_RelationsTracker relations = pawn.relations;
            if (relations == null)
            {
                return null;
            }
            return relations.GetFirstDirectRelationPawn(RR_DefOf.Creator, null);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHumanOfColony(this Pawn pawn)
        {
            if (pawn == null)
            {
                Log.Error("Null check failed -> IsHumanOfColony");
                return false;
            }
            if (pawn.IsHumanlikeMechanoid())
            {
                return false;
            }
            if (!Utilities.cachedPawn.TryGetValue(pawn, out bool result))
            {
                if (!pawn.IsColonist || pawn.IsSlave || pawn.IsPrisoner)
                {
                    return false;
                }
                result = Utilities.cachedPawn[pawn] = !pawn.IsHumanlikeMechanoid();
            }
            return result;
        }

        public static int GetHumansInFactionCount(Faction faction)
        {
            if (faction == null)
            {
                return 0;
            }
            int num = 0;
            using (List<Pawn>.Enumerator enumerator = PawnsFinder.AllMaps_SpawnedPawnsInFaction(faction).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsHumanOfColony())
                    {
                        num++;
                    }
                }
            }
            return num;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPersonaOfColony(this Pawn pawn)
        {
            if (pawn == null)
            {
                Log.Error("Null check failed -> IsPersonaOfColony");
                return false;
            }
            if (!pawn.IsHumanlikeMechanoid())
            {
                return false;
            }
            if (!Utilities.cachedPawn.TryGetValue(pawn, out bool result))
            {
                if (!pawn.IsColonist || pawn.IsSlave || pawn.IsPrisoner)
                {
                    return false;
                }
                result = Utilities.cachedPawn[pawn] = pawn.IsHumanlikeMechanoid();
            }
            return result;
        }

        public static int GetPersonaeInFactionCount(Faction faction)
        {
            if (faction == null)
            {
                return 0;
            }
            int num = 0;
            using (List<Pawn>.Enumerator enumerator = PawnsFinder.AllMaps_SpawnedPawnsInFaction(faction).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsPersonaOfColony())
                    {
                        num++;
                    }
                }
            }
            return num;
        }
    }
}