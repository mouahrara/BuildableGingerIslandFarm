using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace BuildableGingerIslandFarm.Patches
{
	internal class BuildingPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(Building), nameof(Building.draw), new Type[] { typeof(SpriteBatch) }),
				postfix: new HarmonyMethod(typeof(BuildingPatch), nameof(DrawPostfix))
			);
		}

		private static void DrawPostfix(Building __instance, SpriteBatch b)
		{
			if (__instance.buildingType.Value.Equals($"{ModEntry.ModManifest.UniqueID}_IslandFarmhouseMailbox"))
			{
				GameLocation parentLocation = __instance.GetParentLocation();

				if (parentLocation is IslandWest islandWest)
				{
					if (islandWest.farmhouseMailbox.Value && Game1.mailbox.Count > 0)
					{
						float offsetX = -8f;
						float offsetY = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
						float layerDepth = (__instance.tileX.Value + 1) * 64 / 10000f + __instance.tileY.Value * 64 / 10000f;

						b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.tileX.Value * 64 + offsetX, __instance.tileY.Value * 64 - 96 - 48 + offsetY)), new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth + 1E-06f);
						b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(__instance.tileX.Value * 64 + 32 + 4 + offsetX, __instance.tileY.Value * 64 - 64 - 24 - 8 + offsetY)), new Rectangle(189, 423, 15, 13), Color.White, 0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, layerDepth + 1E-05f);
					}
				}
			}
		}
	}
}
