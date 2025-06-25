using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace VexedThings
{
    // Anthropophobia.

    [UsedImplicitly]
    public class ThoughtWorker_HumanVsAnthropophobia : ThoughtWorker
    {
        public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
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
            if (other.story.traits.DegreeOfTrait(RR_DefOf.Anthropophobia) != 0)
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_AnthropophobiaVsHuman : ThoughtWorker
    {
        public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
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
            if (p.story.traits.DegreeOfTrait(RR_DefOf.Anthropophobia) != 0)
            {
                return ThoughtState.ActiveAtStage(1);
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_AnthropophobeVsAnthropophile : ThoughtWorker
    {
        public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
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
        public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
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
        public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
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
        public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
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
        public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
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


    // Precepts
    public class ThoughtWorker_Precept_PreferredXenotypeVSPersona_Social : ThoughtWorker_Precept_Social
    {
        public override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.BiotechActive || !ModsConfig.IdeologyActive || other.genes == null)
            {
                return ThoughtState.Inactive;
            }
            if (other.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }

    public class Thought_Situational_Precept_PersonaeInColony : Thought_Situational
    {
        public override float MoodOffset()
        {
            return BaseMoodOffset * Methods.GetPersonaeInFactionCount(pawn.Faction);
        }
    }

    public class ThoughtWorker_Precept_PersonaeInColony : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            return p.IsColonist && p.IsHumanOfColony() && !p.IsSlave && !p.IsPrisoner && Methods.GetPersonaeInFactionCount(p.Faction) > 0;
        }
    }

    public class ThoughtWorker_Precept_NoPersonaeInColony : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            return p.IsColonist && p.IsHumanOfColony() && !p.IsSlave && !p.IsPrisoner && Methods.GetPersonaeInFactionCount(p.Faction) <= 0;
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_Precept_PersonaeDisliked_Social : ThoughtWorker_Precept_Social
    {
        public override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (other.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            if (other.story.traits.DegreeOfTrait(RR_DefOf.Anthropophobia) == 0)
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(1);
        }
    }

    public class ThoughtWorker_Precept_PersonaeAbhorrent_Social : ThoughtWorker_Precept_Social
    {
        public override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (other.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(1);
        }
    }

    public class ThoughtWorker_Precept_PersonaeSelf : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (p == null || p.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            if (Methods.GetHumansInFactionCount(p.Faction) <= 0)
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(1);
        }
    }

    public class ThoughtWorker_Precept_PersonaeVeneration_Social : ThoughtWorker_Precept_Social
    {
        public override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (!p.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            if (other.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(1);
        }
    }

    public class ThoughtWorker_Precept_Personae_LeaderRole : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.IdeologyActive || p.Faction == null)
            {
                return ThoughtState.Inactive;
            }
            if (p.IsPersonaOfColony())
            {
                return ThoughtState.Inactive;
            }
            _ = p.Ideo;
            if (p.IsHumanOfColony())
            {
                foreach (Pawn item in p.MapHeld.mapPawns.SpawnedPawnsInFaction(p.Faction))
                {
                    if (item.IsPersonaOfColony())
                    {
                        Precept_Role precept_Role = item.Ideo?.GetRole(item);
                        if (precept_Role != null && precept_Role.ideo == p.Ideo && precept_Role.def == PreceptDefOf.IdeoRole_Leader)
                        {
                            return ThoughtState.ActiveAtStage(1);
                        }
                    }
                }
            }
            return ThoughtState.Inactive;
        }
    }

    public class ThoughtWorker_Precept_Personae_MoralistRole : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.IdeologyActive || p.Faction == null)
            {
                return ThoughtState.Inactive;
            }
            if (p.IsPersonaOfColony())
            {
                return ThoughtState.Inactive;
            }
            _ = p.Ideo;
            if (p.IsHumanOfColony())
            {
                foreach (Pawn item in p.MapHeld.mapPawns.SpawnedPawnsInFaction(p.Faction))
                {
                    if (item.IsPersonaOfColony())
                    {
                        Precept_Role precept_Role = item.Ideo?.GetRole(item);
                        if (precept_Role != null && precept_Role.ideo == p.Ideo && precept_Role.def == PreceptDefOf.IdeoRole_Moralist)
                        {
                            return ThoughtState.ActiveAtStage(1);
                        }
                    }
                }
            }
            return ThoughtState.Inactive;
        }
    }

    public class Thought_Situational_Precept_HumansInColony : Thought_Situational
    {
        public override float MoodOffset()
        {
            return BaseMoodOffset * Methods.GetHumansInFactionCount(pawn.Faction);
        }
    }

    public class ThoughtWorker_Precept_HumansInColony : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            return p.IsColonist && p.IsPersonaOfColony() && !p.IsSlave && !p.IsPrisoner && Methods.GetHumansInFactionCount(p.Faction) > 0;
        }
    }

    public class ThoughtWorker_Precept_NoHumansInColony : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            return p.IsColonist && p.IsPersonaOfColony() && !p.IsSlave && !p.IsPrisoner && Methods.GetHumansInFactionCount(p.Faction) <= 0;
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_Precept_HumansDisliked_Social : ThoughtWorker_Precept_Social
    {
        public override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (!other.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            if (other.story.traits.HasTrait(TraitDefOf.Transhumanist) && other.health.hediffSet.CountAddedAndImplantedParts() >= 6)
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(1);
        }
    }

    public class ThoughtWorker_Precept_HumansAbhorrent_Social : ThoughtWorker_Precept_Social
    {
        public override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (p.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            if (!other.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(1);
        }
    }

    public class ThoughtWorker_Precept_HumansSelf : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (p == null || !p.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            if (Methods.GetPersonaeInFactionCount(p.Faction) <= 0)
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(1);
        }
    }

    public class ThoughtWorker_Precept_HumansVeneration_Social : ThoughtWorker_Precept_Social
    {
        public override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (p.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            if (!other.IsFleshylike())
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(1);
        }
    }

    public class ThoughtWorker_Precept_Humans_LeaderRole : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.IdeologyActive || p.Faction == null)
            {
                return ThoughtState.Inactive;
            }
            if (p.IsHumanOfColony())
            {
                return ThoughtState.Inactive;
            }
            _ = p.Ideo;
            if (p.IsPersonaOfColony())
            {
                foreach (Pawn item in p.MapHeld.mapPawns.SpawnedPawnsInFaction(p.Faction))
                {
                    if (item.IsHumanOfColony())
                    {
                        Precept_Role precept_Role = item.Ideo?.GetRole(item);
                        if (precept_Role != null && precept_Role.ideo == p.Ideo && precept_Role.def == PreceptDefOf.IdeoRole_Leader)
                        {
                            return ThoughtState.ActiveAtStage(1);
                        }
                    }
                }
            }
            return ThoughtState.Inactive;
        }
    }

    public class ThoughtWorker_Precept_Humans_MoralistRole : ThoughtWorker_Precept
    {
        public override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.IdeologyActive || p.Faction == null)
            {
                return ThoughtState.Inactive;
            }
            if (p.IsHumanOfColony())
            {
                return ThoughtState.Inactive;
            }
            _ = p.Ideo;
            if (p.IsPersonaOfColony())
            {
                foreach (Pawn item in p.MapHeld.mapPawns.SpawnedPawnsInFaction(p.Faction))
                {
                    if (item.IsHumanOfColony())
                    {
                        Precept_Role precept_Role = item.Ideo?.GetRole(item);
                        if (precept_Role != null && precept_Role.ideo == p.Ideo && precept_Role.def == PreceptDefOf.IdeoRole_Moralist)
                        {
                            return ThoughtState.ActiveAtStage(1);
                        }
                    }
                }
            }
            return ThoughtState.Inactive;
        }
    }
}