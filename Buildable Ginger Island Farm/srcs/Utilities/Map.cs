using xTile;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using StardewModdingAPI.Events;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class MapUtility
	{
		public static void EditIslandWest(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_W"))
			{
				e.Edit(asset =>
				{
					Map map = asset.AsMap().Data;

					FixGrass(map);
					RemoveBushes(map);
				}, AssetEditPriority.Late);
			}
		}

		public static void EditIslandHouseCave(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Maps/Island_House_Cave"))
			{
				e.Edit(asset =>
				{
					Map map = asset.AsMap().Data;

					MoveIslandHouseCaveTilesFromAlwaysFrontLayerToFrontLayer(map);
				});
			}
		}

		private static void FixGrass(Map map)
		{
			if (Compatibility.IsIslandOverhaulLoaded)
			{
				IslandOverhaulFixGrass(map);
			}
			else if (Compatibility.IsModestMapsGingerIslandFarmLoaded)
			{
				ModestMapsGingerIslandFixGrass(map);
			}
			else
			{
				DefaultFixGrass(map);
			}
		}

		private static void	IslandOverhaulFixGrass(Map map)
		{
			Layer backLayer = map.GetLayer("Back");

			if (backLayer is not null)
			{
				backLayer.Tiles[new(73, 40)] = backLayer.Tiles[new(74, 43)];
				backLayer.Tiles[new(80, 41)] = backLayer.Tiles[new(72, 40)];
				backLayer.Tiles[new(81, 41)] = backLayer.Tiles[new(74, 43)];
				for (int i = 73; i < 76; i++)
				{
					backLayer.Tiles[new(i, 41)] = backLayer.Tiles[new(72, 42)];
				}
				backLayer.Tiles[new(79, 41)] = backLayer.Tiles[new(72, 42)];
				for (int i = 73; i < 82; i++)
				{
					for (int j = 42; j < 44; j++)
					{
						backLayer.Tiles[new(i, j)] = backLayer.Tiles[new(72, 42)];
					}
				}
			}
		}

		private static void	ModestMapsGingerIslandFixGrass(Map map)
		{
			Layer backLayer = map.GetLayer("Back");

			if (backLayer is not null)
			{
				for (int i = 74; i < 81; i++)
				{
					backLayer.Tiles[new(i, 42)] = backLayer.Tiles[new(73, 42)];
				}
			}
		}

		private static void	DefaultFixGrass(Map map)
		{
			Layer backLayer = map.GetLayer("Back");

			if (backLayer is not null)
			{
				backLayer.Tiles[new(72, 40)] = backLayer.Tiles[new(72, 39)];
				backLayer.Tiles[new(73, 40)] = backLayer.Tiles[new(73, 39)];
				backLayer.Tiles[new(72, 41)] = backLayer.Tiles[new(79, 36)];
				backLayer.Tiles[new(73, 41)] = backLayer.Tiles[new(90, 40)];
				backLayer.Tiles[new(74, 41)] = backLayer.Tiles[new(73, 39)];
				backLayer.Tiles[new(75, 41)] = backLayer.Tiles[new(73, 39)];
				backLayer.Tiles[new(79, 41)] = backLayer.Tiles[new(73, 39)];
				backLayer.Tiles[new(80, 41)] = backLayer.Tiles[new(73, 39)];
				backLayer.Tiles[new(81, 41)] = backLayer.Tiles[new(91, 40)];
			}
		}

		private static void	RemoveBushes(Map map)
		{
			if (Compatibility.IsModestMapsGingerIslandFarmLoaded)
			{
				ModestMapsGingerIslandRemoveBushes(map);
			}
			else
			{
				DefaultRemoveBushes(map);
			}
		}

		private static void	ModestMapsGingerIslandRemoveBushes(Map map)
		{
			Layer frontLayer = map.GetLayer("Front");
			Layer buildingsLayer = map.GetLayer("Buildings");

			if (frontLayer is not null)
			{
				frontLayer.Tiles[new(73, 34)] = null;
			}
			if (buildingsLayer is not null)
			{
				buildingsLayer.Tiles[new(73, 35)] = null;
			}
			buildingsLayer.Tiles[new(73, 34)] = buildingsLayer.Tiles[new(82, 39)];
			DefaultRemoveBushes(map);
		}

		private static void	DefaultRemoveBushes(Map map)
		{
			Layer frontLayer = map.GetLayer("Front");
			Layer buildingsLayer = map.GetLayer("Buildings");

			if (frontLayer is not null)
			{
				for (int i = 81; i < 83; i++)
				{
					frontLayer.Tiles[new(i, 38)] = null;
				}
				frontLayer.Tiles[new(73, 39)] = null;
			}
			if (buildingsLayer is not null)
			{
				for (int i = 81; i < 83; i++)
				{
					buildingsLayer.Tiles[new(i, 39)] = null;
				}
				buildingsLayer.Tiles[new(73, 40)] = null;
			}
		}

		private static void	MoveIslandHouseCaveTilesFromAlwaysFrontLayerToFrontLayer(Map map)
		{
			Layer alwaysFrontLayer = map.GetLayer("AlwaysFront");
			Layer frontLayer = map.GetLayer("Front");

			if (alwaysFrontLayer is not null && frontLayer is not null)
			{
				for (int i = 1; i < 2; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						const string tileSheetId = "untitled tile sheet2";
						Location position = new(i, j);
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
