using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VexedThings
{
	public class JobDriver_RepairHumanlikeMechanoids : JobDriver
	{
		protected Pawn Deliveree
		{
			get
			{
				return job.targetA.Pawn;
			}
		}

		protected int TicksPerHeal
		{
			get
			{
				return Mathf.RoundToInt(1f / pawn.GetStatValue(RR_DefOf.TendSpeed_Synth, true, -1) * 120f);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref ticksToNextRepair, "ticksToNextRepair", 0, false);
		}

		public override void Notify_DamageTaken(DamageInfo dinfo)
		{
			base.Notify_DamageTaken(dinfo);
			if (dinfo.Def.ExternalViolenceFor(pawn) && pawn.Faction != Faction.OfPlayer && pawn == Deliveree)
			{
				pawn.jobs.CheckForJobOverride();
			}
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (Deliveree != pawn && !pawn.Reserve(Deliveree, job, 1, -1, null, errorOnFailed))
			{
				return false;
			}
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOn(() => pawn != null && pawn.IsHumanlikeMechanoid() && pawn.Faction == Faction.OfPlayer && Deliveree.playerSettings != null || (pawn == Deliveree && pawn.Faction == Faction.OfPlayer && pawn.playerSettings != null && !pawn.playerSettings.selfTend));
			AddEndCondition(delegate
			{
				if (pawn.Faction == Faction.OfPlayer && HealthAIUtility.ShouldBeTendedNowByPlayer(Deliveree))
				{
					return JobCondition.Ongoing;
				}
				if ((job.playerForced || pawn.Faction != Faction.OfPlayer) && Deliveree.health.HasHediffsNeedingTend(false))
				{
					return JobCondition.Ongoing;
				}
				return JobCondition.Succeeded;
			});
			this.FailOnAggroMentalState(TargetIndex.A);
			PathEndMode interactionCell = PathEndMode.None;
			if (Deliveree == pawn)
			{
				interactionCell = PathEndMode.OnCell;
			}
			else if (Deliveree.InBed())
			{
				interactionCell = PathEndMode.InteractionCell;
			}
			else if (Deliveree != pawn)
			{
				interactionCell = PathEndMode.ClosestTouch;
			}
			Toil gotoToil = Toils_Goto.GotoThing(TargetIndex.A, interactionCell);
			yield return gotoToil;
			int ticks = (int)(1f / pawn.GetStatValue(RR_DefOf.TendSpeed_Synth, true, -1) * 600f);
			Toil waitToil;
			if (!job.draftedTend)
			{
				waitToil = Toils_General.Wait(ticks, TargetIndex.None);
			}
			else
			{
				waitToil = Toils_General.WaitWith(TargetIndex.A, ticks, false, true, false, TargetIndex.None);
				waitToil.AddFinishAction(delegate
				{
					if (Deliveree != null && Deliveree != pawn && Deliveree.CurJob != null && (Deliveree.CurJob.def == JobDefOf.Wait || Deliveree.CurJob.def == JobDefOf.Wait_MaintainPosture))
					{
						Deliveree.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
					}
				});
			}
			waitToil.FailOnCannotTouch(TargetIndex.A, interactionCell).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f).WithEffect(RR_DefOf.RepairingSynthetic, TargetIndex.A, null).PlaySustainerOrSound(SoundDefOf.RepairMech_Touch, 1f);
			waitToil.activeSkill = (() => SkillDefOf.Crafting);
			waitToil.handlingFacing = true;
			waitToil.tickAction = delegate ()
			{
				ticksToNextRepair--;
				if (ticksToNextRepair <= 0)
				{
					Deliveree.needs.food.CurLevel -= Deliveree.GetStatValue(RR_DefOf.PersonaEnergyLossPerHP, true, -1);
					MechRepairUtility.RepairTick(Deliveree);
					ticksToNextRepair = TicksPerHeal;
				}
				if (pawn == Deliveree && pawn.Faction != Faction.OfPlayer && pawn.IsHashIntervalTick(100) && !pawn.Position.Fogged(pawn.Map))
				{
					FleckMaker.ThrowMetaIcon(pawn.Position, pawn.Map, RR_DefOf.RR_PersonaFleckRepair, 0.42f);
				}
				if (pawn != Deliveree)
				{
					pawn.rotationTracker.FaceTarget(Deliveree);
				}
			};
			yield return Toils_Jump.JumpIf(waitToil, () => !Deliveree.health.HasHediffsNeedingTend(false));
			yield return waitToil;
			yield return FinalizeRepair(Deliveree);
			yield return Toils_Jump.Jump(gotoToil);
			yield break;
		}

		public static Toil FinalizeRepair(Pawn pawn)
		{
			Toil toil = ToilMaker.MakeToil("FinalizeTend");
			toil.initAction = delegate ()
			{
				Pawn actor = toil.actor;
				if (actor.skills != null)
				{
					actor.skills.Learn(SkillDefOf.Crafting, 250f, false);
				}
				TendUtility.DoTend(actor, pawn, null);
				if (toil.actor.CurJob.endAfterTendedOnce)
				{
					actor.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}

		protected int ticksToNextRepair;
	}
}