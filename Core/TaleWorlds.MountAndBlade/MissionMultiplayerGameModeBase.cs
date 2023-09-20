using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public abstract class MissionMultiplayerGameModeBase : MissionNetwork
	{
		public abstract bool IsGameModeHidingAllAgentVisuals { get; }

		public abstract bool IsGameModeUsingOpposingTeams { get; }

		public SpawnComponent SpawnComponent { get; private set; }

		public MultiplayerMissionAgentVisualSpawnComponent AgentVisualSpawnComponent { get; private set; }

		private protected bool CanGameModeSystemsTickThisFrame { protected get; private set; }

		public abstract MissionLobbyComponent.MultiplayerGameType GetMissionType();

		public virtual bool CheckIfOvertime()
		{
			return false;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this.MultiplayerTeamSelectComponent = base.Mission.GetMissionBehavior<MultiplayerTeamSelectComponent>();
			this.MissionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this.GameModeBaseClient = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this.NotificationsComponent = base.Mission.GetMissionBehavior<MultiplayerGameNotificationsComponent>();
			this.RoundController = base.Mission.GetMissionBehavior<MultiplayerRoundController>();
			this.WarmupComponent = base.Mission.GetMissionBehavior<MultiplayerWarmupComponent>();
			this.TimerComponent = base.Mission.GetMissionBehavior<MultiplayerTimerComponent>();
			this.SpawnComponent = Mission.Current.GetMissionBehavior<SpawnComponent>();
			this.AgentVisualSpawnComponent = base.Mission.GetMissionBehavior<MultiplayerMissionAgentVisualSpawnComponent>();
			this._lastPerkTickTime = Mission.Current.CurrentTime;
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (Mission.Current.CurrentTime - this._lastPerkTickTime >= 1f)
			{
				this._lastPerkTickTime = Mission.Current.CurrentTime;
				MPPerkObject.TickAllPeerPerks((int)(this._lastPerkTickTime / 1f));
			}
		}

		public virtual bool CheckForWarmupEnd()
		{
			return false;
		}

		public virtual bool CheckForRoundEnd()
		{
			return false;
		}

		public virtual bool CheckForMatchEnd()
		{
			return false;
		}

		public virtual bool UseCultureSelection()
		{
			return false;
		}

		public virtual bool UseRoundController()
		{
			return false;
		}

		public virtual Team GetWinnerTeam()
		{
			return null;
		}

		public virtual void OnPeerChangedTeam(NetworkCommunicator peer, Team oldTeam, Team newTeam)
		{
		}

		public override void OnMissionRestart()
		{
			base.OnMissionRestart();
			this.ClearPeerCounts();
			this._lastPerkTickTime = Mission.Current.CurrentTime;
		}

		public void ClearPeerCounts()
		{
			List<MissionPeer> list = VirtualPlayer.Peers<MissionPeer>();
			for (int i = 0; i < list.Count; i++)
			{
				MissionPeer missionPeer = list[i];
				missionPeer.AssistCount = 0;
				missionPeer.DeathCount = 0;
				missionPeer.KillCount = 0;
				missionPeer.Score = 0;
				missionPeer.ResetRequestedKickPollCount();
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new KillDeathCountChange(missionPeer.GetNetworkPeer(), null, missionPeer.KillCount, missionPeer.AssistCount, missionPeer.DeathCount, missionPeer.Score));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
		}

		public bool ShouldSpawnVisualsForServer(NetworkCommunicator spawningNetworkPeer)
		{
			if (GameNetwork.IsDedicatedServer)
			{
				return false;
			}
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			if (missionPeer != null)
			{
				MissionPeer component = spawningNetworkPeer.GetComponent<MissionPeer>();
				return (!this.IsGameModeHidingAllAgentVisuals && component.Team == missionPeer.Team) || spawningNetworkPeer.IsServerPeer;
			}
			return false;
		}

		public void HandleAgentVisualSpawning(NetworkCommunicator spawningNetworkPeer, AgentBuildData spawningAgentBuildData, int troopCountInFormation = 0, bool useCosmetics = true)
		{
			MissionPeer component = spawningNetworkPeer.GetComponent<MissionPeer>();
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new SyncPerksForCurrentlySelectedTroop(spawningNetworkPeer, component.Perks[component.SelectedTroopIndex]));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, spawningNetworkPeer);
			component.HasSpawnedAgentVisuals = true;
			component.EquipmentUpdatingExpired = false;
			if (useCosmetics)
			{
				this.AgentVisualSpawnComponent.AddCosmeticItemsToEquipment(spawningAgentBuildData.AgentOverridenSpawnEquipment, this.AgentVisualSpawnComponent.GetUsedCosmeticsFromPeer(component, spawningAgentBuildData.AgentCharacter));
			}
			if (!this.IsGameModeHidingAllAgentVisuals)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new CreateAgentVisuals(spawningNetworkPeer, spawningAgentBuildData, component.SelectedTroopIndex, troopCountInFormation));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, spawningNetworkPeer);
				return;
			}
			if (!spawningNetworkPeer.IsServerPeer)
			{
				GameNetwork.BeginModuleEventAsServer(spawningNetworkPeer);
				GameNetwork.WriteMessage(new CreateAgentVisuals(spawningNetworkPeer, spawningAgentBuildData, component.SelectedTroopIndex, troopCountInFormation));
				GameNetwork.EndModuleEventAsServer();
			}
		}

		public virtual bool AllowCustomPlayerBanners()
		{
			return true;
		}

		public int GetScoreForKill(Agent killedAgent)
		{
			return 20;
		}

		public virtual float GetTroopNumberMultiplierForMissingPlayer(MissionPeer spawningPeer)
		{
			return 1f;
		}

		public int GetCurrentGoldForPeer(MissionPeer peer)
		{
			return peer.Representative.Gold;
		}

		public void ChangeCurrentGoldForPeer(MissionPeer peer, int newAmount)
		{
			if (newAmount >= 0)
			{
				newAmount = MBMath.ClampInt(newAmount, 0, 2000);
			}
			if (peer.Peer.Communicator.IsConnectionActive)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SyncGoldsForSkirmish(peer.Peer, newAmount));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			}
			if (this.GameModeBaseClient != null)
			{
				this.GameModeBaseClient.OnGoldAmountChangedForRepresentative(peer.Representative, newAmount);
			}
		}

		protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			if (this.GameModeBaseClient.IsGameModeUsingGold)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					if (networkCommunicator != networkPeer)
					{
						MissionRepresentativeBase component = networkCommunicator.GetComponent<MissionRepresentativeBase>();
						if (component != null)
						{
							GameNetwork.BeginModuleEventAsServer(networkPeer);
							GameNetwork.WriteMessage(new SyncGoldsForSkirmish(component.Peer, component.Gold));
							GameNetwork.EndModuleEventAsServer();
						}
					}
				}
			}
		}

		public virtual bool CheckIfPlayerCanDespawn(MissionPeer missionPeer)
		{
			return false;
		}

		public override void OnPreMissionTick(float dt)
		{
			this.CanGameModeSystemsTickThisFrame = false;
			this._gameModeSystemTickTimer += dt;
			if (this._gameModeSystemTickTimer >= 0.25f)
			{
				this._gameModeSystemTickTimer -= 0.25f;
				this.CanGameModeSystemsTickThisFrame = true;
			}
		}

		public const int GoldCap = 2000;

		public const float PerkTickPeriod = 1f;

		public const float GameModeSystemTickPeriod = 0.25f;

		private float _lastPerkTickTime;

		public MultiplayerTeamSelectComponent MultiplayerTeamSelectComponent;

		protected MissionLobbyComponent MissionLobbyComponent;

		protected MultiplayerGameNotificationsComponent NotificationsComponent;

		public MultiplayerRoundController RoundController;

		public MultiplayerWarmupComponent WarmupComponent;

		public MultiplayerTimerComponent TimerComponent;

		protected MissionMultiplayerGameModeBaseClient GameModeBaseClient;

		private float _gameModeSystemTickTimer;
	}
}
