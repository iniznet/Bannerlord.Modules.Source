using System;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.GauntletUI.SceneNotification;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI
{
	public class MultiplayerGauntletUISubModule : MBSubModuleBase
	{
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			if (!this._initialized)
			{
				if (!Utilities.CommandLineArgumentExists("VisualTests"))
				{
					GauntletSceneNotification.Current.RegisterContextProvider(new MultiplayerSceneNotificationContextProvider());
				}
				this._initialized = true;
			}
		}

		private bool _initialized;
	}
}
