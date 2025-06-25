using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VexedThings.HarmonyPatches
{
    // MISC / PATCHES
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        private static readonly Type patchType = typeof(HarmonyPatches);
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("vexedtrees.RimRobots");

            harmony.Patch(AccessTools.PropertyGetter(typeof(Pawn), "HarmedByVacuum"),
            postfix: new HarmonyMethod(patchType, nameof(HarmedByVacuumPostfix)));
            harmony.Patch(AccessTools.PropertyGetter(typeof(Pawn), "ConcernedByVacuum"),
            postfix: new HarmonyMethod(patchType, nameof(ConcernedByVacuumPostfix)));
        }
        // Toggles if pawns with the "concernedWithVacuum" & "harmedByVacuum" extension will be affected by space exposure.
        public static void HarmedByVacuumPostfix(Pawn __instance, ref bool __result)
        {
            if (__instance.def.HasModExtension<HumanlikeMechanoidsExtension>() && __instance.def.GetModExtension<HumanlikeMechanoidsExtension>().harmedByVacuum)
            {
                __result = true;
            }
        }
        public static void ConcernedByVacuumPostfix(Pawn __instance, ref bool __result)
        {
            if (__instance.def.HasModExtension<HumanlikeMechanoidsExtension>() && __instance.def.GetModExtension<HumanlikeMechanoidsExtension>().concernedWithVacuum)
            {
                __result = true;
            }
        }
    }

    // Toggles if pawns with the "canBeStunnedByEMP" extension can be stunned by EMP attacks.
    [HarmonyPatch(typeof(StunHandler), "CanBeStunnedByDamage")]
    public class AffectedByEMP_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(StunHandler __instance, ref bool __result, Thing ___parent, DamageDef def)
        {
            if (___parent is Pawn pawn)
            {
                if (def == DamageDefOf.EMP && pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().canBeStunnedByEMP)
                {
                    __result = true;
                }
                if (ModsConfig.BiotechActive && def == DamageDefOf.MechBandShockwave && pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().canBeStunnedByEMP)
                {
                    __result = true;
                }
                //This isn't related to the "canBeStunnedByEMP" extension, this is just to reduce clutter.
                if (ModsConfig.AnomalyActive && def == DamageDefOf.NerveStun && pawn.IsHumanlikeMechanoid())
                {
                    __result = false;
                }
            }
        }
    }
    [HarmonyPatch(typeof(StunHandler), "CanAdaptToDamage")]
    public class CanAdaptToDamage_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(StunHandler __instance, ref bool __result, Thing ___parent, DamageDef def)
        {
            if (___parent is Pawn pawn)
            {
                if (def == DamageDefOf.EMP && pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().canBeStunnedByEMP)
                {
                    __result = true;
                }
            }
        }
    }

    internal class Drugs_HarmonyPatch
    {
        // If a pawn will generate with an addiction to X drug.

        // If a ingester can consume X drug.

        // If a ingester will consume X combat-drug.

        // If a ingester will generate with X combat-drug.

        // If a ingester will generate "administer X drug" health recipes.
    }

    // Toggles if pawns with the "dislikesDarkness" extension will get the darkness mood debuff.
    [HarmonyPatch(typeof(ThoughtWorker_Dark), "CurrentStateInternal")]
    public class CurrentStateInternal_Patch
    {
        [HarmonyPostfix]
        public static void Listener(Pawn p, ref ThoughtState __result)
        {
            if (p.def.HasModExtension<HumanlikeMechanoidsExtension>() && p.def.GetModExtension<HumanlikeMechanoidsExtension>().seeInDarkness)
            {
                __result = ThoughtState.Inactive;
            }
        }
    }

    // Toggles if pawns with the "pawnCanPerceiveBeauty" extension will change opinion depending on the beauty of a ingester.
    internal class PawnBeautySocial_HarmonyPatch
    {
        [HarmonyPatch(typeof(ThoughtWorker_Ugly), "CurrentSocialStateInternal")]
        public class ThoughtWorker_Ugly_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn pawn, ref ThoughtState __result)
            {
                if (pawn != null && pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().disregardsPawnBeauty)
                {
                    __result = false;
                }
            }
        }
        [HarmonyPatch(typeof(ThoughtWorker_Pretty), "CurrentSocialStateInternal")]
        public class ThoughtWorker_Pretty_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn pawn, ref ThoughtState __result)
            {
                if (pawn != null && pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().disregardsPawnBeauty)
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
                if (pawn != null && pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().disregardsPawnBeauty)
                {
                    __result = false;
                }
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
            HumanlikeMechanoidsExtension pawn = ___pawn.GetModExt();
            return pawn == null || !pawn.disableBreathVapors;
        }
    }

    // Translates the given need bars on pawns tagged as "IsHumanlikeMechanoid" with the given custom keys.
    internal class Need_Patchs
    {
        [HarmonyPatch(typeof(Need), "LabelCap", MethodType.Getter)]
        public class LabelCapFood_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(ref string __result, Pawn ___pawn, Need __instance)
            {
                if (__instance.def.defName == "Food" && Methods.IsHumanlikeMechanoid(___pawn))
                {
                    __result = "RR_EnergyNeed".Translate();
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
                        "RR_EnergyNeed".Translate(),
                        __instance.CurLevelPercentage.ToStringPercent(),
                        __instance.CurLevel,
                        __instance.MaxLevel,
                        "RR_EnergyNeedDescription".Translate()
                    });
                    return;
                }
            }
        }

        [HarmonyPatch(typeof(Need), "LabelCap", MethodType.Getter)]
        public class LabelCapRest_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(ref string __result, Pawn ___pawn, Need __instance)
            {
                if (__instance.def.defName == "Rest" && Methods.IsHumanlikeMechanoid(___pawn))
                {
                    __result = "RR_MemoryNeed".Translate();
                }
            }
        }
        [HarmonyPatch(typeof(Need), "GetTipString")]
        public class GetTipStringRest_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(ref string __result, Pawn ___pawn, Need __instance)
            {
                if (__instance.def.defName == "Rest" && Methods.IsHumanlikeMechanoid(___pawn))
                {
                    __result = string.Format("{0}: {1}\n{2}", new object[]
                    {
                        "RR_MemoryNeed".Translate(),
                        __instance.CurLevelPercentage.ToStringPercent(),
                        "RR_MemoryNeedDescription".Translate()
                    });
                    return;
                }
            }
        }
    }

    internal class AnomalyPatches
    {
        // Toggles if pawns with the "revenantTargetable" extension will be targeted by revenants.
        [HarmonyPatch(typeof(RevenantUtility), "ValidTarget")]
        public static class ValidTarget_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn pawn, ref bool __result)
            {
                if (pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && !pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().revenantTargetable)
                {
                    __result = false;
                }
            }
        }

        // Controls if pawns with the "IsHumanlikeMechanoid" tag can be affected by flesh manipulation.
        [HarmonyPatch(typeof(FleshbeastUtility), "TryGiveMutation")]
        public static class TryGiveMutation_HarmonyPatch
        {
            [HarmonyPrefix]
            public static void Postfix(ref bool __result, Pawn pawn, HediffDef mutationDef)
            {
                if (pawn.def.HasModExtension<HumanlikeMechanoidsExtension>())
                {
                    __result = false;
                }
            }
        }

        [HarmonyPatch]
        public static class Verb_CastTargetEffectBiomutationLance_HarmonyPatch
        {
            [HarmonyTargetMethods]
            public static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.DeclaredMethod(typeof(Verb_CastTargetEffectBiomutationLance), "ValidateTarget", null, null);
                yield break;
            }
            [HarmonyPostfix]
            public static void Postfix(ref bool __result, LocalTargetInfo target)
            {
                if (__result)
                {
                    if (target.Thing is Pawn pawn && pawn.IsHumanlikeMechanoid())
                    {
                        Messages.Message("RR_SynthCannotBeFleshshaped".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                        __result = false;
                        return;
                    }
                }
            }
        }

        [HarmonyPatch]
        public static class CompAbilityEffect_FleshbeastFromCorpse_HarmonyPatch
        {
            [HarmonyTargetMethods]
            public static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.DeclaredMethod(typeof(CompAbilityEffect_FleshbeastFromCorpse), "Apply", null, null);
                yield break;
            }
            [HarmonyPrefix]
            public static bool Prefix(LocalTargetInfo target)
            {
                if (target.Thing is Corpse corpse && corpse.InnerPawn.IsHumanlikeMechanoid())
                {
                    Messages.Message("RR_SynthCorpseCannotBeFleshshaped".Translate(), MessageTypeDefOf.RejectInput, false);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch]
        public static class CompAbilityEffect_UnnaturalHealing_HarmonyPatch
        {
            [HarmonyTargetMethods]
            public static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.DeclaredMethod(typeof(CompAbilityEffect_UnnaturalHealing), "Valid", null, null);
                yield break;
            }
            [HarmonyPostfix]
            public static void Postfix(ref bool __result, LocalTargetInfo target, bool throwMessages = false)
            {
                if (__result)
                {
                    if (target.Thing is Pawn pawn && pawn.IsHumanlikeMechanoid())
                    {
                        if (throwMessages)
                        {
                            Messages.Message("RR_SynthCannotBeUnnatHealed".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                        }
                        __result = false;
                        return;
                    }
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
        [HarmonyPatch(typeof(CompRottable), "Active", MethodType.Getter)]
        public static class RottableActive_HarmonyPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(CompRottable __instance, ref bool __result)
            {
                if (__instance.parent is Corpse corpse && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsImperishable)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
        // Toggles if corpses of pawns with the "corpseIsEdible" extension will be edible.
        [HarmonyPatch(typeof(Corpse), "IngestibleNow", MethodType.Getter)]
        public class IngestibleNow_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(Corpse __instance, ref bool __result)
            {
                if (!__result)
                    return;
                if (__instance is Corpse corpse && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsInedible)
                {
                    __result = false;
                }
            }
        }
        // Consistency to block certain things that use/target fleshy corpses. Uses the "corpseIsEdible" extension to obtain a value.
        [HarmonyPatch(typeof(CompTargetable), "ValidateTarget")]
        public static class BaseTargetValidator_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(LocalTargetInfo target, CompTargetable __instance, ref bool __result)
            {
                Corpse corpse = (Corpse)target.Thing;
                if (__result && __instance.Props.fleshCorpsesOnly && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsInedible && !corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().isResurrectedNormally)
                {
                    __result = false;
                }
            }
        }
        // Toggles if corpses of pawns with the "corpseIsDisturbing" extension will disturb pawns.
        [HarmonyPatch(typeof(Corpse), "GiveObservedThought")]
        public class GiveObservedThought_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Corpse __instance, ref Thought_Memory __result)
            {
                if (__instance.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && __instance.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsNotDisturbing)
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
                if (__instance is Corpse corpse && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsNotDisturbing)
                {
                    __result = null;
                }
            }
        }
        [HarmonyPatch(typeof(Designator_ExtractSkull), "CanDesignateThing")]
        public static class Designator_ExtractSkull_CanDesignateThing_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(ref AcceptanceReport __result, Thing t)
            {
                if (__result)
                {
                    if (t is Corpse corpse && corpse.InnerPawn.IsHumanlikeMechanoid())
                    {
                        __result = false;
                    }
                }
            }
        }

        internal class Resurrection_HarmonyPatch
        {
            [HarmonyPatch(typeof(CompAbilityEffect_Resurrect), "Valid")]
            public static class Resurrection_Valid_HarmonyPatch
            {
                [HarmonyPostfix]
                public static void Postfix(LocalTargetInfo target, ref bool __result)
                {
                    if (__result)
                    {
                        if (target.Thing is Corpse corpse && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().isResurrectedNormally)
                        {
                            __result = true;
                        }
                    }
                }
            }
            [HarmonyPatch(typeof(ResurrectionUtility), "TryResurrectWithSideEffects")]
            public static class ResurrectionUtility_ResurrectWithSideEffects_Patch
            {
                [HarmonyPrefix]
                public static bool Prefix(Pawn pawn)
                {
                    return !pawn.IsHumanlikeMechanoid();
                }
            }
        }

        [HarmonyPatch(typeof(Alert_ColonistLeftUnburied), "IsCorpseOfColonist")]
        public class Alert_IsCorpseOfColonist_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(Corpse corpse, ref bool __result)
            {
                if (!__result)
                {
                    return;
                }
                Pawn innerPawn = corpse.InnerPawn;
                if (innerPawn != null && corpse.InnerPawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>().corpseIsNotDisturbing)
                {
                    __result = false;
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
                __result = "RR_SynthVsVampire".Translate(prisoner.Named("PAWN"));
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
                        Messages.Message("RR_SynthVsVampire".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
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
                            Messages.Message("RR_SynthVsVampire".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                        }
                        __result = false;
                        return;
                    }
                }
            }
        }
        [HarmonyPatch]
        public static class ITab_PrisonerHemogenFarm_HarmonyPatch
        {
            public static MethodBase TargetMethod()
            {
                Type[] nestedTypes = typeof(ITab_Pawn_Visitor).GetNestedTypes(AccessTools.all);
                for (int i = 0; i < nestedTypes.Length; i++)
                {
                    foreach (MethodInfo methodInfo in nestedTypes[i].GetMethods(AccessTools.all))
                    {
                        if (methodInfo.Name.Contains("CanUsePrisonerInteractionMode"))
                        {
                            return methodInfo;
                        }
                    }
                }
                return null;
            }
            public static void Postfix(Pawn pawn, PrisonerInteractionModeDef mode, ref bool __result)
            {
                if (mode == PrisonerInteractionModeDefOf.HemogenFarm && pawn.IsHumanlikeMechanoid())
                {
                    __result = false;
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
                            Messages.Message("RR_SynthCannotBeImplanted".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
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
                if (target.Thing is Pawn pawn && pawn.IsHumanlikeMechanoid() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().incapableOfLove)
                {
                    if (throwMessages)
                    {
                        Messages.Message("RR_SynthIsIncapableOfLove".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.RejectInput, false);
                    }
                    __result = false;
                    return;
                }
            }
        }
    }
    [HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), "RandomSelectionWeight")]
    public static class RandomSelectionWeight_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
        {
            HumanlikeMechanoidsExtension initExtension = initiator.GetModExt();
            if (initiator.IsHumanlikeMechanoid() && initExtension.incapableOfLove)
            {
                __result = 0f;
                return;
            }
            HumanlikeMechanoidsExtension recipExtension = recipient.GetModExt();
            if (recipient.IsHumanlikeMechanoid() && recipExtension.incapableOfLove)
            {
                __result = 0f;
            }
        }
    }
    [HarmonyPatch(typeof(RelationsUtility), "RomanceEligible")]
    public static class RomanceEligible_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Listener(Pawn pawn, ref AcceptanceReport __result)
        {
            HumanlikeMechanoidsExtension modExtension = pawn.GetModExt();
            if (pawn.IsHumanlikeMechanoid() && modExtension.incapableOfLove)
            {
                __result = "RR_SynthIsIncapableOfLove".Translate(pawn.Named("PAWN"));
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
                HumanlikeMechanoidsExtension pawn = humanlike.GetModExt();
                return pawn == null || !pawn.incapableOfLove;
            }
        }
    }

    // Disables pawns from disliking robots for their "xenotypes".
    internal class Precept_Xenotypes_HarmonyPatch
    {
        [HarmonyPatch(typeof(ThoughtWorker_Precept_SelfDislikedXenotype), "ShouldHaveThought")]
        public class SelfDisliked_HarmonyPatch
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

        [HarmonyPatch(typeof(ThoughtWorker_Precept_PreferredXenotype_Social), "ShouldHaveThought")]
        public class PreferredXeno_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, Pawn otherPawn, ref ThoughtState __result)
            {
                if (!__result.Active)
                {
                    return;
                }
                if (Methods.IsHumanlikeMechanoid(otherPawn))
                {
                    __result = ThoughtState.Inactive;
                }
            }
        }

        [HarmonyPatch(typeof(ThoughtWorker_Precept_ColonyXenotypeMakeup), "ShouldHaveThought")]
        public class ColonyXenos_HarmonyPatch
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

        [HarmonyPatch(typeof(TaleUtility), "Notify_PawnDied")]
        public static class Notify_BiologicalPawnDied_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn victim, DamageInfo? dinfo)
            {
                if (victim.IsHumanOfColony())
                {
                    if (((dinfo != null) ? dinfo.GetValueOrDefault().Instigator : null) is Pawn pawn)
                    {
                        Find.HistoryEventsManager.RecordEvent(new HistoryEvent(RR_DefOf.RR_BiologicalPawnDied, new SignalArgs(pawn.Named(HistoryEventArgsNames.Doer))), true);
                        return;
                    }
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(RR_DefOf.RR_BiologicalPawnDied), true);
                }
            }
        }

        [HarmonyPatch(typeof(TaleUtility), "Notify_PawnDied")]
        public static class Notify_MechanicalPawnDied_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn victim, DamageInfo? dinfo)
            {
                if (victim.IsPersonaOfColony())
                {
                    if (((dinfo != null) ? dinfo.GetValueOrDefault().Instigator : null) is Pawn pawn)
                    {
                        Find.HistoryEventsManager.RecordEvent(new HistoryEvent(RR_DefOf.RR_MechanicalPawnDied, new SignalArgs(pawn.Named(HistoryEventArgsNames.Doer))), true);
                        return;
                    }
                    Find.HistoryEventsManager.RecordEvent(new HistoryEvent(RR_DefOf.RR_MechanicalPawnDied), true);
                }
            }
        }
    }

    // This stops pawns from attempting to tend IsHumanlikeMechanoid pawns.
    internal class MiscTend_Patch
    {
        [HarmonyPatch(typeof(HealthAIUtility), "ShouldBeTendedNowByPlayer")]
        public static class ShouldBeTendedNowByPlayer_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn pawn, ref bool __result)
            {
                if (pawn.IsHumanlikeMechanoid())
                {
                    __result = false;
                }
            }
        }
        [HarmonyPatch(typeof(HealthAIUtility), "ShouldBeTendedNowByPlayerUrgent")]
        public static class ShouldBeTendedNowByPlayerUrgent_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn pawn, ref bool __result)
            {
                if (pawn.IsHumanlikeMechanoid())
                {
                    __result = false;
                }
            }
        }
        [HarmonyPatch(typeof(HediffUtility), "CanHealNaturally")]
        public static class CanHealNaturally_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Hediff_Injury hd, ref bool __result)
            {
                if (hd.pawn.IsHumanlikeMechanoid())
                {
                    __result = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(HediffSet))]
    [HarmonyPatch("CalculatePain")]
    internal class CalculatePain_HarmonyPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref float __result, Pawn ___pawn)
        {
            HumanlikeMechanoidsExtension pawnExtensions = ___pawn.GetModExt();
            if (pawnExtensions != null && !pawnExtensions.canFeelPain)
            {
                __result = 0f;
                return false;
            }
            return true;
        }
    }

    // This stops pawns from running wild.
    internal class Mentalbreaks_Patches
    {
        [HarmonyPatch(typeof(MentalBreakWorker_RunWild), "BreakCanOccur")]
        public static class BreakCanOccur_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn pawn, ref bool __result)
            {
                if (pawn != null && pawn.IsHumanlikeMechanoid() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().canBeStunnedByEMP)
                {
                    __result = true;
                }
                else
                {
                    __result = false;
                }
            }
        }
        [HarmonyPatch(typeof(MentalBreakWorker_RunWild), "TryStart")]
        public static class TryStart_HarmonyPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn pawn, ref bool __result)
            {
                if (pawn != null && pawn.IsHumanlikeMechanoid() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().canBeStunnedByEMP)
                {
                    __result = true;
                }
                else
                {
                    __result = false;
                }
            }
        }
    }


    // Controls if pawns with the "IsHumanlikeMechanoid" tag will recieve "demand" thoughts.
    [HarmonyPatch(typeof(ThoughtWorker_AgeReversalDemanded), "ShouldHaveThought")]
    public class AgeReversalShouldHaveThought_HarmonyPatch
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
    [HarmonyPatch(typeof(ThoughtWorker_NeedNeuralSupercharge), "ShouldHaveThought")]
    public static class NeuralSuperchargeShouldHaveThought_HarmonyPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn p, ref ThoughtState __result)
        {
            if (p.IsHumanlikeMechanoid())
            {
                __result = ThoughtState.Inactive;
                return false;
            }
            return true;
        }
    }


    // Disables certain buildings from accepting given ingester.
    [HarmonyPatch(typeof(Building_GrowthVat), "CanAcceptPawn")]
    public static class GrowthVat_CanAcceptPawn_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref AcceptanceReport __result)
        {
            if (pawn.IsHumanlikeMechanoid())
            {
                __result = "RR_SynthCannotUtilize".Translate(pawn.Named("PAWN"));
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
                __result = "RR_SynthNotGenetic".Translate();
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
                __result = "RR_SynthCannotBeScanned".Translate();
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
                __result = "RR_SynthCannotUtilize".Translate();
            }
        }
    }
    [HarmonyPatch(typeof(CompBiosculpterPod), "MakeCarryToBiosculpterJob")]
    public static class BiosculpterPod_MakeCarryToBiosculpterJob_HarmonyPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn willBeCarried)
        {
            return !willBeCarried.IsHumanlikeMechanoid();
        }
    }
    [HarmonyPatch(typeof(CompNeuralSupercharger), "CanAutoUse")]
    public static class CanAutoUse_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (__result && pawn.IsHumanlikeMechanoid())
            {
                __result = false;
            }
        }
    }
    [HarmonyPatch(typeof(CompNeuralSupercharger), "CompFloatMenuOptions")]
    public static class CompFloatMenuOptions_HarmonyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CompNeuralSupercharger __instance, Pawn selPawn, ref IEnumerable<FloatMenuOption> __result)
        {
            bool flag = selPawn.IsHumanlikeMechanoid();
            if (flag)
            {
                __result = null;
            }
        }
    }


    // VFE Ancients patch, credit to the VFE team for this one.
    [HarmonyPatch]
    public static class VFEAncients_Pawn_PowerTracker_CanGetPowers_Patch
    {
        public static bool Prepare()
        {
            VFEAncients_Pawn_PowerTracker_CanGetPowers_Patch.targetMethod = AccessTools.Method("VFEAncients.Pawn_PowerTracker:CanGetPowers", null, null);
            return VFEAncients_Pawn_PowerTracker_CanGetPowers_Patch.targetMethod != null;
        }
        public static MethodBase TargetMethod()
        {
            return VFEAncients_Pawn_PowerTracker_CanGetPowers_Patch.targetMethod;
        }
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (pawn.IsHumanlikeMechanoid())
            {
                __result = false;
            }
        }
        public static MethodInfo targetMethod;
    }


    // LISTERS / PATCHERS

    // Disables listed jobs from being performed by given ingester.
    [HarmonyPatch(typeof(Pawn_JobTracker), "StartJob")]
    internal class StartJob_HarmonyPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Job newJob, Pawn ___pawn)
        {
            HumanlikeMechanoidsExtension pawn = ___pawn.GetModExt();
            List<JobDef> list;
            if (pawn == null)
            {
                list = null;
            }
            else
            {
                HumanlikeMechanoidsExtension patch = pawn;
                list = (patch?.blackListedJobs);
            }
            List<JobDef> list2 = list;
            return list2 == null || !list2.Contains(newJob.def);
        }
    }

    // Disables certain needs from being applied to given ingester.
    [HarmonyPatch(typeof(Pawn_NeedsTracker), "ShouldHaveNeed")]
    internal class ShouldHaveNeed_HarmonyPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(NeedDef nd, Pawn ___pawn)
        {
            HumanlikeMechanoidsExtension pawn = ___pawn.GetModExt();
            List<NeedDef> list;
            if (pawn == null)
            {
                list = null;
            }
            else
            {
                HumanlikeMechanoidsExtension patch = pawn;
                list = (patch?.blackListedNeeds);
            }
            List<NeedDef> list2 = list;
            return list2 == null || !list2.Contains(nd);
        }
    }

    // Disables certain hediffs from being applied to given ingester.
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
            HumanlikeMechanoidsExtension pawn = ___pawn.GetModExt();
            List<HediffDef> list;
            if (pawn == null)
            {
                list = null;
            }
            else
            {
                HumanlikeMechanoidsExtension patch = pawn;
                list = (patch?.blackListedHediffs);
            }
            List<HediffDef> list2 = list;
            return list2 == null || !list2.Contains(hediff.def);
        }
    }

    //This stops drugs that do not have the "VexedThingsItemExtension" extension from being administered to IsHumanlikeMechanoid pawns.
    [HarmonyPatch(typeof(Recipe_AdministerIngestible), "ApplyOnPawn")]
    public class AdministerApplyOnPawn_HarmonyPatch
    {
        [HarmonyPrefix]
        public static bool Listener(Bill __instance, Pawn pawn, List<Thing> ingredients)
        {
            if (pawn.IsHumanlikeMechanoid() && !ingredients[0].def.HasModExtension<VexedThingsItemExtension>() && !ingredients[0].def.GetModExtension<VexedThingsItemExtension>().isIngestibleByPersonaeOnly)
            {
                return false;
            }
            return true;
        }
    }

    // This displays mechanoid specific health capabilities, and changes "self-tend" to "self-repair".
    [HarmonyPatch(typeof(HealthCardUtility), "DrawOverviewTab")]
    public static class DrawOverviewTab_HarmonyPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref float __result, Rect rect, Pawn pawn, float curY)
        {
            if (pawn.IsHumanlikeMechanoid())
            {
                __result = DrawOverviewTabPersonaMech(rect, pawn, curY);
                return false;
            }
            return true;
        }
        private static float DrawOverviewTabPersonaMech(Rect rect, Pawn pawn, float curY)
        {
            curY += 4f;
            bool flag = false;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = new Color(0.9f, 0.9f, 0.9f);
            if (pawn.foodRestriction != null && pawn.foodRestriction.Configurable && !pawn.DevelopmentalStage.Baby() && pawn.needs?.food != null && !pawn.IsMutant)
            {
                Rect rect3 = new Rect(0f, curY, rect.width, 23f);
                flag = true;
                TooltipHandler.TipRegionByKey(rect3, "RR_FuelRestrictionShort");
                Widgets.DrawHighlightIfMouseover(rect3);
                Rect rect4 = rect3;
                rect4.xMax = rect3.center.x - 4f;
                Rect rect5 = rect3;
                rect5.xMin = rect3.center.x + 4f;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rect4, string.Format("{0}:", "RR_FuelRestriction".Translate()));
                Text.Anchor = TextAnchor.UpperLeft;
                if (Widgets.ButtonText(rect5, pawn.foodRestriction.CurrentFoodPolicy.label))
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach (FoodPolicy restriction in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
                    {
                        list.Add(new FloatMenuOption(restriction.label, delegate
                        {
                            pawn.foodRestriction.CurrentFoodPolicy = restriction;
                        }));
                    }
                    list.Add(new FloatMenuOption("RR_ManageFuelRestrictions".Translate() + "...", delegate
                    {
                        Find.WindowStack.Add(new Dialog_ManageFoodPolicies(pawn.foodRestriction.CurrentFoodPolicy));
                    }));
                    Find.WindowStack.Add(new FloatMenu(list));
                }
                curY += rect3.height + 4f;
            }
            if (Current.ProgramState == ProgramState.Playing && pawn.IsColonist && !pawn.Dead && !pawn.DevelopmentalStage.Baby() && pawn.playerSettings != null)
            {
                Rect rect6 = new Rect(0f, curY, rect.width, 23f);
                flag = true;
                TooltipHandler.TipRegion(rect6, "RR_SelfRepairTip".Translate(Faction.OfPlayer.def.pawnsPlural, 0.7f.ToStringPercent()).CapitalizeFirst());
                Widgets.DrawHighlightIfMouseover(rect6);
                Rect rect7 = rect6;
                rect7.xMax = rect6.center.x - 4f;
                Rect rect2 = rect6;
                rect2.xMin = rect6.center.x + 4f;
                rect2.width = rect2.height;
                rect2.ContractedBy(4f);
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rect7, string.Format("{0}:", "RR_SelfRepair".Translate()));
                Text.Anchor = TextAnchor.UpperLeft;
                bool selfTend = pawn.playerSettings.selfTend;
                Widgets.Checkbox(rect2.x, rect2.y, ref pawn.playerSettings.selfTend, rect2.height);
                if (pawn.playerSettings.selfTend && !selfTend)
                {
                    if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Smithing))
                    {
                        pawn.playerSettings.selfTend = false;
                        Messages.Message("RR_MustBeCapableToRepair".Translate(pawn.LabelShort, pawn), MessageTypeDefOf.RejectInput, historical: false);
                    }
                    else if (pawn.needs.food != null && pawn.needs.food.CurLevel <= 0f)
                    {
                        Messages.Message("RR_MustHaveEnergyToRepair".Translate(pawn.LabelShort, pawn), MessageTypeDefOf.RejectInput, historical: false);
                    }
                    else if (pawn.workSettings.GetPriority(WorkTypeDefOf.Smithing) == 0)
                    {
                        Messages.Message("RR_MustBeAssignedToRepair".Translate(pawn.LabelShort, pawn), MessageTypeDefOf.CautionInput, historical: false);
                    }
                }
                curY += rect6.height + 10f;
            }
            if (flag)
            {
                Widgets.DrawLineHorizontal(rect.x - 8f, curY, rect.width, Color.gray);
            }
            curY += 10f;
            HumanlikeMechanoidsExtension pawnExtensions = pawn.GetModExt();
            bool flag2 = pawn != null && pawnExtensions.canFeelPain;
            if (pawnExtensions.canFeelPain)
            {
                Pair<string, Color> painLabel = HealthCardUtility.GetPainLabel(pawn);
                string painTip = HealthCardUtility.GetPainTip(pawn);
                DrawLeftRow(rect, ref curY, "PainLevel".Translate(), painLabel.First, painLabel.Second, painTip);
            }
            curY += 6f;
            Text.Font = GameFont.Small;
            if (!pawn.Dead)
            {
                IEnumerable<PawnCapacityDef> source;
                if (!pawn.def.race.Humanlike)
                {
                    if (pawn.def.race.Animal)
                    {
                        source = from x in DefDatabase<PawnCapacityDef>.AllDefs
                                 where x.showOnAnimals
                                 select x;
                    }
                    else
                    {
                        source = from x in DefDatabase<PawnCapacityDef>.AllDefs
                                 where x.showOnMechanoids
                                 select x;
                    }
                }
                else
                {
                    source = from x in DefDatabase<PawnCapacityDef>.AllDefs
                             where x.showOnHumanlikes
                             select x;
                }
                foreach (PawnCapacityDef pawnCapacityDef in from act in source
                                                            orderby act.listOrder
                                                            select act)
                {
                    if (PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, pawnCapacityDef))
                    {
                        PawnCapacityDef activityLocal = pawnCapacityDef;
                        Pair<string, Color> efficiencyLabel = HealthCardUtility.GetEfficiencyLabel(pawn, pawnCapacityDef);
                        Func<string> textGetter = delegate ()
                        {
                            if (pawn.Dead)
                            {
                                return "";
                            }
                            return HealthCardUtility.GetPawnCapacityTip(pawn, activityLocal);
                        };
                        HealthCardUtility.DrawLeftRow(rect, ref curY, ((!pawnCapacityDef.labelMechanoids.NullOrEmpty()) ? pawnCapacityDef.labelMechanoids : pawnCapacityDef.label).CapitalizeFirst(), efficiencyLabel.First, efficiencyLabel.Second, new TipSignal(textGetter, pawn.thingIDNumber ^ (int)pawnCapacityDef.index));
                    }
                }
                return curY;
            }
            return curY;
        }
        private static void DrawLeftRow(Rect rect, ref float curY, string leftLabel, string rightLabel, Color rightLabelColor, TipSignal tipSignal)
        {
            Rect rect2 = new Rect(17f, curY, rect.width - 34f - 10f, 22f);
            if (Mouse.IsOver(rect2))
            {
                using (new TextBlock(HealthCardUtility.HighlightColor))
                {
                    GUI.DrawTexture(rect2, TexUI.HighlightTex);
                }
            }
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect2, leftLabel);
            GUI.color = rightLabelColor;
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect2, rightLabel);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect3 = new Rect(0f, curY, rect.width, 20f);
            if (Mouse.IsOver(rect3))
            {
                TooltipHandler.TipRegion(rect3, tipSignal);
            }
            curY += rect2.height;
        }
    }
}