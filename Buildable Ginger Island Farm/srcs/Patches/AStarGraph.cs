using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace BuildableGingerIslandFarm.Patches
{
	internal class AStarGraphPatch
	{
		private static readonly Type AStarGraphType = Type.GetType("StardewValley.Mobile.AStarGraph, StardewValley");
		private static readonly Type AStarNodeType = Type.GetType("StardewValley.Mobile.AStarNode, StardewValley");
		private static readonly MethodInfo XGetter = AStarNodeType.GetProperty("x", BindingFlags.Public | BindingFlags.Instance).GetGetMethod();
		private static readonly MethodInfo YGetter = AStarNodeType.GetProperty("y", BindingFlags.Public | BindingFlags.Instance).GetGetMethod();

		internal static void Apply(Harmony harmony)
		{
			if (Constants.TargetPlatform == GamePlatform.Android)
			{
				harmony.Patch(
					original: AccessTools.Method(AStarGraphType, "GetShortestPathAStar", new Type[] { AStarNodeType, AStarNodeType }),
					transpiler: new HarmonyMethod(typeof(AStarGraphPatch), nameof(GetShortestPathAStarTranspiler))
				);
			}
		}

		private static IEnumerable<CodeInstruction> GetShortestPathAStarTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				List<CodeInstruction> list = instructions.ToList();

				for (int i = 0; i < list.Count - 4; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Ldloc_1) && list[i + 1].opcode.Equals(OpCodes.Ldloc_S) && list[i + 2].opcode.Equals(OpCodes.Callvirt) && list[i + 2].operand.Equals(typeof(HashSet<>).MakeGenericType(AStarNodeType).GetMethod("Contains", BindingFlags.Public | BindingFlags.Instance)) && list[i + 3].opcode.Equals(OpCodes.Brtrue_S))
					{
						CodeInstruction[] newInstructions = new CodeInstruction[]
						{
							list[i + 1],
							new(OpCodes.Ldloc_3),
							new(OpCodes.Call, typeof(AStarGraphPatch).GetMethod(nameof(IsBlockingTile), BindingFlags.NonPublic | BindingFlags.Static, new Type[] { AStarNodeType, AStarNodeType })),
							list[i + 3]
						};

						list.InsertRange(i + 4, newInstructions);
					}
				}
				return list;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(JunimoHut)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}

		private static bool IsBlockingTile(object neighbouringNode, object fromNode)
		{
			if (Game1.currentLocation is not IslandWest islandWest)
				return false;

			int x = (int)XGetter.Invoke(neighbouringNode, null);
			int y = (int)YGetter.Invoke(neighbouringNode, null);
			int fromX = (int)XGetter.Invoke(fromNode, null);
			int fromY = (int)YGetter.Invoke(fromNode, null);

			static bool IsHorizontalTransitionBetween(int x1, int x2, int a, int b)
			{
				return (x1 == a && x2 == b) || (x1 == b && x2 == a);
			}

			static bool AreBothAtY(int y1, int y2, int value)
			{
				return y1 == value && y2 == value;
			}

			if (!Game1.player.hasOrWillReceiveMail("Island_UpgradeHouse"))
			{
				if ((IsHorizontalTransitionBetween(fromX, x, 73, 74) || IsHorizontalTransitionBetween(fromX, x, 80, 81)) && AreBothAtY(fromY, y, 40))
				{
					return true;
				}
			}
			else
			{
				Building islandFarmhouse = islandWest.getBuildingByType($"{ModEntry.ModManifest.UniqueID}_IslandFarmhouse");

				if (islandFarmhouse is not null && (IsHorizontalTransitionBetween(fromX, x, islandFarmhouse.tileX.Value - 1, islandFarmhouse.tileX.Value) || IsHorizontalTransitionBetween(fromX, x, islandFarmhouse.tileX.Value + islandFarmhouse.tilesWide.Value - 1, islandFarmhouse.tileX.Value + islandFarmhouse.tilesWide.Value)) && AreBothAtY(fromY, y, islandFarmhouse.tileY.Value + 3))
				{
					return true;
				}
			}
			return false;
		}
	}
}
