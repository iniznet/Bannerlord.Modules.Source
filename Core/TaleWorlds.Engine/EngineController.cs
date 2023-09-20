using System;
using TaleWorlds.Engine.InputSystem;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.Engine
{
	public static class EngineController
	{
		[EngineCallback]
		internal static void Initialize()
		{
			IInputContext inputContext = null;
			Input.Initialize(new EngineInputManager(), inputContext);
			Common.PlatformFileHelper = new PlatformFileHelperPC(Utilities.GetApplicationName());
		}

		[EngineCallback]
		internal static void OnConfigChange()
		{
			NativeConfig.OnConfigChanged();
			if (EngineController.ConfigChange != null)
			{
				EngineController.ConfigChange();
			}
		}

		public static event Action ConfigChange;

		[EngineCallback]
		internal static void OnConstrainedStateChange(bool isConstrained)
		{
			Action<bool> onConstrainedStateChanged = EngineController.OnConstrainedStateChanged;
			if (onConstrainedStateChanged == null)
			{
				return;
			}
			onConstrainedStateChanged(isConstrained);
		}

		public static event Action<bool> OnConstrainedStateChanged;

		internal static void OnApplicationTick(float dt)
		{
			Input.Update();
			Screen.Update();
		}

		[EngineCallback]
		public static string GetVersionStr()
		{
			return ApplicationVersion.FromParametersFile(null).ToString();
		}

		[EngineCallback]
		public static string GetApplicationPlatformName()
		{
			return ApplicationPlatform.CurrentPlatform.ToString();
		}

		[EngineCallback]
		public static string GetModulesVersionStr()
		{
			string text = "";
			foreach (ModuleInfo moduleInfo in ModuleHelper.GetModules())
			{
				text = string.Concat(new object[] { text, moduleInfo.Name, "#", moduleInfo.Version, "\n" });
			}
			return text;
		}

		[EngineCallback]
		internal static void OnControllerDisconnection()
		{
			ScreenManager.OnControllerDisconnect();
		}
	}
}
