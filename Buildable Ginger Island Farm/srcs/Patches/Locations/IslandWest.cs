using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using xTile.Dimensions;
using StardewValley;
using StardewValley.Locations;
using BuildableGingerIslandFarm.Utilities;

namespace BuildableGingerIslandFarm.Patches
{
	internal class IslandWestPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.leftClick), new Type[] { typeof(int), typeof(int), typeof(Farmer) }),
				prefix: new HarmonyMethod(typeof(IslandWestPatch), nameof(ShippingBinActionPrefix)),
				postfix: new HarmonyMethod(typeof(IslandWestPatch), nameof(ShippingBinActionPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.checkAction), new Type[] { typeof(Location), typeof(xTile.Dimensions.Rectangle), typeof(Farmer) }),
				prefix: new HarmonyMethod(typeof(IslandWestPatch), nameof(ShippingBinActionPrefix)),
				postfix: new HarmonyMethod(typeof(IslandWestPatch), nameof(ShippingBinActionPostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.ApplyFarmHouseRestore)),
				prefix: new HarmonyMethod(typeof(IslandWestPatch), nameof(ApplyFarmHouseRestorePrefix))
			);
		}

		private static void ShippingBinActionPrefix(IslandWest __instance)
		{
			__instance.shippingBinPosition = new Point(int.MinValue, int.MinValue);
		}

		private static void ShippingBinActionPostfix(IslandWest __instance)
		{
			__instance.shippingBinPosition = new Point(0, 1);
		}

		private static void ApplyFarmHouseRestorePrefix(IslandWest __instance)
		{
			GingerIslandFarmUtility.RemoveShippingBin(__instance);
		}
	}
}
