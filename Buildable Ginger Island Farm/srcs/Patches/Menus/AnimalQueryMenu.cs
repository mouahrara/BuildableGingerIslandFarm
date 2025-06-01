using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
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
			if (Constants.TargetPlatform == GamePlatform.Android)
			{
				harmony.Patch(
					original: AccessTools.PropertyGetter(typeof(AnimalQueryMenu), "selectedBuildingIndex"),
					transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(GetLocationFromNameFarmTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.receiveLeftClick)),
					transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(ReceiveLeftClickTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.prepareForAnimalPlacement)),
					transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(GetLocationFromNameFarmTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(AnimalQueryMenu), "OnClickTickButton"),
					transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(GetFarmTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(AnimalQueryMenu), "OnClickMove"),
					prefix: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(OnClickMovePrefix))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(AnimalQueryMenu), "UnhighlightBuildings"),
					transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(GetLocationFromNameFarmTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.receiveGamePadButton)),
					transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(ReceiveGamePadButtonTranspiler))
				);
			}
			else
			{
				harmony.Patch(
					original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.receiveLeftClick), new Type[] { typeof(int), typeof(int), typeof(bool) }),
					prefix: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(ReceiveLeftClickPrefix)),
					transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(GetFarmTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.prepareForAnimalPlacement)),
					transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(GetFarmTranspiler))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.performHoverAction)),
					transpiler: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(GetFarmTranspiler))
				);
			}
		}

		private static IEnumerable<CodeInstruction> ReceiveLeftClickTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				List<CodeInstruction> list = GetLocationFromNameFarmTranspiler(instructions, original).ToList();
				bool hasSkippedFirstMatch = false;

				for (int i = 0; i < list.Count - 7; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Ldarg_0) && list[i + 1].opcode.Equals(OpCodes.Ldfld) && list[i + 1].operand.Equals(typeof(AnimalQueryMenu).GetField("_selectedBuilding", BindingFlags.NonPublic | BindingFlags.Instance)) && list[i + 2].opcode.Equals(OpCodes.Ldfld) && list[i + 2].operand.Equals(typeof(Building).GetField(nameof(Building.indoors), BindingFlags.Public | BindingFlags.Instance)) && list[i + 3].opcode.Equals(OpCodes.Callvirt) && list[i + 4].opcode.Equals(OpCodes.Isinst) && list[i + 4].operand.Equals(typeof(AnimalHouse)) && list[i + 5].opcode.Equals(OpCodes.Callvirt) && list[i + 5].operand.Equals(typeof(AnimalHouse).GetMethod(nameof(AnimalHouse.isFull), BindingFlags.Public | BindingFlags.Instance)) && list[i + 6].opcode.Equals(OpCodes.Brtrue_S))
					{
						if (hasSkippedFirstMatch)
						{
							CodeInstruction[] newInstructions = new CodeInstruction[]
							{
								new(OpCodes.Ldarg_0),
								new(OpCodes.Ldfld, typeof(AnimalQueryMenu).GetField("_selectedBuilding", BindingFlags.NonPublic | BindingFlags.Instance)),
								new(OpCodes.Ldarg_0),
								new(OpCodes.Ldfld, typeof(AnimalQueryMenu).GetField(nameof(AnimalQueryMenu.animal), BindingFlags.NonPublic | BindingFlags.Instance)),
								new(OpCodes.Callvirt, typeof(FarmAnimal).GetProperty(nameof(FarmAnimal.home), BindingFlags.Public | BindingFlags.Instance).GetGetMethod()),
								new(OpCodes.Callvirt, typeof(object).GetMethod(nameof(Equals), new[] { typeof(object) })),
								list[i + 6]
							};

							list.InsertRange(i + 7, newInstructions);
						}
						else
						{
							hasSkippedFirstMatch = true;
						}
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

		private static void OnClickMovePrefix(AnimalQueryMenu __instance)
		{
			TargetLocation = __instance.animal?.home?.GetParentLocation() ?? Game1.getFarm();
		}

		private static IEnumerable<CodeInstruction> ReceiveGamePadButtonTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				List<CodeInstruction> list = instructions.ToList();

				for (int i = 0; i < list.Count - 1; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Castclass) && list[i].operand.Equals(typeof(Farm)))
					{
						list[i + 1].labels.AddRange(list[i].labels);
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

		private static void ReceiveLeftClickPrefix(AnimalQueryMenu __instance, int x, int y)
		{
			if (Game1.globalFade || __instance.movingAnimal || __instance.confirmingSell)
				return;

			ClickableTextureComponent moveHomeButton = (ClickableTextureComponent)typeof(AnimalQueryMenu).GetField(nameof(__instance.moveHomeButton), BindingFlags.Public | BindingFlags.Instance).GetValue(__instance);

			if (moveHomeButton.containsPoint(x, y))
			{
				TargetLocation = __instance.animal?.home?.GetParentLocation() ?? Game1.getFarm();
			}
		}

		private static IEnumerable<CodeInstruction> GetFarmTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				List<CodeInstruction> list = instructions.ToList();

				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Call) && list[i].operand.Equals(typeof(Game1).GetMethod(nameof(Game1.getFarm))))
					{
						CodeInstruction[] replacementInstructions = new CodeInstruction[]
						{
							new(OpCodes.Call, typeof(AnimalQueryMenuPatch).GetMethod(nameof(GetTargetLocation), BindingFlags.NonPublic | BindingFlags.Static)) { labels = list[i].labels }
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
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(AnimalQueryMenu)}.{original.Name}: {e}", LogLevel.Error);
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
							new(OpCodes.Call, typeof(AnimalQueryMenuPatch).GetMethod(nameof(GetTargetLocation), BindingFlags.NonPublic | BindingFlags.Static)) { labels = list[i].labels }
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

		private static GameLocation GetTargetLocation()
		{
			return TargetLocation;
		}
	}
}
