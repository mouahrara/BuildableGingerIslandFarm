using System;
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
			harmony.Patch(
				original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.ApplyMapOverride), new Type[] { typeof(string), typeof(Microsoft.Xna.Framework.Rectangle?), typeof(Microsoft.Xna.Framework.Rectangle?) }),
				prefix: new HarmonyMethod(typeof(GameLocationPatch), nameof(ApplyMapOverridePrefix))
			);
		}

		private static void IsBuildableLocationPrefix(GameLocation __instance)
		{
			if (__instance.Name.Equals("IslandWest"))
			{
				GingerIslandFarmUtility.MakeBuildable();
			}
		}

		private static bool ApplyMapOverridePrefix(GameLocation __instance, string map_name)
		{
			if (__instance.Name.Equals("IslandWest"))
			{
				return map_name != "Island_House_Bin";
			}
			return true;
		}
	}
}
