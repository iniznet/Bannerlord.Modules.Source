using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002A7 RID: 679
	public class MultiplayerPollComponent : MissionNetwork
	{
		// Token: 0x06002573 RID: 9587 RVA: 0x0008DD5B File Offset: 0x0008BF5B
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._notificationsComponent = base.Mission.GetMissionBehavior<MultiplayerGameNotificationsComponent>();
		}

		// Token: 0x06002574 RID: 9588 RVA: 0x0008DD85 File Offset: 0x0008BF85
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

		// Token: 0x06002575 RID: 9589 RVA: 0x0008DDA0 File Offset: 0x0008BFA0
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

		// Token: 0x06002576 RID: 9590 RVA: 0x0008DDF4 File Offset: 0x0008BFF4
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

		// Token: 0x06002577 RID: 9591 RVA: 0x0008DE84 File Offset: 0x0008C084
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

		// Token: 0x06002578 RID: 9592 RVA: 0x0008DEAC File Offset: 0x0008C0AC
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

		// Token: 0x06002579 RID: 9593 RVA: 0x0008DED2 File Offset: 0x0008C0D2
		private void UpdatePollProgress(int votesAccepted, int votesRejected)
		{
			Action<int, int> onPollUpdated = this.OnPollUpdated;
			if (onPollUpdated == null)
			{
				return;
			}
			onPollUpdated(votesAccepted, votesRejected);
		}

		// Token: 0x0600257A RID: 9594 RVA: 0x0008DEE6 File Offset: 0x0008C0E6
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

		// Token: 0x0600257B RID: 9595 RVA: 0x0008DF14 File Offset: 0x0008C114
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

		// Token: 0x0600257C RID: 9596 RVA: 0x0008DF5C File Offset: 0x0008C15C
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

		// Token: 0x0600257D RID: 9597 RVA: 0x0008DF90 File Offset: 0x0008C190
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
							if (networkCommunicator != targetPeer)
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

		// Token: 0x0600257E RID: 9598 RVA: 0x0008E0EC File Offset: 0x0008C2EC
		private void OpenKickPlayerPoll(NetworkCommunicator targetPeer, NetworkCommunicator pollCreatorPeer, bool banPlayer, List<NetworkCommunicator> participantsToVote)
		{
			MissionPeer component = pollCreatorPeer.GetComponent<MissionPeer>();
			MissionPeer component2 = targetPeer.GetComponent<MissionPeer>();
			this._ongoingPoll = new MultiplayerPollComponent.KickPlayerPoll(this._missionLobbyComponent.MissionType, participantsToVote, targetPeer, component.Team);
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

		// Token: 0x0600257F RID: 9599 RVA: 0x0008E1A0 File Offset: 0x0008C3A0
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

		// Token: 0x06002580 RID: 9600 RVA: 0x0008E230 File Offset: 0x0008C430
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

		// Token: 0x06002581 RID: 9601 RVA: 0x0008E288 File Offset: 0x0008C488
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

		// Token: 0x06002582 RID: 9602 RVA: 0x0008E374 File Offset: 0x0008C574
		private void StartChangeGamePollOnServer(NetworkCommunicator pollCreatorPeer, string gameType, string scene)
		{
			if (this._ongoingPoll == null)
			{
				List<NetworkCommunicator> list = GameNetwork.NetworkPeers.ToList<NetworkCommunicator>();
				this._ongoingPoll = new MultiplayerPollComponent.ChangeGamePoll(this._missionLobbyComponent.MissionType, list, gameType, scene);
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

		// Token: 0x06002583 RID: 9603 RVA: 0x0008E40B File Offset: 0x0008C60B
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

		// Token: 0x06002584 RID: 9604 RVA: 0x0008E43F File Offset: 0x0008C63F
		private void ShowChangeGamePoll(string gameType, string scene)
		{
		}

		// Token: 0x06002585 RID: 9605 RVA: 0x0008E444 File Offset: 0x0008C644
		private void OnChangeGamePollClosedOnServer(MultiplayerPollComponent.MultiplayerPoll multiplayerPoll)
		{
			MultiplayerPollComponent.ChangeGamePoll changeGamePoll = multiplayerPoll as MultiplayerPollComponent.ChangeGamePoll;
			MultiplayerOptions.MultiplayerOptionsAccessMode multiplayerOptionsAccessMode = MultiplayerOptions.MultiplayerOptionsAccessMode.NextMapOptions;
			MultiplayerOptions.OptionType.GameType.SetValue(changeGamePoll.GameType, multiplayerOptionsAccessMode);
			MultiplayerOptions.Instance.OnGameTypeChanged(multiplayerOptionsAccessMode);
			MultiplayerOptions.OptionType.Map.SetValue(changeGamePoll.MapName, multiplayerOptionsAccessMode);
			this._missionLobbyComponent.SetStateEndingAsServer();
		}

		// Token: 0x06002586 RID: 9606 RVA: 0x0008E48C File Offset: 0x0008C68C
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register<PollRequestRejected>(new GameNetworkMessage.ServerMessageHandlerDelegate<PollRequestRejected>(this.HandleServerEventPollRequestRejected));
				registerer.Register<PollProgress>(new GameNetworkMessage.ServerMessageHandlerDelegate<PollProgress>(this.HandleServerEventUpdatePollProgress));
				registerer.Register<PollCancelled>(new GameNetworkMessage.ServerMessageHandlerDelegate<PollCancelled>(this.HandleServerEventPollCancelled));
				registerer.Register<KickPlayerPollOpened>(new GameNetworkMessage.ServerMessageHandlerDelegate<KickPlayerPollOpened>(this.HandleServerEventKickPlayerPollOpened));
				registerer.Register<KickPlayerPollClosed>(new GameNetworkMessage.ServerMessageHandlerDelegate<KickPlayerPollClosed>(this.HandleServerEventKickPlayerPollClosed));
				registerer.Register<NetworkMessages.FromServer.ChangeGamePoll>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.ChangeGamePoll>(this.HandleServerEventChangeGamePoll));
				return;
			}
			if (GameNetwork.IsServer)
			{
				registerer.Register<PollResponse>(new GameNetworkMessage.ClientMessageHandlerDelegate<PollResponse>(this.HandleClientEventPollResponse));
				registerer.Register<KickPlayerPollRequested>(new GameNetworkMessage.ClientMessageHandlerDelegate<KickPlayerPollRequested>(this.HandleClientEventKickPlayerPollRequested));
				registerer.Register<NetworkMessages.FromClient.ChangeGamePoll>(new GameNetworkMessage.ClientMessageHandlerDelegate<NetworkMessages.FromClient.ChangeGamePoll>(this.HandleClientEventChangeGamePoll));
			}
		}

		// Token: 0x06002587 RID: 9607 RVA: 0x0008E54A File Offset: 0x0008C74A
		private bool HandleClientEventChangeGamePoll(NetworkCommunicator peer, NetworkMessages.FromClient.ChangeGamePoll message)
		{
			this.StartChangeGamePollOnServer(peer, message.GameType, message.Map);
			return true;
		}

		// Token: 0x06002588 RID: 9608 RVA: 0x0008E560 File Offset: 0x0008C760
		private bool HandleClientEventKickPlayerPollRequested(NetworkCommunicator peer, KickPlayerPollRequested message)
		{
			this.OpenKickPlayerPollOnServer(peer, message.PlayerPeer, message.BanPlayer);
			return true;
		}

		// Token: 0x06002589 RID: 9609 RVA: 0x0008E576 File Offset: 0x0008C776
		private bool HandleClientEventPollResponse(NetworkCommunicator peer, PollResponse message)
		{
			this.ApplyVote(peer, message.Accepted);
			return true;
		}

		// Token: 0x0600258A RID: 9610 RVA: 0x0008E586 File Offset: 0x0008C786
		private void HandleServerEventChangeGamePoll(NetworkMessages.FromServer.ChangeGamePoll message)
		{
			this.ShowChangeGamePoll(message.GameType, message.Map);
		}

		// Token: 0x0600258B RID: 9611 RVA: 0x0008E59A File Offset: 0x0008C79A
		private void HandleServerEventKickPlayerPollOpened(KickPlayerPollOpened message)
		{
			this.OpenKickPlayerPoll(message.PlayerPeer, message.InitiatorPeer, message.BanPlayer, null);
		}

		// Token: 0x0600258C RID: 9612 RVA: 0x0008E5B5 File Offset: 0x0008C7B5
		private void HandleServerEventUpdatePollProgress(PollProgress message)
		{
			this.UpdatePollProgress(message.VotesAccepted, message.VotesRejected);
		}

		// Token: 0x0600258D RID: 9613 RVA: 0x0008E5C9 File Offset: 0x0008C7C9
		private void HandleServerEventPollCancelled(PollCancelled message)
		{
			this.CancelPoll();
		}

		// Token: 0x0600258E RID: 9614 RVA: 0x0008E5D1 File Offset: 0x0008C7D1
		private void HandleServerEventKickPlayerPollClosed(KickPlayerPollClosed message)
		{
			this.CloseKickPlayerPoll(message.Accepted, message.PlayerPeer);
		}

		// Token: 0x0600258F RID: 9615 RVA: 0x0008E5E5 File Offset: 0x0008C7E5
		private void HandleServerEventPollRequestRejected(PollRequestRejected message)
		{
			this.RejectPoll((MultiplayerPollRejectReason)message.Reason);
		}

		// Token: 0x04000DD1 RID: 3537
		public const int MinimumParticipantCountRequired = 3;

		// Token: 0x04000DD2 RID: 3538
		public Action<MissionPeer, MissionPeer, bool> OnKickPollOpened;

		// Token: 0x04000DD3 RID: 3539
		public Action<MultiplayerPollRejectReason> OnPollRejected;

		// Token: 0x04000DD4 RID: 3540
		public Action<int, int> OnPollUpdated;

		// Token: 0x04000DD5 RID: 3541
		public Action OnPollClosed;

		// Token: 0x04000DD6 RID: 3542
		public Action OnPollCancelled;

		// Token: 0x04000DD7 RID: 3543
		private MissionLobbyComponent _missionLobbyComponent;

		// Token: 0x04000DD8 RID: 3544
		private MultiplayerGameNotificationsComponent _notificationsComponent;

		// Token: 0x04000DD9 RID: 3545
		private MultiplayerPollComponent.MultiplayerPoll _ongoingPoll;

		// Token: 0x020005C9 RID: 1481
		private abstract class MultiplayerPoll
		{
			// Token: 0x170009AC RID: 2476
			// (get) Token: 0x06003BE2 RID: 15330 RVA: 0x000F06B1 File Offset: 0x000EE8B1
			public MultiplayerPollComponent.MultiplayerPoll.Type PollType { get; }

			// Token: 0x170009AD RID: 2477
			// (get) Token: 0x06003BE3 RID: 15331 RVA: 0x000F06B9 File Offset: 0x000EE8B9
			// (set) Token: 0x06003BE4 RID: 15332 RVA: 0x000F06C1 File Offset: 0x000EE8C1
			public bool IsOpen { get; private set; }

			// Token: 0x170009AE RID: 2478
			// (get) Token: 0x06003BE5 RID: 15333 RVA: 0x000F06CA File Offset: 0x000EE8CA
			private int OpenTime { get; }

			// Token: 0x170009AF RID: 2479
			// (get) Token: 0x06003BE6 RID: 15334 RVA: 0x000F06D2 File Offset: 0x000EE8D2
			// (set) Token: 0x06003BE7 RID: 15335 RVA: 0x000F06DA File Offset: 0x000EE8DA
			private int CloseTime { get; set; }

			// Token: 0x170009B0 RID: 2480
			// (get) Token: 0x06003BE8 RID: 15336 RVA: 0x000F06E3 File Offset: 0x000EE8E3
			public List<NetworkCommunicator> ParticipantsToVote
			{
				get
				{
					return this._participantsToVote;
				}
			}

			// Token: 0x06003BE9 RID: 15337 RVA: 0x000F06EC File Offset: 0x000EE8EC
			protected MultiplayerPoll(MissionLobbyComponent.MultiplayerGameType gameType, MultiplayerPollComponent.MultiplayerPoll.Type pollType, List<NetworkCommunicator> participantsToVote)
			{
				this._gameType = gameType;
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

			// Token: 0x06003BEA RID: 15338 RVA: 0x000F073E File Offset: 0x000EE93E
			public virtual bool IsCancelled()
			{
				return false;
			}

			// Token: 0x06003BEB RID: 15339 RVA: 0x000F0741 File Offset: 0x000EE941
			public virtual List<NetworkCommunicator> GetPollProgressReceivers()
			{
				return GameNetwork.NetworkPeers.ToList<NetworkCommunicator>();
			}

			// Token: 0x06003BEC RID: 15340 RVA: 0x000F0750 File Offset: 0x000EE950
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

			// Token: 0x06003BED RID: 15341 RVA: 0x000F07E3 File Offset: 0x000EE9E3
			public void Close()
			{
				this.CloseTime = Environment.TickCount;
				this.IsOpen = false;
			}

			// Token: 0x06003BEE RID: 15342 RVA: 0x000F07F7 File Offset: 0x000EE9F7
			public void Cancel()
			{
				this.Close();
			}

			// Token: 0x06003BEF RID: 15343 RVA: 0x000F0800 File Offset: 0x000EEA00
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

			// Token: 0x06003BF0 RID: 15344 RVA: 0x000F0850 File Offset: 0x000EEA50
			public bool GotEnoughAcceptVotesToEnd()
			{
				bool flag;
				if (this._gameType == MissionLobbyComponent.MultiplayerGameType.Skirmish || this._gameType == MissionLobbyComponent.MultiplayerGameType.Captain)
				{
					flag = this.AcceptedByAllParticipants();
				}
				else
				{
					flag = this.AcceptedByMajority();
				}
				return flag;
			}

			// Token: 0x06003BF1 RID: 15345 RVA: 0x000F0888 File Offset: 0x000EEA88
			private bool GotEnoughRejectVotesToEnd()
			{
				bool flag;
				if (this._gameType == MissionLobbyComponent.MultiplayerGameType.Skirmish || this._gameType == MissionLobbyComponent.MultiplayerGameType.Captain)
				{
					flag = this.RejectedByAtLeastOneParticipant();
				}
				else
				{
					flag = this.RejectedByMajority();
				}
				return flag;
			}

			// Token: 0x06003BF2 RID: 15346 RVA: 0x000F08BF File Offset: 0x000EEABF
			private bool AcceptedByAllParticipants()
			{
				return this.AcceptedCount == this.GetPollParticipantCount();
			}

			// Token: 0x06003BF3 RID: 15347 RVA: 0x000F08CF File Offset: 0x000EEACF
			private bool AcceptedByMajority()
			{
				return (float)this.AcceptedCount / (float)this.GetPollParticipantCount() > 0.50001f;
			}

			// Token: 0x06003BF4 RID: 15348 RVA: 0x000F08E7 File Offset: 0x000EEAE7
			private bool RejectedByAtLeastOneParticipant()
			{
				return this.RejectedCount > 0;
			}

			// Token: 0x06003BF5 RID: 15349 RVA: 0x000F08F2 File Offset: 0x000EEAF2
			private bool RejectedByMajority()
			{
				return (float)this.RejectedCount / (float)this.GetPollParticipantCount() > 0.50001f;
			}

			// Token: 0x06003BF6 RID: 15350 RVA: 0x000F090A File Offset: 0x000EEB0A
			private int GetPollParticipantCount()
			{
				return this._participantsToVote.Count + this.AcceptedCount + this.RejectedCount;
			}

			// Token: 0x06003BF7 RID: 15351 RVA: 0x000F0925 File Offset: 0x000EEB25
			private bool ResultsFinalized()
			{
				return this.GotEnoughAcceptVotesToEnd() || this.GotEnoughRejectVotesToEnd() || this._participantsToVote.Count == 0;
			}

			// Token: 0x04001E30 RID: 7728
			private const int TimeoutInSeconds = 30;

			// Token: 0x04001E31 RID: 7729
			public Action<MultiplayerPollComponent.MultiplayerPoll> OnClosedOnServer;

			// Token: 0x04001E32 RID: 7730
			public Action<MultiplayerPollComponent.MultiplayerPoll> OnCancelledOnServer;

			// Token: 0x04001E33 RID: 7731
			public int AcceptedCount;

			// Token: 0x04001E34 RID: 7732
			public int RejectedCount;

			// Token: 0x04001E35 RID: 7733
			private readonly List<NetworkCommunicator> _participantsToVote;

			// Token: 0x04001E36 RID: 7734
			private readonly MissionLobbyComponent.MultiplayerGameType _gameType;

			// Token: 0x02000701 RID: 1793
			public enum Type
			{
				// Token: 0x0400234E RID: 9038
				KickPlayer,
				// Token: 0x0400234F RID: 9039
				BanPlayer,
				// Token: 0x04002350 RID: 9040
				ChangeGame
			}
		}

		// Token: 0x020005CA RID: 1482
		private class KickPlayerPoll : MultiplayerPollComponent.MultiplayerPoll
		{
			// Token: 0x170009B1 RID: 2481
			// (get) Token: 0x06003BF8 RID: 15352 RVA: 0x000F0947 File Offset: 0x000EEB47
			public NetworkCommunicator TargetPeer { get; }

			// Token: 0x06003BF9 RID: 15353 RVA: 0x000F094F File Offset: 0x000EEB4F
			public KickPlayerPoll(MissionLobbyComponent.MultiplayerGameType gameType, List<NetworkCommunicator> participantsToVote, NetworkCommunicator targetPeer, Team team)
				: base(gameType, MultiplayerPollComponent.MultiplayerPoll.Type.KickPlayer, participantsToVote)
			{
				this.TargetPeer = targetPeer;
				this._team = team;
			}

			// Token: 0x06003BFA RID: 15354 RVA: 0x000F0969 File Offset: 0x000EEB69
			public override bool IsCancelled()
			{
				return !this.TargetPeer.IsConnectionActive || this.TargetPeer.QuitFromMission;
			}

			// Token: 0x06003BFB RID: 15355 RVA: 0x000F0988 File Offset: 0x000EEB88
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

			// Token: 0x04001E3B RID: 7739
			public const int RequestLimitPerPeer = 2;

			// Token: 0x04001E3C RID: 7740
			private readonly Team _team;
		}

		// Token: 0x020005CB RID: 1483
		private class BanPlayerPoll : MultiplayerPollComponent.MultiplayerPoll
		{
			// Token: 0x170009B2 RID: 2482
			// (get) Token: 0x06003BFC RID: 15356 RVA: 0x000F09F4 File Offset: 0x000EEBF4
			public NetworkCommunicator TargetPeer { get; }

			// Token: 0x06003BFD RID: 15357 RVA: 0x000F09FC File Offset: 0x000EEBFC
			public BanPlayerPoll(MissionLobbyComponent.MultiplayerGameType gameType, List<NetworkCommunicator> participantsToVote, NetworkCommunicator targetPeer)
				: base(gameType, MultiplayerPollComponent.MultiplayerPoll.Type.BanPlayer, participantsToVote)
			{
				this.TargetPeer = targetPeer;
			}
		}

		// Token: 0x020005CC RID: 1484
		private class ChangeGamePoll : MultiplayerPollComponent.MultiplayerPoll
		{
			// Token: 0x170009B3 RID: 2483
			// (get) Token: 0x06003BFE RID: 15358 RVA: 0x000F0A0E File Offset: 0x000EEC0E
			public string GameType { get; }

			// Token: 0x170009B4 RID: 2484
			// (get) Token: 0x06003BFF RID: 15359 RVA: 0x000F0A16 File Offset: 0x000EEC16
			public string MapName { get; }

			// Token: 0x06003C00 RID: 15360 RVA: 0x000F0A1E File Offset: 0x000EEC1E
			public ChangeGamePoll(MissionLobbyComponent.MultiplayerGameType currentGameType, List<NetworkCommunicator> participantsToVote, string gameType, string scene)
				: base(currentGameType, MultiplayerPollComponent.MultiplayerPoll.Type.ChangeGame, participantsToVote)
			{
				this.GameType = gameType;
				this.MapName = scene;
			}
		}
	}
}
