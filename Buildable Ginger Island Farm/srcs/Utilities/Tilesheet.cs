using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class TilesheetUtility
	{
		private static readonly string[] AssetKeys = new string[]
		{
			"assets/CollingBlueGrassRecolor.png",
			"assets/DaisyNikoEarthyRecolour.png",
			"assets/GweniaczekMedievalBuildings.png",
			"assets/default.png"
		};
		private static IRawTextureData[] Assets;

		private static IRawTextureData[] GetAssets()
		{
			if (Assets is null)
			{
				Assets = new IRawTextureData[AssetKeys.Length];
				for (int i = 0; i < AssetKeys.Length; i++)
				{
					Assets[i] = ModEntry.Helper.ModContent.Load<IRawTextureData>(AssetKeys[i]);
				}
			}
			return Assets;
		}

		public static void EditIslandTileSheet(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Maps/island_tilesheet_1"))
			{
				e.Edit(asset =>
				{
					IAssetDataForImage TileSheet = asset.AsImage();

					EditTileSheet(TileSheet, 0);
					EditTileSheet(TileSheet, 112);
				}, AssetEditPriority.Late);
			}
		}

		private static void	EditTileSheet(IAssetDataForImage TileSheet, int x)
		{
			Color[] area = new Color[112 * 16];

			MainThreadUtility.Run(() => TileSheet.Data.GetData(0, new Rectangle(x, 624, 112, 16), area, 0, area.Length));

			IRawTextureData asset = GetMatchingAsset(area);

			if (asset is not null)
			{
				ApplyReplacement(TileSheet, area, asset, x);
			}
		}

		private static IRawTextureData GetMatchingAsset(Color[] area)
		{
			foreach (IRawTextureData asset in GetAssets())
			{
				if (IsTileSheetAreaMatching(area, asset))
				{
					return asset;
				}
			}
			return null;
		}

		private static bool	IsTileSheetAreaMatching(Color[] area, IRawTextureData asset)
		{
			Color[] reference = GetRegionData(asset, new Rectangle(0, 0, 112, 16));
			Color[] mask = HasMasks(asset) ? GetRegionData(asset, new Rectangle(112, 0, 112, 16)) : null;

			for (int i = 0; i < area.Length; i++)
			{
				if (mask is not null && mask[i].A > 0)
				{
					continue;
				}
				if (area[i] != reference[i])
				{
					return false;
				}
			}
			return true;
		}

		private static void	ApplyReplacement(IAssetDataForImage TileSheet, Color[] area, IRawTextureData asset, int x)
		{
			Color[] replacement = GetRegionData(asset, new Rectangle(0, 16, 112, 16));
			Color[] mask = HasMasks(asset) ? GetRegionData(asset, new Rectangle(112, 16, 112, 16)) : null;

			for (int i = 0; i < area.Length; i++)
			{
				if (mask is not null && mask[i].A > 0)
				{
					continue;
				}
				area[i] = replacement[i];
			}
			MainThreadUtility.Run(() => TileSheet.Data.SetData(0, new Rectangle(x, 624, 112, 16), area, 0, area.Length));
		}

		private static bool	HasMasks(IRawTextureData asset)
		{
			return asset.Width >= 224;
		}

		private static Color[]	GetRegionData(IRawTextureData asset, Rectangle region)
		{
			Color[] data = new Color[region.Width * region.Height];

			for (int row = 0; row < region.Height; row++)
			{
				int sourceIndex = (region.Y + row) * asset.Width + region.X;
				int destIndex = row * region.Width;

				Array.Copy(asset.Data, sourceIndex, data, destIndex, region.Width);
			}
			return data;
		}
	}
}
