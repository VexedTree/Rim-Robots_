using System.Collections.Generic;
using RimWorld;
using Verse;

namespace VexedThings
{
	internal class CompProps_SynthMaker : CompProperties
	{
		public CompProps_SynthMaker()
		{
			compClass = typeof(Comp_SynthMaker);
		}

		public List<PawnKindDef> pawnKinds;
	}

	internal class Comp_SynthMaker : ThingComp
	{
		private CompProps_SynthMaker Props
		{
			get
			{
				return (CompProps_SynthMaker)props;
			}
		}
		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			if (pawnKind == null)
			{
				pawnKind = Props.pawnKinds.RandomElement();
			}
		}
		public override void CompTick()
		{
			{
				Spawn();
			}
		}
		public void Spawn()
		{
			Faction faction = FactionUtility.DefaultFactionFrom(pawnKind.defaultFactionType);
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKind, faction, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, false));
			Pawn_ApparelTracker apparel = pawn.apparel;
			if (apparel != null)
			{
				apparel.DestroyAll(DestroyMode.Vanish);
			}
			GenSpawn.Spawn(pawn, parent.Position, parent.Map, WipeMode.Vanish);
			parent.Destroy(DestroyMode.Vanish);
		}
		private PawnKindDef pawnKind;
	}
}