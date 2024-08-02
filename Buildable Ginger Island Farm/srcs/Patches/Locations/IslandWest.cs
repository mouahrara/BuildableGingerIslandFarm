using HarmonyLib;
using StardewValley.Locations;
using BuildableGingerIslandFarm.Utilities;

namespace BuildableGingerIslandFarm.Patches
{
	internal class IslandWestPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.ApplyFarmHouseRestore)),
				postfix: new HarmonyMethod(typeof(IslandWestPatch), nameof(ApplyFarmHouseRestorePostfix))
			);
		}

		private static void ApplyFarmHouseRestorePostfix()
		{
			GingerIslandFarmUtility.RemoveShippingBin();
		}
	}
}
