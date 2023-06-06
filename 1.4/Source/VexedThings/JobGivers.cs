using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace VexedThings
{
	public class JobGiver_RepairSelf : ThinkNode_JobGiver
		{
			protected override Job TryGiveJob(Pawn pawn)
			{
				if (!pawn.RaceProps.Humanlike || !pawn.health.HasHediffsNeedingTend(false) || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || pawn.InAggroMentalState)
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

	public class JobGiver_RepairPersonae : JobDriver_TendPatient
		{
			protected override IEnumerable<Toil> MakeNewToils()
			{
				this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
				this.FailOn(() => (this.MedicineUsed != null && this.pawn.Faction == Faction.OfPlayer && this.Deliveree.playerSettings != null && !this.Deliveree.playerSettings.medCare.AllowsMedicine(this.MedicineUsed.def)) || (this.pawn == this.Deliveree && this.pawn.Faction == Faction.OfPlayer && this.pawn.playerSettings != null && !this.pawn.playerSettings.selfTend));
				base.AddEndCondition(delegate
				{
					if (this.pawn.Faction == Faction.OfPlayer && HealthAIUtility.ShouldBeTendedNowByPlayer(this.Deliveree))
					{
						return JobCondition.Ongoing;
					}
					if ((!this.job.playerForced && this.pawn.Faction == Faction.OfPlayer) || !this.Deliveree.health.HasHediffsNeedingTend(false))
					{
						return JobCondition.Succeeded;
					}
					return JobCondition.Ongoing;
				});
				this.FailOnAggroMentalState(TargetIndex.A);
				PathEndMode interactionCell = PathEndMode.None;
				if (base.Deliveree == this.pawn)
				{
					interactionCell = PathEndMode.OnCell;
				}
				else if (base.Deliveree.InBed())
				{
					interactionCell = PathEndMode.InteractionCell;
				}
				else if (base.Deliveree != this.pawn)
				{
					interactionCell = PathEndMode.ClosestTouch;
				}
				Toil gotoToil = Toils_Goto.GotoThing(TargetIndex.A, interactionCell);
				yield return gotoToil;
				int ticks = (int)(1f / this.pawn.GetStatValue(RR_DefOf.TendSpeed_Synth, true, -1) * 600f);
				Toil waitToil;
				if (!this.job.draftedTend)
				{
					waitToil = Toils_General.Wait(ticks, TargetIndex.None);
				}
				else
				{
					waitToil = Toils_General.WaitWith(TargetIndex.A, ticks, false, true, false, TargetIndex.None);
					waitToil.AddFinishAction(delegate
					{
						if (this.Deliveree != null && this.Deliveree != this.pawn && this.Deliveree.CurJob != null && (this.Deliveree.CurJob.def == JobDefOf.Wait || this.Deliveree.CurJob.def == JobDefOf.Wait_MaintainPosture))
						{
							this.Deliveree.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
						}
					});
			}
			waitToil.FailOnCannotTouch(TargetIndex.A, interactionCell).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f).PlaySustainerOrSound(SoundDefOf.Recipe_ButcherCorpseMechanoid, 1f);
			waitToil.activeSkill = (() => SkillDefOf.Medicine);
			waitToil.handlingFacing = true;
			waitToil.tickAction = delegate ()
			{
				if (this.pawn == this.Deliveree && this.pawn.Faction != Faction.OfPlayer && this.pawn.IsHashIntervalTick(100) && !this.pawn.Position.Fogged(this.pawn.Map))
				{
					FleckMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, RR_DefOf.RepairingCog, 0.42f);
				}
				if (this.pawn != this.Deliveree)
				{
					this.pawn.rotationTracker.FaceTarget(this.Deliveree);
				}
			};
			yield return waitToil;
			yield return Toils_Tend.FinalizeTend(base.Deliveree);
			yield return Toils_Jump.Jump(gotoToil);
			yield break;
		}
	}
}
