using System;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace BuildableGingerIslandFarm.Patches
{
	internal class CarpenterMenuPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.CanDemolishThis), new Type[] { typeof(Building) }),
				postfix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(CanDemolishThisPostfix))
			);
		}

		private static void CanDemolishThisPostfix(CarpenterMenu __instance, Building building, ref bool __result)
		{
			string text = building?.buildingType.Value;

			switch (text)
			{
				case "Shipping Bin":
					if (__instance.TargetLocation.Name.Equals("IslandWest") && !__instance.TargetLocation.HasMinBuildings(text, 2))
					{
						__result = false;
					}
					break;
			}
		}
	}
}
