using System;
using TaleWorlds.Engine.InputSystem;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.Engine
{
	// Token: 0x0200003F RID: 63
	public static class EngineController
	{
		// Token: 0x0600058D RID: 1421 RVA: 0x00003110 File Offset: 0x00001310
		[EngineCallback]
		internal static void Initialize()
		{
			IInputContext inputContext = null;
			Input.Initialize(new EngineInputManager(), inputContext);
			Common.PlatformFileHelper = new PlatformFileHelperPC(Utilities.GetApplicationName());
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x00003139 File Offset: 0x00001339
		[EngineCallback]
		internal static void OnConfigChange()
		{
			NativeConfig.OnConfigChanged();
			if (EngineController.ConfigChange != null)
			{
				EngineController.ConfigChange();
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600058F RID: 1423 RVA: 0x00003154 File Offset: 0x00001354
		// (remove) Token: 0x06000590 RID: 1424 RVA: 0x00003188 File Offset: 0x00001388
		public static event Action ConfigChange;

		// Token: 0x06000591 RID: 1425 RVA: 0x000031BB File Offset: 0x000013BB
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

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000592 RID: 1426 RVA: 0x000031D0 File Offset: 0x000013D0
		// (remove) Token: 0x06000593 RID: 1427 RVA: 0x00003204 File Offset: 0x00001404
		public static event Action<bool> OnConstrainedStateChanged;

		// Token: 0x06000594 RID: 1428 RVA: 0x00003237 File Offset: 0x00001437
		internal static void OnApplicationTick(float dt)
		{
			Input.Update();
			Screen.Update();
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x00003244 File Offset: 0x00001444
		[EngineCallback]
		public static string GetVersionStr()
		{
			return ApplicationVersion.FromParametersFile(null).ToString();
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x00003268 File Offset: 0x00001468
		[EngineCallback]
		public static string GetApplicationPlatformName()
		{
			return ApplicationPlatform.CurrentPlatform.ToString();
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x00003288 File Offset: 0x00001488
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

		// Token: 0x06000598 RID: 1432 RVA: 0x0000330C File Offset: 0x0000150C
		[EngineCallback]
		internal static void OnControllerDisconnection()
		{
			ScreenManager.OnControllerDisconnect();
		}
	}
}
