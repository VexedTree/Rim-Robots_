using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;
using Verse.Sound;

namespace VexedThings
{
	[StaticConstructorOnStartup]
	public class Building_ChargePylon : Building_IndustrialChargePylon
	{

		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.GetInspectString());
			bool flag = !this.IsSociallyProper(null, false, false);
			if (flag)
			{
				stringBuilder.AppendLine("InPrisonCell".Translate());
			}
			return stringBuilder.ToString().Trim();
		}

		public Thing TryDispenseFoodOverride()
		{
			this.def.building.soundDispense.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
			Thing thing = ThingMaker.MakeThing(RR_DefOf.FuelNode_SynthDiet, null);
			CompIngredients compIngredients = thing.TryGetComp<CompIngredients>();
            _ = compIngredients != null;
            return thing;
		}
		public void TryDropFood(int amount)
		{
			bool flag = !this.powerComp.PowerOn || amount <= 0 || Find.TickManager.Paused;
			if (!flag)
			{
				Map map = base.Map;
				IntVec3 interactionCell = this.InteractionCell;
				Thing thing = ThingMaker.MakeThing(RR_DefOf.FuelNode_SynthDiet, null);
				GenPlace.TryPlaceThing(thing, interactionCell, map, ThingPlaceMode.Near, null, null, default(Rot4));
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			this.powerComp = base.GetComp<CompPowerTrader>();
		}
    }
}
