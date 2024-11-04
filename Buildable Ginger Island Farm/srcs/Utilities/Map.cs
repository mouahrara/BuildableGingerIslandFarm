using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class MapUtility
	{
		public static void EditIslandHouseCave(AssetRequestedEventArgs e)
		{
			if (e.Name.IsEquivalentTo("Maps/Island_House_Cave"))
			{
				e.Edit(asset =>
				{
					IAssetDataForMap editor = asset.AsMap();

					MoveIslandHouseCaveTilesFromAlwaysFrontLayerToFrontLayer(editor.Data);
				});
			}
		}

		private static void	MoveIslandHouseCaveTilesFromAlwaysFrontLayerToFrontLayer(Map map)
		{
			Layer frontLayer = map.GetLayer("Front");
			Layer alwaysFrontLayer = map.GetLayer("AlwaysFront");

			if (frontLayer is not null && alwaysFrontLayer is not null)
			{
				for (int i = 1; i < 2; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						const string tileSheetId = "untitled tile sheet2";
						Location position = new(i, j);
						Tile frontTile = frontLayer.Tiles[position];
						Tile alwaysFrontTile = alwaysFrontLayer.Tiles[position];

						if (alwaysFrontTile is not null)
						{
							if (alwaysFrontTile.TileSheet.Id.Equals(tileSheetId) && (alwaysFrontTile.TileIndex == 1634 || alwaysFrontTile.TileIndex == 1659))
							{
								frontLayer.Tiles[position] = alwaysFrontLayer.Tiles[position];
								alwaysFrontLayer.Tiles[position] = null;
							}
						}
					}
				}
			}
		}
	}
}
