using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
	public class ConsoleMatchStartEndHandler : MissionNetwork
	{
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._activeOtherPlayers = new List<VirtualPlayer>();
			this._matchState = ConsoleMatchStartEndHandler.MatchState.NotPlaying;
			this._gameModeClient = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this._visualSpawnComponent = base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
			this._visualSpawnComponent.OnMyAgentSpawnedFromVisual += this.AgentVisualSpawnComponentOnOnMyAgentVisualSpawned;
			MissionPeer.OnTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.OnTeamChange);
		}

		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			MissionPeer.OnTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.OnTeamChange);
			if (this._matchState == ConsoleMatchStartEndHandler.MatchState.Playing)
			{
				this._matchState = ConsoleMatchStartEndHandler.MatchState.NotPlaying;
				PlatformServices.MultiplayerGameStateChanged(false);
			}
		}

		private void AgentVisualSpawnComponentOnOnMyAgentVisualSpawned()
		{
			if (!this._gameModeClient.IsInWarmup)
			{
				this._visualSpawnComponent.OnMyAgentSpawnedFromVisual -= this.AgentVisualSpawnComponentOnOnMyAgentVisualSpawned;
				this._inGameCheckActive = true;
			}
		}

		private void OnTeamChange(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (newTeam.Side == -1)
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

		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.MissionPeer != null && !agent.MissionPeer.IsMine && !this._activeOtherPlayers.Contains(agent.MissionPeer.Peer))
			{
				this._activeOtherPlayers.Add(agent.MissionPeer.Peer);
			}
		}

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

		private MissionMultiplayerGameModeBaseClient _gameModeClient;

		private MultiplayerMissionAgentVisualSpawnComponent _visualSpawnComponent;

		private ConsoleMatchStartEndHandler.MatchState _matchState;

		private bool _inGameCheckActive;

		private float _playingCheckTimer;

		private List<VirtualPlayer> _activeOtherPlayers;

		private enum MatchState
		{
			NotPlaying,
			Playing
		}
	}
}
