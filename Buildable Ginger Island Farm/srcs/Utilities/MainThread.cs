using System;
using System.Reflection;
using StardewModdingAPI;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class MainThreadUtility
	{
		private static readonly Type AndroidMainThreadType = Type.GetType("StardewModdingAPI.Mobile.AndroidMainThread, StardewModdingAPI");
		private static readonly PropertyInfo AndroidIsOnMainThreadProperty = AndroidMainThreadType?.GetProperty("IsOnMainThread", BindingFlags.NonPublic | BindingFlags.Static);
		private static readonly MethodInfo AndroidInvokeOnMainThreadMethod = AndroidMainThreadType?.GetMethod("InvokeOnMainThread", BindingFlags.Public | BindingFlags.Static);

		internal static void Run(Action action)
		{
			if (Constants.TargetPlatform == GamePlatform.Android)
			{
				if ((bool)AndroidIsOnMainThreadProperty?.GetValue(null))
				{
					action();
				}
				else
				{
					AndroidInvokeOnMainThreadMethod?.Invoke(null, new object[] { action, null });
				}
			}
			else
			{
				action();
			}
		}
	}
}
