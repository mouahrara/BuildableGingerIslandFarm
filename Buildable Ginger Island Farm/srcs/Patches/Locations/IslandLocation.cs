using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace BuildableGingerIslandFarm.Patches
{
	internal class IslandLocationPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandLocation), nameof(IslandLocation.isCollidingPosition), new Type[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool), typeof(bool) }),
				postfix: new HarmonyMethod(typeof(IslandLocationPatch), nameof(IsCollidingPositionPostfix))
			);
		}

		private static void IsCollidingPositionPostfix(IslandLocation __instance, Rectangle position, ref bool __result)
		{
			if (__instance is not IslandWest islandWest || __result)
				return;

			if (!Game1.player.hasOrWillReceiveMail("Island_UpgradeHouse"))
			{
				if (position.Intersects(new Rectangle(74 * Game1.tileSize, 40 * Game1.tileSize, 0, Game1.tileSize)) || position.Intersects(new Rectangle(81 * Game1.tileSize, 40 * Game1.tileSize, 0, Game1.tileSize)))
				{
					__result = true;
				}
			}
			else
			{
				Building islandFarmhouse = islandWest.getBuildingByType($"{ModEntry.ModManifest.UniqueID}_IslandFarmhouse");

				if (islandFarmhouse is not null && (position.Intersects(new Rectangle(islandFarmhouse.tileX.Value * Game1.tileSize, (islandFarmhouse.tileY.Value + 3) * Game1.tileSize, 0, Game1.tileSize)) || position.Intersects(new Rectangle((islandFarmhouse.tileX.Value + islandFarmhouse.tilesWide.Value) * Game1.tileSize, (islandFarmhouse.tileY.Value + 3) * Game1.tileSize, 0, Game1.tileSize))))
				{
					__result = true;
				}
			}
		}
	}
}
