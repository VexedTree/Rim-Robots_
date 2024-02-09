using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace VexedThings
{
    // Anthropophobia.

    [UsedImplicitly]
    public class ThoughtWorker_HumanVsAnthropophobia : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!Utilities.BioVsMechConflict(p.def, other.def) || !RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            if (other.story.traits.DegreeOfTrait(RR_DefOf.Anthropophobia) == 1 && p.health.hediffSet.CountAddedAndImplantedParts() >= 12)
            {
                return false;
            }
            if (!other.story.traits.HasTrait(RR_DefOf.Anthropophobia))
            {
                return false;
            }
            if (other.story.traits.DegreeOfTrait(RR_DefOf.Anthropophobia) != -1)
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_AnthropophobiaVsHuman : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!Utilities.BioVsMechConflict(p.def, other.def) || !RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            if (p.story.traits.DegreeOfTrait(RR_DefOf.Anthropophobia) == 1 && other.health.hediffSet.CountAddedAndImplantedParts() >= 12)
            {
                return false;
            }
            if (!p.story.traits.HasTrait(RR_DefOf.Anthropophobia))
            {
                return false;
            }
            if (p.story.traits.DegreeOfTrait(RR_DefOf.Anthropophobia) != -1)
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_AnthropophobeVsAnthropophile : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            return p.RaceProps.Humanlike && other.RaceProps.Humanlike && p.story.traits.HasTrait(RR_DefOf.Anthropophobia) && other.story.traits.HasTrait(RR_DefOf.Anthropophobia) && p.story.traits.DegreeOfTrait(RR_DefOf.Anthropophobia) != other.story.traits.DegreeOfTrait(RR_DefOf.Anthropophobia) && RelationsUtility.PawnsKnowEachOther(p, other);
        }
    }

    [UsedImplicitly]
    public class Thought_AnthropophobeVsAnthropophile : Thought_SituationalSocial
    {
        public override float OpinionOffset()
        {
            if (pawn.story.traits.DegreeOfTrait(RR_DefOf.Anthropophobia) != 1)
            {
                return -15f;
            }
            return -25f;
        }
    }


    // Automatonophobia.

    [UsedImplicitly]
    public class ThoughtWorker_SynthVsAutomatonophobia : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!Utilities.BioVsMechConflict(p.def, other.def) || !RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            if (!other.story.traits.HasTrait(RR_DefOf.Automatonophobia))
            {
                return false;
            }
            if (other.story.traits.HasTrait(RR_DefOf.Automatonophobia))
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_AutomatonophobiaVsSynth : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!Utilities.BioVsMechConflict(p.def, other.def) || !RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            if (!p.story.traits.HasTrait(RR_DefOf.Automatonophobia))
            {
                return false;
            }
            if (p.story.traits.HasTrait(RR_DefOf.Automatonophobia))
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }

    // Transhumanism.

    [UsedImplicitly]
    public class ThoughtWorker_SynthVsTranshumanism : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!Utilities.BioVsMechConflict(p.def, other.def) || !RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            if (!other.story.traits.HasTrait(TraitDefOf.Transhumanist))
            {
                return false;
            }
            if (other.story.traits.HasTrait(RR_DefOf.Automatonophobia))
            {
                return false;
            }
            if (other.story.traits.HasTrait(TraitDefOf.Transhumanist))
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_TranshumanismVsSynth : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!Utilities.BioVsMechConflict(p.def, other.def) || !RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }
            if (!p.story.traits.HasTrait(TraitDefOf.Transhumanist))
            {
                return false;
            }
            if (p.story.traits.HasTrait(RR_DefOf.Automatonophobia))
            {
                return false;
            }
            if (p.story.traits.HasTrait(TraitDefOf.Transhumanist))
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }
}
