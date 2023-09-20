using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class MissionMatchHistoryComponent : MissionNetwork
	{
		public static MissionMatchHistoryComponent CreateIfConditionsAreMet()
		{
			if (NetworkMain.GameClient.IsInGame && NetworkMain.GameClient.LastBattleIsOfficial)
			{
				return new MissionMatchHistoryComponent();
			}
			Debug.Print(string.Format("Failed to create {0}. NetworkMain.GameClient.IsInGame: {1}, NetworkMain.GameClient.LastBattleIsOfficial: {2}", typeof(MissionMatchHistoryComponent).Name, NetworkMain.GameClient.IsInGame, NetworkMain.GameClient.LastBattleIsOfficial), 0, 12, 17592186044416UL);
			return null;
		}

		private MissionMatchHistoryComponent()
		{
			this._recordedHistory = false;
			this.LoadMatchhHistory();
			MatchInfo matchInfo;
			if (MatchHistory.TryGetMatchInfo(NetworkMain.GameClient.CurrentMatchId, ref matchInfo))
			{
				this._matchInfo = matchInfo;
			}
			else
			{
				this._matchInfo = new MatchInfo();
				this._matchInfo.MatchId = NetworkMain.GameClient.CurrentMatchId;
			}
			this._matchInfo.MatchDate = DateTime.Now;
		}

		private async void LoadMatchhHistory()
		{
			await MatchHistory.LoadMatchHistory();
		}

		public override void OnBehaviorInitialize()
		{
			MissionMultiplayerGameModeBaseClient missionBehavior = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			MultiplayerGameType multiplayerGameType = ((missionBehavior != null) ? missionBehavior.GameType : 0);
			this._matchInfo.GameType = multiplayerGameType.ToString();
		}

		public override void AfterStart()
		{
			base.AfterStart();
			string strValue = MultiplayerOptionsExtensions.GetStrValue(14, 0);
			string strValue2 = MultiplayerOptionsExtensions.GetStrValue(15, 0);
			string strValue3 = MultiplayerOptionsExtensions.GetStrValue(13, 0);
			this._matchInfo.Faction1 = strValue;
			this._matchInfo.Faction2 = strValue2;
			this._matchInfo.Map = strValue3;
			MissionPeer.OnTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.TeamChange);
			base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._matchInfo.MatchType = BannerlordNetwork.LobbyMissionType.ToString();
		}

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			registerer.RegisterBaseHandler<MissionStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventMissionStateChange));
		}

		private void TeamChange(NetworkCommunicator player, Team oldTeam, Team nextTeam)
		{
			this._matchInfo.AddOrUpdatePlayer(player.VirtualPlayer.Id.ToString(), player.VirtualPlayer.UserName, player.ForcedAvatarIndex, nextTeam.Side);
		}

		private void HandleServerEventMissionStateChange(GameNetworkMessage baseMessage)
		{
			if (((MissionStateChange)baseMessage).CurrentState == 2 && !this._recordedHistory)
			{
				MissionScoreboardComponent missionBehavior = base.Mission.GetMissionBehavior<MissionScoreboardComponent>();
				if (missionBehavior != null && !missionBehavior.IsOneSided)
				{
					int roundScore = missionBehavior.GetRoundScore(1);
					int roundScore2 = missionBehavior.GetRoundScore(0);
					BattleSideEnum matchWinnerSide = missionBehavior.GetMatchWinnerSide();
					this._matchInfo.WinnerTeam = matchWinnerSide;
					this._matchInfo.AttackerScore = roundScore;
					this._matchInfo.DefenderScore = roundScore2;
					MissionScoreboardComponent.MissionScoreboardSide[] sides = missionBehavior.Sides;
					for (int i = 0; i < sides.Length; i++)
					{
						foreach (MissionPeer missionPeer in sides[i].Players)
						{
							this._matchInfo.TryUpdatePlayerStats(missionPeer.Peer.Id.ToString(), missionPeer.KillCount, missionPeer.DeathCount, missionPeer.AssistCount);
						}
					}
				}
				MatchHistory.AddMatch(this._matchInfo);
				MatchHistory.Serialize();
				this._recordedHistory = true;
			}
		}

		public override void OnRemoveBehavior()
		{
			if (this._matchInfo.GameType != "Duel" && !this._recordedHistory)
			{
				this._matchInfo.WinnerTeam = -1;
				MissionScoreboardComponent missionBehavior = base.Mission.GetMissionBehavior<MissionScoreboardComponent>();
				if (missionBehavior != null)
				{
					int roundScore = missionBehavior.GetRoundScore(1);
					int roundScore2 = missionBehavior.GetRoundScore(0);
					this._matchInfo.AttackerScore = roundScore;
					this._matchInfo.DefenderScore = roundScore2;
					MissionScoreboardComponent.MissionScoreboardSide[] sides = missionBehavior.Sides;
					for (int i = 0; i < sides.Length; i++)
					{
						foreach (MissionPeer missionPeer in sides[i].Players)
						{
							this._matchInfo.TryUpdatePlayerStats(missionPeer.Peer.Id.ToString(), missionPeer.KillCount, missionPeer.DeathCount, missionPeer.AssistCount);
						}
					}
				}
				MatchHistory.AddMatch(this._matchInfo);
				MatchHistory.Serialize();
				this._recordedHistory = true;
			}
			MissionPeer.OnTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.TeamChange);
			base.OnRemoveBehavior();
		}

		private bool _recordedHistory;

		private MatchInfo _matchInfo;
	}
}
