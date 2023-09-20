using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	[OverrideView(typeof(MissionMultiplayerDeathCardUIHandler))]
	public class MissionGauntletDeathCard : MissionView
	{
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

		private void OnMainAgentVisualSpawned()
		{
			this._dataSource.Deactivate();
		}

		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			if (affectedAgent.IsMine && blow.DamageType != -1)
			{
				this._dataSource.OnMainAgentRemoved(affectorAgent, blow);
			}
		}

		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._dataSource.OnFinalize();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>().OnMyAgentVisualSpawned -= this.OnMainAgentVisualSpawned;
			this._dataSource = null;
			this._gauntletLayer = null;
		}

		private MPDeathCardVM _dataSource;

		private GauntletLayer _gauntletLayer;

		private MissionPeer _myPeer;
	}
}
