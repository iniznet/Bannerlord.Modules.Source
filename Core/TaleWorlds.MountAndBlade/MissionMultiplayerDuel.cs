using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200029E RID: 670
	public class MissionMultiplayerDuel : MissionMultiplayerGameModeBase
	{
		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x0600248E RID: 9358 RVA: 0x000873A3 File Offset: 0x000855A3
		public override bool IsGameModeHidingAllAgentVisuals
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x0600248F RID: 9359 RVA: 0x000873A6 File Offset: 0x000855A6
		public override bool IsGameModeUsingOpposingTeams
		{
			get
			{
				return false;
			}
		}

		// Token: 0x14000054 RID: 84
		// (add) Token: 0x06002490 RID: 9360 RVA: 0x000873AC File Offset: 0x000855AC
		// (remove) Token: 0x06002491 RID: 9361 RVA: 0x000873E4 File Offset: 0x000855E4
		public event MissionMultiplayerDuel.OnDuelEndedDelegate OnDuelEnded;

		// Token: 0x06002492 RID: 9362 RVA: 0x00087419 File Offset: 0x00085619
		public override MissionLobbyComponent.MultiplayerGameType GetMissionType()
		{
			return MissionLobbyComponent.MultiplayerGameType.Duel;
		}

		// Token: 0x06002493 RID: 9363 RVA: 0x0008741C File Offset: 0x0008561C
		public override void AfterStart()
		{
			base.AfterStart();
			Mission.Current.SetMissionCorpseFadeOutTimeInSeconds(1f);
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			Banner banner = new Banner(@object.BannerKey, @object.BackgroundColor1, @object.ForegroundColor1);
			base.Mission.Teams.Add(BattleSideEnum.Attacker, @object.BackgroundColor1, @object.ForegroundColor1, banner, false, false, true);
		}

		// Token: 0x06002494 RID: 9364 RVA: 0x0008748C File Offset: 0x0008568C
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._duelAreaFlags.AddRange(Mission.Current.Scene.FindEntitiesWithTagExpression("area_flag(_\\d+)*"));
			List<GameEntity> list = new List<GameEntity>();
			list.AddRange(Mission.Current.Scene.FindEntitiesWithTagExpression("area_box(_\\d+)*"));
			this._cachedSelectedAreaFlags = new KeyValuePair<int, TroopType>[this._duelAreaFlags.Count];
			for (int i = 0; i < list.Count; i++)
			{
				VolumeBox firstScriptOfType = list[i].GetFirstScriptOfType<VolumeBox>();
				this._areaBoxes.Add(firstScriptOfType);
			}
			this._cachedSelectedVolumeBoxes = new VolumeBox[this._areaBoxes.Count];
		}

		// Token: 0x06002495 RID: 9365 RVA: 0x00087534 File Offset: 0x00085734
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			registerer.Register<NetworkMessages.FromClient.DuelRequest>(new GameNetworkMessage.ClientMessageHandlerDelegate<NetworkMessages.FromClient.DuelRequest>(this.HandleClientEventDuelRequest));
			registerer.Register<DuelResponse>(new GameNetworkMessage.ClientMessageHandlerDelegate<DuelResponse>(this.HandleClientEventDuelRequestAccepted));
			registerer.Register<RequestChangePreferredTroopType>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestChangePreferredTroopType>(this.HandleClientEventDuelRequestChangePreferredTroopType));
		}

		// Token: 0x06002496 RID: 9366 RVA: 0x0008756C File Offset: 0x0008576C
		protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			networkPeer.AddComponent<DuelMissionRepresentative>();
		}

		// Token: 0x06002497 RID: 9367 RVA: 0x00087578 File Offset: 0x00085778
		protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			component.Team = base.Mission.AttackerTeam;
			this._peersAndSelections.Add(new KeyValuePair<MissionPeer, TroopType>(component, TroopType.Invalid));
		}

		// Token: 0x06002498 RID: 9368 RVA: 0x000875B0 File Offset: 0x000857B0
		private bool HandleClientEventDuelRequest(NetworkCommunicator peer, NetworkMessages.FromClient.DuelRequest message)
		{
			if (peer != null && peer.GetComponent<MissionPeer>() != null && peer.GetComponent<MissionPeer>().ControlledAgent != null && message.RequestedAgent != null && message.RequestedAgent.IsActive())
			{
				this.DuelRequestReceived(peer.GetComponent<MissionPeer>(), message.RequestedAgent.MissionPeer);
			}
			return true;
		}

		// Token: 0x06002499 RID: 9369 RVA: 0x00087604 File Offset: 0x00085804
		private bool HandleClientEventDuelRequestAccepted(NetworkCommunicator peer, DuelResponse message)
		{
			if (((peer != null) ? peer.GetComponent<MissionPeer>() : null) != null && peer.GetComponent<MissionPeer>().ControlledAgent != null)
			{
				NetworkCommunicator peer2 = message.Peer;
				if (((peer2 != null) ? peer2.GetComponent<MissionPeer>() : null) != null && message.Peer.GetComponent<MissionPeer>().ControlledAgent != null)
				{
					this.DuelRequestAccepted(message.Peer.GetComponent<DuelMissionRepresentative>().ControlledAgent, peer.GetComponent<DuelMissionRepresentative>().ControlledAgent);
				}
			}
			return true;
		}

		// Token: 0x0600249A RID: 9370 RVA: 0x00087674 File Offset: 0x00085874
		private bool HandleClientEventDuelRequestChangePreferredTroopType(NetworkCommunicator peer, RequestChangePreferredTroopType message)
		{
			this.OnPeerSelectedPreferredTroopType(peer.GetComponent<MissionPeer>(), message.TroopType);
			return true;
		}

		// Token: 0x0600249B RID: 9371 RVA: 0x0008768C File Offset: 0x0008588C
		public override bool CheckIfPlayerCanDespawn(MissionPeer missionPeer)
		{
			for (int i = 0; i < this._activeDuels.Count; i++)
			{
				if (this._activeDuels[i].IsPeerInThisDuel(missionPeer))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600249C RID: 9372 RVA: 0x000876C6 File Offset: 0x000858C6
		public void OnPlayerDespawn(MissionPeer missionPeer)
		{
			missionPeer.GetComponent<DuelMissionRepresentative>();
		}

		// Token: 0x0600249D RID: 9373 RVA: 0x000876D0 File Offset: 0x000858D0
		public void DuelRequestReceived(MissionPeer requesterPeer, MissionPeer requesteePeer)
		{
			if (!this.IsThereARequestBetweenPeers(requesterPeer, requesteePeer) && !this.IsHavingDuel(requesterPeer) && !this.IsHavingDuel(requesteePeer))
			{
				MissionMultiplayerDuel.DuelInfo duelInfo = new MissionMultiplayerDuel.DuelInfo(requesterPeer, requesteePeer, this.GetNextAvailableDuelAreaIndex(requesterPeer.ControlledAgent));
				this._duelRequests.Add(duelInfo);
				(requesteePeer.Representative as DuelMissionRepresentative).DuelRequested(requesterPeer.ControlledAgent, duelInfo.DuelAreaTroopType);
			}
		}

		// Token: 0x0600249E RID: 9374 RVA: 0x00087738 File Offset: 0x00085938
		private KeyValuePair<int, TroopType> GetNextAvailableDuelAreaIndex(Agent requesterAgent)
		{
			TroopType troopType = TroopType.Invalid;
			for (int i = 0; i < this._peersAndSelections.Count; i++)
			{
				if (this._peersAndSelections[i].Key == requesterAgent.MissionPeer)
				{
					troopType = this._peersAndSelections[i].Value;
					break;
				}
			}
			if (troopType == TroopType.Invalid)
			{
				troopType = this.GetAgentTroopType(requesterAgent);
			}
			bool flag = false;
			int num = 0;
			for (int j = 0; j < this._duelAreaFlags.Count; j++)
			{
				GameEntity gameEntity = this._duelAreaFlags[j];
				int num2 = int.Parse(gameEntity.Tags.Single((string ft) => ft.StartsWith("area_flag_")).Replace("area_flag_", ""));
				int flagIndex = num2 - 1;
				if (this._activeDuels.All((MissionMultiplayerDuel.DuelInfo ad) => ad.DuelAreaIndex != flagIndex) && this._restartingDuels.All((MissionMultiplayerDuel.DuelInfo ad) => ad.DuelAreaIndex != flagIndex) && this._restartPreparationDuels.All((MissionMultiplayerDuel.DuelInfo ad) => ad.DuelAreaIndex != flagIndex))
				{
					TroopType troopType2 = (gameEntity.HasTag("flag_infantry") ? TroopType.Infantry : (gameEntity.HasTag("flag_archery") ? TroopType.Ranged : TroopType.Cavalry));
					if (!flag && troopType2 == troopType)
					{
						flag = true;
						num = 0;
					}
					if (!flag || troopType2 == troopType)
					{
						this._cachedSelectedAreaFlags[num] = new KeyValuePair<int, TroopType>(flagIndex, troopType2);
						num++;
					}
				}
			}
			return this._cachedSelectedAreaFlags[MBRandom.RandomInt(num)];
		}

		// Token: 0x0600249F RID: 9375 RVA: 0x000878E0 File Offset: 0x00085AE0
		public void DuelRequestAccepted(Agent requesterAgent, Agent requesteeAgent)
		{
			MissionMultiplayerDuel.DuelInfo duelInfo = this._duelRequests.FirstOrDefault((MissionMultiplayerDuel.DuelInfo dr) => dr.IsPeerInThisDuel(requesterAgent.MissionPeer) && dr.IsPeerInThisDuel(requesteeAgent.MissionPeer));
			if (duelInfo != null)
			{
				this.PrepareDuel(duelInfo);
			}
		}

		// Token: 0x060024A0 RID: 9376 RVA: 0x00087923 File Offset: 0x00085B23
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			this.CheckRestartPreparationDuels();
			this.CheckForRestartingDuels();
			this.CheckDuelsToStart();
			this.CheckDuelRequestTimeouts();
			this.CheckEndedDuels();
		}

		// Token: 0x060024A1 RID: 9377 RVA: 0x0008794C File Offset: 0x00085B4C
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (!affectedAgent.IsHuman)
			{
				return;
			}
			if (affectedAgent.MissionPeer.Team.IsDefender)
			{
				MissionMultiplayerDuel.DuelInfo duelInfo = null;
				for (int i = 0; i < this._activeDuels.Count; i++)
				{
					if (this._activeDuels[i].IsPeerInThisDuel(affectedAgent.MissionPeer))
					{
						duelInfo = this._activeDuels[i];
					}
				}
				if (duelInfo != null && !this._endingDuels.Contains(duelInfo))
				{
					duelInfo.OnDuelEnding();
					this._endingDuels.Add(duelInfo);
					return;
				}
			}
			else
			{
				for (int j = this._duelRequests.Count - 1; j >= 0; j--)
				{
					if (this._duelRequests[j].IsPeerInThisDuel(affectedAgent.MissionPeer))
					{
						this._duelRequests.RemoveAt(j);
					}
				}
			}
		}

		// Token: 0x060024A2 RID: 9378 RVA: 0x00087A13 File Offset: 0x00085C13
		private Team ActivateAndGetDuelTeam()
		{
			if (this._deactiveDuelTeams.Count <= 0)
			{
				return base.Mission.Teams.Add(BattleSideEnum.Defender, uint.MaxValue, uint.MaxValue, null, true, false, false);
			}
			return this._deactiveDuelTeams.Dequeue();
		}

		// Token: 0x060024A3 RID: 9379 RVA: 0x00087A46 File Offset: 0x00085C46
		private void DeactivateDuelTeam(Team team)
		{
			this._deactiveDuelTeams.Enqueue(team);
		}

		// Token: 0x060024A4 RID: 9380 RVA: 0x00087A54 File Offset: 0x00085C54
		private bool IsHavingDuel(MissionPeer peer)
		{
			return this._activeDuels.AnyQ((MissionMultiplayerDuel.DuelInfo d) => d.IsPeerInThisDuel(peer));
		}

		// Token: 0x060024A5 RID: 9381 RVA: 0x00087A88 File Offset: 0x00085C88
		private bool IsThereARequestBetweenPeers(MissionPeer requesterAgent, MissionPeer requesteeAgent)
		{
			for (int i = 0; i < this._duelRequests.Count; i++)
			{
				if (this._duelRequests[i].IsPeerInThisDuel(requesterAgent) && this._duelRequests[i].IsPeerInThisDuel(requesteeAgent))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060024A6 RID: 9382 RVA: 0x00087AD8 File Offset: 0x00085CD8
		private void CheckDuelsToStart()
		{
			for (int i = this._activeDuels.Count - 1; i >= 0; i--)
			{
				MissionMultiplayerDuel.DuelInfo duelInfo = this._activeDuels[i];
				if (!duelInfo.Started && duelInfo.Timer.IsPast && duelInfo.IsDuelStillValid(false))
				{
					this.StartDuel(duelInfo);
				}
			}
		}

		// Token: 0x060024A7 RID: 9383 RVA: 0x00087B34 File Offset: 0x00085D34
		private void CheckDuelRequestTimeouts()
		{
			for (int i = this._duelRequests.Count - 1; i >= 0; i--)
			{
				MissionMultiplayerDuel.DuelInfo duelInfo = this._duelRequests[i];
				if (duelInfo.Timer.IsPast)
				{
					this._duelRequests.Remove(duelInfo);
				}
			}
		}

		// Token: 0x060024A8 RID: 9384 RVA: 0x00087B84 File Offset: 0x00085D84
		private void CheckForRestartingDuels()
		{
			for (int i = this._restartingDuels.Count - 1; i >= 0; i--)
			{
				if (!this._restartingDuels[i].IsDuelStillValid(true))
				{
					Debug.Print("!_restartingDuels[i].IsDuelStillValid(true)", 0, Debug.DebugColor.White, 17592186044416UL);
				}
				this._duelRequests.Add(this._restartingDuels[i]);
				this.PrepareDuel(this._restartingDuels[i]);
				this._restartingDuels.RemoveAt(i);
			}
		}

		// Token: 0x060024A9 RID: 9385 RVA: 0x00087C08 File Offset: 0x00085E08
		private void CheckEndedDuels()
		{
			for (int i = this._endingDuels.Count - 1; i >= 0; i--)
			{
				MissionMultiplayerDuel.DuelInfo duelInfo = this._endingDuels[i];
				if (duelInfo.Timer.IsPast)
				{
					this.EndDuel(duelInfo);
					this._endingDuels.RemoveAt(i);
					if (!duelInfo.ChallengeEnded)
					{
						this._restartPreparationDuels.Add(duelInfo);
					}
				}
			}
		}

		// Token: 0x060024AA RID: 9386 RVA: 0x00087C74 File Offset: 0x00085E74
		private void CheckRestartPreparationDuels()
		{
			for (int i = this._restartPreparationDuels.Count - 1; i >= 0; i--)
			{
				MissionMultiplayerDuel.DuelInfo duelInfo = this._restartPreparationDuels[i];
				Agent controlledAgent = duelInfo.RequesterPeer.ControlledAgent;
				Agent controlledAgent2 = duelInfo.RequesteePeer.ControlledAgent;
				if ((controlledAgent == null || controlledAgent.IsActive()) && (controlledAgent2 == null || controlledAgent2.IsActive()))
				{
					this._restartPreparationDuels.RemoveAt(i);
					this._restartingDuels.Add(duelInfo);
				}
			}
		}

		// Token: 0x060024AB RID: 9387 RVA: 0x00087CEC File Offset: 0x00085EEC
		private void PrepareDuel(MissionMultiplayerDuel.DuelInfo duel)
		{
			this._duelRequests.Remove(duel);
			if (!this.IsHavingDuel(duel.RequesteePeer) && !this.IsHavingDuel(duel.RequesterPeer))
			{
				this._activeDuels.Add(duel);
				Team team = (duel.Started ? duel.DuelingTeam : this.ActivateAndGetDuelTeam());
				duel.OnDuelPreparation(team);
				for (int i = 0; i < this._duelRequests.Count; i++)
				{
					if (this._duelRequests[i].DuelAreaIndex == duel.DuelAreaIndex)
					{
						this._duelRequests[i].UpdateDuelAreaIndex(this.GetNextAvailableDuelAreaIndex(this._duelRequests[i].RequesterPeer.ControlledAgent));
					}
				}
				return;
			}
			Debug.FailedAssert("IsHavingDuel(duel.RequesteePeer) || IsHavingDuel(duel.RequesterPeer)", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MultiplayerGameModeLogics\\ServerGameModeLogics\\MissionMultiplayerDuel.cs", "PrepareDuel", 697);
		}

		// Token: 0x060024AC RID: 9388 RVA: 0x00087DC8 File Offset: 0x00085FC8
		private void StartDuel(MissionMultiplayerDuel.DuelInfo duel)
		{
			duel.OnDuelStarted();
		}

		// Token: 0x060024AD RID: 9389 RVA: 0x00087DD0 File Offset: 0x00085FD0
		private void EndDuel(MissionMultiplayerDuel.DuelInfo duel)
		{
			this._activeDuels.Remove(duel);
			duel.OnDuelEnded();
			this.CleanSpawnedEntitiesInDuelArea(duel.DuelAreaIndex);
			if (duel.ChallengeEnded)
			{
				TroopType troopType = TroopType.Invalid;
				MissionPeer challengeWinnerPeer = duel.ChallengeWinnerPeer;
				if (((challengeWinnerPeer != null) ? challengeWinnerPeer.ControlledAgent : null) != null)
				{
					troopType = this.GetAgentTroopType(challengeWinnerPeer.ControlledAgent);
				}
				MissionMultiplayerDuel.OnDuelEndedDelegate onDuelEnded = this.OnDuelEnded;
				if (onDuelEnded != null)
				{
					onDuelEnded(challengeWinnerPeer, troopType);
				}
				this.DeactivateDuelTeam(duel.DuelingTeam);
				this.HandleEndedChallenge(duel);
			}
		}

		// Token: 0x060024AE RID: 9390 RVA: 0x00087E50 File Offset: 0x00086050
		private TroopType GetAgentTroopType(Agent requesterAgent)
		{
			TroopType troopType = TroopType.Invalid;
			switch (requesterAgent.Character.DefaultFormationClass)
			{
			case FormationClass.Infantry:
			case FormationClass.HeavyInfantry:
				troopType = TroopType.Infantry;
				break;
			case FormationClass.Ranged:
				troopType = TroopType.Ranged;
				break;
			case FormationClass.Cavalry:
			case FormationClass.HorseArcher:
			case FormationClass.LightCavalry:
			case FormationClass.HeavyCavalry:
				troopType = TroopType.Cavalry;
				break;
			}
			return troopType;
		}

		// Token: 0x060024AF RID: 9391 RVA: 0x00087EA0 File Offset: 0x000860A0
		private void CleanSpawnedEntitiesInDuelArea(int duelAreaIndex)
		{
			int num = duelAreaIndex + 1;
			int num2 = 0;
			for (int i = 0; i < this._areaBoxes.Count; i++)
			{
				if (this._areaBoxes[i].GameEntity.HasTag(string.Format("{0}_{1}", "area_box", num)))
				{
					this._cachedSelectedVolumeBoxes[num2] = this._areaBoxes[i];
					num2++;
				}
			}
			for (int j = 0; j < Mission.Current.ActiveMissionObjects.Count; j++)
			{
				SpawnedItemEntity spawnedItemEntity;
				if ((spawnedItemEntity = Mission.Current.ActiveMissionObjects[j] as SpawnedItemEntity) != null && !spawnedItemEntity.IsDeactivated)
				{
					for (int k = 0; k < num2; k++)
					{
						if (this._cachedSelectedVolumeBoxes[k].IsPointIn(spawnedItemEntity.GameEntity.GlobalPosition))
						{
							spawnedItemEntity.RequestDeletionOnNextTick();
							break;
						}
					}
				}
			}
		}

		// Token: 0x060024B0 RID: 9392 RVA: 0x00087F80 File Offset: 0x00086180
		private void HandleEndedChallenge(MissionMultiplayerDuel.DuelInfo duel)
		{
			MissionPeer challengeWinnerPeer = duel.ChallengeWinnerPeer;
			MissionPeer challengeLoserPeer = duel.ChallengeLoserPeer;
			if (challengeWinnerPeer != null)
			{
				DuelMissionRepresentative component = challengeWinnerPeer.GetComponent<DuelMissionRepresentative>();
				DuelMissionRepresentative component2 = challengeLoserPeer.GetComponent<DuelMissionRepresentative>();
				MultiplayerClassDivisions.MPHeroClass mpheroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(challengeWinnerPeer, true);
				MultiplayerClassDivisions.MPHeroClass mpheroClassForPeer2 = MultiplayerClassDivisions.GetMPHeroClassForPeer(challengeLoserPeer, true);
				float num = (float)MathF.Max(100, component2.Bounty) * MathF.Max(1f, (float)mpheroClassForPeer.TroopCasualCost / (float)mpheroClassForPeer2.TroopCasualCost) * MathF.Pow(2.7182817f, (float)component.NumberOfWins / 10f);
				component.OnDuelWon(num);
				if (challengeWinnerPeer.Peer.Communicator.IsConnectionActive)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new DuelPointsUpdateMessage(component));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
				component2.ResetBountyAndNumberOfWins();
				if (challengeLoserPeer.Peer.Communicator.IsConnectionActive)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new DuelPointsUpdateMessage(component2));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
			}
			PeerComponent peerComponent = challengeWinnerPeer ?? duel.RequesterPeer;
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new DuelEnded(peerComponent.GetNetworkPeer()));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

		// Token: 0x060024B1 RID: 9393 RVA: 0x0008808C File Offset: 0x0008628C
		public int GetDuelAreaIndexIfDuelTeam(Team team)
		{
			if (team.IsDefender)
			{
				return this._activeDuels.FirstOrDefaultQ((MissionMultiplayerDuel.DuelInfo ad) => ad.DuelingTeam == team).DuelAreaIndex;
			}
			return -1;
		}

		// Token: 0x060024B2 RID: 9394 RVA: 0x000880D4 File Offset: 0x000862D4
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.IsHuman && agent.Team != null && agent.Team.IsDefender)
			{
				for (int i = 0; i < this._activeDuels.Count; i++)
				{
					if (this._activeDuels[i].IsPeerInThisDuel(agent.MissionPeer))
					{
						this._activeDuels[i].OnAgentBuild(agent);
						return;
					}
				}
			}
		}

		// Token: 0x060024B3 RID: 9395 RVA: 0x00088140 File Offset: 0x00086340
		protected override void HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			if (!networkPeer.IsServerPeer)
			{
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					DuelMissionRepresentative component = networkCommunicator.GetComponent<DuelMissionRepresentative>();
					if (component != null)
					{
						GameNetwork.BeginModuleEventAsServer(networkPeer);
						GameNetwork.WriteMessage(new DuelPointsUpdateMessage(component));
						GameNetwork.EndModuleEventAsServer();
					}
				}
				for (int i = 0; i < this._activeDuels.Count; i++)
				{
					GameNetwork.BeginModuleEventAsServer(networkPeer);
					GameNetwork.WriteMessage(new DuelPreparationStartedForTheFirstTime(this._activeDuels[i].RequesterPeer.GetNetworkPeer(), this._activeDuels[i].RequesteePeer.GetNetworkPeer(), this._activeDuels[i].DuelAreaIndex));
					GameNetwork.EndModuleEventAsServer();
				}
			}
		}

		// Token: 0x060024B4 RID: 9396 RVA: 0x00088218 File Offset: 0x00086418
		protected override void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			for (int i = 0; i < this._peersAndSelections.Count; i++)
			{
				if (this._peersAndSelections[i].Key == component)
				{
					this._peersAndSelections.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x060024B5 RID: 9397 RVA: 0x00088268 File Offset: 0x00086468
		protected override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (component != null)
			{
				component.Team = null;
			}
		}

		// Token: 0x060024B6 RID: 9398 RVA: 0x00088288 File Offset: 0x00086488
		private void OnPeerSelectedPreferredTroopType(MissionPeer missionPeer, TroopType troopType)
		{
			for (int i = 0; i < this._peersAndSelections.Count; i++)
			{
				if (this._peersAndSelections[i].Key == missionPeer)
				{
					this._peersAndSelections[i] = new KeyValuePair<MissionPeer, TroopType>(missionPeer, troopType);
					return;
				}
			}
		}

		// Token: 0x04000D5A RID: 3418
		public const float DuelRequestTimeOutInSeconds = 10f;

		// Token: 0x04000D5B RID: 3419
		private const int MinBountyGain = 100;

		// Token: 0x04000D5C RID: 3420
		private const string AreaBoxTagPrefix = "area_box";

		// Token: 0x04000D5D RID: 3421
		private const string AreaFlagTagPrefix = "area_flag";

		// Token: 0x04000D5E RID: 3422
		public const int NumberOfDuelAreas = 16;

		// Token: 0x04000D5F RID: 3423
		public const float DuelEndInSeconds = 2f;

		// Token: 0x04000D60 RID: 3424
		private const float DuelRequestTimeOutServerToleranceInSeconds = 0.5f;

		// Token: 0x04000D61 RID: 3425
		private const float CorpseFadeOutTimeInSeconds = 1f;

		// Token: 0x04000D63 RID: 3427
		private List<GameEntity> _duelAreaFlags = new List<GameEntity>();

		// Token: 0x04000D64 RID: 3428
		private List<VolumeBox> _areaBoxes = new List<VolumeBox>();

		// Token: 0x04000D65 RID: 3429
		private List<MissionMultiplayerDuel.DuelInfo> _duelRequests = new List<MissionMultiplayerDuel.DuelInfo>();

		// Token: 0x04000D66 RID: 3430
		private List<MissionMultiplayerDuel.DuelInfo> _activeDuels = new List<MissionMultiplayerDuel.DuelInfo>();

		// Token: 0x04000D67 RID: 3431
		private List<MissionMultiplayerDuel.DuelInfo> _endingDuels = new List<MissionMultiplayerDuel.DuelInfo>();

		// Token: 0x04000D68 RID: 3432
		private List<MissionMultiplayerDuel.DuelInfo> _restartingDuels = new List<MissionMultiplayerDuel.DuelInfo>();

		// Token: 0x04000D69 RID: 3433
		private List<MissionMultiplayerDuel.DuelInfo> _restartPreparationDuels = new List<MissionMultiplayerDuel.DuelInfo>();

		// Token: 0x04000D6A RID: 3434
		private readonly Queue<Team> _deactiveDuelTeams = new Queue<Team>();

		// Token: 0x04000D6B RID: 3435
		private List<KeyValuePair<MissionPeer, TroopType>> _peersAndSelections = new List<KeyValuePair<MissionPeer, TroopType>>();

		// Token: 0x04000D6C RID: 3436
		private VolumeBox[] _cachedSelectedVolumeBoxes;

		// Token: 0x04000D6D RID: 3437
		private KeyValuePair<int, TroopType>[] _cachedSelectedAreaFlags;

		// Token: 0x020005B3 RID: 1459
		private class DuelInfo
		{
			// Token: 0x170009A2 RID: 2466
			// (get) Token: 0x06003B89 RID: 15241 RVA: 0x000EF894 File Offset: 0x000EDA94
			public MissionPeer RequesterPeer
			{
				get
				{
					return this._challengers[0].MissionPeer;
				}
			}

			// Token: 0x170009A3 RID: 2467
			// (get) Token: 0x06003B8A RID: 15242 RVA: 0x000EF8A7 File Offset: 0x000EDAA7
			public MissionPeer RequesteePeer
			{
				get
				{
					return this._challengers[1].MissionPeer;
				}
			}

			// Token: 0x170009A4 RID: 2468
			// (get) Token: 0x06003B8B RID: 15243 RVA: 0x000EF8BA File Offset: 0x000EDABA
			// (set) Token: 0x06003B8C RID: 15244 RVA: 0x000EF8C2 File Offset: 0x000EDAC2
			public int DuelAreaIndex { get; private set; }

			// Token: 0x170009A5 RID: 2469
			// (get) Token: 0x06003B8D RID: 15245 RVA: 0x000EF8CB File Offset: 0x000EDACB
			// (set) Token: 0x06003B8E RID: 15246 RVA: 0x000EF8D3 File Offset: 0x000EDAD3
			public TroopType DuelAreaTroopType { get; private set; }

			// Token: 0x170009A6 RID: 2470
			// (get) Token: 0x06003B8F RID: 15247 RVA: 0x000EF8DC File Offset: 0x000EDADC
			// (set) Token: 0x06003B90 RID: 15248 RVA: 0x000EF8E4 File Offset: 0x000EDAE4
			public MissionTime Timer { get; private set; }

			// Token: 0x170009A7 RID: 2471
			// (get) Token: 0x06003B91 RID: 15249 RVA: 0x000EF8ED File Offset: 0x000EDAED
			// (set) Token: 0x06003B92 RID: 15250 RVA: 0x000EF8F5 File Offset: 0x000EDAF5
			public Team DuelingTeam { get; private set; }

			// Token: 0x170009A8 RID: 2472
			// (get) Token: 0x06003B93 RID: 15251 RVA: 0x000EF8FE File Offset: 0x000EDAFE
			// (set) Token: 0x06003B94 RID: 15252 RVA: 0x000EF906 File Offset: 0x000EDB06
			public bool Started { get; private set; }

			// Token: 0x170009A9 RID: 2473
			// (get) Token: 0x06003B95 RID: 15253 RVA: 0x000EF90F File Offset: 0x000EDB0F
			// (set) Token: 0x06003B96 RID: 15254 RVA: 0x000EF917 File Offset: 0x000EDB17
			public bool ChallengeEnded { get; private set; }

			// Token: 0x170009AA RID: 2474
			// (get) Token: 0x06003B97 RID: 15255 RVA: 0x000EF920 File Offset: 0x000EDB20
			public MissionPeer ChallengeWinnerPeer
			{
				get
				{
					if (this._winnerChallengerType != MissionMultiplayerDuel.DuelInfo.ChallengerType.None)
					{
						return this._challengers[(int)this._winnerChallengerType].MissionPeer;
					}
					return null;
				}
			}

			// Token: 0x170009AB RID: 2475
			// (get) Token: 0x06003B98 RID: 15256 RVA: 0x000EF943 File Offset: 0x000EDB43
			public MissionPeer ChallengeLoserPeer
			{
				get
				{
					if (this._winnerChallengerType != MissionMultiplayerDuel.DuelInfo.ChallengerType.None)
					{
						return this._challengers[(this._winnerChallengerType == MissionMultiplayerDuel.DuelInfo.ChallengerType.Requester) ? 1 : 0].MissionPeer;
					}
					return null;
				}
			}

			// Token: 0x06003B99 RID: 15257 RVA: 0x000EF96C File Offset: 0x000EDB6C
			public DuelInfo(MissionPeer requesterPeer, MissionPeer requesteePeer, KeyValuePair<int, TroopType> duelAreaPair)
			{
				this.DuelAreaIndex = duelAreaPair.Key;
				this.DuelAreaTroopType = duelAreaPair.Value;
				this._challengers = new MissionMultiplayerDuel.DuelInfo.Challenger[2];
				this._challengers[0] = new MissionMultiplayerDuel.DuelInfo.Challenger(requesterPeer);
				this._challengers[1] = new MissionMultiplayerDuel.DuelInfo.Challenger(requesteePeer);
				this.Timer = MissionTime.Now + MissionTime.Seconds(10.5f);
			}

			// Token: 0x06003B9A RID: 15258 RVA: 0x000EF9EC File Offset: 0x000EDBEC
			private void DecideRoundWinner()
			{
				bool isConnectionActive = this._challengers[0].MissionPeer.Peer.Communicator.IsConnectionActive;
				bool isConnectionActive2 = this._challengers[1].MissionPeer.Peer.Communicator.IsConnectionActive;
				if (!this.Started)
				{
					if (isConnectionActive == isConnectionActive2)
					{
						this.ChallengeEnded = true;
					}
					else
					{
						this._winnerChallengerType = (isConnectionActive ? MissionMultiplayerDuel.DuelInfo.ChallengerType.Requester : MissionMultiplayerDuel.DuelInfo.ChallengerType.Requestee);
					}
				}
				else
				{
					Agent duelingAgent = this._challengers[0].DuelingAgent;
					Agent duelingAgent2 = this._challengers[1].DuelingAgent;
					if (duelingAgent.IsActive())
					{
						this._winnerChallengerType = MissionMultiplayerDuel.DuelInfo.ChallengerType.Requester;
					}
					else if (duelingAgent2.IsActive())
					{
						this._winnerChallengerType = MissionMultiplayerDuel.DuelInfo.ChallengerType.Requestee;
					}
					else
					{
						if (!isConnectionActive && !isConnectionActive2)
						{
							this.ChallengeEnded = true;
						}
						this._winnerChallengerType = MissionMultiplayerDuel.DuelInfo.ChallengerType.None;
					}
				}
				if (this._winnerChallengerType != MissionMultiplayerDuel.DuelInfo.ChallengerType.None)
				{
					this._challengers[(int)this._winnerChallengerType].IncreaseWinCount();
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new DuelRoundEnded(this._challengers[(int)this._winnerChallengerType].NetworkPeer));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
					if (this._challengers[(int)this._winnerChallengerType].KillCountInDuel == MultiplayerOptions.OptionType.MinScoreToWinDuel.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) || !isConnectionActive || !isConnectionActive2)
					{
						this.ChallengeEnded = true;
					}
				}
			}

			// Token: 0x06003B9B RID: 15259 RVA: 0x000EFB30 File Offset: 0x000EDD30
			public void OnDuelPreparation(Team duelTeam)
			{
				if (!this.Started)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new DuelPreparationStartedForTheFirstTime(this._challengers[0].MissionPeer.GetNetworkPeer(), this._challengers[1].MissionPeer.GetNetworkPeer(), this.DuelAreaIndex));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				}
				this.Started = false;
				this.DuelingTeam = duelTeam;
				this._winnerChallengerType = MissionMultiplayerDuel.DuelInfo.ChallengerType.None;
				for (int i = 0; i < 2; i++)
				{
					this._challengers[i].OnDuelPreparation(this.DuelingTeam);
					this._challengers[i].MissionPeer.GetComponent<DuelMissionRepresentative>().OnDuelPreparation(this._challengers[0].MissionPeer, this._challengers[1].MissionPeer);
				}
				this.Timer = MissionTime.Now + MissionTime.Seconds(3f);
			}

			// Token: 0x06003B9C RID: 15260 RVA: 0x000EFC1C File Offset: 0x000EDE1C
			public void OnDuelStarted()
			{
				this.Started = true;
				this.DuelingTeam.SetIsEnemyOf(this.DuelingTeam, true);
			}

			// Token: 0x06003B9D RID: 15261 RVA: 0x000EFC37 File Offset: 0x000EDE37
			public void OnDuelEnding()
			{
				this.Timer = MissionTime.Now + MissionTime.Seconds(2f);
			}

			// Token: 0x06003B9E RID: 15262 RVA: 0x000EFC54 File Offset: 0x000EDE54
			public void OnDuelEnded()
			{
				if (this.Started)
				{
					this.DuelingTeam.SetIsEnemyOf(this.DuelingTeam, false);
				}
				this.DecideRoundWinner();
				for (int i = 0; i < 2; i++)
				{
					this._challengers[i].OnDuelEnded();
					Agent agent = this._challengers[i].DuelingAgent ?? this._challengers[i].MissionPeer.ControlledAgent;
					if (this.ChallengeEnded && agent != null && agent.IsActive())
					{
						agent.FadeOut(true, false);
					}
					this._challengers[i].MissionPeer.HasSpawnedAgentVisuals = true;
				}
				for (int j = 0; j < 2; j++)
				{
					if (this._challengers[j].MountAgent != null && this._challengers[j].MountAgent.IsActive() && (this.ChallengeEnded || this._challengers[j].MountAgent.RiderAgent == null))
					{
						this._challengers[j].MountAgent.FadeOut(true, false);
					}
				}
			}

			// Token: 0x06003B9F RID: 15263 RVA: 0x000EFD6C File Offset: 0x000EDF6C
			public void OnAgentBuild(Agent agent)
			{
				for (int i = 0; i < 2; i++)
				{
					if (this._challengers[i].MissionPeer == agent.MissionPeer)
					{
						this._challengers[i].SetAgents(agent);
						return;
					}
				}
			}

			// Token: 0x06003BA0 RID: 15264 RVA: 0x000EFDB4 File Offset: 0x000EDFB4
			public bool IsDuelStillValid(bool doNotCheckAgent = false)
			{
				for (int i = 0; i < 2; i++)
				{
					if (!this._challengers[i].MissionPeer.Peer.Communicator.IsConnectionActive || (!doNotCheckAgent && !this._challengers[i].MissionPeer.IsControlledAgentActive))
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x06003BA1 RID: 15265 RVA: 0x000EFE10 File Offset: 0x000EE010
			public bool IsPeerInThisDuel(MissionPeer peer)
			{
				for (int i = 0; i < 2; i++)
				{
					if (this._challengers[i].MissionPeer == peer)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06003BA2 RID: 15266 RVA: 0x000EFE40 File Offset: 0x000EE040
			public void UpdateDuelAreaIndex(KeyValuePair<int, TroopType> duelAreaPair)
			{
				this.DuelAreaIndex = duelAreaPair.Key;
				this.DuelAreaTroopType = duelAreaPair.Value;
			}

			// Token: 0x04001DE6 RID: 7654
			private const float DuelStartCountdown = 3f;

			// Token: 0x04001DE7 RID: 7655
			private readonly MissionMultiplayerDuel.DuelInfo.Challenger[] _challengers;

			// Token: 0x04001DE8 RID: 7656
			private MissionMultiplayerDuel.DuelInfo.ChallengerType _winnerChallengerType = MissionMultiplayerDuel.DuelInfo.ChallengerType.None;

			// Token: 0x020006FD RID: 1789
			private enum ChallengerType
			{
				// Token: 0x0400233F RID: 9023
				None = -1,
				// Token: 0x04002340 RID: 9024
				Requester,
				// Token: 0x04002341 RID: 9025
				Requestee,
				// Token: 0x04002342 RID: 9026
				NumChallengerType
			}

			// Token: 0x020006FE RID: 1790
			private struct Challenger
			{
				// Token: 0x17000A22 RID: 2594
				// (get) Token: 0x06004071 RID: 16497 RVA: 0x000F9F9D File Offset: 0x000F819D
				// (set) Token: 0x06004072 RID: 16498 RVA: 0x000F9FA5 File Offset: 0x000F81A5
				public Agent DuelingAgent { get; private set; }

				// Token: 0x17000A23 RID: 2595
				// (get) Token: 0x06004073 RID: 16499 RVA: 0x000F9FAE File Offset: 0x000F81AE
				// (set) Token: 0x06004074 RID: 16500 RVA: 0x000F9FB6 File Offset: 0x000F81B6
				public Agent MountAgent { get; private set; }

				// Token: 0x17000A24 RID: 2596
				// (get) Token: 0x06004075 RID: 16501 RVA: 0x000F9FBF File Offset: 0x000F81BF
				// (set) Token: 0x06004076 RID: 16502 RVA: 0x000F9FC7 File Offset: 0x000F81C7
				public int KillCountInDuel { get; private set; }

				// Token: 0x06004077 RID: 16503 RVA: 0x000F9FD0 File Offset: 0x000F81D0
				public Challenger(MissionPeer missionPeer)
				{
					this.MissionPeer = missionPeer;
					MissionPeer missionPeer2 = this.MissionPeer;
					this.NetworkPeer = ((missionPeer2 != null) ? missionPeer2.GetNetworkPeer() : null);
					this.DuelingAgent = null;
					this.MountAgent = null;
					this.KillCountInDuel = 0;
				}

				// Token: 0x06004078 RID: 16504 RVA: 0x000FA006 File Offset: 0x000F8206
				public void OnDuelPreparation(Team duelingTeam)
				{
					Agent controlledAgent = this.MissionPeer.ControlledAgent;
					if (controlledAgent != null)
					{
						controlledAgent.FadeOut(true, true);
					}
					this.MissionPeer.Team = duelingTeam;
					this.MissionPeer.HasSpawnedAgentVisuals = true;
				}

				// Token: 0x06004079 RID: 16505 RVA: 0x000FA038 File Offset: 0x000F8238
				public void OnDuelEnded()
				{
					if (this.MissionPeer.Peer.Communicator.IsConnectionActive)
					{
						this.MissionPeer.Team = Mission.Current.AttackerTeam;
					}
				}

				// Token: 0x0600407A RID: 16506 RVA: 0x000FA068 File Offset: 0x000F8268
				public void IncreaseWinCount()
				{
					int killCountInDuel = this.KillCountInDuel;
					this.KillCountInDuel = killCountInDuel + 1;
				}

				// Token: 0x0600407B RID: 16507 RVA: 0x000FA085 File Offset: 0x000F8285
				public void SetAgents(Agent agent)
				{
					this.DuelingAgent = agent;
					this.MountAgent = this.DuelingAgent.MountAgent;
				}

				// Token: 0x04002343 RID: 9027
				public readonly MissionPeer MissionPeer;

				// Token: 0x04002344 RID: 9028
				public readonly NetworkCommunicator NetworkPeer;
			}
		}

		// Token: 0x020005B4 RID: 1460
		// (Invoke) Token: 0x06003BA4 RID: 15268
		public delegate void OnDuelEndedDelegate(MissionPeer winnerPeer, TroopType troopType);
	}
}
