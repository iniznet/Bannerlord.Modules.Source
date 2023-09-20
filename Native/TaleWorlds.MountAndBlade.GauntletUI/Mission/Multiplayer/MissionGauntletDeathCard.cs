using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x0200003B RID: 59
	[OverrideView(typeof(MissionMultiplayerDeathCardUIHandler))]
	public class MissionGauntletDeathCard : MissionView
	{
		// Token: 0x060002D6 RID: 726 RVA: 0x0000FFB0 File Offset: 0x0000E1B0
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			MissionMultiplayerGameModeBaseClient missionBehavior = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this._dataSource = new MPDeathCardVM(missionBehavior.GameType);
			this._gauntletLayer = new GauntletLayer(this.ViewOrderPriority, "GauntletLayer", false);
			this._gauntletLayer.LoadMovie("MultiplayerDeathCard", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().OnMyAgentVisualSpawned += this.OnMainAgentVisualSpawned;
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0001003C File Offset: 0x0000E23C
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (GameNetwork.MyPeer != null && this._myPeer == null)
			{
				this._myPeer = PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer);
			}
			MissionPeer myPeer = this._myPeer;
			if (myPeer != null && myPeer.WantsToSpawnAsBot && this._dataSource.IsActive)
			{
				this._dataSource.Deactivate();
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0001009B File Offset: 0x0000E29B
		private void OnMainAgentVisualSpawned()
		{
			this._dataSource.Deactivate();
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x000100A8 File Offset: 0x0000E2A8
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			if (affectedAgent.IsMine && blow.DamageType != -1)
			{
				this._dataSource.OnMainAgentRemoved(affectorAgent, blow);
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x000100D8 File Offset: 0x0000E2D8
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._dataSource.OnFinalize();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().OnMyAgentVisualSpawned -= this.OnMainAgentVisualSpawned;
			this._dataSource = null;
			this._gauntletLayer = null;
		}

		// Token: 0x04000179 RID: 377
		private MPDeathCardVM _dataSource;

		// Token: 0x0400017A RID: 378
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400017B RID: 379
		private MissionPeer _myPeer;
	}
}
