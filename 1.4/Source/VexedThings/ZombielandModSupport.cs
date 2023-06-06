using Verse;

public class ZombielandSupport
{
	public static bool CanBecomeZombie(Pawn pawn)
	{
		return pawn.def.defName != "BaseSyntheticRace";
	}

	public static bool AttractsZombies(Pawn pawn)
	{
		return pawn.IsSlave == false;
	}
}
