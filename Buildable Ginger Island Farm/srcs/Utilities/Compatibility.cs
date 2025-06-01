namespace BuildableGingerIslandFarm.Utilities
{
	internal class Compatibility
	{
		internal static readonly bool IsRelocateBuildingsAndFarmAnimalsLoaded = ModEntry.Helper.ModRegistry.IsLoaded("mouahrara.RelocateBuildingsAndFarmAnimals");
		internal static readonly bool IsIslandOverhaulLoaded = ModEntry.Helper.ModRegistry.IsLoaded("Lnh.IslandOverhaul");
		internal static readonly bool IsModestMapsGingerIslandFarmLoaded = ModEntry.Helper.ModRegistry.IsLoaded("InkubusMods.ModestGinger");
	}
}
