using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.GameData.Buildings;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class BuildingsUtility
	{
		public static void AddBuildings(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Buildings"))
			{
				AddIslandFarmhouse(e);
				AddIslandFarmhouseMailbox(e);
				AddFarmObelisk(e);
			}
		}

		private static void AddIslandFarmhouse(AssetRequestedEventArgs e)
		{
			e.Edit(asset =>
			{
				asset.AsDictionary<string, BuildingData>().Data.Add($"{ModEntry.ModManifest.UniqueID}_IslandFarmhouse", new BuildingData()
				{
					Name = "IslandFarmhouse",
					Description = "IslandFarmhouse",
					Texture = ModEntry.Helper.ModContent.GetInternalAssetName("assets/Island Farmhouse").Name,
					DrawShadow = false,
					Size = new Point(7, 5),
					SourceRect = new Rectangle(0, 0, 112, 144),
					SortTileOffset = 2f,
					CollisionMap = "XXXXXXX\nXXXXXXX\nXXXXXXX\nOOOOOOO\nXXOOOXX\n",
					Builder = "None",
					HumanDoor = new Point(3, 2),
					NonInstancedIndoorLocation = "IslandFarmHouse",
				});
			});
		}

		private static void AddIslandFarmhouseMailbox(AssetRequestedEventArgs e)
		{
			e.Edit(asset =>
			{
				asset.AsDictionary<string, BuildingData>().Data.Add($"{ModEntry.ModManifest.UniqueID}_IslandFarmhouseMailbox", new BuildingData()
				{
					Name = "IslandFarmhouseMailbox",
					Description = "IslandFarmhouseMailbox",
					Texture = "Maps/island_tilesheet_1",
					DrawShadow = false,
					Size = new Point(1, 1),
					SourceRect = new Rectangle(48, 368, 16, 32),
					Builder = "None",
					DefaultAction = "Mailbox"
				});
			});
		}

		private static void AddFarmObelisk(AssetRequestedEventArgs e)
		{
			e.Edit(asset =>
			{
				asset.AsDictionary<string, BuildingData>().Data.Add($"{ModEntry.ModManifest.UniqueID}_FarmObelisk", new BuildingData()
				{
					Name = "FarmObelisk",
					Description = "FarmObelisk",
					Texture = "Maps/Farm Obelisk",
					DrawShadow = false,
					Size = new Point(3, 2),
					DrawOffset = new Vector2(0, 16),
					Builder = "None",
					DefaultAction = "FarmObelisk"
				});
			});
		}
	}
}
