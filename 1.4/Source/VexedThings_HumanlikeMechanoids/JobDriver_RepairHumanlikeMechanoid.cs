using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace VexedThings
{
	public class JobDriver_RepairHumanlikeMechanoid : JobDriver
	{
		protected Pawn Patient
		{
			get
			{
				return (Pawn)job.GetTarget(TargetIndex.A).Thing;
			}
		}

		protected int TicksPerHeal
		{
			get
			{
				return Mathf.RoundToInt(1f / pawn.GetStatValue(RR_DefOf.TendSpeed_Synth, true, -1) * 120f);
			}
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(Patient, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnForbidden(TargetIndex.A);
			Toil gotoToil = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
			int ticks = (int)(1f / pawn.GetStatValue(StatDefOf.GeneralLaborSpeed, true, -1) * 600f);
			Toil tendToil = Toils_General.Wait(ticks, TargetIndex.None);
			tendToil.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f).PlaySustainerOrSound(SoundDefOf.RepairMech_Touch, 1f);
			if (pawn != Patient)
			{
				tendToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			}
			tendToil.activeSkill = (() => SkillDefOf.Crafting);
			tendToil.handlingFacing = true;
			tendToil.WithEffect(RR_DefOf.RepairingSynthetic, TargetIndex.A, null);
			tendToil.PlaySustainerOrSound(SoundDefOf.RepairMech_Touch, 1f);
			tendToil.tickAction = delegate ()
			{
				if (pawn != Patient)
				{
					pawn.rotationTracker.FaceTarget(Patient);
				}
			};
			Toil repairToil = Toils_General.WaitWith(TargetIndex.A, int.MaxValue, false, true, true, TargetIndex.None);
			repairToil.WithEffect(RR_DefOf.RepairingSynthetic, TargetIndex.A, null);
			repairToil.PlaySustainerOrSound(SoundDefOf.RepairMech_Touch, 1f);
			repairToil.AddPreInitAction(delegate
			{
				ticksToNextRepair = TicksPerHeal;
			});
			repairToil.handlingFacing = true;
			repairToil.tickAction = delegate ()
			{
				ticksToNextRepair--;
				if (ticksToNextRepair <= 0)
				{
					Patient.needs.food.CurLevel -= Patient.GetStatValue(RR_DefOf.PersonaEnergyLossPerHP, true, -1);
					RepairingCurrent(Patient);
					bool flag = Patient.needs.food != null;
					if (flag)
					{
						Patient.needs.food.CurLevel -= 0.01f;
					}
					ticksToNextRepair = TicksPerHeal;
				}
				pawn.rotationTracker.FaceTarget(Patient);
				if (pawn.skills != null)
				{
					pawn.skills.Learn(SkillDefOf.Crafting, 0.05f, false);
				}
			};
			if (pawn != Patient)
			{
				repairToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
			}
			repairToil.AddEndCondition(delegate
			{
				if (!CanRepair(Patient))
				{
					return JobCondition.Succeeded;
				}
				return JobCondition.Ongoing;
			});
			repairToil.activeSkill = () => SkillDefOf.Crafting;
			if (pawn != Patient)
			{
				yield return gotoToil;
			}
			yield return Toils_Jump.JumpIf(repairToil, () => !Patient.health.HasHediffsNeedingTend(false));
			yield return tendToil;
			yield return FinalizeRepair(Patient);
			yield return Toils_Jump.Jump(gotoToil);
			yield return repairToil;
			AddFinishAction(delegate
			{
				if (!job.draftedTend)
				{
					repairToil = Toils_General.Wait(ticks, TargetIndex.None);
				}
				else
				{
					repairToil = Toils_General.WaitWith(TargetIndex.A, ticks, false, true, false, TargetIndex.None);
					repairToil.AddFinishAction(delegate
					{
						if (Patient != null && Patient != pawn && Patient.CurJob != null && (Patient.CurJob.def == JobDefOf.Wait || Patient.CurJob.def == JobDefOf.Wait_MaintainPosture))
						{
							Patient.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
						}
					});
				}
			});
			yield break;
		}

		public static bool CanRepair(Pawn pawn)
		{
			return !pawn.InMentalState && !pawn.IsBurning() && !pawn.IsAttacking() && (pawn.health.HasHediffsNeedingTend(false) || pawn.needs.food.CurLevel <= 0f && GetHediffToHeal(pawn) != null);
		}

		public static Hediff GetHediffToHeal(Pawn persona)
		{
			Hediff hediff = null;
			float num = float.PositiveInfinity;
			foreach (Hediff hediff2 in persona.health.hediffSet.hediffs)
			{
				if (hediff2 is Hediff_Injury && !hediff2.IsPermanent() && hediff2.Severity < num)
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

		public static void RepairingCurrent(Pawn persona)
		{
			Hediff hediffToHeal = GetHediffToHeal(persona);
			if (hediffToHeal != null)
			{
				hediffToHeal.Heal(1f);
			}
		}

		public override void Notify_DamageTaken(DamageInfo dinfo)
		{
			base.Notify_DamageTaken(dinfo);
			if (dinfo.Def.ExternalViolenceFor(pawn) && pawn.Faction != Faction.OfPlayer && pawn == Patient)
			{
				pawn.jobs.CheckForJobOverride();
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref ticksToNextRepair, "ticksToNextRepair", 0, false);
		}
		protected int ticksToNextRepair;
	}
}