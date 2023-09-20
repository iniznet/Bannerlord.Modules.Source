using System;

namespace TaleWorlds.Library
{
	public static class ApplicationPlatform
	{
		public static EngineType CurrentEngine { get; private set; }

		public static Platform CurrentPlatform { get; private set; }

		public static Runtime CurrentRuntimeLibrary { get; private set; }

		public static void Initialize(EngineType engineType, Platform currentPlatform, Runtime currentRuntimeLibrary)
		{
			ApplicationPlatform.CurrentEngine = engineType;
			ApplicationPlatform.CurrentPlatform = currentPlatform;
			ApplicationPlatform.CurrentRuntimeLibrary = currentRuntimeLibrary;
		}

		public static bool IsPlatformWindows()
		{
			return ApplicationPlatform.CurrentPlatform == Platform.WindowsEpic || ApplicationPlatform.CurrentPlatform == Platform.WindowsNoPlatform || ApplicationPlatform.CurrentPlatform == Platform.WindowsSteam || ApplicationPlatform.CurrentPlatform == Platform.WindowsGOG || ApplicationPlatform.CurrentPlatform == Platform.GDKDesktop;
		}

		public static bool IsPlatformConsole()
		{
			return ApplicationPlatform.CurrentPlatform == Platform.Orbis || ApplicationPlatform.CurrentPlatform == Platform.Durango;
		}
	}
}
