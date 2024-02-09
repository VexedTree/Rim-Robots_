using System.Collections.Generic;

using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;
using System;

namespace VexedThings
{
	public class HealthCardUtility_ReversePatch
	{
		[HarmonyReversePatch]
		[HarmonyPatch(typeof(HealthCardUtility), "DrawOverviewTab")]
		internal static float MyDrawLeftRow(Rect leftRect, float curY, string v, string first, Color second, TipSignal tipSignal)
		{
			throw new NotImplementedException();
		}
	}

	internal class HealthCardUtility_HarmonyPatch
	{
		[HarmonyPatch(typeof(HealthCardUtility), "DrawOverviewTab")]
		public static class DrawOverviewTab_HarmonyPatch
		{
			[HarmonyPrefix]
			public static bool Prefix(ref float __result, Rect ___leftRect, Pawn pawn, float curY)
			{
				if (pawn.IsHumanlikeMechanoid())
				{
					__result = DrawOverviewTab(___leftRect, pawn, curY);
					return false;
				}
				return true;
			}

			private static float DrawOverviewTab(Rect ___leftRect, Pawn pawn, float curY)
			{
				curY += 4f;
				Text.Font = GameFont.Tiny;
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = new Color(0.9f, 0.9f, 0.9f);
				string str = "RR.PersonaPawnHCSummary".Translate(pawn.Named("PAWN"));
				Widgets.Label(new Rect(0f, curY, ___leftRect.width, 34f), str.CapitalizeFirst());
				GUI.color = Color.white;
				curY += 34f;
				if (pawn.IsColonist && !pawn.Dead)
				{
					bool selfTend = pawn.playerSettings.selfTend;
					Rect rect = new Rect(0f, curY, ___leftRect.width, 24f);
					Widgets.CheckboxLabeled(rect, "RR.SelfRepair".Translate(), ref pawn.playerSettings.selfTend, false, null, null, false);
					if (pawn.playerSettings.selfTend && !selfTend)
					{
						if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Crafting))
						{
							pawn.playerSettings.selfTend = false;
							Messages.Message("RR.MustBeCapableToRepair".Translate(pawn.LabelShort, pawn), MessageTypeDefOf.RejectInput, false);
						}
						else if (pawn.workSettings.GetPriority(WorkTypeDefOf.Crafting) == 0)
						{
							Messages.Message("RR.MustBeAssignedToRepair".Translate(pawn.LabelShort, pawn), MessageTypeDefOf.CautionInput, false);
						}
					}
				}
				Text.Font = GameFont.Small;
				if (!pawn.Dead)
				{
					IEnumerable<PawnCapacityDef> source;
					if (!pawn.def.race.Humanlike)
					{
						if (pawn.def.race.Animal)
						{
							source = from x in DefDatabase<PawnCapacityDef>.AllDefs
									 where x.showOnAnimals
									 select x;
						}
						else
						{
							source = from x in DefDatabase<PawnCapacityDef>.AllDefs
									 where x.showOnMechanoids
									 select x;
						}
					}
					else
					{
						source = from x in DefDatabase<PawnCapacityDef>.AllDefs
								 where x.showOnHumanlikes
								 select x;
					}
					foreach (PawnCapacityDef pawnCapacityDef in from act in source
																orderby act.listOrder
																select act)
					{
						if (PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, pawnCapacityDef))
						{
							PawnCapacityDef activityLocal = pawnCapacityDef;
							Pair<string, Color> efficiencyLabel = HealthCardUtility.GetEfficiencyLabel(pawn, pawnCapacityDef);
							string textGetter()
							{
								if (pawn.Dead)
								{
									return "";
								}
								return HealthCardUtility.GetPawnCapacityTip(pawn, activityLocal);
							}
							curY = HealthCardUtility_ReversePatch.MyDrawLeftRow(___leftRect, curY, pawnCapacityDef.GetLabelFor(false, false).CapitalizeFirst(), efficiencyLabel.First, efficiencyLabel.Second, new TipSignal(textGetter, pawn.thingIDNumber ^ (int)pawnCapacityDef.index));
						}
					}
					return curY;
				}
				return curY;
			}
		}
	}
}