using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace VexedThings
{
		public class WorkGiver_Repair : WorkGiver_Tend
		{
			public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
			{
                return t is Pawn pawn2 && pawn.def.HasModExtension<HumanlikeMechanoidsExtension>() && pawn.def.GetModExtension<HumanlikeMechanoidsExtension>().pawnRequiresRepairs && (!this.def.tendToHumanlikesOnly || pawn2.RaceProps.Humanlike) && (!this.def.tendToAnimalsOnly || pawn2.RaceProps.Animal) && WorkGiver_Tend.GoodLayingStatusForTend(pawn2, pawn) && HealthAIUtility.ShouldBeTendedNowByPlayer(pawn2) && pawn.CanReserve(pawn2, 1, -1, null, forced) && (!pawn2.InAggroMentalState || pawn2.health.hediffSet.HasHediff(HediffDefOf.Scaria, false));
            }

			public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
			{
				Pawn pawn2 = t as Pawn;
				Thing thing = HealthAIUtility.FindBestMedicine(pawn, pawn2, false);
				if (thing != null)
				{
					return JobMaker.MakeJob(RR_DefOf.RR_RepairPersonae, pawn2, thing);
				}
				return JobMaker.MakeJob(RR_DefOf.RR_RepairPersonae, pawn2);
			}
		}

		public class WorkGiver_RepairOther : WorkGiver_Repair
		{
			public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
			{
				return base.HasJobOnThing(pawn, t, forced) && pawn != t;
			}
		}

		public class WorkGiver_RepairOtherUrgent : WorkGiver_Tend
		{
			public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
			{
				return base.HasJobOnThing(pawn, t, forced) && HealthAIUtility.ShouldBeTendedNowByPlayerUrgent((Pawn)t);
			}
		}

		public class WorkGiver_RepairSelf : WorkGiver_Repair
		{
			public override ThingRequest PotentialWorkThingRequest
			{
				get
				{
					return ThingRequest.ForGroup(ThingRequestGroup.Undefined);
				}
			}
			public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
			{
				yield return pawn;
				yield break;
			}
			public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
			{
				bool flag = pawn == t && pawn.playerSettings != null && base.HasJobOnThing(pawn, t, forced);
				if (flag && !pawn.playerSettings.selfTend)
				{
					JobFailReason.Is("RR.SelfRepairDisabled".Translate(), null);
				}
				return flag && pawn.playerSettings.selfTend;
			}
		}

		public class WorkGiver_RepairSelfEmergency : WorkGiver_RepairOther
		{
			public override Job NonScanJob(Pawn pawn)
			{
				if (!this.HasJobOnThing(pawn, pawn, false) || !HealthAIUtility.ShouldBeTendedNowByPlayerUrgent(pawn))
				{
					return null;
				}
				ThinkResult thinkResult = jobpack.TryIssueJobPackage(pawn, default);
				if (thinkResult.IsValid)
				{
					return thinkResult.Job;
				}
				return null;
			}
			private static readonly JobGiver_RepairSelf jobpack = new JobGiver_RepairSelf();
		}
	}
