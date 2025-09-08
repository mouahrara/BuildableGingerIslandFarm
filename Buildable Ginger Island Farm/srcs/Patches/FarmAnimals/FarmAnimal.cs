using System;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace BuildableGingerIslandFarm.Patches
{
	internal class FarmAnimalPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.updateWhenNotCurrentLocation), new Type[] { typeof(Building), typeof(GameTime), typeof(GameLocation) }),
				prefix: new HarmonyMethod(typeof(FarmAnimalPatch), nameof(UpdateWhenNotCurrentLocationPrefix))
			);
		}

		private static void UpdateWhenNotCurrentLocationPrefix(FarmAnimal __instance, GameTime time, GameLocation environment)
		{
			if (Game1.shouldTimePass() && Game1.IsMasterGame)
			{
				FarmAnimal followTarget = (FarmAnimal)typeof(FarmAnimal).GetField("_followTarget", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

				if (__instance.uniqueFrameAccumulator != -1 && followTarget is not null && !FarmAnimal.GetFollowRange(followTarget, 1).Contains(__instance.StandingPixel))
				{
					__instance.uniqueFrameAccumulator = -1;
				}
				if (__instance.uniqueFrameAccumulator != -1)
				{
					__instance.uniqueFrameAccumulator += time.ElapsedGameTime.Milliseconds;
					if (__instance.uniqueFrameAccumulator > 500)
					{
						__instance.uniqueFrameAccumulator = 0;
						if (Game1.random.NextDouble() < 0.4)
						{
							__instance.uniqueFrameAccumulator = -1;
						}
					}
					if (__instance.IsActuallySwimming())
					{
						__instance.MovePosition(time, Game1.viewport, environment);
					}
				}
				else
				{
					__instance.MovePosition(time, Game1.viewport, environment);
				}
			}
		}
	}
}
