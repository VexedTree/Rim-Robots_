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
        public bool pawnCannotPerceiveBeauty  = true;

        public bool areSyntheticPawnsVictims;
        public bool onlyPersonaeCanIngest;

        public List<HediffDef> blackListedHediffs = new List<HediffDef>();
        public List<JobDef> blackListedJobs = new List<JobDef>();
        public List<ThingDef> whiteListedDrugs = new List<ThingDef>();
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
        public static ThoughtDef AutomatonophobiaVsSynth;
        public static TraitDef Anthropophobia;
        public static ThoughtDef AnthropophobiaVsHuman;

        public static FleshTypeDef VexedMechanical;
        public static FleshTypeDef VexedSynthetic;

        public static JobDef RR_RepairPersonae;

        public static StatDef TendSpeed_Synth;
        public static StatDef PersonaEnergyLossPerHP;

        public static FleckDef RepairingCog;

        public static EffecterDef RepairingSynthetic;
    }
    public static class SyntheticPawnsExtensions
    {
        public static HumanlikeMechanoidsExtension FetchExtension(this Pawn pawn)
        {
            return pawn.def.GetModExtension<HumanlikeMechanoidsExtension>();
        }
    }
}
