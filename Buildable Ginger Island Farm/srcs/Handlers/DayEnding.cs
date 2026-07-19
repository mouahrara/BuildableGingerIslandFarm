using StardewModdingAPI.Events;
using BuildableGingerIslandFarm.Utilities;

namespace BuildableGingerIslandFarm.Handlers
{
	internal static class DayEndingHandler
	{
		/// <inheritdoc cref="IGameLoopEvents.DayEnding"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, DayEndingEventArgs e)
		{
			// Update Ginger Island grass spread
			GingerIslandFarmUtility.UpdateGrassSpread();
		}
	}
}
