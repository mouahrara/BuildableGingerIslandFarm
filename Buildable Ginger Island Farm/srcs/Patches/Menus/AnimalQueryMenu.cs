using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace BuildableGingerIslandFarm.Patches
{
	internal class AnimalQueryMenuPatch
	{
		private static readonly PerScreen<GameLocation> targetLocation = new();

		public static GameLocation TargetLocation
		{
			get => targetLocation.Value;
			set => targetLocation.Value = value;
		}

		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.receiveLeftClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
				prefix: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(ReceiveLeftClickPrefix)),
				transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(Transpiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.prepareForAnimalPlacement)),
				transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(Transpiler))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.performHoverAction)),
				transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(Transpiler))
			);
		}

		private static bool ReceiveLeftClickPrefix(AnimalQueryMenu __instance, int x, int y)
		{
			if (Game1.globalFade || __instance.movingAnimal || __instance.confirmingSell)
				return true;

			if (__instance.moveHomeButton.containsPoint(x, y))
			{
				TargetLocation = __instance.animal?.home?.GetParentLocation() ?? Game1.getFarm();
			}
			return true;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				foreach (CodeInstruction instruction in instructions)
				{
					if (instruction.opcode.Equals(OpCodes.Call) && instruction.operand.Equals(typeof(Game1).GetMethod(nameof(Game1.getFarm))))
					{
						instruction.operand = typeof(AnimalQueryMenuPatch).GetMethod(nameof(GetTargetLocation), BindingFlags.NonPublic | BindingFlags.Static);
					}
				}
				return instructions;
			}
			catch (Exception e)
			{
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(AnimalQueryMenu)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}

		private static GameLocation GetTargetLocation()
		{
			return TargetLocation;
		}
	}
}
