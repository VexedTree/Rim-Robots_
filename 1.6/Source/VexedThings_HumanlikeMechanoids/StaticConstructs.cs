using RimWorld;
using System.Linq;
using Verse;
using System.Text;
using Verse.Sound;

namespace VexedThings
{
	[StaticConstructorOnStartup]
	public static class StartupEditors
	{
		// Allows / disallows traits on a pawn at generation.
		public static void TraitGateway(Pawn pawn)
		{
			HumanlikeMechanoidsExtension modExtension = pawn.def.GetModExtension<HumanlikeMechanoidsExtension>();
			if (modExtension != null && !modExtension.willSpawnWithTraits)
			{
				foreach (Trait trait in pawn.story.traits.allTraits.ToList<Trait>())
				{
					pawn.story.traits.RemoveTrait(trait, false);
				}
			}
		}
	}
}
