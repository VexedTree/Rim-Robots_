using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace VexedThings
{
	public class WorkGiver_RepairPersona : WorkGiver_Scanner
	{
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.InteractionCell;
			}
		}

		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
			}
		}

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return WorkGiver_RepairPersona.HasJobOn(pawn, t, forced);
		}

		public static bool HasJobOn(Pawn pawn, Thing t, bool forced)
		{
			Pawn pawn2 = (Pawn)t;
			if (pawn2 == null || !pawn2.IsHumanlikeMechanoid())
			{
				return false;
			}
			if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Crafting))
			{
				return false;
			}
			if (!WorkGiver_RepairPersona.GoodLayingStatusForTend(pawn2, pawn, forced))
			{
				return false;
			}
			if (pawn != pawn2)
			{
				if (pawn2.CurJobDef == RR_DefOf.RR_RepairPersonae)
				{
					return false;
				}
				if (pawn2.HostileTo(pawn))
				{
					return false;
				}
				if (t.IsForbidden(pawn))
				{
					return false;
				}
				if (!pawn.CanReserveAndReach(t, PathEndMode.InteractionCell, Danger.Deadly, 1, -1, null, false))
				{
					return false;
				}
			}
			return JobDriver_RepairPersonae.CanRepairNow(pawn2);
		}


		public static bool GoodLayingStatusForTend(Pawn patient, Pawn doctor, bool forced)
		{
			if (patient == doctor)
			{
				return true;
			}
			if (patient.RaceProps.Humanlike)
			{
				return patient.InBed();
			}
			return patient.GetPosture() > PawnPosture.Standing;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(RR_DefOf.RR_RepairPersonae, t);
		}
	}
}
