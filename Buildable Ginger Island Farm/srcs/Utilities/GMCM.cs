namespace BuildableGingerIslandFarm.Utilities
{
	public sealed class ModConfig
	{
		public bool	AllowGrassSpread = true;
		public bool	AllowSlimeSpawn = true;
		public bool	AllowBuildingInSlimeArea = false;
	}

	internal class GMCMUtility
	{
		internal static void Initialize()
		{
			ReadConfig();
			Register();
		}

		private static void ReadConfig()
		{
			ModEntry.Config = ModEntry.Helper.ReadConfig<ModConfig>();
		}

		private static void Register()
		{
			// Get Generic Mod Config Menu's API
			GenericModConfigMenu.IGenericModConfigMenuApi gmcm = ModEntry.Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

			if (gmcm is not null)
			{
				// Register mod
				gmcm.Register(
					mod: ModEntry.ModManifest,
					reset: () => ModEntry.Config = new ModConfig(),
					save: () => ModEntry.Helper.WriteConfig(ModEntry.Config)
				);

				// Main
				gmcm.AddBoolOption(
					mod: ModEntry.ModManifest,
					name: () => ModEntry.Helper.Translation.Get("GMCM.AllowGrassSpread.Title"),
					tooltip: () => ModEntry.Helper.Translation.Get("GMCM.AllowGrassSpread.Tooltip"),
					getValue: () => ModEntry.Config.AllowGrassSpread,
					setValue: (value) =>
					{
						ModEntry.Config.AllowGrassSpread = value;
						GingerIslandFarmUtility.UpdateGrassSpread();
					}
				);
				gmcm.AddBoolOption(
					mod: ModEntry.ModManifest,
					name: () => ModEntry.Helper.Translation.Get("GMCM.AllowSlimeSpawn.Title"),
					tooltip: () => ModEntry.Helper.Translation.Get("GMCM.AllowSlimeSpawn.Tooltip"),
					getValue: () => ModEntry.Config.AllowSlimeSpawn,
					setValue: (value) =>
					{
						ModEntry.Config.AllowSlimeSpawn = value;
						GingerIslandFarmUtility.UpdateSlimeSpawn();
					}
				);
				gmcm.AddBoolOption(
					mod: ModEntry.ModManifest,
					name: () => ModEntry.Helper.Translation.Get("GMCM.AllowBuildingInSlimeArea.Title"),
					tooltip: () => ModEntry.Helper.Translation.Get("GMCM.AllowBuildingInSlimeArea.Tooltip"),
					getValue: () => ModEntry.Config.AllowBuildingInSlimeArea,
					setValue: (value) =>
					{
						ModEntry.Config.AllowBuildingInSlimeArea = value;
						GingerIslandFarmUtility.UpdateSlimeArea();
					}
				);
			}
		}
	}
}
