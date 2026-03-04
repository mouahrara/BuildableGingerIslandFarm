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

					EditTileSheet(TileSheet);
				}, AssetEditPriority.Early);
			}
		}

		private static void	EditTileSheet(IAssetDataForImage TileSheet)
		{
			Texture2D source = ModEntry.Helper.ModContent.Load<Texture2D>("assets/default");

			TileSheet.PatchImage(source, null, new Rectangle(0, 627, 112, 13), PatchMode.Replace);
			TileSheet.PatchImage(source, null, new Rectangle(112, 627, 112, 13), PatchMode.Replace);
		}
	}
}
