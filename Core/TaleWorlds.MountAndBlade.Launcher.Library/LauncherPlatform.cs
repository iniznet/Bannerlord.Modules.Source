using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade.Launcher.Library
{
	public static class LauncherPlatform
	{
		public static LauncherPlatformType PlatformType
		{
			get
			{
				return LauncherPlatform._platformType;
			}
		}

		public static void Initialize()
		{
			LauncherPlatform._platformType = LauncherPlatform.ReadWindowsPlatformFromFile();
			Assembly assembly = null;
			if (LauncherPlatform.PlatformType == LauncherPlatformType.Steam)
			{
				assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.MountAndBlade.Launcher.Steam.dll", true);
			}
			if (assembly != null)
			{
				Type[] types = assembly.GetTypes();
				Type type = null;
				foreach (Type type2 in types)
				{
					if (type2.GetInterfaces().Contains(typeof(IPlatformModuleExtension)))
					{
						type = type2;
						break;
					}
				}
				LauncherPlatform._platformModuleExtension = (IPlatformModuleExtension)type.GetConstructor(new Type[0]).Invoke(new object[0]);
			}
			if (LauncherPlatform._platformModuleExtension != null)
			{
				ModuleHelper.InitializePlatformModuleExtension(LauncherPlatform._platformModuleExtension);
			}
		}

		public static void Destroy()
		{
			ModuleHelper.ClearPlatformModuleExtension();
			LauncherPlatform._platformModuleExtension = null;
		}

		private static LauncherPlatformType ReadWindowsPlatformFromFile()
		{
			LauncherPlatformType launcherPlatformType = LauncherPlatformType.None;
			if (LauncherPlatform.IsGdk())
			{
				launcherPlatformType = LauncherPlatformType.Gdk;
			}
			else if (LauncherPlatform.IsSteam())
			{
				launcherPlatformType = LauncherPlatformType.Steam;
			}
			else if (LauncherPlatform.IsEpic())
			{
				launcherPlatformType = LauncherPlatformType.Epic;
			}
			else if (LauncherPlatform.IsGog())
			{
				launcherPlatformType = LauncherPlatformType.Gog;
			}
			return launcherPlatformType;
		}

		private static bool IsSteam()
		{
			return File.Exists(BasePath.Name + "Modules/Native/" + "steam.target");
		}

		private static bool IsGog()
		{
			return File.Exists(BasePath.Name + "Modules/Native/" + "gog.target");
		}

		private static bool IsGdk()
		{
			return false;
		}

		private static bool IsEpic()
		{
			return File.Exists(BasePath.Name + "Modules/Native/" + "epic.target");
		}

		public static void SetLauncherMode(bool isLauncherModeActive)
		{
			IPlatformModuleExtension platformModuleExtension = LauncherPlatform._platformModuleExtension;
			if (platformModuleExtension == null)
			{
				return;
			}
			platformModuleExtension.SetLauncherMode(isLauncherModeActive);
		}

		private static LauncherPlatformType _platformType;

		private static IPlatformModuleExtension _platformModuleExtension;
	}
}
