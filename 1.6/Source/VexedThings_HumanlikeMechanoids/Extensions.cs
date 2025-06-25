using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VexedThings
{
    public class HumanlikeMechanoidsExtension : DefModExtension
    {
        public bool corpseIsNotDisturbing = true;
        public bool corpseIsImperishable = true;
        public bool corpseIsInedible = true;
        public bool canBeStunnedByEMP = true;
        public bool canFallIll = true;
        public bool canRunWild = true;
        public bool canFeelPain = true;
        public bool seeInDarkness = true;
        public bool isResurrectedNormally = true;
        public bool incapableOfLove = true;
        public bool disableBreathVapors = true;
        public bool willSpawnWithTraits = true;
        public bool disregardsPawnBeauty = true;
        public bool concernedWithVacuum = true;
        public bool harmedByVacuum = true;
        public bool revenantTargetable = true;
        public bool devourerDigestable = true;

        public List<HediffDef> blackListedHediffs = new List<HediffDef>();
        public List<JobDef> blackListedJobs = new List<JobDef>();
        public List<NeedDef> blackListedNeeds = new List<NeedDef>();
        public List<ThingDef> whiteListedDrugs = new List<ThingDef>();
    }

    public class RecipeHumanlikeMechanoidsExtension : DefModExtension
    {
        public bool pawnResurrection;
    }

    public class VexedThingsItemExtension : DefModExtension
    {
        public bool isIngestibleByPersonaeOnly = true;
    }

    public class VexedThingsHediffExtension : DefModExtension
    {
        public bool onlyAffectsPersonae = true;
    }

    public class VexedThingsEntityExtension : DefModExtension
    {

        public bool messagePersonaRegurgitation = true;
    }

    [DefOf]
    public static class RR_DefOf
    {
        static RR_DefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RR_DefOf));
        }
        public static TraitDef Automatonophobia;
        public static TraitDef Anthropophobia;
        public static ThoughtDef AutomatonophobiaVsSynth;
        public static ThoughtDef AnthropophobiaVsHuman;
        public static ThoughtDef RR_BiologicalPawns_Veneration_Died;
        public static ThoughtDef RR_MechanicalPawns_Veneration_Died;

        public static FleshTypeDef VexedMechanical;

        public static JobDef RR_RepairPersonae;

        public static HediffDef RR_HumanlikeMechanoidHibernation;
        public static HediffDef RR_HumanlikeMechanoidResurrectionMadness;

        public static StatDef TendSpeed_Synth;
        public static StatDef PersonaEnergyLossPerHP;

        public static EffecterDef RepairingSynthetic;

        public static ThingDef FuelNode_SynthDiet;
        public static ThingDef IndustrialChargePylon;
        public static FleckDef RR_PersonaFleckCharging;
        public static FleckDef RR_PersonaFleckRepair;

        public static SoundDef ReactorCharge_Sustained;

        public static PawnRelationDef Creator;

        [MayRequireIdeology]
        public static HistoryEventDef RR_MechanicalPawnDied;
        [MayRequireIdeology]
        public static HistoryEventDef RR_BiologicalPawnDied;
    }

    public class NumConvert
    {
        int num = Rand.RangeInclusive(0, 8);
        public static implicit operator int(NumConvert i)
        {
            return i.num;
        }
    }

    public static class SyntheticPawnsExtensions
    {
        public static HumanlikeMechanoidsExtension GetModExt(this Pawn pawn)
        {
            return pawn.def.GetModExtension<HumanlikeMechanoidsExtension>();
        }
        public static HumanlikeMechanoidsExtension GetModExt(this Corpse corpse)
        {
            return corpse.InnerPawn.def.GetModExtension<HumanlikeMechanoidsExtension>();
        }
    }
}