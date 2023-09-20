using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerPollComponent : MissionNetwork
	{
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._notificationsComponent = base.Mission.GetMissionBehavior<MultiplayerGameNotificationsComponent>();
		}

		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			MultiplayerPollComponent.MultiplayerPoll ongoingPoll = this._ongoingPoll;
			if (ongoingPoll == null)
			{
				return;
			}
			ongoingPoll.Tick();
		}

		public void Vote(bool accepted)
		{
			if (GameNetwork.IsServer)
			{
				if (GameNetwork.MyPeer != null)
				{
					this.ApplyVote(GameNetwork.MyPeer, accepted);
					return;
				}
			}
			else if (this._ongoingPoll != null && this._ongoingPoll.IsOpen)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new PollResponse(accepted));
				GameNetwork.EndModuleEventAsClient();
			}
		}

		private void ApplyVote(NetworkCommunicator peer, bool accepted)
		{
			if (this._ongoingPoll != null && this._ongoingPoll.ApplyVote(peer, accepted))
			{
				List<NetworkCommunicator> pollProgressReceivers = this._ongoingPoll.GetPollProgressReceivers();
				int count = pollProgressReceivers.Count;
				for (int i = 0; i < count; i++)
				{
					GameNetwork.BeginModuleEventAsServer(pollProgressReceivers[i]);
					GameNetwork.WriteMessage(new PollProgress(this._ongoingPoll.AcceptedCount, this._ongoingPoll.RejectedCount));
					GameNetwork.EndModuleEventAsServer();
				}
				this.UpdatePollProgress(this._ongoingPoll.AcceptedCount, this._ongoingPoll.RejectedCount);
			}
		}

		private void RejectPollOnServer(NetworkCommunicator pollCreatorPeer, MultiplayerPollRejectReason rejectReason)
		{
			if (pollCreatorPeer.IsMine)
			{
				this.RejectPoll(rejectReason);
				return;
			}
			GameNetwork.BeginModuleEventAsServer(pollCreatorPeer);
			GameNetwork.WriteMessage(new PollRequestRejected((int)rejectReason));
			GameNetwork.EndModuleEventAsServer();
		}

		private void RejectPoll(MultiplayerPollRejectReason rejectReason)
		{
			if (!GameNetwork.IsDedicatedServer)
			{
				this._notificationsComponent.PollRejected(rejectReason);
			}
			Action<MultiplayerPollRejectReason> onPollRejected = this.OnPollRejected;
			if (onPollRejected == null)
			{
				return;
			}
			onPollRejected(rejectReason);
		}

		private void UpdatePollProgress(int votesAccepted, int votesRejected)
		{
			Action<int, int> onPollUpdated = this.OnPollUpdated;
			if (onPollUpdated == null)
			{
				return;
			}
			onPollUpdated(votesAccepted, votesRejected);
		}

		private void CancelPoll()
		{
			if (this._ongoingPoll != null)
			{
				this._ongoingPoll.Cancel();
				this._ongoingPoll = null;
			}
			Action onPollCancelled = this.OnPollCancelled;
			if (onPollCancelled == null)
			{
				return;
			}
			onPollCancelled();
		}

		private void OnPollCancelledOnServer(MultiplayerPollComponent.MultiplayerPoll multiplayerPoll)
		{
			List<NetworkCommunicator> pollProgressReceivers = multiplayerPoll.GetPollProgressReceivers();
			int count = pollProgressReceivers.Count;
			for (int i = 0; i < count; i++)
			{
				GameNetwork.BeginModuleEventAsServer(pollProgressReceivers[i]);
				GameNetwork.WriteMessage(new PollCancelled());
				GameNetwork.EndModuleEventAsServer();
			}
			this.CancelPoll();
		}

		public void RequestKickPlayerPoll(NetworkCommunicator peer, bool banPlayer)
		{
			if (GameNetwork.IsServer)
			{
				if (GameNetwork.MyPeer != null)
				{
					this.OpenKickPlayerPollOnServer(GameNetwork.MyPeer, peer, banPlayer);
					return;
				}
			}
			else
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new KickPlayerPollRequested(peer, banPlayer));
				GameNetwork.EndModuleEventAsClient();
			}
		}

		private void OpenKickPlayerPollOnServer(NetworkCommunicator pollCreatorPeer, NetworkCommunicator targetPeer, bool banPlayer)
		{
			if (this._ongoingPoll == null)
			{
				bool flag = pollCreatorPeer != null && pollCreatorPeer.IsConnectionActive;
				bool flag2 = targetPeer != null && targetPeer.IsConnectionActive;
				if (flag && flag2)
				{
					if (!targetPeer.IsSynchronized)
					{
						this.RejectPollOnServer(pollCreatorPeer, MultiplayerPollRejectReason.KickPollTargetNotSynced);
						return;
					}
					MissionPeer component = pollCreatorPeer.GetComponent<MissionPeer>();
					if (component != null)
					{
						if (component.RequestedKickPollCount >= 2)
						{
							this.RejectPollOnServer(pollCreatorPeer, MultiplayerPollRejectReason.TooManyPollRequests);
							return;
						}
						List<NetworkCommunicator> list = new List<NetworkCommunicator>();
						foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
						{
							if (networkCommunicator != targetPeer && networkCommunicator.IsSynchronized)
							{
								MissionPeer component2 = networkCommunicator.GetComponent<MissionPeer>();
								if (component2 != null && component2.Team == component.Team)
								{
									list.Add(networkCommunicator);
								}
							}
						}
						int count = list.Count;
						if (count + 1 >= 3)
						{
							this.OpenKickPlayerPoll(targetPeer, pollCreatorPeer, false, list);
							for (int i = 0; i < count; i++)
							{
								GameNetwork.BeginModuleEventAsServer(this._ongoingPoll.ParticipantsToVote[i]);
								GameNetwork.WriteMessage(new KickPlayerPollOpened(pollCreatorPeer, targetPeer, banPlayer));
								GameNetwork.EndModuleEventAsServer();
							}
							GameNetwork.BeginModuleEventAsServer(targetPeer);
							GameNetwork.WriteMessage(new KickPlayerPollOpened(pollCreatorPeer, targetPeer, banPlayer));
							GameNetwork.EndModuleEventAsServer();
							component.IncrementRequestedKickPollCount();
							return;
						}
						this.RejectPollOnServer(pollCreatorPeer, MultiplayerPollRejectReason.NotEnoughPlayersToOpenPoll);
						return;
					}
				}
			}
			else
			{
				this.RejectPollOnServer(pollCreatorPeer, MultiplayerPollRejectReason.HasOngoingPoll);
			}
		}

		private void OpenKickPlayerPoll(NetworkCommunicator targetPeer, NetworkCommunicator pollCreatorPeer, bool banPlayer, List<NetworkCommunicator> participantsToVote)
		{
			MissionPeer component = pollCreatorPeer.GetComponent<MissionPeer>();
			MissionPeer component2 = targetPeer.GetComponent<MissionPeer>();
			this._ongoingPoll = new MultiplayerPollComponent.KickPlayerPoll(this._missionLobbyComponent, participantsToVote, targetPeer, component.Team);
			if (GameNetwork.IsServer)
			{
				MultiplayerPollComponent.MultiplayerPoll ongoingPoll = this._ongoingPoll;
				ongoingPoll.OnClosedOnServer = (Action<MultiplayerPollComponent.MultiplayerPoll>)Delegate.Combine(ongoingPoll.OnClosedOnServer, new Action<MultiplayerPollComponent.MultiplayerPoll>(this.OnKickPlayerPollClosedOnServer));
				MultiplayerPollComponent.MultiplayerPoll ongoingPoll2 = this._ongoingPoll;
				ongoingPoll2.OnCancelledOnServer = (Action<MultiplayerPollComponent.MultiplayerPoll>)Delegate.Combine(ongoingPoll2.OnCancelledOnServer, new Action<MultiplayerPollComponent.MultiplayerPoll>(this.OnPollCancelledOnServer));
			}
			Action<MissionPeer, MissionPeer, bool> onKickPollOpened = this.OnKickPollOpened;
			if (onKickPollOpened != null)
			{
				onKickPollOpened(component, component2, banPlayer);
			}
			if (GameNetwork.MyPeer == pollCreatorPeer)
			{
				this.Vote(true);
			}
		}

		private void OnKickPlayerPollClosedOnServer(MultiplayerPollComponent.MultiplayerPoll multiplayerPoll)
		{
			MultiplayerPollComponent.KickPlayerPoll kickPlayerPoll = multiplayerPoll as MultiplayerPollComponent.KickPlayerPoll;
			bool flag = kickPlayerPoll.GotEnoughAcceptVotesToEnd();
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new KickPlayerPollClosed(kickPlayerPoll.TargetPeer, flag));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
			this.CloseKickPlayerPoll(flag, kickPlayerPoll.TargetPeer);
			if (flag)
			{
				DisconnectInfo disconnectInfo = kickPlayerPoll.TargetPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
				disconnectInfo.Type = DisconnectType.KickedByPoll;
				kickPlayerPoll.TargetPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", disconnectInfo);
				GameNetwork.AddNetworkPeerToDisconnectAsServer(kickPlayerPoll.TargetPeer);
			}
		}

		private void CloseKickPlayerPoll(bool accepted, NetworkCommunicator targetPeer)
		{
			if (this._ongoingPoll != null)
			{
				this._ongoingPoll.Close();
				this._ongoingPoll = null;
			}
			Action onPollClosed = this.OnPollClosed;
			if (onPollClosed != null)
			{
				onPollClosed();
			}
			if (!GameNetwork.IsDedicatedServer && accepted && !targetPeer.IsMine)
			{
				this._notificationsComponent.PlayerKicked(targetPeer);
			}
		}

		private void OnBanPlayerPollClosedOnServer(MultiplayerPollComponent.MultiplayerPoll multiplayerPoll)
		{
			MissionPeer component = (multiplayerPoll as MultiplayerPollComponent.BanPlayerPoll).TargetPeer.GetComponent<MissionPeer>();
			if (component != null)
			{
				NetworkCommunicator networkPeer = component.GetNetworkPeer();
				DisconnectInfo disconnectInfo = networkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
				disconnectInfo.Type = DisconnectType.BannedByPoll;
				networkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", disconnectInfo);
				GameNetwork.AddNetworkPeerToDisconnectAsServer(networkPeer);
				if (GameNetwork.IsClient)
				{
					BannedPlayerManagerCustomGameClient.AddBannedPlayer(component.Peer.Id, GameNetwork.IsDedicatedServer ? (-1) : (Environment.TickCount + 600000));
				}
				else if (GameNetwork.IsDedicatedServer)
				{
					BannedPlayerManagerCustomGameServer.AddBannedPlayer(component.Peer.Id, component.GetPeer().UserName, GameNetwork.IsDedicatedServer ? (-1) : (Environment.TickCount + 600000));
				}
				if (GameNetwork.IsDedicatedServer)
				{
					throw new NotImplementedException();
				}
				NetworkMain.GameClient.KickPlayer(component.Peer.Id, true);
			}
		}

		private void StartChangeGamePollOnServer(NetworkCommunicator pollCreatorPeer, string gameType, string scene)
		{
			if (this._ongoingPoll == null)
			{
				List<NetworkCommunicator> list = GameNetwork.NetworkPeers.ToList<NetworkCommunicator>();
				this._ongoingPoll = new MultiplayerPollComponent.ChangeGamePoll(this._missionLobbyComponent, list, gameType, scene);
				if (GameNetwork.IsServer)
				{
					MultiplayerPollComponent.MultiplayerPoll ongoingPoll = this._ongoingPoll;
					ongoingPoll.OnClosedOnServer = (Action<MultiplayerPollComponent.MultiplayerPoll>)Delegate.Combine(ongoingPoll.OnClosedOnServer, new Action<MultiplayerPollComponent.MultiplayerPoll>(this.OnChangeGamePollClosedOnServer));
				}
				if (!GameNetwork.IsDedicatedServer)
				{
					this.ShowChangeGamePoll(gameType, scene);
				}
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new NetworkMessages.FromServer.ChangeGamePoll(gameType, scene));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
				return;
			}
			this.RejectPollOnServer(pollCreatorPeer, MultiplayerPollRejectReason.HasOngoingPoll);
		}

		private void StartChangeGamePoll(string gameType, string map)
		{
			if (GameNetwork.IsServer)
			{
				if (GameNetwork.MyPeer != null)
				{
					this.StartChangeGamePollOnServer(GameNetwork.MyPeer, gameType, map);
					return;
				}
			}
			else
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new NetworkMessages.FromClient.ChangeGamePoll(gameType, map));
				GameNetwork.EndModuleEventAsClient();
			}
		}

		private void ShowChangeGamePoll(string gameType, string scene)
		{
		}

		private void OnChangeGamePollClosedOnServer(MultiplayerPollComponent.MultiplayerPoll multiplayerPoll)
		{
			MultiplayerPollComponent.ChangeGamePoll changeGamePoll = multiplayerPoll as MultiplayerPollComponent.ChangeGamePoll;
			MultiplayerOptions.MultiplayerOptionsAccessMode multiplayerOptionsAccessMode = MultiplayerOptions.MultiplayerOptionsAccessMode.NextMapOptions;
			MultiplayerOptions.OptionType.GameType.SetValue(changeGamePoll.GameType, multiplayerOptionsAccessMode);
			MultiplayerOptions.Instance.OnGameTypeChanged(multiplayerOptionsAccessMode);
			MultiplayerOptions.OptionType.Map.SetValue(changeGamePoll.MapName, multiplayerOptionsAccessMode);
			this._missionLobbyComponent.SetStateEndingAsServer();
		}

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.RegisterBaseHandler<PollRequestRejected>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventPollRequestRejected));
				registerer.RegisterBaseHandler<PollProgress>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventUpdatePollProgress));
				registerer.RegisterBaseHandler<PollCancelled>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventPollCancelled));
				registerer.RegisterBaseHandler<KickPlayerPollOpened>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventKickPlayerPollOpened));
				registerer.RegisterBaseHandler<KickPlayerPollClosed>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventKickPlayerPollClosed));
				registerer.RegisterBaseHandler<NetworkMessages.FromServer.ChangeGamePoll>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventChangeGamePoll));
				return;
			}
			if (GameNetwork.IsServer)
			{
				registerer.RegisterBaseHandler<PollResponse>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventPollResponse));
				registerer.RegisterBaseHandler<KickPlayerPollRequested>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventKickPlayerPollRequested));
				registerer.RegisterBaseHandler<NetworkMessages.FromClient.ChangeGamePoll>(new GameNetworkMessage.ClientMessageHandlerDelegate<GameNetworkMessage>(this.HandleClientEventChangeGamePoll));
			}
		}

		private bool HandleClientEventChangeGamePoll(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			NetworkMessages.FromClient.ChangeGamePoll changeGamePoll = (NetworkMessages.FromClient.ChangeGamePoll)baseMessage;
			this.StartChangeGamePollOnServer(peer, changeGamePoll.GameType, changeGamePoll.Map);
			return true;
		}

		private bool HandleClientEventKickPlayerPollRequested(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			KickPlayerPollRequested kickPlayerPollRequested = (KickPlayerPollRequested)baseMessage;
			this.OpenKickPlayerPollOnServer(peer, kickPlayerPollRequested.PlayerPeer, kickPlayerPollRequested.BanPlayer);
			return true;
		}

		private bool HandleClientEventPollResponse(NetworkCommunicator peer, GameNetworkMessage baseMessage)
		{
			PollResponse pollResponse = (PollResponse)baseMessage;
			this.ApplyVote(peer, pollResponse.Accepted);
			return true;
		}

		private void HandleServerEventChangeGamePoll(GameNetworkMessage baseMessage)
		{
			NetworkMessages.FromServer.ChangeGamePoll changeGamePoll = (NetworkMessages.FromServer.ChangeGamePoll)baseMessage;
			this.ShowChangeGamePoll(changeGamePoll.GameType, changeGamePoll.Map);
		}

		private void HandleServerEventKickPlayerPollOpened(GameNetworkMessage baseMessage)
		{
			KickPlayerPollOpened kickPlayerPollOpened = (KickPlayerPollOpened)baseMessage;
			this.OpenKickPlayerPoll(kickPlayerPollOpened.PlayerPeer, kickPlayerPollOpened.InitiatorPeer, kickPlayerPollOpened.BanPlayer, null);
		}

		private void HandleServerEventUpdatePollProgress(GameNetworkMessage baseMessage)
		{
			PollProgress pollProgress = (PollProgress)baseMessage;
			this.UpdatePollProgress(pollProgress.VotesAccepted, pollProgress.VotesRejected);
		}

		private void HandleServerEventPollCancelled(GameNetworkMessage baseMessage)
		{
			this.CancelPoll();
		}

		private void HandleServerEventKickPlayerPollClosed(GameNetworkMessage baseMessage)
		{
			KickPlayerPollClosed kickPlayerPollClosed = (KickPlayerPollClosed)baseMessage;
			this.CloseKickPlayerPoll(kickPlayerPollClosed.Accepted, kickPlayerPollClosed.PlayerPeer);
		}

		private void HandleServerEventPollRequestRejected(GameNetworkMessage baseMessage)
		{
			PollRequestRejected pollRequestRejected = (PollRequestRejected)baseMessage;
			this.RejectPoll((MultiplayerPollRejectReason)pollRequestRejected.Reason);
		}

		public const int MinimumParticipantCountRequired = 3;

		public Action<MissionPeer, MissionPeer, bool> OnKickPollOpened;

		public Action<MultiplayerPollRejectReason> OnPollRejected;

		public Action<int, int> OnPollUpdated;

		public Action OnPollClosed;

		public Action OnPollCancelled;

		private MissionLobbyComponent _missionLobbyComponent;

		private MultiplayerGameNotificationsComponent _notificationsComponent;

		private MultiplayerPollComponent.MultiplayerPoll _ongoingPoll;

		private abstract class MultiplayerPoll
		{
			public MultiplayerPollComponent.MultiplayerPoll.Type PollType { get; }

			public bool IsOpen { get; private set; }

			private int OpenTime { get; }

			private int CloseTime { get; set; }

			public List<NetworkCommunicator> ParticipantsToVote
			{
				get
				{
					return this._participantsToVote;
				}
			}

			protected MultiplayerPoll(MissionLobbyComponent missionLobbyComponent, MultiplayerPollComponent.MultiplayerPoll.Type pollType, List<NetworkCommunicator> participantsToVote)
			{
				this._missionLobbyComponent = missionLobbyComponent;
				this.PollType = pollType;
				if (participantsToVote != null)
				{
					this._participantsToVote = participantsToVote;
				}
				this.OpenTime = Environment.TickCount;
				this.CloseTime = 0;
				this.AcceptedCount = 0;
				this.RejectedCount = 0;
				this.IsOpen = true;
			}

			public virtual bool IsCancelled()
			{
				return false;
			}

			public virtual List<NetworkCommunicator> GetPollProgressReceivers()
			{
				return GameNetwork.NetworkPeers.ToList<NetworkCommunicator>();
			}

			public void Tick()
			{
				if (GameNetwork.IsServer)
				{
					for (int i = this._participantsToVote.Count - 1; i >= 0; i--)
					{
						if (!this._participantsToVote[i].IsConnectionActive)
						{
							this._participantsToVote.RemoveAt(i);
						}
					}
					if (this.IsCancelled())
					{
						Action<MultiplayerPollComponent.MultiplayerPoll> onCancelledOnServer = this.OnCancelledOnServer;
						if (onCancelledOnServer == null)
						{
							return;
						}
						onCancelledOnServer(this);
						return;
					}
					else if (this.OpenTime < Environment.TickCount - 30000 || this.ResultsFinalized())
					{
						Action<MultiplayerPollComponent.MultiplayerPoll> onClosedOnServer = this.OnClosedOnServer;
						if (onClosedOnServer == null)
						{
							return;
						}
						onClosedOnServer(this);
					}
				}
			}

			public void Close()
			{
				this.CloseTime = Environment.TickCount;
				this.IsOpen = false;
			}

			public void Cancel()
			{
				this.Close();
			}

			public bool ApplyVote(NetworkCommunicator peer, bool accepted)
			{
				bool flag = false;
				if (this._participantsToVote.Contains(peer))
				{
					if (accepted)
					{
						this.AcceptedCount++;
					}
					else
					{
						this.RejectedCount++;
					}
					this._participantsToVote.Remove(peer);
					flag = true;
				}
				return flag;
			}

			public bool GotEnoughAcceptVotesToEnd()
			{
				MissionLobbyComponent.MultiplayerGameType missionType = this._missionLobbyComponent.MissionType;
				bool flag;
				if (missionType == MissionLobbyComponent.MultiplayerGameType.Skirmish || missionType == MissionLobbyComponent.MultiplayerGameType.Captain)
				{
					flag = this.AcceptedByAllParticipants();
				}
				else
				{
					flag = this.AcceptedByMajority();
				}
				return flag;
			}

			private bool GotEnoughRejectVotesToEnd()
			{
				MissionLobbyComponent.MultiplayerGameType missionType = this._missionLobbyComponent.MissionType;
				bool flag;
				if (missionType == MissionLobbyComponent.MultiplayerGameType.Skirmish || missionType == MissionLobbyComponent.MultiplayerGameType.Captain)
				{
					flag = this.RejectedByAtLeastOneParticipant();
				}
				else
				{
					flag = this.RejectedByMajority();
				}
				return flag;
			}

			private bool AcceptedByAllParticipants()
			{
				return this.AcceptedCount == this.GetPollParticipantCount();
			}

			private bool AcceptedByMajority()
			{
				return (float)this.AcceptedCount / (float)this.GetPollParticipantCount() > 0.50001f;
			}

			private bool RejectedByAtLeastOneParticipant()
			{
				return this.RejectedCount > 0;
			}

			private bool RejectedByMajority()
			{
				return (float)this.RejectedCount / (float)this.GetPollParticipantCount() > 0.50001f;
			}

			private int GetPollParticipantCount()
			{
				return this._participantsToVote.Count + this.AcceptedCount + this.RejectedCount;
			}

			private bool ResultsFinalized()
			{
				return this.GotEnoughAcceptVotesToEnd() || this.GotEnoughRejectVotesToEnd() || this._participantsToVote.Count == 0;
			}

			private const int TimeoutInSeconds = 30;

			public Action<MultiplayerPollComponent.MultiplayerPoll> OnClosedOnServer;

			public Action<MultiplayerPollComponent.MultiplayerPoll> OnCancelledOnServer;

			public int AcceptedCount;

			public int RejectedCount;

			private readonly List<NetworkCommunicator> _participantsToVote;

			private readonly MissionLobbyComponent _missionLobbyComponent;

			public enum Type
			{
				KickPlayer,
				BanPlayer,
				ChangeGame
			}
		}

		private class KickPlayerPoll : MultiplayerPollComponent.MultiplayerPoll
		{
			public NetworkCommunicator TargetPeer { get; }

			public KickPlayerPoll(MissionLobbyComponent missionLobbyComponent, List<NetworkCommunicator> participantsToVote, NetworkCommunicator targetPeer, Team team)
				: base(missionLobbyComponent, MultiplayerPollComponent.MultiplayerPoll.Type.KickPlayer, participantsToVote)
			{
				this.TargetPeer = targetPeer;
				this._team = team;
			}

			public override bool IsCancelled()
			{
				return !this.TargetPeer.IsConnectionActive || this.TargetPeer.QuitFromMission;
			}

			public override List<NetworkCommunicator> GetPollProgressReceivers()
			{
				List<NetworkCommunicator> list = new List<NetworkCommunicator>();
				foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
				{
					MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
					if (component != null && component.Team == this._team)
					{
						list.Add(networkCommunicator);
					}
				}
				return list;
			}

			public const int RequestLimitPerPeer = 2;

			private readonly Team _team;
		}

		private class BanPlayerPoll : MultiplayerPollComponent.MultiplayerPoll
		{
			public NetworkCommunicator TargetPeer { get; }

			public BanPlayerPoll(MissionLobbyComponent missionLobbyComponent, List<NetworkCommunicator> participantsToVote, NetworkCommunicator targetPeer)
				: base(missionLobbyComponent, MultiplayerPollComponent.MultiplayerPoll.Type.BanPlayer, participantsToVote)
			{
				this.TargetPeer = targetPeer;
			}
		}

		private class ChangeGamePoll : MultiplayerPollComponent.MultiplayerPoll
		{
			public string GameType { get; }

			public string MapName { get; }

			public ChangeGamePoll(MissionLobbyComponent missionLobbyComponent, List<NetworkCommunicator> participantsToVote, string gameType, string scene)
				: base(missionLobbyComponent, MultiplayerPollComponent.MultiplayerPoll.Type.ChangeGame, participantsToVote)
			{
				this.GameType = gameType;
				this.MapName = scene;
			}
		}
	}
}
