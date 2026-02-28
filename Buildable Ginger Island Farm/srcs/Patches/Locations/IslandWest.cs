using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using BuildableGingerIslandFarm.Utilities;

namespace BuildableGingerIslandFarm.Patches
{
	internal class IslandWestPatch
	{
		private static readonly FieldInfo AppliedMapOverridesField = typeof(GameLocation).GetField("_appliedMapOverrides", BindingFlags.NonPublic | BindingFlags.Instance);

		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.leftClick), new Type[] { typeof(int), typeof(int), typeof(Farmer) }),
				transpiler: new HarmonyMethod(typeof(IslandWestPatch), nameof(ShippingBinActionTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.checkAction), new Type[] { typeof(xTile.Dimensions.Location), typeof(xTile.Dimensions.Rectangle), typeof(Farmer) }),
				transpiler: new HarmonyMethod(typeof(IslandWestPatch), nameof(ShippingBinActionTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.ApplyFarmHouseRestore)),
				prefix: new HarmonyMethod(typeof(IslandWestPatch), nameof(ApplyFarmHouseRestorePrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.ApplyFarmObeliskBuild)),
				prefix: new HarmonyMethod(typeof(IslandWestPatch), nameof(ApplyFarmObeliskBuildPrefix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.draw), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(IslandWestPatch), nameof(DrawTranspiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandWest), nameof(IslandWest.UpdateWhenCurrentLocation), new Type[] { typeof(GameTime) }),
				transpiler: new HarmonyMethod(typeof(IslandWestPatch), nameof(UpdateWhenCurrentLocationTranspiler))
			);
		}

		private static IEnumerable<CodeInstruction> ShippingBinActionTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			return StateTranspiler(instructions, original, "farmhouseRestored");
		}

		private static bool ApplyFarmHouseRestorePrefix(IslandWest __instance)
		{
			if (__instance.map is not null)
			{
				HashSet<string> appliedMapOverrides = (HashSet<string>)AppliedMapOverridesField.GetValue(__instance);

				if (!appliedMapOverrides.Contains("Island_House_Cave"))
				{
					__instance.ApplyMapOverride("Island_House_Cave", null, new Rectangle(95, 30, 3, 4));
				}
			}
			GingerIslandFarmUtility.ApplyFarmHouseRestore(__instance);
			return false;
		}

		private static bool ApplyFarmObeliskBuildPrefix(IslandWest __instance)
		{
			GingerIslandFarmUtility.ApplyFarmObeliskBuild(__instance);
			return false;
		}

		private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			return FarmhouseMailboxTranspiler(FarmhouseRestoredTranspiler(instructions, original), original);
		}

		private static IEnumerable<CodeInstruction> UpdateWhenCurrentLocationTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			return FarmhouseRestoredTranspiler(instructions, original);
		}

		private static IEnumerable<CodeInstruction> FarmhouseRestoredTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			return StateTranspiler(instructions, original, "farmhouseRestored");
		}

		private static IEnumerable<CodeInstruction> FarmhouseMailboxTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			return StateTranspiler(instructions, original, "farmhouseMailbox");
		}

		private static IEnumerable<CodeInstruction> StateTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original, string field)
		{
			try
			{
				List<CodeInstruction> list = instructions.ToList();

				for (int i = 0; i < list.Count - 3; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Ldarg_0) && list[i + 1].opcode.Equals(OpCodes.Ldfld) && list[i + 1].operand.Equals(typeof(IslandWest).GetField(field, BindingFlags.Public | BindingFlags.Instance)) && list[i + 2].opcode.Equals(OpCodes.Callvirt) && list[i + 2].operand.Equals(typeof(Netcode.NetFieldBase<bool, Netcode.NetBool>).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance).GetGetMethod()))
					{
						list.Insert(i, new(OpCodes.Ldc_I4_0) { labels = list[i].labels });
						i++;
						list.RemoveRange(i, 3);
					}
				}
				return list;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(IslandWest)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}
	}
}
