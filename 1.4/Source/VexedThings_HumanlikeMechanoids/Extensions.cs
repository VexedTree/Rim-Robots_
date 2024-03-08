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
        public bool incapableOfLove = true;
        public bool disableBreathVapors = true;
        public bool willSpawnWithTraits = true;
        public bool pawnRequiresRepairs = true;
        public bool pawnCannotPerceiveBeauty = true;

        public bool areSyntheticPawnsVictims;
        public bool onlyPersonaeCanIngest;

        public List<HediffDef> blackListedHediffs = new List<HediffDef>();
        public List<JobDef> blackListedJobs = new List<JobDef>();
    }

    public class VexedThingsItemExtension : DefModExtension
    {
        public bool isIngestibleByPersonaeOnly = true;
    }
    public class VexedThingsHediffExtension : DefModExtension
    {
        public bool affectsPersonae = true;
    }

    internal class PersonaCapacityLabel : DefModExtension
    {
        public string personaCapLabel = string.Empty;
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

        public static JobDef RR_RepairPersonae;

        public static HediffDef RR_HumanlikeMechanoidHibernation;

        public static StatDef TendSpeed_Synth;
        public static StatDef PersonaEnergyLossPerHP;

        public static EffecterDef RepairingSynthetic;

        public static ThingDef FuelNode_SynthDiet;
        public static ThingDef IndustrialChargePylon;
        public static FleckDef RR_PersonaFleckCharging;
        public static FleckDef RR_PersonaFleckRepair;

        public static SoundDef ReactorCharge_Sustained;
    }

    public class Need_RemoveFoodNeed : Need_Food
    {
        public Need_RemoveFoodNeed(Pawn pawn) : base(pawn)
        {
        }
        public override void NeedInterval()
        {
            CurLevel = 1f;
        }
    }

    public static class SyntheticPawnsExtensions
    {
        public static HumanlikeMechanoidsExtension FetchExtension(this Pawn pawn)
        {
            return pawn.def.GetModExtension<HumanlikeMechanoidsExtension>();
        }
    }
}