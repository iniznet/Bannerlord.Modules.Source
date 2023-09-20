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
	// Token: 0x02000002 RID: 2
	public class StoryModeGauntletUISubModule : MBSubModuleBase
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
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

		// Token: 0x06000003 RID: 3 RVA: 0x00002084 File Offset: 0x00000284
		private void OnScreenManagerPushScreen(ScreenBase pushedScreen)
		{
			MapScreen mapScreen;
			if (!this._registered && (mapScreen = pushedScreen as MapScreen) != null)
			{
				mapScreen.MapNotificationView.RegisterMapNotificationType(typeof(ConspiracyQuestMapNotification), typeof(ConspiracyQuestMapNotificationItemVM));
				this._registered = true;
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020C9 File Offset: 0x000002C9
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

		// Token: 0x04000001 RID: 1
		private bool _registered;
	}
}
