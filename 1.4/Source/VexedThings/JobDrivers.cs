using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace VexedThings
{
	public class JobDriver_RepairPersonae : JobDriver
	{
		protected Pawn Persona
		{
			get
			{
				return (Pawn)this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		protected int TicksPerHeal
		{
			get
			{
				return Mathf.RoundToInt(1f / this.pawn.GetStatValue(RR_DefOf.TendSpeed_Synth, true, -1) * 120f);
			}
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.Persona, this.job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnForbidden(TargetIndex.A);
			this.FailOn(() => this.Persona.IsAttacking());
			Toil gotoToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			int ticks = (int)(1f / this.pawn.GetStatValue(RR_DefOf.TendSpeed_Synth, true, -1) * 600f);
			Toil tendToil = Toils_General.Wait(ticks, TargetIndex.None);
			tendToil.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f).PlaySustainerOrSound(SoundDefOf.RepairMech_Touch, 1f);
			if (this.pawn != this.Persona)
			{
				tendToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			}
			tendToil.activeSkill = (() => SkillDefOf.Crafting);
			tendToil.handlingFacing = true;
			tendToil.WithEffect(RR_DefOf.RepairingSynthetic, TargetIndex.A, null);
			tendToil.PlaySustainerOrSound(SoundDefOf.RepairMech_Touch, 1f);
			tendToil.tickAction = delegate ()
			{
				if (this.pawn != this.Persona)
				{
					this.pawn.rotationTracker.FaceTarget(this.Persona);
				}
			};
			Toil repairToil = Toils_General.Wait(int.MaxValue, TargetIndex.None);
			repairToil.WithEffect(RR_DefOf.RepairingSynthetic, TargetIndex.A, null);
			repairToil.PlaySustainerOrSound(SoundDefOf.RepairMech_Touch, 1f);
			repairToil.AddPreInitAction(delegate
			{
				this.ticksToNextRepair = this.TicksPerHeal;
			});
			repairToil.handlingFacing = true;
			repairToil.tickAction = delegate ()
			{
				this.ticksToNextRepair--;
				if (this.ticksToNextRepair <= 0)
				{
					this.Persona.needs.food.CurLevel -= this.Persona.GetStatValue(RR_DefOf.PersonaEnergyLossPerHP, true, -1);
					JobDriver_RepairPersonae.RepairingCurrent(this.Persona);
					this.ticksToNextRepair = this.TicksPerHeal;
				}
				this.pawn.rotationTracker.FaceTarget(this.Persona);
				if (this.pawn.skills != null)
				{
					this.pawn.skills.Learn(SkillDefOf.Crafting, 0.05f, false);
				}
			};
			if (this.pawn != this.Persona)
			{
				repairToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			}
			repairToil.AddEndCondition(delegate
			{
				if (!JobDriver_RepairPersonae.CanRepairNow(this.Persona))
				{
					return JobCondition.Succeeded;
				}
				return JobCondition.Ongoing;
			});
			repairToil.activeSkill = (() => SkillDefOf.Crafting);
			if (this.pawn != this.Persona)
			{
				yield return gotoToil;
			}
			yield return Toils_Jump.JumpIf(repairToil, () => !this.Persona.health.HasHediffsNeedingTend(false));
			yield return tendToil;
			yield return JobDriver_RepairPersonae.FinalizeRepair(this.Persona);
			yield return Toils_Jump.Jump(gotoToil);
			yield return repairToil;
			base.AddFinishAction(delegate
			{
				if (this.Persona != null && this.Persona != this.pawn && this.Persona.CurJob != null && (this.Persona.CurJob.def == JobDefOf.Wait || this.Persona.CurJob.def == JobDefOf.Wait_MaintainPosture))
				{
					this.Persona.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
				}
			});
			yield break;
		}

		public static bool CanRepairNow(Pawn pawn)
		{
			return !pawn.InMentalState && !pawn.IsBurning() && !pawn.IsAttacking() && (pawn.health.HasHediffsNeedingTend(false) || JobDriver_RepairPersonae.GetHediffToHeal(pawn) != null);
		}

		public static Toil FinalizeRepair(Pawn persona)
		{
			Toil toil = ToilMaker.MakeToil("FinalizeTend");
			toil.initAction = delegate ()
			{
				Pawn actor = toil.actor;
				if (actor.skills != null)
				{
					actor.skills.Learn(SkillDefOf.Crafting, 250f, false);
				}
				TendUtility.DoTend(actor, persona, null);
				if (toil.actor.CurJob.endAfterTendedOnce)
				{
					actor.jobs.EndCurrentJob(JobCondition.Succeeded, true, true);
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}

		public static Hediff GetHediffToHeal(Pawn persona)
		{
			Hediff hediff = null;
			float num = float.PositiveInfinity;
			foreach (Hediff hediff2 in persona.health.hediffSet.hediffs)
			{
				if (hediff2 is Hediff_Injury && hediff2.Severity < num && !hediff2.IsPermanent())
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

		public static void RepairingCurrent(Pawn persona)
		{
			Hediff hediffToHeal = JobDriver_RepairPersonae.GetHediffToHeal(persona);
			if (hediffToHeal != null)
			{
				hediffToHeal.Heal(1f);
			}
		}

		public override void Notify_DamageTaken(DamageInfo dinfo)
		{
			base.Notify_DamageTaken(dinfo);
			if (dinfo.Def.ExternalViolenceFor(this.pawn) && this.pawn.Faction != Faction.OfPlayer && this.pawn == this.Persona)
			{
				this.pawn.jobs.CheckForJobOverride();
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.ticksToNextRepair, "ticksToNextRepair", 0, false);
		}
		protected int ticksToNextRepair;
	}
}