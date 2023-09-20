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
	// Token: 0x0200003E RID: 62
	[OverrideView(typeof(MultiplayerEndOfRoundUIHandler))]
	public class MissionGauntletEndOfRoundUIHandler : MissionView
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060002EC RID: 748 RVA: 0x0001050E File Offset: 0x0000E70E
		private IRoundComponent RoundComponent
		{
			get
			{
				return this._mpGameModeBase.RoundComponent;
			}
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0001051C File Offset: 0x0000E71C
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

		// Token: 0x060002EE RID: 750 RVA: 0x00010660 File Offset: 0x0000E860
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

		// Token: 0x060002EF RID: 751 RVA: 0x00010718 File Offset: 0x0000E918
		private void RoundStarted()
		{
			ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			this._dataSource.IsShown = false;
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00010742 File Offset: 0x0000E942
		private void OnRoundPropertiesChanged()
		{
			if (this.RoundComponent.RoundCount != 0 && this._missionLobbyComponent.CurrentMultiplayerState != 2)
			{
				this._dataSource.Refresh();
			}
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0001076A File Offset: 0x0000E96A
		private void ShowEndOfRoundUI()
		{
			this.ShowEndOfRoundUI(false);
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00010774 File Offset: 0x0000E974
		private void ShowEndOfRoundUI(bool isForced)
		{
			if (isForced || (this.RoundComponent.RoundCount != 0 && this._missionLobbyComponent.CurrentMultiplayerState != 2))
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, false);
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(false, 3);
				this._dataSource.IsShown = true;
			}
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x000107C9 File Offset: 0x0000E9C9
		private void OnPostMatchEnded()
		{
			ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
			this._dataSource.IsShown = false;
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x000107E3 File Offset: 0x0000E9E3
		private void OnMVPSelected(MissionPeer mvpPeer, int mvpCount)
		{
			this._dataSource.OnMVPSelected(mvpPeer);
		}

		// Token: 0x04000185 RID: 389
		private MultiplayerEndOfRoundVM _dataSource;

		// Token: 0x04000186 RID: 390
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000187 RID: 391
		private MissionLobbyComponent _missionLobbyComponent;

		// Token: 0x04000188 RID: 392
		private MissionScoreboardComponent _scoreboardComponent;

		// Token: 0x04000189 RID: 393
		private MissionMultiplayerGameModeBaseClient _mpGameModeBase;
	}
}
