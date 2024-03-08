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
			return HasJobOn(pawn, t, forced);
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
				if (pawn2.needs.food.CurLevel <= 0f)
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
			return JobDriver_RepairHumanlikeMechanoid.CanRepair(pawn2);
		}

		public static bool GoodLayingStatusForTend(Pawn patient, Pawn doctor, bool forced)
		{
			if (patient == doctor)
			{
				return patient.playerSettings.selfTend && (forced || !patient.InBed());
			}
			return patient.InBed();
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn pawn2 = t as Pawn;
			return JobMaker.MakeJob(RR_DefOf.RR_RepairPersonae, pawn2);
		}
	}

	public class WorkGiver_RepairPersonaOther : WorkGiver_RepairPersona
	{
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return base.HasJobOnThing(pawn, t, forced) && pawn != t;
		}
	}

	public class WorkGiver_RepairPersonaOtherUrgent : WorkGiver_RepairPersonaOther
	{
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return base.HasJobOnThing(pawn, t, forced) && HealthAIUtility.ShouldBeTendedNowByPlayerUrgent((Pawn)t);
		}
	}

	public class WorkGiver_RepairPersonaSelf : WorkGiver_RepairPersona
	{
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			yield return pawn;
			yield break;
		}

		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Undefined);
			}
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			bool flag = pawn == t && pawn.playerSettings != null && base.HasJobOnThing(pawn, t, forced);
			if (flag && !pawn.playerSettings.selfTend)
			{
				JobFailReason.Is("RR_SelfRepairDisabled".Translate(), null);
			}
			return flag && pawn.playerSettings.selfTend;
		}
	}
	public class WorkGiver_RepairPersonaSelfEmergency : WorkGiver_RepairPersonaSelf
	{
		public override Job NonScanJob(Pawn pawn)
		{
			if (!HasJobOnThing(pawn, pawn, false) || !HealthAIUtility.ShouldBeTendedNowByPlayerUrgent(pawn))
			{
				return null;
			}
			ThinkResult thinkResult = jgp.TryIssueJobPackage(pawn, default);
			if (thinkResult.IsValid)
			{
				return thinkResult.Job;
			}
			return null;
		}

		private static JobGiver_RepairHumanlikeMechanoid jgp = new JobGiver_RepairHumanlikeMechanoid();
	}
}