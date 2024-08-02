using HarmonyLib;
using StardewValley;
using BuildableGingerIslandFarm.Utilities;

namespace BuildableGingerIslandFarm.Patches
{
	internal class GameLocationPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.IsBuildableLocation)),
				prefix: new HarmonyMethod(typeof(GameLocationPatch), nameof(IsBuildableLocationPrefix))
			);
		}

		private static void IsBuildableLocationPrefix(GameLocation __instance)
		{
			if (__instance.Name.Equals("IslandWest"))
			{
				GingerIslandFarmUtility.MakeBuildable();
			}
		}
	}
}
