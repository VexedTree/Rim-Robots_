using AlienRace;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;

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


    // Precepts
    public class ThoughtWorker_Precept_PreferredXenotypeVSPersona_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.BiotechActive || !ModsConfig.IdeologyActive || other.genes == null)
            {
                return ThoughtState.Inactive;
            }
            if (other.IsNotFlesh())
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_Precept_PersonaeDisliked_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (other.IsNotFlesh())
            {
                return ThoughtState.Inactive;
            }
            if (other.story.traits.DegreeOfTrait(RR_DefOf.Anthropophobia) != 1)
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(1);
        }
    }

    public class ThoughtWorker_Precept_PersonaeAbhorrent_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (other.IsNotFlesh())
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(1);
        }
    }

    public class ThoughtWorker_Precept_PersonaeAbhorrentSelf : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.BiotechActive || !ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (p == null || p.IsNotFlesh())
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_Precept_Personae : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (!p.IsNotFlesh())
            {
                return ThoughtState.Inactive;
            }
            Lord lord = p.GetLord();
            if (lord != null && lord.ownedPawns.Any((Pawn c) => Utilities.BioVsMechConflict(c.def, p.def)))
            {
                return true;
            }
            Caravan caravan = p.GetCaravan();
            if (caravan != null && caravan.PawnsListForReading.Any((Pawn c) => Utilities.BioVsMechConflict(c.def, p.def)))
            {
                return true;
            }
            Map mapHeld = p.MapHeld;
            if (mapHeld != null)
            {
                Faction faction = p.Faction;
                if (faction != null)
                {
                    if (mapHeld.mapPawns.SpawnedPawnsInFaction(faction).Any((Pawn c) => Utilities.BioVsMechConflict(c.def, p.def)))
                    {
                        return true;
                    }
                }
                else if (mapHeld.mapPawns.AllPawnsSpawned.Any((Pawn c) => Utilities.BioVsMechConflict(c.def, p.def) && !p.HostileTo(c)))
                {
                    return true;
                }
            }
            return false;
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_Precept_HumansDisliked_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (!other.IsNotFlesh())
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
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn other)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (!other.IsNotFlesh())
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(1);
        }
    }

    public class ThoughtWorker_Precept_HumansAbhorrentSelf : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.BiotechActive || !ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (p == null || !p.IsNotFlesh())
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }

    [UsedImplicitly]
    public class ThoughtWorker_Precept_Humans : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (!ModsConfig.IdeologyActive)
            {
                return ThoughtState.Inactive;
            }
            if (p.IsNotFlesh())
            {
                return ThoughtState.Inactive;
            }
            Lord lord = p.GetLord();
            if (lord != null && lord.ownedPawns.Any((Pawn c) => Utilities.BioVsMechConflict(c.def, p.def)))
            {
                return true;
            }
            Caravan caravan = p.GetCaravan();
            if (caravan != null && caravan.PawnsListForReading.Any((Pawn c) => Utilities.BioVsMechConflict(c.def, p.def)))
            {
                return true;
            }
            Map mapHeld = p.MapHeld;
            if (mapHeld != null)
            {
                Faction faction = p.Faction;
                if (faction != null)
                {
                    if (mapHeld.mapPawns.SpawnedPawnsInFaction(faction).Any((Pawn c) => Utilities.BioVsMechConflict(c.def, p.def)))
                    {
                        return true;
                    }
                }
                else if (mapHeld.mapPawns.AllPawnsSpawned.Any((Pawn c) => Utilities.BioVsMechConflict(c.def, p.def) && !p.HostileTo(c)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}