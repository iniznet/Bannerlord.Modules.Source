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
	public class DuelMissionRepresentative : MissionRepresentativeBase
	{
		public int Bounty { get; private set; }

		public int Score { get; private set; }

		public int NumberOfWins { get; private set; }

		private bool _isInDuel
		{
			get
			{
				return base.MissionPeer != null && base.MissionPeer.Team != null && base.MissionPeer.Team.IsDefender;
			}
		}

		public override void Initialize()
		{
			this._requesters = new List<Tuple<MissionPeer, MissionTime>>();
			if (GameNetwork.IsServerOrRecorder)
			{
				this._missionMultiplayerDuel = Mission.Current.GetMissionBehavior<MissionMultiplayerDuel>();
			}
			Mission.Current.SetMissionMode(MissionMode.Duel, true);
		}

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
								GameNetwork.WriteMessage(new NetworkMessages.FromClient.DuelRequest(focusedAgent.Index));
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

		private void HandleServerEventDuelRequest(NetworkMessages.FromServer.DuelRequest message)
		{
			Agent agentFromIndex = Mission.MissionNetworkHelper.GetAgentFromIndex(message.RequesterAgentIndex, false);
			Mission.MissionNetworkHelper.GetAgentFromIndex(message.RequestedAgentIndex, false);
			this.DuelRequested(agentFromIndex, message.SelectedAreaTroopType);
		}

		private void HandleServerEventDuelSessionStarted(DuelSessionStarted message)
		{
			this.OnDuelPreparation(message.RequesterPeer.GetComponent<MissionPeer>(), message.RequestedPeer.GetComponent<MissionPeer>());
		}

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

		private void HandleServerEventDuelEnded(DuelEnded message)
		{
			Action<MissionPeer> onDuelEndedEvent = this.OnDuelEndedEvent;
			if (onDuelEndedEvent == null)
			{
				return;
			}
			onDuelEndedEvent(message.WinnerPeer.GetComponent<MissionPeer>());
		}

		private void HandleServerEventDuelRoundEnded(DuelRoundEnded message)
		{
			Action<MissionPeer> onDuelRoundEndedEvent = this.OnDuelRoundEndedEvent;
			if (onDuelRoundEndedEvent == null)
			{
				return;
			}
			onDuelRoundEndedEvent(message.WinnerPeer.GetComponent<MissionPeer>());
		}

		private void HandleServerPointUpdate(DuelPointsUpdateMessage message)
		{
			DuelMissionRepresentative component = message.NetworkCommunicator.GetComponent<DuelMissionRepresentative>();
			component.Bounty = message.Bounty;
			component.Score = message.Score;
			component.NumberOfWins = message.NumberOfWins;
		}

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
					GameNetwork.WriteMessage(new NetworkMessages.FromServer.DuelRequest(requesterAgent.Index, base.ControlledAgent.Index, selectedAreaTroopType));
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

		public void OnObjectFocused(IFocusable focusedObject)
		{
			this._focusedObject = focusedObject;
		}

		public void OnObjectFocusLost()
		{
			this._focusedObject = null;
		}

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

		public void ResetBountyAndNumberOfWins()
		{
			this.Bounty = 0;
			this.NumberOfWins = 0;
		}

		public void OnDuelWon(float gainedScore)
		{
			this.Bounty += (int)(gainedScore / 5f);
			this.Score += (int)gainedScore;
			int numberOfWins = this.NumberOfWins;
			this.NumberOfWins = numberOfWins + 1;
		}

		public const int DuelPrepTime = 3;

		public Action<MissionPeer, TroopType> OnDuelRequestedEvent;

		public Action<MissionPeer> OnDuelRequestSentEvent;

		public Action<MissionPeer, int> OnDuelPrepStartedEvent;

		public Action OnAgentSpawnedWithoutDuelEvent;

		public Action<MissionPeer, MissionPeer, int> OnDuelPreparationStartedForTheFirstTimeEvent;

		public Action<MissionPeer> OnDuelEndedEvent;

		public Action<MissionPeer> OnDuelRoundEndedEvent;

		public Action<TroopType> OnMyPreferredZoneChanged;

		private List<Tuple<MissionPeer, MissionTime>> _requesters;

		private MissionMultiplayerDuel _missionMultiplayerDuel;

		private IFocusable _focusedObject;
	}
}
