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
	public class MissionMultiplayerDuel : MissionMultiplayerGameModeBase
	{
		public override bool IsGameModeHidingAllAgentVisuals
		{
			get
			{
				return true;
			}
		}

		public override bool IsGameModeUsingOpposingTeams
		{
			get
			{
				return false;
			}
		}

		public event MissionMultiplayerDuel.OnDuelEndedDelegate OnDuelEnded;

		public override MissionLobbyComponent.MultiplayerGameType GetMissionType()
		{
			return MissionLobbyComponent.MultiplayerGameType.Duel;
		}

		public override void AfterStart()
		{
			base.AfterStart();
			Mission.Current.SetMissionCorpseFadeOutTimeInSeconds(1f);
			BasicCultureObject @object = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			Banner banner = new Banner(@object.BannerKey, @object.BackgroundColor1, @object.ForegroundColor1);
			base.Mission.Teams.Add(BattleSideEnum.Attacker, @object.BackgroundColor1, @object.ForegroundColor1, banner, false, false, true);
		}

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

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			registerer.RegisterBaseHandler<NetworkMessages.FromClient.DuelRequest>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventDuelRequest));
			registerer.RegisterBaseHandler<DuelResponse>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventDuelRequestAccepted));
			registerer.RegisterBaseHandler<RequestChangePreferredTroopType>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventDuelRequestChangePreferredTroopType));
		}

		protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
		{
			networkPeer.AddComponent<DuelMissionRepresentative>();
		}

		protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			component.Team = base.Mission.AttackerTeam;
			this._peersAndSelections.Add(new KeyValuePair<MissionPeer, TroopType>(component, TroopType.Invalid));
		}

		private bool HandleClientEventDuelRequest(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			NetworkMessages.FromClient.DuelRequest duelRequest = (NetworkMessages.FromClient.DuelRequest)baseMessage;
			if (peer != null && peer.GetComponent<MissionPeer>() != null && peer.GetComponent<MissionPeer>().ControlledAgent != null && duelRequest.RequestedAgent != null && duelRequest.RequestedAgent.IsActive())
			{
				this.DuelRequestReceived(peer.GetComponent<MissionPeer>(), duelRequest.RequestedAgent.MissionPeer);
			}
			return true;
		}

		private bool HandleClientEventDuelRequestAccepted(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			DuelResponse duelResponse = (DuelResponse)baseMessage;
			if (((peer != null) ? peer.GetComponent<MissionPeer>() : null) != null && peer.GetComponent<MissionPeer>().ControlledAgent != null)
			{
				NetworkCommunicator peer2 = duelResponse.Peer;
				if (((peer2 != null) ? peer2.GetComponent<MissionPeer>() : null) != null && duelResponse.Peer.GetComponent<MissionPeer>().ControlledAgent != null)
				{
					this.DuelRequestAccepted(duelResponse.Peer.GetComponent<DuelMissionRepresentative>().ControlledAgent, peer.GetComponent<DuelMissionRepresentative>().ControlledAgent);
				}
			}
			return true;
		}

		private bool HandleClientEventDuelRequestChangePreferredTroopType(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			RequestChangePreferredTroopType requestChangePreferredTroopType = (RequestChangePreferredTroopType)baseMessage;
			this.OnPeerSelectedPreferredTroopType(peer.GetComponent<MissionPeer>(), requestChangePreferredTroopType.TroopType);
			return true;
		}

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

		public void OnPlayerDespawn(MissionPeer missionPeer)
		{
			missionPeer.GetComponent<DuelMissionRepresentative>();
		}

		public void DuelRequestReceived(MissionPeer requesterPeer, MissionPeer requesteePeer)
		{
			if (!this.IsThereARequestBetweenPeers(requesterPeer, requesteePeer) && !this.IsHavingDuel(requesterPeer) && !this.IsHavingDuel(requesteePeer))
			{
				MissionMultiplayerDuel.DuelInfo duelInfo = new MissionMultiplayerDuel.DuelInfo(requesterPeer, requesteePeer, this.GetNextAvailableDuelAreaIndex(requesterPeer.ControlledAgent));
				this._duelRequests.Add(duelInfo);
				(requesteePeer.Representative as DuelMissionRepresentative).DuelRequested(requesterPeer.ControlledAgent, duelInfo.DuelAreaTroopType);
			}
		}

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

		public void DuelRequestAccepted(Agent requesterAgent, Agent requesteeAgent)
		{
			MissionMultiplayerDuel.DuelInfo duelInfo = this._duelRequests.FirstOrDefault((MissionMultiplayerDuel.DuelInfo dr) => dr.IsPeerInThisDuel(requesterAgent.MissionPeer) && dr.IsPeerInThisDuel(requesteeAgent.MissionPeer));
			if (duelInfo != null)
			{
				this.PrepareDuel(duelInfo);
			}
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			this.CheckRestartPreparationDuels();
			this.CheckForRestartingDuels();
			this.CheckDuelsToStart();
			this.CheckDuelRequestTimeouts();
			this.CheckEndedDuels();
		}

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

		private Team ActivateAndGetDuelTeam()
		{
			if (this._deactiveDuelTeams.Count <= 0)
			{
				return base.Mission.Teams.Add(BattleSideEnum.Defender, uint.MaxValue, uint.MaxValue, null, true, false, false);
			}
			return this._deactiveDuelTeams.Dequeue();
		}

		private void DeactivateDuelTeam(Team team)
		{
			this._deactiveDuelTeams.Enqueue(team);
		}

		private bool IsHavingDuel(MissionPeer peer)
		{
			return this._activeDuels.AnyQ((MissionMultiplayerDuel.DuelInfo d) => d.IsPeerInThisDuel(peer));
		}

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
			Debug.FailedAssert("IsHavingDuel(duel.RequesteePeer) || IsHavingDuel(duel.RequesterPeer)", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MultiplayerGameModeLogics\\ServerGameModeLogics\\MissionMultiplayerDuel.cs", "PrepareDuel", 700);
		}

		private void StartDuel(MissionMultiplayerDuel.DuelInfo duel)
		{
			duel.OnDuelStarted();
		}

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

		public int GetDuelAreaIndexIfDuelTeam(Team team)
		{
			if (team.IsDefender)
			{
				return this._activeDuels.FirstOrDefaultQ((MissionMultiplayerDuel.DuelInfo ad) => ad.DuelingTeam == team).DuelAreaIndex;
			}
			return -1;
		}

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

		protected override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
			MissionPeer component = networkPeer.GetComponent<MissionPeer>();
			if (component != null)
			{
				component.Team = null;
			}
		}

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

		public const float DuelRequestTimeOutInSeconds = 10f;

		private const int MinBountyGain = 100;

		private const string AreaBoxTagPrefix = "area_box";

		private const string AreaFlagTagPrefix = "area_flag";

		public const int NumberOfDuelAreas = 16;

		public const float DuelEndInSeconds = 2f;

		private const float DuelRequestTimeOutServerToleranceInSeconds = 0.5f;

		private const float CorpseFadeOutTimeInSeconds = 1f;

		private List<GameEntity> _duelAreaFlags = new List<GameEntity>();

		private List<VolumeBox> _areaBoxes = new List<VolumeBox>();

		private List<MissionMultiplayerDuel.DuelInfo> _duelRequests = new List<MissionMultiplayerDuel.DuelInfo>();

		private List<MissionMultiplayerDuel.DuelInfo> _activeDuels = new List<MissionMultiplayerDuel.DuelInfo>();

		private List<MissionMultiplayerDuel.DuelInfo> _endingDuels = new List<MissionMultiplayerDuel.DuelInfo>();

		private List<MissionMultiplayerDuel.DuelInfo> _restartingDuels = new List<MissionMultiplayerDuel.DuelInfo>();

		private List<MissionMultiplayerDuel.DuelInfo> _restartPreparationDuels = new List<MissionMultiplayerDuel.DuelInfo>();

		private readonly Queue<Team> _deactiveDuelTeams = new Queue<Team>();

		private List<KeyValuePair<MissionPeer, TroopType>> _peersAndSelections = new List<KeyValuePair<MissionPeer, TroopType>>();

		private VolumeBox[] _cachedSelectedVolumeBoxes;

		private KeyValuePair<int, TroopType>[] _cachedSelectedAreaFlags;

		private class DuelInfo
		{
			public MissionPeer RequesterPeer
			{
				get
				{
					return this._challengers[0].MissionPeer;
				}
			}

			public MissionPeer RequesteePeer
			{
				get
				{
					return this._challengers[1].MissionPeer;
				}
			}

			public int DuelAreaIndex { get; private set; }

			public TroopType DuelAreaTroopType { get; private set; }

			public MissionTime Timer { get; private set; }

			public Team DuelingTeam { get; private set; }

			public bool Started { get; private set; }

			public bool ChallengeEnded { get; private set; }

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

			public DuelInfo(MissionPeer requesterPeer, MissionPeer requesteePeer, KeyValuePair<int, TroopType> duelAreaPair)
			{
				this.DuelAreaIndex = duelAreaPair.Key;
				this.DuelAreaTroopType = duelAreaPair.Value;
				this._challengers = new MissionMultiplayerDuel.DuelInfo.Challenger[2];
				this._challengers[0] = new MissionMultiplayerDuel.DuelInfo.Challenger(requesterPeer);
				this._challengers[1] = new MissionMultiplayerDuel.DuelInfo.Challenger(requesteePeer);
				this.Timer = MissionTime.Now + MissionTime.Seconds(10.5f);
			}

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

			public void OnDuelStarted()
			{
				this.Started = true;
				this.DuelingTeam.SetIsEnemyOf(this.DuelingTeam, true);
			}

			public void OnDuelEnding()
			{
				this.Timer = MissionTime.Now + MissionTime.Seconds(2f);
			}

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

			public void UpdateDuelAreaIndex(KeyValuePair<int, TroopType> duelAreaPair)
			{
				this.DuelAreaIndex = duelAreaPair.Key;
				this.DuelAreaTroopType = duelAreaPair.Value;
			}

			private const float DuelStartCountdown = 3f;

			private readonly MissionMultiplayerDuel.DuelInfo.Challenger[] _challengers;

			private MissionMultiplayerDuel.DuelInfo.ChallengerType _winnerChallengerType = MissionMultiplayerDuel.DuelInfo.ChallengerType.None;

			private enum ChallengerType
			{
				None = -1,
				Requester,
				Requestee,
				NumChallengerType
			}

			private struct Challenger
			{
				public Agent DuelingAgent { get; private set; }

				public Agent MountAgent { get; private set; }

				public int KillCountInDuel { get; private set; }

				public Challenger(MissionPeer missionPeer)
				{
					this.MissionPeer = missionPeer;
					MissionPeer missionPeer2 = this.MissionPeer;
					this.NetworkPeer = ((missionPeer2 != null) ? missionPeer2.GetNetworkPeer() : null);
					this.DuelingAgent = null;
					this.MountAgent = null;
					this.KillCountInDuel = 0;
				}

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

				public void OnDuelEnded()
				{
					if (this.MissionPeer.Peer.Communicator.IsConnectionActive)
					{
						this.MissionPeer.Team = Mission.Current.AttackerTeam;
					}
				}

				public void IncreaseWinCount()
				{
					int killCountInDuel = this.KillCountInDuel;
					this.KillCountInDuel = killCountInDuel + 1;
				}

				public void SetAgents(Agent agent)
				{
					this.DuelingAgent = agent;
					this.MountAgent = this.DuelingAgent.MountAgent;
				}

				public readonly MissionPeer MissionPeer;

				public readonly NetworkCommunicator NetworkPeer;
			}
		}

		public delegate void OnDuelEndedDelegate(MissionPeer winnerPeer, TroopType troopType);
	}
}
