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
	public class SandBoxGauntletUISubModule : MBSubModuleBase
	{
		public override void OnCampaignStart(Game game, object starterObject)
		{
			base.OnCampaignStart(game, starterObject);
			if (!this._gameStarted && game.GameType is Campaign)
			{
				this._gameStarted = true;
			}
		}

		public override void OnGameEnd(Game game)
		{
			base.OnGameEnd(game);
			if (this._gameStarted && game.GameType is Campaign)
			{
				this._gameStarted = false;
				GauntletGameNotification.OnFinalize();
			}
		}

		public override void BeginGameStart(Game game)
		{
			base.BeginGameStart(game);
			if (Campaign.Current != null)
			{
				Campaign.Current.VisualCreator.MapEventVisualCreator = new GauntletMapEventVisualCreator();
			}
		}

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

		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			base.OnGameStart(game, gameStarterObject);
			if (!this._gameStarted && game.GameType is Campaign)
			{
				this._gameStarted = true;
			}
		}

		private bool _gameStarted;

		private bool _initialized;
	}
}
