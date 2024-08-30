using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class ConsoleCommandsUtility
	{
		internal static void Register()
		{
			ModEntry.Helper.ConsoleCommands.Add("bgif_build", "This command builds the specified building.\n\nUsage: bgif_build [name] [x] [y] [skipSafetyChecks = false]\nBuilds the specified building at the given coordinates. If the name includes spaces, quote them (e.g. \"Junimo Hut\").\n- name: The exact name of the building.\n- x: The x-coordinate where to build the building.\n- y: The y-coordinate where to build the building.\n- skipSafetyChecks: Whether to skip safety checks, allowing to build in a location that wouldn't normally allow buildings.", BGIF_build);
			ModEntry.Helper.ConsoleCommands.Add("bgif_movebuilding", "This command moves building from specified source coordinates to specified destination coordinates.\n\nUsage: bgif_movebuilding [sourceX] [sourceY] [destinationX] [destinationY]\nMoves building from specified source coordinates to specified destination coordinates. The destination coordinates are the upper-left corner of the building's footprint.\n- sourceX: The x-coordinate of the building to be moved.\n- sourceY: The y-coordinate of the building to be moved.\n- destinationX: The x-coordinate where to move the building.\n- destinationY: The y-coordinate where to move the building.", BGIF_movebuilding);
			ModEntry.Helper.ConsoleCommands.Add("bgif_removebuilding", "This command removes building at specified coordinates.\n\nUsage: bgif_removebuilding [x] [y]\nRemoves building at specified coordinates.\n- x: The x-coordinate of the building to be removed.\n- y: The y-coordinate of the building to be removed.", BGIF_removebuilding);
			ModEntry.Helper.ConsoleCommands.Add("bgif_removebuildings", "This command removes all buildings of the Ginger Island Farm.\n\nUsage: bgif_removebuildings\nRemoves all buildings of the Ginger Island Farm.", BGIF_removebuildings);
		}

		private static void BGIF_build(string command, string[] args)
		{
			if (args.Length < 3)
			{
				ModEntry.Monitor.Log("Invalid number of arguments. Expected mandatory fields [name, x, y] followed by an optional [skipSafetyChecks] field.", LogLevel.Error);
				return;
			}
			if (!ArgUtility.TryGet(args, 0, out string name, out string error) || !ArgUtility.TryGetVector2(args, 1, out Vector2 position, out error, true) || !ArgUtility.TryGetOptionalBool(args, 3, out bool skipSafetyChecks, out error, false))
			{
				ModEntry.Monitor.Log(error, LogLevel.Error);
				return;
			}

			GameLocation location = Game1.getLocationFromName("IslandWest");

			if (!Game1.buildingData.TryGetValue(name, out _))
			{
				ModEntry.Monitor.Log($"Can't construct building '{name}', no data found matching that ID.", LogLevel.Error);
				return;
			}
			if (location.buildStructure(name, position, Game1.player, out Building building, skipSafetyChecks: skipSafetyChecks))
			{
				building.daysOfConstructionLeft.Value = 0;
				ModEntry.Monitor.Log($"{name} has been built at position {position}.", LogLevel.Info);
			}
			else
			{
				ModEntry.Monitor.Log($"Can't construct '{name}', at position {position}.", LogLevel.Error);
			}
		}

		private static void BGIF_movebuilding(string command, string[] args)
		{
			if (args.Length < 4)
			{
				ModEntry.Monitor.Log("Invalid number of arguments. Expected mandatory fields [sourceX, sourceY, destinationX, destinationY].", LogLevel.Error);
				return;
			}
			if (!ArgUtility.TryGetVector2(args, 0, out Vector2 sourcePosition, out string error, true) || !ArgUtility.TryGetVector2(args, 2, out Vector2 destinationPosition, out error, true))
			{
				ModEntry.Monitor.Log(error, LogLevel.Error);
				return;
			}

			GameLocation location = Game1.getLocationFromName("IslandWest");
			Building building = location.getBuildingAt(sourcePosition);

			if (building is not null)
			{
				GameLocation indoors = building.GetIndoors();

				building.tileX.Value = (int)destinationPosition.X;
				building.tileY.Value = (int)destinationPosition.Y;
				if (indoors is not null)
				{
					building.updateInteriorWarps();
					if (indoors is AnimalHouse)
					{
						foreach (FarmAnimal animal in location.Animals.Values)
						{
							if (animal.home == building)
							{
								animal.warpHome();
							}
						}
					}
				}
				ModEntry.Monitor.Log($"{building.buildingType} has been moved to position {destinationPosition}.", LogLevel.Info);
			}
			else
			{
				ModEntry.Monitor.Log($"No building at position {sourcePosition}.", LogLevel.Error);
			}
		}

		private static void BGIF_removebuilding(string command, string[] args)
		{
			if (args.Length < 2)
			{
				ModEntry.Monitor.Log("Invalid number of arguments. Expected mandatory fields [x, y].", LogLevel.Error);
				return;
			}
			if (!ArgUtility.TryGetVector2(args, 0, out Vector2 position, out string error, true))
			{
				ModEntry.Monitor.Log(error, LogLevel.Error);
				return;
			}

			GameLocation location = Game1.getLocationFromName("IslandWest");
			Building building = location.getBuildingAt(position);

			if (building is not null)
			{
				GameLocation indoors = building.GetIndoors();

				if (indoors is AnimalHouse animalHouse)
				{
					foreach (FarmAnimal animal in indoors.Animals.Values)
					{
						if (animalHouse.animalsThatLiveHere.Contains(animal.myID.Value))
						{
							animalHouse.animalsThatLiveHere.Remove(animal.myID.Value);
							animal.health.Value = -1;
						}
					}
					foreach (FarmAnimal animal in location.Animals.Values)
					{
						if (animalHouse.animalsThatLiveHere.Contains(animal.myID.Value))
						{
							animalHouse.animalsThatLiveHere.Remove(animal.myID.Value);
							animal.health.Value = -1;
						}
					}
				}
				location.buildings.Remove(building);
				ModEntry.Monitor.Log($"{building.buildingType} has been removed.", LogLevel.Info);
			}
			else
			{
				ModEntry.Monitor.Log($"No building at position {position}.", LogLevel.Error);
			}
		}

		private static void BGIF_removebuildings(string command, string[] args)
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");
			int count = location.buildings.Count;

			for (int i = 0; i < location.buildings.Count; i++)
			{
				GameLocation indoors = location.buildings[i].GetIndoors();

				if (indoors is AnimalHouse animalHouse)
				{
					foreach (FarmAnimal animal in indoors.Animals.Values)
					{
						if (animalHouse.animalsThatLiveHere.Contains(animal.myID.Value))
						{
							animalHouse.animalsThatLiveHere.Remove(animal.myID.Value);
							animal.health.Value = -1;
						}
					}
					foreach (FarmAnimal animal in location.Animals.Values)
					{
						if (animalHouse.animalsThatLiveHere.Contains(animal.myID.Value))
						{
							animalHouse.animalsThatLiveHere.Remove(animal.myID.Value);
							animal.health.Value = -1;
						}
					}
				}
				location.buildings.RemoveAt(i--);
			}
			ModEntry.Monitor.Log($"{count} buildings have been removed.", LogLevel.Info);
		}
	}
}
