using System;
using SandBox.View.Map;
using StoryMode.GauntletUI.Permissions;
using StoryMode.GauntletUI.Tutorial;
using StoryMode.ViewModelCollection.Map;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace StoryMode.GauntletUI
{
	public class StoryModeGauntletUISubModule : MBSubModuleBase
	{
		public override void OnGameInitializationFinished(Game game)
		{
			base.OnGameInitializationFinished(game);
			if (!(game.GameType is MultiplayerGame))
			{
				GauntletTutorialSystem.OnInitialize();
				StoryModePermissionsSystem.OnInitialize();
				ScreenManager.OnPushScreen += new ScreenManager.OnPushScreenEvent(this.OnScreenManagerPushScreen);
			}
		}

		private void OnScreenManagerPushScreen(ScreenBase pushedScreen)
		{
			MapScreen mapScreen;
			if (!this._registered && (mapScreen = pushedScreen as MapScreen) != null)
			{
				mapScreen.MapNotificationView.RegisterMapNotificationType(typeof(ConspiracyQuestMapNotification), typeof(ConspiracyQuestMapNotificationItemVM));
				this._registered = true;
			}
		}

		public override void OnGameEnd(Game game)
		{
			base.OnGameEnd(game);
			if (!(game.GameType is MultiplayerGame))
			{
				GauntletTutorialSystem.OnUnload();
				StoryModePermissionsSystem.OnUnload();
				ScreenManager.OnPushScreen -= new ScreenManager.OnPushScreenEvent(this.OnScreenManagerPushScreen);
			}
			this._registered = false;
		}

		private bool _registered;
	}
}
