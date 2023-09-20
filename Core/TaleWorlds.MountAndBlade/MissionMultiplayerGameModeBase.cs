using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002A1 RID: 673
	public abstract class MissionMultiplayerGameModeBase : MissionNetwork
	{
		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x060024FA RID: 9466
		public abstract bool IsGameModeHidingAllAgentVisuals { get; }

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x060024FB RID: 9467
		public abstract bool IsGameModeUsingOpposingTeams { get; }

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x060024FC RID: 9468 RVA: 0x0008A866 File Offset: 0x00088A66
		// (set) Token: 0x060024FD RID: 9469 RVA: 0x0008A86E File Offset: 0x00088A6E
		public SpawnComponent SpawnComponent { get; private set; }

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x060024FE RID: 9470 RVA: 0x0008A877 File Offset: 0x00088A77
		// (set) Token: 0x060024FF RID: 9471 RVA: 0x0008A87F File Offset: 0x00088A7F
		public MultiplayerMissionAgentVisualSpawnComponent AgentVisualSpawnComponent { get; private set; }

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x06002500 RID: 9472 RVA: 0x0008A888 File Offset: 0x00088A88
		// (set) Token: 0x06002501 RID: 9473 RVA: 0x0008A890 File Offset: 0x00088A90
		private protected bool CanGameModeSystemsTickThisFrame { protected get; private set; }

		// Token: 0x06002502 RID: 9474
		public abstract MissionLobbyComponent.MultiplayerGameType GetMissionType();

		// Token: 0x06002503 RID: 9475 RVA: 0x0008A899 File Offset: 0x00088A99
		public virtual bool CheckIfOvertime()
		{
			return false;
		}

		// Token: 0x06002504 RID: 9476 RVA: 0x0008A89C File Offset: 0x00088A9C
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

		// Token: 0x06002505 RID: 9477 RVA: 0x0008A958 File Offset: 0x00088B58
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (Mission.Current.CurrentTime - this._lastPerkTickTime >= 1f)
			{
				this._lastPerkTickTime = Mission.Current.CurrentTime;
				MPPerkObject.TickAllPeerPerks((int)(this._lastPerkTickTime / 1f));
			}
		}

		// Token: 0x06002506 RID: 9478 RVA: 0x0008A9A6 File Offset: 0x00088BA6
		public virtual bool CheckForWarmupEnd()
		{
			return false;
		}

		// Token: 0x06002507 RID: 9479 RVA: 0x0008A9A9 File Offset: 0x00088BA9
		public virtual bool CheckForRoundEnd()
		{
			return false;
		}

		// Token: 0x06002508 RID: 9480 RVA: 0x0008A9AC File Offset: 0x00088BAC
		public virtual bool CheckForMatchEnd()
		{
			return false;
		}

		// Token: 0x06002509 RID: 9481 RVA: 0x0008A9AF File Offset: 0x00088BAF
		public virtual bool UseCultureSelection()
		{
			return false;
		}

		// Token: 0x0600250A RID: 9482 RVA: 0x0008A9B2 File Offset: 0x00088BB2
		public virtual bool UseRoundController()
		{
			return false;
		}

		// Token: 0x0600250B RID: 9483 RVA: 0x0008A9B5 File Offset: 0x00088BB5
		public virtual Team GetWinnerTeam()
		{
			return null;
		}

		// Token: 0x0600250C RID: 9484 RVA: 0x0008A9B8 File Offset: 0x00088BB8
		public virtual void OnPeerChangedTeam(NetworkCommunicator peer, Team oldTeam, Team newTeam)
		{
		}

		// Token: 0x0600250D RID: 9485 RVA: 0x0008A9BA File Offset: 0x00088BBA
		public override void OnMissionRestart()
		{
			base.OnMissionRestart();
			this.ClearPeerCounts();
			this._lastPerkTickTime = Mission.Current.CurrentTime;
		}

		// Token: 0x0600250E RID: 9486 RVA: 0x0008A9D8 File Offset: 0x00088BD8
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

		// Token: 0x0600250F RID: 9487 RVA: 0x0008AA5C File Offset: 0x00088C5C
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

		// Token: 0x06002510 RID: 9488 RVA: 0x0008AABC File Offset: 0x00088CBC
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

		// Token: 0x06002511 RID: 9489 RVA: 0x0008AB7D File Offset: 0x00088D7D
		public virtual bool AllowCustomPlayerBanners()
		{
			return true;
		}

		// Token: 0x06002512 RID: 9490 RVA: 0x0008AB80 File Offset: 0x00088D80
		public int GetScoreForKill(Agent killedAgent)
		{
			return 20;
		}

		// Token: 0x06002513 RID: 9491 RVA: 0x0008AB84 File Offset: 0x00088D84
		public virtual float GetTroopNumberMultiplierForMissingPlayer(MissionPeer spawningPeer)
		{
			return 1f;
		}

		// Token: 0x06002514 RID: 9492 RVA: 0x0008AB8B File Offset: 0x00088D8B
		public int GetCurrentGoldForPeer(MissionPeer peer)
		{
			return peer.Representative.Gold;
		}

		// Token: 0x06002515 RID: 9493 RVA: 0x0008AB98 File Offset: 0x00088D98
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

		// Token: 0x06002516 RID: 9494 RVA: 0x0008AC00 File Offset: 0x00088E00
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

		// Token: 0x06002517 RID: 9495 RVA: 0x0008AC84 File Offset: 0x00088E84
		public virtual bool CheckIfPlayerCanDespawn(MissionPeer missionPeer)
		{
			return false;
		}

		// Token: 0x06002518 RID: 9496 RVA: 0x0008AC87 File Offset: 0x00088E87
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

		// Token: 0x04000D92 RID: 3474
		public const int GoldCap = 2000;

		// Token: 0x04000D93 RID: 3475
		public const float PerkTickPeriod = 1f;

		// Token: 0x04000D94 RID: 3476
		public const float GameModeSystemTickPeriod = 0.25f;

		// Token: 0x04000D95 RID: 3477
		private float _lastPerkTickTime;

		// Token: 0x04000D98 RID: 3480
		public MultiplayerTeamSelectComponent MultiplayerTeamSelectComponent;

		// Token: 0x04000D99 RID: 3481
		protected MissionLobbyComponent MissionLobbyComponent;

		// Token: 0x04000D9A RID: 3482
		protected MultiplayerGameNotificationsComponent NotificationsComponent;

		// Token: 0x04000D9B RID: 3483
		public MultiplayerRoundController RoundController;

		// Token: 0x04000D9C RID: 3484
		public MultiplayerWarmupComponent WarmupComponent;

		// Token: 0x04000D9D RID: 3485
		public MultiplayerTimerComponent TimerComponent;

		// Token: 0x04000D9E RID: 3486
		protected MissionMultiplayerGameModeBaseClient GameModeBaseClient;

		// Token: 0x04000DA0 RID: 3488
		private float _gameModeSystemTickTimer;
	}
}
