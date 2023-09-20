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
	public class MultiplayerGameNotificationsComponent : MissionNetwork
	{
		public static int NotificationCount
		{
			get
			{
				return 18;
			}
		}

		public void WarmupEnding()
		{
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattleWarmupEnding, 30, -1, null, null);
		}

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

		public void PreparationStarted()
		{
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.BattlePreparationStart, -1, -1, null, null);
		}

		public void FlagsXRemoved(FlagCapturePoint removedFlag)
		{
			int flagChar = removedFlag.FlagChar;
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXRemoved, flagChar, -1, null, null);
		}

		public void FlagXRemaining(FlagCapturePoint remainingFlag)
		{
			int flagChar = remainingFlag.FlagChar;
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXRemaining, flagChar, -1, null, null);
		}

		public void FlagsWillBeRemovedInXSeconds(int timeLeft)
		{
			this.ShowNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagsWillBeRemoved, new int[] { timeLeft });
		}

		public void FlagXCapturedByTeamX(SynchedMissionObject flag, Team capturingTeam)
		{
			FlagCapturePoint flagCapturePoint = flag as FlagCapturePoint;
			int num = ((flagCapturePoint != null) ? flagCapturePoint.FlagChar : 65);
			Team team = ((capturingTeam.Side == BattleSideEnum.Attacker) ? base.Mission.Teams.Defender : base.Mission.Teams.Attacker);
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByYourTeam, num, -1, capturingTeam, null);
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FlagXCapturedByOtherTeam, num, -1, team, null);
		}

		public void GoldCarriedFromPreviousRound(int carriedGoldAmount, NetworkCommunicator syncToPeer)
		{
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.GoldCarriedFromPreviousRound, carriedGoldAmount, -1, null, syncToPeer);
		}

		public void PlayerIsInactive(NetworkCommunicator peer)
		{
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.PlayerIsInactive, -1, -1, null, peer);
		}

		public void FormationAutoFollowEnforced(NetworkCommunicator peer)
		{
			this.HandleNewNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.FormationAutoFollowEnforced, -1, -1, null, peer);
		}

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

		public void PlayerKicked(NetworkCommunicator kickedPeer)
		{
			this.ShowNotification(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum.PlayerIsKicked, new int[] { kickedPeer.Index });
		}

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

		private void SendNotificationToEveryone(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum message, int param1 = -1, int param2 = -1)
		{
			this.ShowNotification(message, new int[] { param1, param2 });
			GameNetwork.BeginBroadcastModuleEvent();
			GameNetwork.WriteMessage(new NotificationMessage((int)message, param1, param2));
			GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
		}

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

		private TextObject ToNotificationString(MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum value, NotificationProperty attribute, params int[] parameters)
		{
			if (parameters.Length != 0)
			{
				this.SetGameTextVariables(value, parameters);
			}
			return GameTexts.FindText(attribute.StringId, null);
		}

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

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.RegisterBaseHandler<NotificationMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventServerMessage));
			}
		}

		private void HandleServerEventServerMessage(GameNetworkMessage baseMessage)
		{
			NotificationMessage notificationMessage = (NotificationMessage)baseMessage;
			this.ShowNotification((MultiplayerGameNotificationsComponent.MultiplayerNotificationEnum)notificationMessage.Message, new int[] { notificationMessage.ParameterOne, notificationMessage.ParameterTwo });
		}

		protected override void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
		{
			bool isServerPeer = clientConnectionInfo.NetworkPeer.IsServerPeer;
		}

		protected override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
		{
			bool isServer = GameNetwork.IsServer;
		}

		private enum MultiplayerNotificationEnum
		{
			[NotificationProperty("str_battle_warmup_ending_in_x_seconds", "event:/ui/mission/multiplayer/lastmanstanding", "")]
			BattleWarmupEnding,
			[NotificationProperty("str_battle_preparation_start", "event:/ui/mission/multiplayer/roundstart", "")]
			BattlePreparationStart,
			[NotificationProperty("str_round_result_win_lose", "event:/ui/mission/multiplayer/victory", "event:/ui/mission/multiplayer/defeat")]
			BattleYouHaveXTheRound,
			[NotificationProperty("str_mp_mission_game_over_draw", "", "")]
			GameOverDraw,
			[NotificationProperty("str_mp_mission_game_over_victory", "", "")]
			GameOverVictory,
			[NotificationProperty("str_mp_mission_game_over_defeat", "", "")]
			GameOverDefeat,
			[NotificationProperty("str_mp_flag_removed", "event:/ui/mission/multiplayer/pointsremoved", "")]
			FlagXRemoved,
			[NotificationProperty("str_sergeant_a_one_flag_remaining", "event:/ui/mission/multiplayer/pointsremoved", "")]
			FlagXRemaining,
			[NotificationProperty("str_sergeant_a_flags_will_be_removed", "event:/ui/mission/multiplayer/pointwarning", "")]
			FlagsWillBeRemoved,
			[NotificationProperty("str_sergeant_a_flag_captured_by_your_team", "event:/ui/mission/multiplayer/pointcapture", "event:/ui/mission/multiplayer/pointlost")]
			FlagXCapturedByYourTeam,
			[NotificationProperty("str_sergeant_a_flag_captured_by_other_team", "event:/ui/mission/multiplayer/pointcapture", "event:/ui/mission/multiplayer/pointlost")]
			FlagXCapturedByOtherTeam,
			[NotificationProperty("str_gold_carried_from_previous_round", "", "")]
			GoldCarriedFromPreviousRound,
			[NotificationProperty("str_player_is_inactive", "", "")]
			PlayerIsInactive,
			[NotificationProperty("str_has_ongoing_poll", "", "")]
			HasOngoingPoll,
			[NotificationProperty("str_too_many_poll_requests", "", "")]
			TooManyPollRequests,
			[NotificationProperty("str_kick_poll_target_not_synced", "", "")]
			KickPollTargetNotSynced,
			[NotificationProperty("str_not_enough_players_to_open_poll", "", "")]
			NotEnoughPlayersToOpenPoll,
			[NotificationProperty("str_player_is_kicked", "", "")]
			PlayerIsKicked,
			[NotificationProperty("str_formation_autofollow_enforced", "", "")]
			FormationAutoFollowEnforced,
			Count
		}
	}
}
