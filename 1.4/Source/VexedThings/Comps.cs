using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace VexedThings
{

    public class CompProps_SynthMaker : CompProperties
    {
        public CompProps_SynthMaker()
        {
            this.compClass = typeof(Comp_SynthMaker);
        }
        public PawnKindDef pawnKind;
    }

    public class Comp_SynthMaker : ThingComp
    {
        public CompProps_SynthMaker Spawnprops
        {
            get
            {
                return this.props as CompProps_SynthMaker;
            }
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            this.SpawnPawn();
            this.parent.Destroy(DestroyMode.Vanish);
        }
        public void SpawnPawn()
        {
            PawnKindDef pawnKind = this.Spawnprops.pawnKind;
            Faction ofPlayer = Faction.OfPlayer;
            PawnGenerationContext context = PawnGenerationContext.NonPlayer;
            int tile = -1;
            bool forceGenerateNewPawn = true;
            bool allowDead = false;
            bool allowDowned = false;
            bool canGeneratePawnRelations = false;
            bool mustBeCapableOfViolence = false;
            float colonistRelationChanceFactor = 1f;
            bool forceAddFreeWarmLayerIfNeeded = false;
            bool allowGay = true;
            bool allowPregnant = false;
            bool allowFood = false;
            bool allowAddictions = false;
            bool inhabitant = false;
            bool certainlyBeenInCryptosleep = false;
            bool forceRedressWorldPawnIfFormerColonist = false;
            bool worldPawnFactionDoesntMatter = false;
            float biocodeWeaponChance = 0f;
            float biocodeApparelChance = 0f;
            Pawn extraPawnForExtraRelationChance = null;
            float relationWithExtraPawnChanceFactor = 1f;
            Predicate<Pawn> validatorPreGear = null;
            Predicate<Pawn> validatorPostGear = null;
            IEnumerable<TraitDef> forcedTraits = null;
            IEnumerable<TraitDef> prohibitedTraits = null;
            float? num = new float?(0f);
            float? num2 = new float?(0f);
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKind, ofPlayer, context, tile, forceGenerateNewPawn, allowDead, allowDowned, canGeneratePawnRelations, mustBeCapableOfViolence, colonistRelationChanceFactor, forceAddFreeWarmLayerIfNeeded, allowGay, allowPregnant, allowFood, allowAddictions, inhabitant, certainlyBeenInCryptosleep, forceRedressWorldPawnIfFormerColonist, worldPawnFactionDoesntMatter, biocodeWeaponChance, biocodeApparelChance, extraPawnForExtraRelationChance, relationWithExtraPawnChanceFactor, validatorPreGear, validatorPostGear, forcedTraits, prohibitedTraits, null, num, num2, null, null, null, null, null, true, false, false, false, null, null, null, null, null, 1f, DevelopmentalStage.Adult, null, null, null, false));
            Pawn_ApparelTracker apparel = pawn.apparel;
            if (apparel != null)
            {
                apparel.DestroyAll(DestroyMode.Vanish);
            }
            GenSpawn.Spawn(pawn, this.parent.Position, this.parent.Map, WipeMode.Vanish);
        }
    }
}
