using System;
using SandBox.GauntletUI.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.GauntletUI.SceneNotification;

namespace SandBox.GauntletUI
{
	// Token: 0x02000010 RID: 16
	public class SandBoxGauntletUISubModule : MBSubModuleBase
	{
		// Token: 0x060000AF RID: 175 RVA: 0x00006E5C File Offset: 0x0000505C
		public override void OnCampaignStart(Game game, object starterObject)
		{
			base.OnCampaignStart(game, starterObject);
			if (!this._gameStarted && game.GameType is Campaign)
			{
				this._gameStarted = true;
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00006E82 File Offset: 0x00005082
		public override void OnGameEnd(Game game)
		{
			base.OnGameEnd(game);
			if (this._gameStarted && game.GameType is Campaign)
			{
				this._gameStarted = false;
				GauntletGameNotification.OnFinalize();
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00006EAC File Offset: 0x000050AC
		public override void BeginGameStart(Game game)
		{
			base.BeginGameStart(game);
			if (Campaign.Current != null)
			{
				Campaign.Current.VisualCreator.MapEventVisualCreator = new GauntletMapEventVisualCreator();
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00006ED0 File Offset: 0x000050D0
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
			if (!this._initialized)
			{
				if (!Utilities.CommandLineArgumentExists("VisualTests"))
				{
					GauntletSceneNotification.Current.RegisterContextProvider(new SandboxSceneNotificationContextProvider());
				}
				this._initialized = true;
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00006F02 File Offset: 0x00005102
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			base.OnGameStart(game, gameStarterObject);
			if (!this._gameStarted && game.GameType is Campaign)
			{
				this._gameStarted = true;
			}
		}

		// Token: 0x04000056 RID: 86
		private bool _gameStarted;

		// Token: 0x04000057 RID: 87
		private bool _initialized;
	}
}
