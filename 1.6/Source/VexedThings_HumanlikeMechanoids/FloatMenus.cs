using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;

namespace VexedThings
{
	internal class Drug_FloatMenus
	{
		public static void AddPersonaOrders(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
		{
			foreach (Thing t2 in IntVec3.FromVector3(clickPos).GetThingList(pawn.Map))
			{
				Thing t = t2;
				if (t.def.ingestible != null && pawn.IsHumanlikeMechanoid() && t.def.ingestible.showIngestFloatOption && pawn.RaceProps.CanEverEat(t) && t.IngestibleNow)
				{
					Pawn_NeedsTracker needs = pawn.needs;
					if ((needs?.food) != null && t != null && pawn != null || t.def.IsDrug)
					{
						string text;
						if (t.def.ingestible.ingestCommandString.NullOrEmpty())
						{
							text = "ConsumeThing".Translate(t.LabelShort, t);
						}
						else
						{
							text = t.def.ingestible.ingestCommandString.Formatted(t.LabelShort);
						}
						if (!t.IsSociallyProper(pawn))
						{
							text = text + ": " + "ReservedForPrisoners".Translate().CapitalizeFirst();
						}
						FloatMenuOption floatMenuOption;
						if (!pawn.CanReach(t, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn))
						{
							floatMenuOption = new FloatMenuOption(text + ": " + "NoPath".Translate().CapitalizeFirst(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
						}
						else if (!t.def.IsDrug && !pawn.WillEat(t, null, true, false))
						{
							floatMenuOption = new FloatMenuOption(text + ": " + "RR_UnsuitableFuel".Translate().CapitalizeFirst(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
						}
						else if (t.def.IsDrug && pawn.IsHumanlikeMechanoid() && !pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().whiteListedDrugs.Contains(t.def))
						{
							floatMenuOption = new FloatMenuOption(text + ": " + "RR_UnsuitableIngestDrug_Personae".Translate().CapitalizeFirst(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
						}
						else if (t.def.IsDrug && pawn != null && !pawn.IsHumanlikeMechanoid() && t.def.GetModExtension<VexedThingsItemExtension>().isIngestibleByPersonaeOnly)
						{
							floatMenuOption = new FloatMenuOption(text + ": " + "RR_UnsuitableIngestDrug_Humans".Translate().CapitalizeFirst(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
						}
						else
						{
							MenuOptionPriority priority = (t is Corpse) ? MenuOptionPriority.Low : MenuOptionPriority.Default;
							bool maxAmountToPickup = FoodUtility.GetMaxAmountToPickup(t, pawn, FoodUtility.WillIngestStackCountOf(pawn, t.def, FoodUtility.NutritionForEater(pawn, t))) != 0;
							floatMenuOption = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text, delegate ()
							{
								int maxAmountToPickup2 = FoodUtility.GetMaxAmountToPickup(t, pawn, FoodUtility.WillIngestStackCountOf(pawn, t.def, FoodUtility.NutritionForEater(pawn, t)));
								if (maxAmountToPickup2 == 0)
								{
									return;
								}
								t.SetForbidden(false, true);
								Job job = JobMaker.MakeJob(JobDefOf.Ingest, t);
								job.count = maxAmountToPickup2;
								pawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
							}, priority, null, null, 0f, null, null, true, 0), pawn, t, "ReservedBy", null);
							if (!maxAmountToPickup)
							{
								floatMenuOption.action = null;
							}
						}
						opts.Add(floatMenuOption);
					}
				}
			}
		}
	}
}
