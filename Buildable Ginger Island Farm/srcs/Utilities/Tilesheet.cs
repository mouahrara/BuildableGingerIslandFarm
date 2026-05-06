using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class TilesheetUtility
	{
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
			Texture2D source = GetTileSheetSource(TileSheet, x);

			if (source is not null)
			{
				TileSheet.PatchImage(source, new Rectangle(0, 13, 112, 13), new Rectangle(x, 627, 112, 13), PatchMode.Replace);
			}
		}

		private static Texture2D GetTileSheetSource(IAssetDataForImage TileSheet, int x)
		{
			Texture2D daisyNikoEarthyRecolourSource = ModEntry.Helper.ModContent.Load<Texture2D>("assets/DaisyNikoEarthyRecolour");
			Texture2D defaultSource = ModEntry.Helper.ModContent.Load<Texture2D>("assets/default");

			if (IsTileSheetAreaMatching(TileSheet, daisyNikoEarthyRecolourSource, x))
			{
				return daisyNikoEarthyRecolourSource;
			}
			else if (IsTileSheetAreaMatching(TileSheet, defaultSource, x))
			{
				return defaultSource;
			}
			else
			{
				return null;
			}
		}

		private static bool	IsTileSheetAreaMatching(IAssetDataForImage TileSheet, Texture2D source, int x)
		{
			Color[] area = new Color[112 * 13];
			Color[] reference = new Color[112 * 13];

			TileSheet.Data.GetData(0, new Rectangle(x, 627, 112, 13), area, 0, area.Length);
			source.GetData(0, new Rectangle(0, 0, 112, 13), reference, 0, reference.Length);
			for (int i = 0; i < area.Length; i++)
			{
				if (area[i] != reference[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
