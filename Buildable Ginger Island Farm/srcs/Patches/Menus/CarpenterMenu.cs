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
	internal class CarpenterMenuPatch
	{
		internal static void Apply(Harmony harmony)
		{
			if (Constants.TargetPlatform == GamePlatform.Android)
			{
				harmony.Patch(
					original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.setUpForBuildingPlacement)),
					transpiler: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(SetUpForBuildingPlacementTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.receiveLeftClick)),
					transpiler: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ReceiveLeftClickTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(CarpenterMenu), "OnClickOK"),
					transpiler: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(GetLocationFromNameFarmTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(CarpenterMenu), "OnReleaseCancelButton"),
					transpiler: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(GetLocationFromNameFarmTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.returnToCarpentryMenu)),
					transpiler: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(GetLocationFromNameFarmTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(CarpenterMenu), "resetBounds"),
					transpiler: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(ResetBoundsTranspiler))
				);
			}
		}

		private static IEnumerable<CodeInstruction> SetUpForBuildingPlacementTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				List<CodeInstruction> list = GetLocationFromNameFarmTranspiler(instructions, original).ToList();

				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Ldstr) && list[i].operand.Equals("Farm"))
					{
						CodeInstruction[] replacementInstructions = new CodeInstruction[]
						{
							new(OpCodes.Ldarg_0) { labels = list[i].labels },
							new(OpCodes.Ldfld, typeof(CarpenterMenu).GetField(nameof(CarpenterMenu.TargetLocation), BindingFlags.Public | BindingFlags.Instance)),
							new(OpCodes.Call, typeof(GameLocation).GetProperty(nameof(GameLocation.NameOrUniqueName), BindingFlags.Public | BindingFlags.Instance).GetGetMethod())
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
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(CarpenterMenu)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}

		private static IEnumerable<CodeInstruction> ReceiveLeftClickTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original, ILGenerator iLGenerator)
		{
			try
			{
				List<CodeInstruction> list = GetLocationFromNameFarmTranspiler(instructions, original).ToList();

				for (int i = 0; i < list.Count - 5; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Ldarg_0) && list[i + 1].opcode.Equals(OpCodes.Ldfld) && list[i + 1].operand.Equals(typeof(CarpenterMenu).GetField(nameof(CarpenterMenu.appearanceButton), BindingFlags.Public | BindingFlags.Instance)) && list[i + 5].opcode.Equals(OpCodes.Brfalse_S))
					{
						CodeInstruction[] replacementInstructions = new CodeInstruction[]
						{
							new(list[i].opcode) { labels = list[i].labels },
							list[i + 1],
							list[i + 5],
							new(list[i].opcode, list[i].operand),
							list[i + 1],
						};

						list.InsertRange(i, replacementInstructions);
						i += replacementInstructions.Length;
						list.RemoveRange(i, 2);
					}
					if (list[i].opcode.Equals(OpCodes.Ldloc_3) && list[i + 1].opcode.Equals(OpCodes.Callvirt) && list[i + 1].operand.Equals(typeof(Farm).GetMethod(nameof(Farm.GetMainFarmHouse), BindingFlags.Public | BindingFlags.Instance)))
					{
						Label label = iLGenerator.DefineLabel();
						CodeInstruction[] replacementInstructions = new CodeInstruction[]
						{
							new(OpCodes.Ldloc_3) { labels = list[i].labels },
							new(OpCodes.Isinst, typeof(Farm)),
							new(OpCodes.Stloc_S, (sbyte)4),
							new(OpCodes.Ldloc_S, (sbyte)4),
							new(OpCodes.Brtrue_S, label),
							new(OpCodes.Ret),
							new(OpCodes.Ldloc_S, (sbyte)4) { labels = { label } },
							list[i + 1]
						};

						list.InsertRange(i, replacementInstructions);
						i += replacementInstructions.Length;
						list.RemoveRange(i, 2);
					}
				}
				return list;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(CarpenterMenu)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}

		private static IEnumerable<CodeInstruction> ResetBoundsTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				List<CodeInstruction> list = instructions.ToList();

				for (int i = 0; i < list.Count - 1; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Call) && list[i].operand.Equals(typeof(Game1).GetMethod(nameof(Game1.getFarm), BindingFlags.Public | BindingFlags.Static)))
					{
						CodeInstruction[] replacementInstructions = new CodeInstruction[]
						{
							new(OpCodes.Ldarg_0) { labels = list[i].labels },
							new(OpCodes.Ldfld, typeof(CarpenterMenu).GetField(nameof(CarpenterMenu.TargetLocation), BindingFlags.Public | BindingFlags.Instance))
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
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(CarpenterMenu)}.{original.Name}: {e}", LogLevel.Error);
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
							new(OpCodes.Ldfld, typeof(CarpenterMenu).GetField(nameof(CarpenterMenu.TargetLocation), BindingFlags.Public | BindingFlags.Instance))
						};
						bool castclass = list[i + 2].opcode.Equals(OpCodes.Castclass) && list[i + 2].operand.Equals(typeof(Farm));

						list.InsertRange(i, replacementInstructions);
						i += replacementInstructions.Length;
						list.RemoveRange(i, castclass ? 3 : 2);
					}
				}
				return list;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(CarpenterMenu)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}
	}
}
