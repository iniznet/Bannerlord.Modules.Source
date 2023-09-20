using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x0200003D RID: 61
	[OverrideView(typeof(MultiplayerEndOfBattleUIHandler))]
	public class MissionGauntletEndOfBattle : MissionView
	{
		// Token: 0x060002E7 RID: 743 RVA: 0x00010438 File Offset: 0x0000E638
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

		// Token: 0x060002E8 RID: 744 RVA: 0x000104C5 File Offset: 0x0000E6C5
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._lobbyComponent.OnPostMatchEnded -= this.OnPostMatchEnded;
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x000104E4 File Offset: 0x0000E6E4
		private void OnPostMatchEnded()
		{
			this._dataSource.OnBattleEnded();
		}

		// Token: 0x060002EA RID: 746 RVA: 0x000104F1 File Offset: 0x0000E6F1
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this._dataSource.OnTick(dt);
		}

		// Token: 0x04000182 RID: 386
		private MultiplayerEndOfBattleVM _dataSource;

		// Token: 0x04000183 RID: 387
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000184 RID: 388
		private MissionLobbyComponent _lobbyComponent;
	}
}
