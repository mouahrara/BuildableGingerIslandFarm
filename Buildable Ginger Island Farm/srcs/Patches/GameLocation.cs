using System;
using HarmonyLib;
using StardewValley;
using StardewValley.Buildings;
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
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.OnBuildingMoved), new Type[] { typeof(Building) }),
				postfix: new HarmonyMethod(typeof(GameLocationPatch), nameof(OnBuildingMovedPostfix))
			);
		}

		private static void IsBuildableLocationPrefix(GameLocation __instance)
		{
			if (__instance.Name.Equals("IslandWest"))
			{
				GingerIslandFarmUtility.MakeBuildable();
			}
		}

		private static void OnBuildingMovedPostfix(Building building)
		{
			building.updateInteriorWarps();
		}
	}
}
