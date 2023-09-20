using System;
using System.Linq;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002A4 RID: 676
	public class MultiplayerGameNotificationsComponent : MissionNetwork
	{
		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x0600254A RID: 9546 RVA: 0x0008CBCF File Offset: 0x0008ADCF
		public static int NotificationCount
		{
			get
			{
				return 18;
			}
		}

		// Token: 0x0600254B RID: 9547 RVA: 0x0008CBD3 File Offset: 0x0008ADD3
		public void WarmupEnding()
		{
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattleWarmupEnding, 30, -1, null, null);
		}

		// Token: 0x0600254C RID: 9548 RVA: 0x0008CBE4 File Offset: 0x0008ADE4
		public void GameOver(Team winnerTeam)
		{
			if (winnerTeam == null)
			{
				this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GameOverDraw, -1, -1, null, null);
				return;
			}
			Team team = ((winnerTeam.Side == BattleSideEnum.Attacker) ? base.Mission.Teams.Defender : base.Mission.Teams.Attacker);
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GameOverVictory, -1, -1, winnerTeam, null);
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GameOverDefeat, -1, -1, team, null);
		}

		// Token: 0x0600254D RID: 9549 RVA: 0x0008CC42 File Offset: 0x0008AE42
		public void PreparationStarted()
		{
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattlePreparationStart, -1, -1, null, null);
		}

		// Token: 0x0600254E RID: 9550 RVA: 0x0008CC50 File Offset: 0x0008AE50
		public void FlagsXRemoved(FlagCapturePoint removedFlag)
		{
			int flagChar = removedFlag.FlagChar;
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXRemoved, flagChar, -1, null, null);
		}

		// Token: 0x0600254F RID: 9551 RVA: 0x0008CC70 File Offset: 0x0008AE70
		public void FlagXRemaining(FlagCapturePoint remainingFlag)
		{
			int flagChar = remainingFlag.FlagChar;
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXRemaining, flagChar, -1, null, null);
		}

		// Token: 0x06002550 RID: 9552 RVA: 0x0008CC8F File Offset: 0x0008AE8F
		public void FlagsWillBeRemovedInXSeconds(int timeLeft)
		{
			this.ShowNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagsWillBeRemoved, new int[] { timeLeft });
		}

		// Token: 0x06002551 RID: 9553 RVA: 0x0008CCA4 File Offset: 0x0008AEA4
		public void FlagXCapturedByTeamX(SynchedMissionObject flag, Team capturingTeam)
		{
			FlagCapturePoint flagCapturePoint = flag as FlagCapturePoint;
			int num = ((flagCapturePoint != null) ? flagCapturePoint.FlagChar : 65);
			Team team = ((capturingTeam.Side == BattleSideEnum.Attacker) ? base.Mission.Teams.Defender : base.Mission.Teams.Attacker);
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByYourTeam, num, -1, capturingTeam, null);
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByOtherTeam, num, -1, team, null);
		}

		// Token: 0x06002552 RID: 9554 RVA: 0x0008CD0C File Offset: 0x0008AF0C
		public void GoldCarriedFromPreviousRound(int carriedGoldAmount, NetworkCommunicator syncToPeer)
		{
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GoldCarriedFromPreviousRound, carriedGoldAmount, -1, null, syncToPeer);
		}

		// Token: 0x06002553 RID: 9555 RVA: 0x0008CD27 File Offset: 0x0008AF27
		public void PlayerIsInactive(NetworkCommunicator peer)
		{
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.PlayerIsInactive, -1, -1, null, peer);
		}

		// Token: 0x06002554 RID: 9556 RVA: 0x0008CD35 File Offset: 0x0008AF35
		public void FormationAutoFollowEnforced(NetworkCommunicator peer)
		{
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FormationAutoFollowEnforced, -1, -1, null, peer);
		}

		// Token: 0x06002555 RID: 9557 RVA: 0x0008CD44 File Offset: 0x0008AF44
		public void PollRejected(MultiplayerPollRejectReason reason)
		{
			if (reason == MultiplayerPollRejectReason.TooManyPollRequests)
			{
				this.ShowNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.TooManyPollRequests, Array.Empty<int>());
				return;
			}
			if (reason == MultiplayerPollRejectReason.HasOngoingPoll)
			{
				this.ShowNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.HasOngoingPoll, Array.Empty<int>());
				return;
			}
			if (reason == MultiplayerPollRejectReason.NotEnoughPlayersToOpenPoll)
			{
				this.ShowNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.NotEnoughPlayersToOpenPoll, new int[] { 3 });
				return;
			}
			if (reason == MultiplayerPollRejectReason.KickPollTargetNotSynced)
			{
				this.ShowNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.KickPollTargetNotSynced, Array.Empty<int>());
				return;
			}
			Debug.FailedAssert("Notification of a PollRejectReason is missing (" + reason + ")", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\MissionNetworkLogics\\MultiplayerGameNotificationsComponent.cs", "PollRejected", 153);
		}

		// Token: 0x06002556 RID: 9558 RVA: 0x0008CDC6 File Offset: 0x0008AFC6
		public void PlayerKicked(NetworkCommunicator kickedPeer)
		{
			this.ShowNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.PlayerIsKicked, new int[] { kickedPeer.Index });
		}

		// Token: 0x06002557 RID: 9559 RVA: 0x0008CDDF File Offset: 0x0008AFDF
		private void HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum notification, int param1 = -1, int param2 = -1, Team syncToTeam = null, NetworkCommunicator syncToPeer = null)
		{
			if (syncToPeer != null)
			{
				this.SendNotificationToPeer(syncToPeer, notification, param1, param2);
				return;
			}
			if (syncToTeam != null)
			{
				this.SendNotificationToTeam(syncToTeam, notification, param1, param2);
				return;
			}
			this.SendNotificationToEveryone(notification, param1, param2);
		}

		// Token: 0x06002558 RID: 9560 RVA: 0x0008CE0C File Offset: 0x0008B00C
		private void ShowNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum notification, params int[] parameters)
		{
			if (!GameNetwork.IsDedicatedServer)
			{
				NotificationProperty notificationProperty = (NotificationProperty)notification.GetType().GetField(notification.ToString()).GetCustomAttributes(typeof(NotificationProperty), false)
					.Single<object>();
				if (notificationProperty != null)
				{
					int[] array = parameters.Where((int x) => x != -1).ToArray<int>();
					TextObject textObject = this.ToNotificationString(notification, notificationProperty, array);
					string text = this.ToSoundString(notification, notificationProperty, array);
					MBInformationManager.AddQuickInformation(textObject, 0, null, text);
				}
			}
		}

		// Token: 0x06002559 RID: 9561 RVA: 0x0008CEA4 File Offset: 0x0008B0A4
		private void SendNotificationToEveryone(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum message, int param1 = -1, int param2 = -1)
		{
			this.ShowNotification(message, new int[] { param1, param2 });
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new NotificationMessage((int)message, param1, param2));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

		// Token: 0x0600255A RID: 9562 RVA: 0x0008CED4 File Offset: 0x0008B0D4
		private void SendNotificationToPeer(NetworkCommunicator peer, MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum message, int param1 = -1, int param2 = -1)
		{
			if (peer.IsServerPeer)
			{
				this.ShowNotification(message, new int[] { param1, param2 });
				return;
			}
			GameNetwork.BeginModuleEventAsServer(peer);
			GameNetwork.WriteMessage(new NotificationMessage((int)message, param1, param2));
			GameNetwork.EndModuleEventAsServer();
		}

		// Token: 0x0600255B RID: 9563 RVA: 0x0008CF10 File Offset: 0x0008B110
		private void SendNotificationToTeam(Team team, MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum message, int param1 = -1, int param2 = -1)
		{
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			if (!GameNetwork.IsDedicatedServer && ((missionPeer != null) ? missionPeer.Team : null) != null && missionPeer.Team.IsEnemyOf(team))
			{
				this.ShowNotification(message, new int[] { param1, param2 });
			}
			foreach (NetworkCommunicator networkCommunicator in GameNetwork.NetworkPeers)
			{
				MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
				if (((component != null) ? component.Team : null) != null && !component.IsMine && !component.Team.IsEnemyOf(team))
				{
					GameNetwork.BeginModuleEventAsServer(component.Peer);
					GameNetwork.WriteMessage(new NotificationMessage((int)message, param1, param2));
					GameNetwork.EndModuleEventAsServer();
				}
			}
		}

		// Token: 0x0600255C RID: 9564 RVA: 0x0008CFE8 File Offset: 0x0008B1E8
		private string ToSoundString(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum value, NotificationProperty attribute, params int[] parameters)
		{
			string text = string.Empty;
			if (string.IsNullOrEmpty(attribute.SoundIdTwo))
			{
				text = attribute.SoundIdOne;
			}
			else if (value != MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattleYouHaveXTheRound)
			{
				if (value != MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByYourTeam)
				{
					if (value == MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByOtherTeam)
					{
						text = attribute.SoundIdTwo;
					}
				}
				else
				{
					text = attribute.SoundIdOne;
				}
			}
			else
			{
				Team team = ((parameters[0] == 0) ? Mission.Current.AttackerTeam : Mission.Current.DefenderTeam);
				Team team2 = (GameNetwork.IsMyPeerReady ? GameNetwork.MyPeer.GetComponent<MissionPeer>().Team : null);
				text = attribute.SoundIdOne;
				if (team2 != null && team2 != team)
				{
					text = attribute.SoundIdTwo;
				}
			}
			return text;
		}

		// Token: 0x0600255D RID: 9565 RVA: 0x0008D07F File Offset: 0x0008B27F
		private TextObject ToNotificationString(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum value, NotificationProperty attribute, params int[] parameters)
		{
			if (parameters.Length != 0)
			{
				this.SetGameTextVariables(value, parameters);
			}
			return GameTexts.FindText(attribute.StringId, null);
		}

		// Token: 0x0600255E RID: 9566 RVA: 0x0008D09C File Offset: 0x0008B29C
		private void SetGameTextVariables(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum message, params int[] parameters)
		{
			if (parameters.Length == 0)
			{
				return;
			}
			switch (message)
			{
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattleWarmupEnding:
				GameTexts.SetVariable("SECONDS_LEFT", parameters[0]);
				return;
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattlePreparationStart:
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GameOverDraw:
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GameOverVictory:
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GameOverDefeat:
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.PlayerIsInactive:
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.HasOngoingPoll:
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.TooManyPollRequests:
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.KickPollTargetNotSynced:
				break;
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattleYouHaveXTheRound:
			{
				Team team = ((parameters[0] == 0) ? Mission.Current.AttackerTeam : Mission.Current.DefenderTeam);
				Team team2 = (GameNetwork.IsMyPeerReady ? GameNetwork.MyPeer.GetComponent<MissionPeer>().Team : null);
				if (team2 != null)
				{
					GameTexts.SetVariable("IS_WINNER", (team2 == team) ? 1 : 0);
					return;
				}
				break;
			}
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXRemoved:
				GameTexts.SetVariable("PARAM1", ((char)parameters[0]).ToString());
				return;
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXRemaining:
				GameTexts.SetVariable("PARAM1", ((char)parameters[0]).ToString());
				return;
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagsWillBeRemoved:
				GameTexts.SetVariable("PARAM1", parameters[0]);
				return;
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByYourTeam:
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByOtherTeam:
				GameTexts.SetVariable("PARAM1", ((char)parameters[0]).ToString());
				return;
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GoldCarriedFromPreviousRound:
				GameTexts.SetVariable("PARAM1", parameters[0].ToString());
				return;
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.NotEnoughPlayersToOpenPoll:
				GameTexts.SetVariable("MIN_PARTICIPANT_COUNT", parameters[0]);
				break;
			case MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.PlayerIsKicked:
				GameTexts.SetVariable("PLAYER_NAME", GameNetwork.FindNetworkPeer(parameters[0]).UserName);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600255F RID: 9567 RVA: 0x0008D1E9 File Offset: 0x0008B3E9
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register<NotificationMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<NotificationMessage>(this.HandleServerEventServerMessage));
			}
		}

		// Token: 0x06002560 RID: 9568 RVA: 0x0008D204 File Offset: 0x0008B404
		private void HandleServerEventServerMessage(NotificationMessage message)
		{
			this.ShowNotification((MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum)message.Message, new int[] { message.ParameterOne, message.ParameterTwo });
		}

		// Token: 0x06002561 RID: 9569 RVA: 0x0008D22A File Offset: 0x0008B42A
		protected override void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
		{
			bool isServerPeer = clientConnectionInfo.NetworkPeer.IsServerPeer;
		}

		// Token: 0x06002562 RID: 9570 RVA: 0x0008D238 File Offset: 0x0008B438
		protected override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
			bool isServer = GameNetwork.IsServer;
		}

		// Token: 0x020005C6 RID: 1478
		private enum MultiplayerNotificationEnum
		{
			// Token: 0x04001E11 RID: 7697
			[NotificationProperty("str_battle_warmup_ending_in_x_seconds", "event:/ui/mission/multiplayer/lastmanstanding", "")]
			BattleWarmupEnding,
			// Token: 0x04001E12 RID: 7698
			[NotificationProperty("str_battle_preparation_start", "event:/ui/mission/multiplayer/roundstart", "")]
			BattlePreparationStart,
			// Token: 0x04001E13 RID: 7699
			[NotificationProperty("str_round_result_win_lose", "event:/ui/mission/multiplayer/victory", "event:/ui/mission/multiplayer/defeat")]
			BattleYouHaveXTheRound,
			// Token: 0x04001E14 RID: 7700
			[NotificationProperty("str_mp_mission_game_over_draw", "", "")]
			GameOverDraw,
			// Token: 0x04001E15 RID: 7701
			[NotificationProperty("str_mp_mission_game_over_victory", "", "")]
			GameOverVictory,
			// Token: 0x04001E16 RID: 7702
			[NotificationProperty("str_mp_mission_game_over_defeat", "", "")]
			GameOverDefeat,
			// Token: 0x04001E17 RID: 7703
			[NotificationProperty("str_mp_flag_removed", "event:/ui/mission/multiplayer/pointsremoved", "")]
			FlagXRemoved,
			// Token: 0x04001E18 RID: 7704
			[NotificationProperty("str_sergeant_a_one_flag_remaining", "event:/ui/mission/multiplayer/pointsremoved", "")]
			FlagXRemaining,
			// Token: 0x04001E19 RID: 7705
			[NotificationProperty("str_sergeant_a_flags_will_be_removed", "event:/ui/mission/multiplayer/pointwarning", "")]
			FlagsWillBeRemoved,
			// Token: 0x04001E1A RID: 7706
			[NotificationProperty("str_sergeant_a_flag_captured_by_your_team", "event:/ui/mission/multiplayer/pointcapture", "event:/ui/mission/multiplayer/pointlost")]
			FlagXCapturedByYourTeam,
			// Token: 0x04001E1B RID: 7707
			[NotificationProperty("str_sergeant_a_flag_captured_by_other_team", "event:/ui/mission/multiplayer/pointcapture", "event:/ui/mission/multiplayer/pointlost")]
			FlagXCapturedByOtherTeam,
			// Token: 0x04001E1C RID: 7708
			[NotificationProperty("str_gold_carried_from_previous_round", "", "")]
			GoldCarriedFromPreviousRound,
			// Token: 0x04001E1D RID: 7709
			[NotificationProperty("str_player_is_inactive", "", "")]
			PlayerIsInactive,
			// Token: 0x04001E1E RID: 7710
			[NotificationProperty("str_has_ongoing_poll", "", "")]
			HasOngoingPoll,
			// Token: 0x04001E1F RID: 7711
			[NotificationProperty("str_too_many_poll_requests", "", "")]
			TooManyPollRequests,
			// Token: 0x04001E20 RID: 7712
			[NotificationProperty("str_kick_poll_target_not_synced", "", "")]
			KickPollTargetNotSynced,
			// Token: 0x04001E21 RID: 7713
			[NotificationProperty("str_not_enough_players_to_open_poll", "", "")]
			NotEnoughPlayersToOpenPoll,
			// Token: 0x04001E22 RID: 7714
			[NotificationProperty("str_player_is_kicked", "", "")]
			PlayerIsKicked,
			// Token: 0x04001E23 RID: 7715
			[NotificationProperty("str_formation_autofollow_enforced", "", "")]
			FormationAutoFollowEnforced,
			// Token: 0x04001E24 RID: 7716
			Count
		}
	}
}
