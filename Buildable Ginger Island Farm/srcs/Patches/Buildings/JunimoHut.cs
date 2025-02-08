using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace BuildableGingerIslandFarm.Patches
{
	internal class JunimoHutPatch
	{
		internal static void Apply(Harmony harmony)
		{
			harmony.Patch(
				original: AccessTools.Method(typeof(JunimoHut), nameof(JunimoHut.dayUpdate), new Type[] { typeof(int) }),
				transpiler: new HarmonyMethod(typeof(JunimoHutPatch), nameof(Transpiler)),
				postfix: new HarmonyMethod(typeof(JunimoHutPatch), nameof(DayUpdatePostfix))
			);
			harmony.Patch(
				original: AccessTools.Method(typeof(JunimoHut), nameof(JunimoHut.draw), new Type[] { typeof(SpriteBatch) }),
				transpiler: new HarmonyMethod(typeof(JunimoHutPatch), nameof(Transpiler))
			);
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
		{
			try
			{
				List<CodeInstruction> list = instructions.ToList();

				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].opcode.Equals(OpCodes.Call) && list[i].operand.Equals(typeof(Game1).GetProperty(nameof(Game1.IsWinter)).GetGetMethod()))
					{
						CodeInstruction[] replacementInstructions = new CodeInstruction[]
						{
							new(OpCodes.Ldarg_0) { labels = list[i].labels },
							new(OpCodes.Callvirt, typeof(Building).GetMethod(nameof(Building.GetParentLocation))),
							new(OpCodes.Callvirt, typeof(GameLocation).GetMethod(nameof(GameLocation.IsWinterHere)))
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
				ModEntry.Monitor.Log($"There was an issue modifying the instructions for {typeof(JunimoHut)}.{original.Name}: {e}", LogLevel.Error);
				return instructions;
			}
		}

		private static void DayUpdatePostfix(JunimoHut __instance)
		{
			__instance.shouldSendOutJunimos.Value = true;
		}
	}
}
