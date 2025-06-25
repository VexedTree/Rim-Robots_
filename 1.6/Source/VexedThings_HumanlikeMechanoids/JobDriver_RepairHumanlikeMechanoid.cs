using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace VexedThings
{
	public class JobDriver_RepairHumanlikeMechanoid : JobDriver
	{
		protected Pawn Persona => (Pawn)job.GetTarget(TargetIndex.A).Thing;

		protected int TicksPerHeal => Mathf.RoundToInt(1f / pawn.GetStatValue(RR_DefOf.TendSpeed_Synth, true, -1) * 120f);
		protected int ticksToNextRepair;


		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (pawn.jobs.jobsGivenThisTick > 8)
			{
				pawn.jobs.debugLog = true;
			}
			return pawn.Reserve(Persona, job, 1, -1, null, errorOnFailed);
		}

		public override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnForbidden(TargetIndex.A);
			Toil gotoToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			int ticks = (int)(1f / pawn.GetStatValue(RR_DefOf.TendSpeed_Synth) * 600f);
			Toil tendToil = Toils_General.Wait(ticks);
			tendToil.WithProgressBarToilDelay(TargetIndex.A).PlaySustainerOrSound(SoundDefOf.RepairMech_Touch);
			if (pawn != Persona)
			{
				tendToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			}
			tendToil.activeSkill = () => SkillDefOf.Crafting;
			tendToil.handlingFacing = true;
			tendToil.WithEffect(RR_DefOf.RepairingSynthetic, TargetIndex.A);
			tendToil.PlaySustainerOrSound(SoundDefOf.RepairMech_Touch);
			tendToil.tickAction = delegate
			{
				if (pawn != Persona)
				{
					pawn.rotationTracker.FaceTarget(Persona);
				}
			};
			Toil repairToil = Toils_General.Wait(int.MaxValue);
			repairToil.WithEffect(RR_DefOf.RepairingSynthetic, TargetIndex.A);
			repairToil.PlaySustainerOrSound(SoundDefOf.RepairMech_Touch);
			repairToil.AddPreInitAction(delegate
			{
				ticksToNextRepair = TicksPerHeal;
			});
			repairToil.handlingFacing = true;
			repairToil.tickAction = delegate
			{
				ticksToNextRepair--;
				if (ticksToNextRepair <= 0)
				{
					Persona.needs.food.CurLevel -= Persona.GetStatValue(RR_DefOf.PersonaEnergyLossPerHP, true, -1);
					RepairTick(Persona);
					ticksToNextRepair = TicksPerHeal;
				}
				pawn.rotationTracker.FaceTarget(Persona);
				if (pawn.skills != null)
				{
					pawn.skills.Learn(SkillDefOf.Crafting, 0.05f);
				}
			};
			if (pawn != Persona)
			{
				repairToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			}
			repairToil.AddEndCondition(() => CanRepair(Persona) ? JobCondition.Ongoing : JobCondition.Succeeded);
			repairToil.activeSkill = () => SkillDefOf.Crafting;
			if (pawn != Persona)
			{
				yield return gotoToil;
			}
			yield return Toils_Jump.JumpIf(repairToil, () => !Persona.health.HasHediffsNeedingTend());
			yield return tendToil;
			yield return FinalizeRepair(Persona);
			yield return Toils_Jump.Jump(gotoToil);
			yield return repairToil;
			AddFinishAction(delegate
			{
				if (Persona != null && Persona != pawn && Persona.CurJob != null && (Persona.CurJob.def == JobDefOf.Wait || Persona.CurJob.def == JobDefOf.Wait_MaintainPosture))
				{
					Persona.jobs.EndCurrentJob(JobCondition.InterruptForced);
				}
			});
		}

		public static Toil FinalizeRepair(Pawn patient)
		{
			Toil toil = ToilMaker.MakeToil("FinalizeTend");
			toil.initAction = delegate
			{
				Pawn actor = toil.actor;
				if (actor.skills != null)
				{
					actor.skills.Learn(SkillDefOf.Crafting, 250f);
				}
				TendUtility.DoTend(actor, patient, null);
				if (toil.actor.CurJob.endAfterTendedOnce)
				{
					actor.jobs.EndCurrentJob(JobCondition.Succeeded);
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}

		public override void Notify_DamageTaken(DamageInfo dinfo)
		{
			base.Notify_DamageTaken(dinfo);
			if (dinfo.Def.ExternalViolenceFor(pawn) && pawn.Faction != Faction.OfPlayer && pawn == Persona)
			{
				pawn.jobs.CheckForJobOverride();
			}
		}

		public static bool CanRepair(Pawn pawn)
		{
			return !pawn.InMentalState && pawn.needs.food.CurLevel > 0f && GetHediffToRepair(pawn) != null;
		}

		public static Hediff GetHediffToRepair(Pawn pawn)
		{
			Hediff hediff = null;
			float num = float.PositiveInfinity;
			foreach (Hediff hediff2 in pawn.health.hediffSet.hediffs)
			{
				if (hediff2 is Hediff_Injury && hediff2.Severity < num)
				{
					num = hediff2.Severity;
					hediff = hediff2;
				}
			}
			if (hediff != null)
			{
				return hediff;
			}
			return null;
		}

		public static void RepairTick(Pawn pawn)
		{
			GetHediffToRepair(pawn)?.Heal(1f);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToNextRepair, "ticksToNextRepair", 0);
		}
	}
}