using StardewModdingAPI.Events;
using BuildableGingerIslandFarm.Utilities;

namespace BuildableGingerIslandFarm.Handlers
{
	internal static class AssetRequestedHandler
	{
		/// <inheritdoc cref="IContentEvents.AssetRequested"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, AssetRequestedEventArgs e)
		{
			// Make Ginger Island Farm always active
			GingerIslandFarmUtility.MakeAlwaysActive(e);

			// Localize menus
			MenuUtility.LocalizeGingerIslandFarmDisplayName(e);
			MenuUtility.LocalizeCarpenterMenuChooseLocation(e);
			MenuUtility.LocalizePurchaseAnimalsMenuChooseLocation(e);
		}
	}
}
