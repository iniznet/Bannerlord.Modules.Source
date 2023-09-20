using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200028A RID: 650
	public class ConsoleMatchStartEndHandler : MissionNetwork
	{
		// Token: 0x0600227E RID: 8830 RVA: 0x0007DA50 File Offset: 0x0007BC50
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._activeOtherPlayers = new List<VirtualPlayer>();
			this._matchState = ConsoleMatchStartEndHandler.MatchState.NotPlaying;
			this._gameModeClient = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this._visualSpawnComponent = base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
			this._visualSpawnComponent.OnMyAgentSpawnedFromVisual += this.AgentVisualSpawnComponentOnOnMyAgentVisualSpawned;
			MissionPeer.OnTeamChanged += this.OnTeamChange;
		}

		// Token: 0x0600227F RID: 8831 RVA: 0x0007DABF File Offset: 0x0007BCBF
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			MissionPeer.OnTeamChanged -= this.OnTeamChange;
			if (this._matchState == ConsoleMatchStartEndHandler.MatchState.Playing)
			{
				this._matchState = ConsoleMatchStartEndHandler.MatchState.NotPlaying;
				PlatformServices.MultiplayerGameStateChanged(false);
			}
		}

		// Token: 0x06002280 RID: 8832 RVA: 0x0007DAEE File Offset: 0x0007BCEE
		private void AgentVisualSpawnComponentOnOnMyAgentVisualSpawned()
		{
			if (!this._gameModeClient.IsInWarmup)
			{
				this._visualSpawnComponent.OnMyAgentSpawnedFromVisual -= this.AgentVisualSpawnComponentOnOnMyAgentVisualSpawned;
				this._inGameCheckActive = true;
			}
		}

		// Token: 0x06002281 RID: 8833 RVA: 0x0007DB1C File Offset: 0x0007BD1C
		private void OnTeamChange(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (newTeam.Side == BattleSideEnum.None)
			{
				if (peer.IsMine)
				{
					this._visualSpawnComponent.OnMyAgentVisualSpawned += this.AgentVisualSpawnComponentOnOnMyAgentVisualSpawned;
					this._inGameCheckActive = false;
					PlatformServices.MultiplayerGameStateChanged(false);
					return;
				}
				int num = this._activeOtherPlayers.IndexOf(peer.VirtualPlayer);
				if (num >= 0)
				{
					this._activeOtherPlayers.RemoveAt(num);
				}
			}
		}

		// Token: 0x06002282 RID: 8834 RVA: 0x0007DB84 File Offset: 0x0007BD84
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.MissionPeer != null && !agent.MissionPeer.IsMine && !this._activeOtherPlayers.Contains(agent.MissionPeer.Peer))
			{
				this._activeOtherPlayers.Add(agent.MissionPeer.Peer);
			}
		}

		// Token: 0x06002283 RID: 8835 RVA: 0x0007DBD4 File Offset: 0x0007BDD4
		public override void OnMissionTick(float dt)
		{
			this._playingCheckTimer -= dt;
			if (this._playingCheckTimer <= 0f)
			{
				this._playingCheckTimer += 1f;
				if (this._inGameCheckActive)
				{
					if (this._activeOtherPlayers.Count > 0)
					{
						if (this._matchState == ConsoleMatchStartEndHandler.MatchState.NotPlaying)
						{
							this._matchState = ConsoleMatchStartEndHandler.MatchState.Playing;
							PlatformServices.MultiplayerGameStateChanged(true);
							return;
						}
					}
					else if (this._matchState == ConsoleMatchStartEndHandler.MatchState.Playing)
					{
						this._matchState = ConsoleMatchStartEndHandler.MatchState.NotPlaying;
						PlatformServices.MultiplayerGameStateChanged(false);
					}
				}
			}
		}

		// Token: 0x04000CDF RID: 3295
		private MissionMultiplayerGameModeBaseClient _gameModeClient;

		// Token: 0x04000CE0 RID: 3296
		private MultiplayerMissionAgentVisualSpawnComponent _visualSpawnComponent;

		// Token: 0x04000CE1 RID: 3297
		private ConsoleMatchStartEndHandler.MatchState _matchState;

		// Token: 0x04000CE2 RID: 3298
		private bool _inGameCheckActive;

		// Token: 0x04000CE3 RID: 3299
		private float _playingCheckTimer;

		// Token: 0x04000CE4 RID: 3300
		private List<VirtualPlayer> _activeOtherPlayers;

		// Token: 0x02000594 RID: 1428
		private enum MatchState
		{
			// Token: 0x04001DAA RID: 7594
			NotPlaying,
			// Token: 0x04001DAB RID: 7595
			Playing
		}
	}
}
