using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.EndOfRound;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	[OverrideView(typeof(MultiplayerEndOfRoundUIHandler))]
	public class MissionGauntletEndOfRoundUIHandler : MissionView
	{
		private IRoundComponent RoundComponent
		{
			get
			{
				return this._mpGameModeBase.RoundComponent;
			}
		}

		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._missionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._scoreboardComponent = base.Mission.GetMissionBehavior<MissionScoreboardComponent>();
			this._mpGameModeBase = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this.ViewOrderPriority = 23;
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._dataSource = new MultiplayerEndOfRoundVM(this._scoreboardComponent, this._missionLobbyComponent, this.RoundComponent);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerEndOfRound", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
			if (this.RoundComponent != null)
			{
				this.RoundComponent.OnRoundStarted += this.RoundStarted;
				this._scoreboardComponent.OnRoundPropertiesChanged += this.OnRoundPropertiesChanged;
				this.RoundComponent.OnPostRoundEnded += this.ShowEndOfRoundUI;
				this._scoreboardComponent.OnMVPSelected += this.OnMVPSelected;
			}
			this._missionLobbyComponent.OnPostMatchEnded += this.OnPostMatchEnded;
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			if (this.RoundComponent != null)
			{
				this.RoundComponent.OnRoundStarted -= this.RoundStarted;
				this._scoreboardComponent.OnRoundPropertiesChanged -= this.OnRoundPropertiesChanged;
				this.RoundComponent.OnPostRoundEnded -= this.ShowEndOfRoundUI;
				this._scoreboardComponent.OnMVPSelected -= this.OnMVPSelected;
			}
			this._missionLobbyComponent.OnPostMatchEnded -= this.OnPostMatchEnded;
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		private void RoundStarted()
		{
			ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			this._dataSource.IsShown = false;
		}

		private void OnRoundPropertiesChanged()
		{
			if (this.RoundComponent.RoundCount != 0 && this._missionLobbyComponent.CurrentMultiplayerState != 2)
			{
				this._dataSource.Refresh();
			}
		}

		private void ShowEndOfRoundUI()
		{
			this.ShowEndOfRoundUI(false);
		}

		private void ShowEndOfRoundUI(bool isForced)
		{
			if (isForced || (this.RoundComponent.RoundCount != 0 && this._missionLobbyComponent.CurrentMultiplayerState != 2))
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, false);
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(false, 3);
				this._dataSource.IsShown = true;
			}
		}

		private void OnPostMatchEnded()
		{
			ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
			this._dataSource.IsShown = false;
		}

		private void OnMVPSelected(MissionPeer mvpPeer, int mvpCount)
		{
			this._dataSource.OnMVPSelected(mvpPeer);
		}

		private MultiplayerEndOfRoundVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private MissionLobbyComponent _missionLobbyComponent;

		private MissionScoreboardComponent _scoreboardComponent;

		private MissionMultiplayerGameModeBaseClient _mpGameModeBase;
	}
}
