using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade.MissionRepresentatives
{
	// Token: 0x020003E2 RID: 994
	public class DuelMissionRepresentative : MissionRepresentativeBase
	{
		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x06003463 RID: 13411 RVA: 0x000D8F34 File Offset: 0x000D7134
		// (set) Token: 0x06003464 RID: 13412 RVA: 0x000D8F3C File Offset: 0x000D713C
		public int Bounty { get; private set; }

		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x06003465 RID: 13413 RVA: 0x000D8F45 File Offset: 0x000D7145
		// (set) Token: 0x06003466 RID: 13414 RVA: 0x000D8F4D File Offset: 0x000D714D
		public int Score { get; private set; }

		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x06003467 RID: 13415 RVA: 0x000D8F56 File Offset: 0x000D7156
		// (set) Token: 0x06003468 RID: 13416 RVA: 0x000D8F5E File Offset: 0x000D715E
		public int NumberOfWins { get; private set; }

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x06003469 RID: 13417 RVA: 0x000D8F67 File Offset: 0x000D7167
		private bool _isInDuel
		{
			get
			{
				return base.MissionPeer != null && base.MissionPeer.Team != null && base.MissionPeer.Team.IsDefender;
			}
		}

		// Token: 0x0600346A RID: 13418 RVA: 0x000D8F90 File Offset: 0x000D7190
		public override void Initialize()
		{
			this._requesters = new List<Tuple<MissionPeer, MissionTime>>();
			if (GameNetwork.IsServerOrRecorder)
			{
				this._missionMultiplayerDuel = Mission.Current.GetMissionBehavior<MissionMultiplayerDuel>();
			}
			Mission.Current.SetMissionMode(MissionMode.Duel, true);
		}

		// Token: 0x0600346B RID: 13419 RVA: 0x000D8FC0 File Offset: 0x000D71C0
		public void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
				networkMessageHandlerRegisterer.Register<NetworkMessages.FromServer.DuelRequest>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.DuelRequest>(this.HandleServerEventDuelRequest));
				networkMessageHandlerRegisterer.Register<DuelSessionStarted>(new GameNetworkMessage.ServerMessageHandlerDelegate<DuelSessionStarted>(this.HandleServerEventDuelSessionStarted));
				networkMessageHandlerRegisterer.Register<DuelPreparationStartedForTheFirstTime>(new GameNetworkMessage.ServerMessageHandlerDelegate<DuelPreparationStartedForTheFirstTime>(this.HandleServerEventDuelStarted));
				networkMessageHandlerRegisterer.Register<DuelEnded>(new GameNetworkMessage.ServerMessageHandlerDelegate<DuelEnded>(this.HandleServerEventDuelEnded));
				networkMessageHandlerRegisterer.Register<DuelRoundEnded>(new GameNetworkMessage.ServerMessageHandlerDelegate<DuelRoundEnded>(this.HandleServerEventDuelRoundEnded));
				networkMessageHandlerRegisterer.Register<DuelPointsUpdateMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<DuelPointsUpdateMessage>(this.HandleServerPointUpdate));
			}
		}

		// Token: 0x0600346C RID: 13420 RVA: 0x000D9048 File Offset: 0x000D7248
		public void OnInteraction()
		{
			if (this._focusedObject != null)
			{
				DuelZoneLandmark duelZoneLandmark;
				Agent focusedAgent;
				if ((focusedAgent = this._focusedObject as Agent) != null)
				{
					if (focusedAgent.IsActive())
					{
						if (this._requesters.Any((Tuple<MissionPeer, MissionTime> req) => req.Item1 == focusedAgent.MissionPeer))
						{
							for (int i = 0; i < this._requesters.Count; i++)
							{
								if (this._requesters[i].Item1 == base.MissionPeer)
								{
									this._requesters.Remove(this._requesters[i]);
									break;
								}
							}
							MissionRepresentativeBase.PlayerTypes playerTypes = base.PlayerType;
							if (playerTypes == MissionRepresentativeBase.PlayerTypes.Client)
							{
								GameNetwork.BeginModuleEventAsClient();
								GameNetwork.WriteMessage(new DuelResponse(focusedAgent.MissionRepresentative.Peer.Communicator as NetworkCommunicator, true));
								GameNetwork.EndModuleEventAsClient();
								return;
							}
							if (playerTypes != MissionRepresentativeBase.PlayerTypes.Server)
							{
								return;
							}
							this._missionMultiplayerDuel.DuelRequestAccepted(focusedAgent, base.ControlledAgent);
							return;
						}
						else
						{
							MissionRepresentativeBase.PlayerTypes playerTypes = base.PlayerType;
							if (playerTypes == MissionRepresentativeBase.PlayerTypes.Client)
							{
								Action<MissionPeer> onDuelRequestSentEvent = this.OnDuelRequestSentEvent;
								if (onDuelRequestSentEvent != null)
								{
									onDuelRequestSentEvent(focusedAgent.MissionPeer);
								}
								GameNetwork.BeginModuleEventAsClient();
								GameNetwork.WriteMessage(new NetworkMessages.FromClient.DuelRequest(focusedAgent));
								GameNetwork.EndModuleEventAsClient();
								return;
							}
							if (playerTypes != MissionRepresentativeBase.PlayerTypes.Server)
							{
								return;
							}
							this._missionMultiplayerDuel.DuelRequestReceived(base.MissionPeer, focusedAgent.MissionPeer);
							return;
						}
					}
				}
				else if ((duelZoneLandmark = this._focusedObject as DuelZoneLandmark) != null)
				{
					if (this._isInDuel)
					{
						InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=v5EqMSlD}Can't change arena preference while in duel.", null).ToString()));
						return;
					}
					GameNetwork.BeginModuleEventAsClient();
					GameNetwork.WriteMessage(new RequestChangePreferredTroopType(duelZoneLandmark.ZoneTroopType));
					GameNetwork.EndModuleEventAsClient();
					Action<TroopType> onMyPreferredZoneChanged = this.OnMyPreferredZoneChanged;
					if (onMyPreferredZoneChanged == null)
					{
						return;
					}
					onMyPreferredZoneChanged(duelZoneLandmark.ZoneTroopType);
				}
			}
		}

		// Token: 0x0600346D RID: 13421 RVA: 0x000D9215 File Offset: 0x000D7415
		private void HandleServerEventDuelRequest(NetworkMessages.FromServer.DuelRequest message)
		{
			this.DuelRequested(message.RequesterAgent, message.SelectedAreaTroopType);
		}

		// Token: 0x0600346E RID: 13422 RVA: 0x000D9229 File Offset: 0x000D7429
		private void HandleServerEventDuelSessionStarted(DuelSessionStarted message)
		{
			this.OnDuelPreparation(message.RequesterPeer.GetComponent<MissionPeer>(), message.RequestedPeer.GetComponent<MissionPeer>());
		}

		// Token: 0x0600346F RID: 13423 RVA: 0x000D9248 File Offset: 0x000D7448
		private void HandleServerEventDuelStarted(DuelPreparationStartedForTheFirstTime message)
		{
			MissionPeer component = message.RequesterPeer.GetComponent<MissionPeer>();
			MissionPeer component2 = message.RequesteePeer.GetComponent<MissionPeer>();
			Action<MissionPeer, MissionPeer, int> onDuelPreparationStartedForTheFirstTimeEvent = this.OnDuelPreparationStartedForTheFirstTimeEvent;
			if (onDuelPreparationStartedForTheFirstTimeEvent == null)
			{
				return;
			}
			onDuelPreparationStartedForTheFirstTimeEvent(component, component2, message.AreaIndex);
		}

		// Token: 0x06003470 RID: 13424 RVA: 0x000D9285 File Offset: 0x000D7485
		private void HandleServerEventDuelEnded(DuelEnded message)
		{
			Action<MissionPeer> onDuelEndedEvent = this.OnDuelEndedEvent;
			if (onDuelEndedEvent == null)
			{
				return;
			}
			onDuelEndedEvent(message.WinnerPeer.GetComponent<MissionPeer>());
		}

		// Token: 0x06003471 RID: 13425 RVA: 0x000D92A2 File Offset: 0x000D74A2
		private void HandleServerEventDuelRoundEnded(DuelRoundEnded message)
		{
			Action<MissionPeer> onDuelRoundEndedEvent = this.OnDuelRoundEndedEvent;
			if (onDuelRoundEndedEvent == null)
			{
				return;
			}
			onDuelRoundEndedEvent(message.WinnerPeer.GetComponent<MissionPeer>());
		}

		// Token: 0x06003472 RID: 13426 RVA: 0x000D92BF File Offset: 0x000D74BF
		private void HandleServerPointUpdate(DuelPointsUpdateMessage message)
		{
			DuelMissionRepresentative component = message.NetworkCommunicator.GetComponent<DuelMissionRepresentative>();
			component.Bounty = message.Bounty;
			component.Score = message.Score;
			component.NumberOfWins = message.NumberOfWins;
		}

		// Token: 0x06003473 RID: 13427 RVA: 0x000D92F0 File Offset: 0x000D74F0
		public void DuelRequested(Agent requesterAgent, TroopType selectedAreaTroopType)
		{
			this._requesters.Add(new Tuple<MissionPeer, MissionTime>(requesterAgent.MissionPeer, MissionTime.Now + MissionTime.Seconds(10f)));
			switch (base.PlayerType)
			{
			case MissionRepresentativeBase.PlayerTypes.Bot:
				this._missionMultiplayerDuel.DuelRequestAccepted(requesterAgent, base.ControlledAgent);
				return;
			case MissionRepresentativeBase.PlayerTypes.Client:
			{
				if (!base.IsMine)
				{
					GameNetwork.BeginModuleEventAsServer(base.Peer);
					GameNetwork.WriteMessage(new NetworkMessages.FromServer.DuelRequest(requesterAgent, base.ControlledAgent, selectedAreaTroopType));
					GameNetwork.EndModuleEventAsServer();
					return;
				}
				Action<MissionPeer, TroopType> onDuelRequestedEvent = this.OnDuelRequestedEvent;
				if (onDuelRequestedEvent == null)
				{
					return;
				}
				onDuelRequestedEvent(requesterAgent.MissionPeer, selectedAreaTroopType);
				return;
			}
			case MissionRepresentativeBase.PlayerTypes.Server:
			{
				Action<MissionPeer, TroopType> onDuelRequestedEvent2 = this.OnDuelRequestedEvent;
				if (onDuelRequestedEvent2 == null)
				{
					return;
				}
				onDuelRequestedEvent2(requesterAgent.MissionPeer, selectedAreaTroopType);
				return;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06003474 RID: 13428 RVA: 0x000D93B8 File Offset: 0x000D75B8
		public bool CheckHasRequestFromAndRemoveRequestIfNeeded(MissionPeer requestOwner)
		{
			if (requestOwner != null && requestOwner.Representative == this)
			{
				this._requesters.Clear();
				return false;
			}
			Tuple<MissionPeer, MissionTime> tuple = this._requesters.FirstOrDefault((Tuple<MissionPeer, MissionTime> req) => req.Item1 == requestOwner);
			if (tuple == null)
			{
				return false;
			}
			if (requestOwner.ControlledAgent == null || !requestOwner.ControlledAgent.IsActive())
			{
				this._requesters.Remove(tuple);
				return false;
			}
			if (!tuple.Item2.IsPast)
			{
				return true;
			}
			this._requesters.Remove(tuple);
			return false;
		}

		// Token: 0x06003475 RID: 13429 RVA: 0x000D9460 File Offset: 0x000D7660
		public void OnDuelPreparation(MissionPeer requesterPeer, MissionPeer requesteePeer)
		{
			MissionRepresentativeBase.PlayerTypes playerType = base.PlayerType;
			if (playerType != MissionRepresentativeBase.PlayerTypes.Client)
			{
				if (playerType == MissionRepresentativeBase.PlayerTypes.Server)
				{
					Action<MissionPeer, int> onDuelPrepStartedEvent = this.OnDuelPrepStartedEvent;
					if (onDuelPrepStartedEvent != null)
					{
						onDuelPrepStartedEvent((base.MissionPeer == requesterPeer) ? requesteePeer : requesterPeer, 3);
					}
				}
			}
			else if (base.IsMine)
			{
				Action<MissionPeer, int> onDuelPrepStartedEvent2 = this.OnDuelPrepStartedEvent;
				if (onDuelPrepStartedEvent2 != null)
				{
					onDuelPrepStartedEvent2((base.MissionPeer == requesterPeer) ? requesteePeer : requesterPeer, 3);
				}
			}
			else
			{
				GameNetwork.BeginModuleEventAsServer(base.Peer);
				GameNetwork.WriteMessage(new DuelSessionStarted(requesterPeer.GetNetworkPeer(), requesteePeer.GetNetworkPeer()));
				GameNetwork.EndModuleEventAsServer();
			}
			Tuple<MissionPeer, MissionTime> tuple = this._requesters.FirstOrDefault((Tuple<MissionPeer, MissionTime> req) => req.Item1 == requesterPeer);
			if (tuple != null)
			{
				this._requesters.Remove(tuple);
			}
		}

		// Token: 0x06003476 RID: 13430 RVA: 0x000D953F File Offset: 0x000D773F
		public void OnObjectFocused(IFocusable focusedObject)
		{
			this._focusedObject = focusedObject;
		}

		// Token: 0x06003477 RID: 13431 RVA: 0x000D9548 File Offset: 0x000D7748
		public void OnObjectFocusLost()
		{
			this._focusedObject = null;
		}

		// Token: 0x06003478 RID: 13432 RVA: 0x000D9551 File Offset: 0x000D7751
		public override void OnAgentSpawned()
		{
			if (base.ControlledAgent.Team != null && base.ControlledAgent.Team.Side == BattleSideEnum.Attacker)
			{
				Action onAgentSpawnedWithoutDuelEvent = this.OnAgentSpawnedWithoutDuelEvent;
				if (onAgentSpawnedWithoutDuelEvent == null)
				{
					return;
				}
				onAgentSpawnedWithoutDuelEvent();
			}
		}

		// Token: 0x06003479 RID: 13433 RVA: 0x000D9583 File Offset: 0x000D7783
		public void ResetBountyAndNumberOfWins()
		{
			this.Bounty = 0;
			this.NumberOfWins = 0;
		}

		// Token: 0x0600347A RID: 13434 RVA: 0x000D9594 File Offset: 0x000D7794
		public void OnDuelWon(float gainedScore)
		{
			this.Bounty += (int)(gainedScore / 5f);
			this.Score += (int)gainedScore;
			int numberOfWins = this.NumberOfWins;
			this.NumberOfWins = numberOfWins + 1;
		}

		// Token: 0x0400164C RID: 5708
		public const int DuelPrepTime = 3;

		// Token: 0x0400164D RID: 5709
		public Action<MissionPeer, TroopType> OnDuelRequestedEvent;

		// Token: 0x0400164E RID: 5710
		public Action<MissionPeer> OnDuelRequestSentEvent;

		// Token: 0x0400164F RID: 5711
		public Action<MissionPeer, int> OnDuelPrepStartedEvent;

		// Token: 0x04001650 RID: 5712
		public Action OnAgentSpawnedWithoutDuelEvent;

		// Token: 0x04001651 RID: 5713
		public Action<MissionPeer, MissionPeer, int> OnDuelPreparationStartedForTheFirstTimeEvent;

		// Token: 0x04001652 RID: 5714
		public Action<MissionPeer> OnDuelEndedEvent;

		// Token: 0x04001653 RID: 5715
		public Action<MissionPeer> OnDuelRoundEndedEvent;

		// Token: 0x04001654 RID: 5716
		public Action<TroopType> OnMyPreferredZoneChanged;

		// Token: 0x04001655 RID: 5717
		private List<Tuple<MissionPeer, MissionTime>> _requesters;

		// Token: 0x04001656 RID: 5718
		private MissionMultiplayerDuel _missionMultiplayerDuel;

		// Token: 0x04001657 RID: 5719
		private IFocusable _focusedObject;
	}
}
