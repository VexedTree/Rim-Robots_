using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace VexedThings
{
	public class Building_IndustrialChargePylon : Building
	{
		public bool CanDispenseNow
		{
			get
			{
				return this.powerComp.PowerOn;
			}
		}

		public virtual ThingDef DispensableDef
		{
			get
			{
				return RR_DefOf.FuelNode_SynthDiet;
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			this.powerComp = base.GetComp<CompPowerTrader>();
		}
		public virtual Thing TryDispenseFood()
		{
			if (!this.CanDispenseNow)
			{
				return null;
			}
			Log.Error("Could not produce battery to pawn.");
			return null;
		}
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			IEnumerator<Gizmo> enumerator = null;
			yield return BuildCopyCommandUtility.FindAllowedDesignator(ThingDefOf.Hopper, true);
			yield break;
			yield break;
		}
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.GetInspectString());
			if (!this.IsSociallyProper(null, false, false))
			{
				stringBuilder.AppendLine("InPrisonCell".Translate());
			}
			return stringBuilder.ToString().Trim();
		}

		public CompPowerTrader powerComp;

		public static int CollectDuration = 50;
	}
}
