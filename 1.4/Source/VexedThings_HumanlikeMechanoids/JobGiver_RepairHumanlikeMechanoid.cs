using Verse;
using Verse.AI;
using RimWorld;

namespace VexedThings
{
	public class JobGiver_RepairHumanlikeMechanoid : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (!pawn.IsHumanlikeMechanoid() || !pawn.health.HasHediffsNeedingTend(false) || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || pawn.InAggroMentalState || pawn.needs.food == null)
			{
				return null;
			}
			if (pawn.IsColonist && pawn.WorkTypeIsDisabled(WorkTypeDefOf.Crafting))
			{
				return null;
			}
			Job job = JobMaker.MakeJob(RR_DefOf.RR_RepairPersonae, pawn);
			job.endAfterTendedOnce = true;
			return job;
		}
	}
}