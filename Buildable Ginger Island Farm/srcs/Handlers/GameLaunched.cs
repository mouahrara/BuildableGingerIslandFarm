using StardewModdingAPI.Events;
using BuildableGingerIslandFarm.Utilities;

namespace BuildableGingerIslandFarm.Handlers
{
	internal static class GameLaunchedHandler
	{
		/// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, GameLaunchedEventArgs e)
		{
			// Register console commands
			ConsoleCommandsUtility.Register();

			// Initialize GMCM
			GMCMUtility.Initialize();
		}
	}
}
