using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.AI;

namespace VexedThings.HarmonyPatches
{
    // MISC / PATCHES

    // Toggles if pawns with the "canBeStunnedByEMP" extension can be stunned by EMP attacks.
    [HarmonyPatch(typeof(StunHandler), "get_AffectedByEMP")]
    public class AffectedByEMP_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(StunHandler __instance, ref bool __result, Thing ___parent)
        {
            if (___parent is Pawn pawn)
            {
                if (pawn != null && pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().canBeStunnedByEMP)
                {
                    __result = true;
                }
            }
        }
    }

    // Toggles if pawns with the "pawnCanPerceiveBeauty" extension will change opinion depending on the beauty of a pawn.
    [HarmonyPatch(typeof(ThoughtWorker_Ugly), "CurrentSocialStateInternal")]
    public class ThoughtWorker_Ugly_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref ThoughtState __result)
        {
            if (pawn != null && pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().pawnCannotPerceiveBeauty)
            {
                __result = false;
            }
        }
    }
    [HarmonyPatch(typeof(ThoughtWorker_Disfigured), "CurrentSocialStateInternal")]
    public class ThoughtWorker_Disfigured_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref ThoughtState __result)
        {
            if (pawn != null && pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().pawnCannotPerceiveBeauty)
            {
                __result = false;
            }
        }
    }

    // Toggles disease incidents targeting pawns with the "canFallIll" extension.
    [HarmonyPatch(typeof(IncidentWorker_DiseaseHuman), "PotentialVictimCandidates")]
    public class PotentialVictimCandidates_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Listener(IIncidentTarget target, ref IEnumerable<Pawn> __result)
        {
            if (__result == null)
            {
                return;
            }
            __result = from pawn in __result where !pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().canFallIll select pawn;
        }
    }

    // Toggles cosemetic breath vapors in cold temperatures on pawns with the "disableBreathVapors" extension.
    [HarmonyPatch(typeof(PawnBreathMoteMaker), "ProcessPostTickVisuals")]
    internal class ProcessPostTickVisuals_HarmonyPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn ___pawn)
        {
            HumanlikeMechanoidsExtension pawn = ___pawn.FetchExtension();
            return pawn == null || !pawn.disableBreathVapors;
        }
    }

    // Translates the given need bars on pawns tagged as "IsHumanlikeMechanoid" with the given custom keys.
    internal class Need_Patchs
    {
        [HarmonyPatch(typeof(Need), "get_LabelCap")]
        public class LabelCapFood_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(ref string __result, Pawn ___pawn, Need __instance)
            {
                if (__instance.def.defName == "Food" && Methods.IsHumanlikeMechanoid(___pawn))
                {
                    __result = "RR.EnergyNeed".Translate();
                }
            }
        }
        [HarmonyPatch(typeof(Need_Food), "GetTipString")]
        public class GetTipStringFood_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(ref string __result, Pawn ___pawn, Need __instance)
            {
                if (__instance.def.defName == "Food" && Methods.IsHumanlikeMechanoid(___pawn))
                {
                    __result = string.Format("{0}: {1} ({2:0.##}/{3:0.##})\n{4}", new object[]
                    {
                        "RR.EnergyNeed".Translate(),
                        __instance.CurLevelPercentage.ToStringPercent(),
                        __instance.CurLevel,
                        __instance.MaxLevel,
                        "RR.EnergyNeedDescription".Translate()
                    });
                    return;
                }
            }
        }

        [HarmonyPatch(typeof(Need), "get_LabelCap")]
        public class LabelCapSleep_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(ref string __result, Pawn ___pawn, Need __instance)
            {
                if (__instance.def.defName == "Rest" && Methods.IsHumanlikeMechanoid(___pawn))
                {
                    __result = "RR.MemoryNeed".Translate();
                }
            }
        }
        [HarmonyPatch(typeof(Need_Food), "GetTipString")]
        public class GetTipStringSleep_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(ref string __result, Pawn ___pawn, Need __instance)
            {
                if (__instance.def.defName == "Rest" && Methods.IsHumanlikeMechanoid(___pawn))
                {
                    __result = string.Format("{0}: {1} ({2:0.##}/{3:0.##})\n{4}", new object[]
                    {
                        "RR.MemoryNeed".Translate(),
                        __instance.CurLevelPercentage.ToStringPercent(),
                        __instance.CurLevel,
                        __instance.MaxLevel,
                        "RR.MemoryNeedDescription".Translate()
                    });
                    return;
                }
            }
        }
    }

    internal class Corpse_Patches
    {
        // Toggles if corpses of pawns with the "corpseIsImperishable" extension will rot.
        [HarmonyPatch(typeof(CompRottable), "RotProgress", MethodType.Setter)]
        public static class RotProgress_HarmonyPatch
        {
            public static bool Prefix(CompRottable __instance)
            {
                return !(__instance.parent is Pawn pawn) || !pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsImperishable;
            }
        }
        [HarmonyPatch(typeof(CompRottable), "CompInspectStringExtra")]
        public static class CompInspectStringExtra_HarmonyPatch
        {
            public static bool Prefix(CompRottable __instance, ref string __result)
            {
                if (__instance.parent is Corpse corpse && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsImperishable)
                {
                    __result = null;
                    return false;
                }
                return true;
            }
        }

        // Toggles if corpses of pawns with the "corpseIsEdible" extension will be edible.
        [HarmonyPatch(typeof(RaceProperties), "CanEverEat", new Type[]
        {
        typeof(Thing)
        })]
        public static class CanEverEat_HarmonyPatch
        {
            public static void Postfix(ref bool __result, Thing t)
            {
                if (t is Corpse corpse && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsInedible)
                {
                    __result = false;
                }
            }
        }

        // Consistency to block certain things that use/target fleshy corpses. Uses the "corpseIsEdible" extension to obtain a value.
        [HarmonyPatch(typeof(CompTargetable), "BaseTargetValidator")]
        public static class BaseTargetValidator_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(CompTargetable __instance, Thing t, ref bool __result)
            {
                if (__result && __instance.Props.fleshCorpsesOnly)
                {
                    if (t is Corpse corpse && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsInedible)
                    {
                        __result = false;
                    }
                }
            }
        }

        // Toggles if corpses of pawns with the "corpseIsDisturbing" extension will disturb pawns.
        [HarmonyPatch(typeof(Corpse), "GiveObservedThought")]
        public class GiveObservedThought_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(Corpse __instance, ref Thought_Memory __result)
            {
                if (__instance is Corpse corpse && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsDisturbing)
                {
                    __result = null;
                }
            }
        }
        [HarmonyPatch(typeof(Corpse), "GiveObservedHistoryEvent")]
        public class GiveObservedHistoryEvent_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(Corpse __instance, Pawn observer, ref HistoryEventDef __result)
            {
                if (__instance is Corpse corpse && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsDisturbing)
                {
                    __result = null;
                }
            }
        }

        // Controls if pawns with the "corpseIsImperishable" extension can have their skulls harvested.
        [HarmonyPatch(typeof(Designator_ExtractSkull), "CanDesignateThing")]
        public static class Designator_ExtractSkull_CanDesignateThing_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(ref AcceptanceReport __result, Thing t)
            {
                if (__result)
                {
                    if (t is Corpse corpse && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsImperishable)
                    {
                        __result = false;
                    }
                }
            }
        }
    }

    // Controls if pawns with the "IsHumanlikeMechanoid" tag can be fed upon by "vampires", coagulated by vampires, or drained for their "blood".
    [HarmonyPatch(typeof(JobGiver_GetHemogen), "CanFeedOnPrisoner")]
    public static class CanFeedOnPrisoner_HarmonyPatch
    {
        public static void Postfix(Pawn bloodfeeder, Pawn prisoner, ref AcceptanceReport __result)
        {
            if (prisoner != null && prisoner.IsHumanlikeMechanoid())
            {
                __result = "RR.SynthVsVampire".Translate(prisoner.Named("PAWN"));
            }
        }
    }
    [HarmonyPatch]
    public static class CompAbilityEffect_BloodfeederBite_HarmonyPatch
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.DeclaredMethod(typeof(CompAbilityEffect_BloodfeederBite), "Valid", null, null);
            yield break;
        }
        public static void Postfix(ref bool __result, LocalTargetInfo target, bool throwMessages = false)
        {
            if (__result)
            {
                if (target.Thing is Pawn pawn && pawn.IsHumanlikeMechanoid())
                {
                    if (throwMessages)
                    {
                        Messages.Message("RR.SynthVsVampire".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                    }
                    __result = false;
                    return;
                }
            }
        }
        [HarmonyPatch]
        public static class CompAbilityEffect_Coagulate_HarmonyPatch
        {
            [HarmonyTargetMethods]
            public static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.DeclaredMethod(typeof(CompAbilityEffect_Coagulate), "Valid", null, null);
                yield break;
            }
            public static void Postfix(ref bool __result, LocalTargetInfo target, bool throwMessages = false)
            {
                if (__result)
                {
                    if (target.Thing is Pawn pawn && pawn.IsHumanlikeMechanoid())
                    {
                        if (throwMessages)
                        {
                            Messages.Message("RR.SynthVsVampire".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                        }
                        __result = false;
                        return;
                    }
                }
            }
        }

        // Controls if pawns with the "IsHumanlikeMechanoid" tag can be implanted with xenogerms.
        [HarmonyPatch]
        public static class CompAbilityEffect_ReimplantXenogerm_HarmonyPatch
        {
            [HarmonyTargetMethods]
            public static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.DeclaredMethod(typeof(CompAbilityEffect_ReimplantXenogerm), "Valid", null, null);
                yield break;
            }
            public static void Postfix(ref bool __result, LocalTargetInfo target, bool throwMessages = false)
            {
                if (__result)
                {
                    if (target.Thing is Pawn pawn && pawn.IsHumanlikeMechanoid())
                    {
                        if (throwMessages)
                        {
                            Messages.Message("RR.SynthCannotBeImplanted".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                        }
                        __result = false;
                        return;
                    }
                }
            }
        }
    }

    // Controls if pawns with the "incapableOfLove" extension will fall in love.
    [HarmonyPatch]
    public static class CompAbilityEffect_WordOfLove_HarmonyPatch
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.DeclaredMethod(typeof(CompAbilityEffect_WordOfLove), "Valid", null, null);
            yield break;
        }
        public static void Postfix(ref bool __result, LocalTargetInfo target, bool throwMessages = false)
        {
            if (__result)
            {
                if (target.Thing is Pawn pawn && pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().incapableOfLove)
                {
                    if (throwMessages)
                    {
                        Messages.Message("RR.SynthIsIncapableOfLove".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                    }
                    __result = false;
                    return;
                }
            }
        }
    }
    [HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), "RandomSelectionWeight")]
    public static class IRandomSelectionWeight_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
        {
            if (initiator.def.HasModExtension<HumanlikeMechanoidsExtension>() && initiator.def.GetModExtension<HumanlikeMechanoidsExtension>().incapableOfLove)
            {
                __result = 0f;
                return;
            }
            if (recipient.def.HasModExtension<HumanlikeMechanoidsExtension>() && recipient.def.GetModExtension<HumanlikeMechanoidsExtension>().incapableOfLove)
            {
                __result = 0f;
            }
        }
    }
    [HarmonyPatch(typeof(RelationsUtility), "RomanceEligible")]
    public static class RomanceEligible_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref AcceptanceReport __result)
        {
            if (pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().incapableOfLove)
            {
                __result = "RR.SynthIsIncapableOfLove".Translate(pawn.Named("PAWN"));
            }
        }
    }
    [HarmonyPatch(typeof(RelationsUtility), "TryDevelopBondRelation")]
    public static class TryDevelopBondRelation_HarmonyPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn humanlike, Pawn animal, ref float baseChance)
        {
            {
                HumanlikeMechanoidsExtension pawn = humanlike.FetchExtension();
                return pawn == null || !pawn.incapableOfLove;
            }
        }
    }

    // Controls if pawns with the "IsHumanlikeMechanoid" tag will demand age reversal.
    [HarmonyPatch(typeof(ThoughtWorker_AgeReversalDemanded), "ShouldHaveThought")]
    public class ShouldHaveThought_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Listener(Pawn p, ref ThoughtState __result)
        {
            if (!__result.Active)
            {
                return;
            }
            if (Methods.IsHumanlikeMechanoid(p))
            {
                __result = ThoughtState.Inactive;
            }
        }
    }

    // Disables certain buildings from accepting given pawn.
    [HarmonyPatch(typeof(Building_GrowthVat), "CanAcceptPawn")]
    public static class GrowthVat_CanAcceptPawn_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref AcceptanceReport __result)
        {
            if (pawn.IsHumanlikeMechanoid())
            {
                __result = "RR.SynthCannotUtilize".Translate(pawn.Named("PAWN"));
            }
        }
    }
    [HarmonyPatch(typeof(Building_GeneExtractor), "CanAcceptPawn")]
    public static class GeneExtractor_CanAcceptPawn_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref AcceptanceReport __result)
        {
            if (pawn.IsHumanlikeMechanoid())
            {
                __result = "RR.SynthNotGenetic".Translate();
            }
        }
    }
    [HarmonyPatch(typeof(Building_SubcoreScanner), "CanAcceptPawn")]
    public static class SubcoreScanner_CanAcceptPawn_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn selPawn, ref AcceptanceReport __result)
        {
            if (selPawn.IsHumanlikeMechanoid())
            {
                __result = "RR.SynthCannotBeScanned".Translate();
            }
        }
    }
    [HarmonyPatch(typeof(CompBiosculpterPod), "CannotUseNowPawnReason")]
    public static class BiosculpterPod_PawnCanUseNow_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref string __result, Pawn p)
        {
            if (p.IsHumanlikeMechanoid())
            {
                __result = "RR.SynthCannotUtilize".Translate();
            }
        }
    }

    // LISTERS / PATCHERS

    // Disables listed jobs from being performed by given pawn.
    [HarmonyPatch(typeof(Pawn_JobTracker), "StartJob")]
    internal class StartJob_HarmonyPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Job newJob, Pawn ___pawn)
        {
            HumanlikeMechanoidsExtension pawn = ___pawn.FetchExtension();
            List<JobDef> list;
            if (pawn == null)
            {
                list = null;
            }
            else
            {
                HumanlikeMechanoidsExtension patch = pawn;
                list = (patch?.disabledJobs);
            }
            List<JobDef> list2 = list;
            return list2 == null || !list2.Contains(newJob.def);
        }
    }

    // Disables certain hediffs from being applied to given pawn.
    [HarmonyPatch(typeof(Pawn_HealthTracker), "AddHediff", new Type[]
    {
        typeof(Hediff),
        typeof(BodyPartRecord),
        typeof(DamageInfo?),
        typeof(DamageWorker.DamageResult)
    })]
    internal class AddHediff_HarmonyPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn_HealthTracker __instance, Pawn ___pawn, ref Hediff hediff)
        {
            if (hediff is Hediff_MissingPart && hediff.Part != null && !__instance.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Contains(hediff.Part))
            {
                return false;
            }
            HumanlikeMechanoidsExtension pawn = ___pawn.FetchExtension();
            List<HediffDef> list;
            if (pawn == null)
            {
                list = null;
            }
            else
            {
                HumanlikeMechanoidsExtension patch = pawn;
                list = (patch?.disabledHediffs);
            }
            List<HediffDef> list2 = list;
            return list2 == null || !list2.Contains(hediff.def);
        }
    }
}