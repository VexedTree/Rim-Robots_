﻿using System.Collections.Generic;
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
			Pawn persona = (Pawn)t;
			if (persona == null || !persona.IsHumanlikeMechanoid())
			{
				return false;
			}
			if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Crafting))
			{
				return false;
			}
			if (!GoodLayingStatusForRepair(persona, pawn, forced))
			{
				return false;
			}
			if (pawn != persona)
			{
				if (persona.CurJobDef == RR_DefOf.RR_RepairPersonae)
				{
					return false;
				}
				if (persona.HostileTo(pawn))
				{
					return false;
				}
				if (persona.IsBurning())
				{
					return false;
				}
				if (persona.IsAttacking())
				{
					return false;
				}
				if (persona.needs.food.CurLevel <= 0f)
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
			return JobDriver_RepairHumanlikeMechanoid.CanRepair(persona);
		}

		public static bool GoodLayingStatusForRepair(Pawn persona, Pawn doctor, bool forced)
		{
			if (persona == doctor)
			{
				return persona.playerSettings.selfTend && (forced || !persona.InBed());
			}
			return persona.InBed();
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(RR_DefOf.RR_RepairPersonae, t);
		}
	}
}