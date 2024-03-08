using RimWorld;
using System.Collections.Generic;
using RimWorld.Planet;
using Verse;

namespace VexedThings
{
	public class Alert_ChargePylonStopped : Alert
	{
		public Alert_ChargePylonStopped()
		{
			if (ModsConfig.BiotechActive)
			{
				defaultLabel = "RR.AlertChargePylonStopped".Translate(RR_DefOf.IndustrialChargePylon.LabelCap);
			}
		}

		private void GetTargets()
		{
			targets.Clear();
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				foreach (Building building in maps[i].listerBuildings.AllBuildingsColonistOfDef(RR_DefOf.IndustrialChargePylon))
				{
					if (!building.TryGetComp<CompToxifier>().CanPolluteNow)
					{
						targets.Add(building);
					}
				}
			}
		}

		public override AlertReport GetReport()
		{
			if (!ModsConfig.BiotechActive)
			{
				return false;
			}
			GetTargets();
			return AlertReport.CulpritsAre(targets);
		}

		public override TaggedString GetExplanation()
		{
			return "RR.AlertChargePylonStoppedDesc".Translate(RR_DefOf.IndustrialChargePylon.label);
		}

		private List<GlobalTargetInfo> targets = new List<GlobalTargetInfo>();
	}
}
