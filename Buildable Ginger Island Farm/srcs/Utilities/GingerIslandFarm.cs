using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using xTile.Layers;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Locations;
using StardewValley.Locations;
using StardewValley.Buildings;

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
			HashSet<Point> tiles = GetInaccessibleAreasTiles();

			foreach (Point tile in tiles)
			{
				location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "f");
			}
		}

		private static void MakeFarmAreaBuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");
			HashSet<Point> tiles = GetFarmAreaTiles();

			foreach (Point tile in tiles)
			{
				location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "true");
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
			HashSet<Point> tiles = GetSlimeAreaTiles();

			foreach (Point tile in tiles)
			{
				location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "f");
			}
		}

		private static void MakeSlimeAreaBuildable()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");
			HashSet<Point> tiles = GetSlimeAreaTiles();

			foreach (Point tile in tiles)
			{
				location.setTileProperty(tile.X, tile.Y, "Back", "Buildable", "true");
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

		public static void RemoveShippingBin()
		{
			GameLocation location = Game1.getLocationFromName("IslandWest");

			if (location is IslandWest islandWest && Game1.player.hasOrWillReceiveMail("Island_UpgradeHouse"))
			{
				Layer frontLayer = islandWest.Map.GetLayer("Front");
				Layer buildingsLayer = islandWest.Map.GetLayer("Buildings");

				frontLayer.Tiles[new(90, 38)] = null;
				frontLayer.Tiles[new(91, 38)] = null;
				buildingsLayer.Tiles[new(90, 39)] = null;
				buildingsLayer.Tiles[new(91, 39)] = null;
				islandWest.shippingBinPosition = new Point(-1000, -1000);
				typeof(IslandWest).GetMethod("resetLocalState", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(islandWest, null);
				if (!islandWest.modData.ContainsKey($"{ModEntry.ModManifest.UniqueID}_ShippingBin"))
				{
					islandWest.buildStructure(new ShippingBin(), new(90, 39), Game1.MasterPlayer, true);
					islandWest.modData.Add($"{ModEntry.ModManifest.UniqueID}_ShippingBin", "T");
				}
			}
		}
	}
}
