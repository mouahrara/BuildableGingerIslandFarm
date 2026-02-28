using System.Collections.Generic;
using Microsoft.Xna.Framework;
using xTile.Layers;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.GameData.Locations;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class GingerIslandFarmUtility
	{
		public static void MakeAlwaysActive(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
			{
				e.Edit(asset =>
				{
					asset.AsDictionary<string, LocationData>().Data["IslandWest"].CreateOnLoad.AlwaysActive = true;
					MakeBuildable();
				});
			}
		}

		public static void MakeBuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");

			if (location is not null)
			{
				if (!location.HasMapPropertyWithValue("CanBuildHere") && Game1.player.hasOrWillReceiveMail("Island_UpgradeHouse"))
				{
					location.Map.Properties.Add("CanBuildHere", "T");
					MakeInaccessibleAreasUnbuildable();
					MakeFarmAreaBuildable();
					UpdateSlimeArea();
				}
			}
		}

		private static void MakeInaccessibleAreasUnbuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");

			if (location is not null)
			{
				HashSet<Point> tiles = GetInaccessibleAreasTiles();

				foreach (Point tile in tiles)
				{
					location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "f");
				}
			}
		}

		private static void MakeFarmAreaBuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");

			if (location is not null)
			{
				HashSet<Point> tiles = GetFarmAreaTiles();

				foreach (Point tile in tiles)
				{
					location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "true");
				}
			}
		}

		public static void UpdateSlimeArea()
		{
			if (ModEntry.Config.AllowBuildingInSlimeArea)
			{
				MakeSlimeAreaBuildable();
			}
			else
			{
				MakeSlimeAreaUnbuildable();
			}
		}

		private static void MakeSlimeAreaUnbuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");

			if (location is not null)
			{
				HashSet<Point> tiles = GetSlimeAreaTiles();

				foreach (Point tile in tiles)
				{
					location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "f");
				}
			}
		}

		private static void MakeSlimeAreaBuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");

			if (location is not null)
			{
				HashSet<Point> tiles = GetSlimeAreaTiles();

				foreach (Point tile in tiles)
				{
					location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "true");
				}
			}
		}

		private static HashSet<Point> GetInaccessibleAreasTiles()
		{
			if (Compatibility.IsIslandOverhaulLoaded)
			{
				return TilesIslandOverhaulUtility.GetInaccessibleAreasTiles();
			}
			else if (Compatibility.IsModestMapsGingerIslandFarmLoaded)
			{
				return TilesModestMapsGingerIslandFarmUtility.GetInaccessibleAreasTiles();
			}
			else
			{
				return TilesDefaultUtility.GetInaccessibleAreasTiles();
			}
		}

		private static HashSet<Point> GetFarmAreaTiles()
		{
			if (Compatibility.IsIslandOverhaulLoaded)
			{
				return TilesIslandOverhaulUtility.GetFarmAreaTiles();
			}
			else if (Compatibility.IsModestMapsGingerIslandFarmLoaded)
			{
				return TilesModestMapsGingerIslandFarmUtility.GetFarmAreaTiles();
			}
			else
			{
				return TilesDefaultUtility.GetFarmAreaTiles();
			}
		}

		private static HashSet<Point> GetSlimeAreaTiles()
		{
			if (Compatibility.IsIslandOverhaulLoaded)
			{
				return TilesIslandOverhaulUtility.GetSlimeAreaTiles();
			}
			else if (Compatibility.IsModestMapsGingerIslandFarmLoaded)
			{
				return TilesModestMapsGingerIslandFarmUtility.GetSlimeAreaTiles();
			}
			else
			{
				return TilesDefaultUtility.GetSlimeAreaTiles();
			}
		}

		public static void	ApplyFarmHouseRestore(IslandWest islandWest)
		{
			if (islandWest.farmhouseRestored.Value)
			{
				RestoreIslandFarmhouse(islandWest);
				RestoreShippingBin(islandWest);
			}
			if (islandWest.farmhouseMailbox.Value)
			{
				RestoreIslandFarmhouseMailbox(islandWest);
			}
		}

		public static void	ApplyFarmObeliskBuild(IslandWest islandWest)
		{
			if (islandWest.farmObelisk.Value)
			{
				RestoreFarmObelisk(islandWest);
			}
		}

		private static void	RestoreIslandFarmhouse(IslandWest islandWest)
		{
			RemoveIslandFarmhouse(islandWest);
			if (Game1.player == Game1.MasterPlayer)
			{
				islandWest.AddDefaultBuilding($"{ModEntry.ModManifest.UniqueID}_IslandFarmhouse", new(74, 37));
			}
		}

		private static void RestoreShippingBin(IslandWest islandWest)
		{
			RemoveShippingBin(islandWest);
			if (Game1.player == Game1.MasterPlayer && !islandWest.modData.ContainsKey($"{ModEntry.ModManifest.UniqueID}_ShippingBin"))
			{
				islandWest.AddDefaultBuilding("Shipping Bin", new(90, 39));
				islandWest.modData.Add($"{ModEntry.ModManifest.UniqueID}_ShippingBin", "T");
			}
		}

		private static void	RestoreIslandFarmhouseMailbox(IslandWest islandWest)
		{
			RemoveIslandFarmhouseMailbox(islandWest);
			if (Game1.player == Game1.MasterPlayer)
			{
				islandWest.AddDefaultBuilding($"{ModEntry.ModManifest.UniqueID}_IslandFarmhouseMailbox", new(81, 40));
			}
		}

		private static void	RestoreFarmObelisk(IslandWest islandWest)
		{
			RemoveFarmObelisk(islandWest);
			if (Game1.player == Game1.MasterPlayer)
			{
				islandWest.AddDefaultBuilding($"{ModEntry.ModManifest.UniqueID}_FarmObelisk", new(71, 35));
			}
		}

		private static void RemoveIslandFarmhouse(IslandWest islandWest)
		{
			Layer alwaysFrontLayer = islandWest.Map.GetLayer("AlwaysFront");
			Layer frontLayer = islandWest.Map.GetLayer("Front");
			Layer buildingsLayer = islandWest.Map.GetLayer("Buildings");
			Layer backLayer = islandWest.Map.GetLayer("Back");

			if (alwaysFrontLayer is not null)
			{
				for (int i = 74; i < 80; i++)
				{
					for (int j = 34; j < 36; j++)
					{
						alwaysFrontLayer.Tiles[new(i, j)] = null;
					}
				}
			}
			if (frontLayer is not null)
			{
				for (int i = 74; i < 81; i++)
				{
					frontLayer.Tiles[new(i, 36)] = null;
				}
			}
			if (buildingsLayer is not null)
			{
				for (int i = 74; i < 81; i++)
				{
					for (int j = 37; j < 40; j++)
					{
						buildingsLayer.Tiles[new(i, j)] = null;
					}
				}
				for (int i = 74; i < 76; i++)
				{
					buildingsLayer.Tiles[new(i, 41)] = null;
				}
				for (int i = 79; i < 81; i++)
				{
					buildingsLayer.Tiles[new(i, 41)] = null;
				}
			}
			if (backLayer is not null)
			{
				for (int i = 75; i < 80; i++)
				{
					backLayer.Tiles[new(i, 40)] = backLayer.Tiles[new(77, 39)];
				}
				for (int i = 76; i < 79; i++)
				{
					backLayer.Tiles[new(i, 41)] = backLayer.Tiles[new(77, 39)];
				}
				backLayer.Tiles[new(74, 40)] = backLayer.Tiles[new(76, 36)];
				backLayer.Tiles[new(80, 40)] = backLayer.Tiles[new(80, 39)];
			}
		}

		private static void RemoveShippingBin(IslandWest islandWest)
		{
			Layer frontLayer = islandWest.Map.GetLayer("Front");
			Layer buildingsLayer = islandWest.Map.GetLayer("Buildings");

			frontLayer.Tiles[new(90, 38)] = null;
			frontLayer.Tiles[new(91, 38)] = null;
			buildingsLayer.Tiles[new(90, 39)] = null;
			buildingsLayer.Tiles[new(91, 39)] = null;
		}

		private static void	RemoveIslandFarmhouseMailbox(IslandWest islandWest)
		{
			Layer frontLayer = islandWest.Map.GetLayer("Front");
			Layer buildingsLayer = islandWest.Map.GetLayer("Buildings");

			frontLayer.Tiles[new(81, 39)] = null;
			buildingsLayer.Tiles[new(81, 40)] = null;
		}

		private static void	RemoveFarmObelisk(IslandWest islandWest)
		{
			Layer frontLayer = islandWest.Map.GetLayer("Front");
			Layer buildingsLayer = islandWest.Map.GetLayer("Buildings");

			frontLayer.Tiles[new(71, 35)] = null;
			frontLayer.Tiles[new(72, 36)] = null;
			buildingsLayer.Tiles[new(71, 36)] = null;
			buildingsLayer.Tiles[new(72, 37)] = null;
			buildingsLayer.Tiles[new(73, 35)] = null;
			buildingsLayer.Tiles[new(73, 36)] = null;
		}
	}
}
