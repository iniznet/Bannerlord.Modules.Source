using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	[OverrideView(typeof(MultiplayerEndOfBattleUIHandler))]
	public class MissionGauntletEndOfBattle : MissionView
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this.ViewOrderPriority = 30;
			this._dataSource = new MultiplayerEndOfBattleVM();
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerEndOfBattle", this._dataSource);
			this._lobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._lobbyComponent.OnPostMatchEnded += this.OnPostMatchEnded;
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._lobbyComponent.OnPostMatchEnded -= this.OnPostMatchEnded;
		}

		private void OnPostMatchEnded()
		{
			this._dataSource.OnBattleEnded();
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.OnTick(dt);
		}

		private MultiplayerEndOfBattleVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private MissionLobbyComponent _lobbyComponent;
	}
}
