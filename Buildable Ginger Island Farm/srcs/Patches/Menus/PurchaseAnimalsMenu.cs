using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace BuildableGingerIslandFarm.Patches
{
	internal class PurchaseAnimalsMenuPatch
	{
		internal static void Apply(Harmony harmony)
		{
			if (Constants.TargetPlatform == GamePlatform.Android)
			{
				harmony.Patch(
					original: AccessTools.PropertyGetter(typeof(PurchaseAnimalsMenu), "selectedBuildingIndex"),
					transpiler: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(GetLocationFromNameFarmTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.setUpForAnimalPlacement)),
					transpiler: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(SetUpForAnimalPlacementTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveGamePadButton)),
					transpiler: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(ReceiveGamePadButtonTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(PurchaseAnimalsMenu), "UnhighlightBuildings"),
					transpiler: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(GetLocationFromNameFarmTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.releaseLeftClick)),
					transpiler: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(GetLocationFromNameFarmTranspiler))
				);
			}
		}

		private static IEnumerable<CodeInstruction> SetUpForAnimalPlacementTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				List<CodeInstruction> list = instructions.ToList();

				for (int i = 0; i < list.Count - 4; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Ldarg_0) && list[i + 1].opcode.Equals(OpCodes.Ldfld) && list[i + 1].operand.Equals(typeof(PurchaseAnimalsMenu).GetField(nameof(PurchaseAnimalsMenu.TargetLocation), BindingFlags.Public | BindingFlags.Instance)) && list[i + 2].opcode.Equals(OpCodes.Isinst) && list[i + 2].operand.Equals(typeof(Farm)) && list[i + 3].opcode.Equals(OpCodes.Brtrue_S))
					{
						list[i + 4].labels.AddRange(list[i].labels);
						list.RemoveRange(i, 4);
					}
				}
				return list;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(PurchaseAnimalsMenu)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}

		private static IEnumerable<CodeInstruction> ReceiveGamePadButtonTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				List<CodeInstruction> list = instructions.ToList();
				object localIndex = null;

				for (int i = 0; i < list.Count - 5; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Call) && list[i].operand.Equals(typeof(Game1).GetProperty(nameof(Game1.currentLocation), BindingFlags.Public | BindingFlags.Static).GetGetMethod()) && list[i + 1].opcode.Equals(OpCodes.Isinst) && list[i + 1].operand.Equals(typeof(Farm)) && list[i + 2].opcode.Equals(OpCodes.Stloc_S) && list[i + 3].opcode.Equals(OpCodes.Ldloc_S) && list[i + 4].opcode.Equals(OpCodes.Brfalse))
					{
						localIndex = list[i + 2].operand;
						list[i + 5].labels.AddRange(list[i].labels);
						list.RemoveRange(i, 5);
					}
					if (list[i].opcode.Equals(OpCodes.Ldloc_S) && list[i].operand.Equals(localIndex))
					{
						CodeInstruction[] replacementInstructions = new CodeInstruction[]
						{
							new(OpCodes.Ldarg_0) { labels = list[i].labels },
							new(OpCodes.Ldfld, typeof(PurchaseAnimalsMenu).GetField(nameof(PurchaseAnimalsMenu.TargetLocation), BindingFlags.Public | BindingFlags.Instance))
						};

						list.InsertRange(i, replacementInstructions);
						i += replacementInstructions.Length;
						list.RemoveAt(i);
					}
				}
				return list;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(PurchaseAnimalsMenu)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}

		private static IEnumerable<CodeInstruction> GetLocationFromNameFarmTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				List<CodeInstruction> list = instructions.ToList();

				for (int i = 0; i < list.Count - 2; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Ldstr) && list[i].operand.Equals("Farm") && list[i + 1].opcode.Equals(OpCodes.Call) && list[i + 1].operand.Equals(typeof(Game1).GetMethod(nameof(Game1.getLocationFromName), BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(string) } )))
					{
						CodeInstruction[] replacementInstructions = new CodeInstruction[]
						{
							new(OpCodes.Ldarg_0) { labels = list[i].labels },
							new(OpCodes.Ldfld, typeof(PurchaseAnimalsMenu).GetField(nameof(PurchaseAnimalsMenu.TargetLocation), BindingFlags.Public | BindingFlags.Instance))
						};
						bool isinst = list[i + 2].opcode.Equals(OpCodes.Isinst) && list[i + 2].operand.Equals(typeof(Farm));

						list.InsertRange(i, replacementInstructions);
						i += replacementInstructions.Length;
						list.RemoveRange(i, isinst ? 3 : 2);
					}
				}
				return list;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(AnimalQueryMenu)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}
	}
}
