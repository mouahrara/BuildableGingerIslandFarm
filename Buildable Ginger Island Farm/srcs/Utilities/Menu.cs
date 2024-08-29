using StardewModdingAPI.Events;
using StardewValley.GameData.Locations;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class MenuUtility
	{
		public static void LocalizeGingerIslandFarmDisplayName(AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
			{
				e.Edit(asset =>
				{
					asset.AsDictionary<string, LocationData>().Data["IslandWest"].DisplayName = ModEntry.Helper.Translation.Get("Menu.GingerIslandFarm.DisplayName");
				});
			}
		}
	}
}
