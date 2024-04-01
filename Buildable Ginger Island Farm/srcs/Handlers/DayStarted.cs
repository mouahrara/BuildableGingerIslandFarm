using StardewModdingAPI.Events;
using BuildableGingerIslandFarm.Utilities;

namespace BuildableGingerIslandFarm.Handlers
{
	internal static class DayStartedHandler
	{
		/// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, DayStartedEventArgs e)
		{
			// Make Ginger Island Farm buildable
			GingerIslandFarmUtility.MakeBuildable();
		}
	}
}
