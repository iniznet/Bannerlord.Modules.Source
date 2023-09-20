using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class MissionMatchHistoryComponent : MissionNetwork
	{
		public MissionMatchHistoryComponent()
		{
			this._recordedHistory = false;
			this.LoadMatchhHistory();
			MatchInfo matchInfo;
			if (MatchHistory.TryGetMatchInfo(NetworkMain.GameClient.CurrentMatchId, out matchInfo))
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
			MissionLobbyComponent.MultiplayerGameType multiplayerGameType = ((missionBehavior != null) ? missionBehavior.GameType : MissionLobbyComponent.MultiplayerGameType.FreeForAll);
			this._matchInfo.GameType = multiplayerGameType.ToString();
		}

		public override void AfterStart()
		{
			base.AfterStart();
			string strValue = MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string strValue2 = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			string strValue3 = MultiplayerOptions.OptionType.Map.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			this._matchInfo.Faction1 = strValue;
			this._matchInfo.Faction2 = strValue2;
			this._matchInfo.Map = strValue3;
			MissionPeer.OnTeamChanged += this.TeamChange;
			base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._matchInfo.MatchType = BannerlordNetwork.LobbyMissionType.ToString();
		}

		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			registerer.RegisterBaseHandler<MissionStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<GameNetworkMessage>(this.HandleServerEventMissionStateChange));
		}

		private void TeamChange(NetworkCommunicator player, Team oldTeam, Team nextTeam)
		{
			this._matchInfo.AddOrUpdatePlayer(player.VirtualPlayer.Id.ToString(), player.VirtualPlayer.UserName, player.ForcedAvatarIndex, (int)nextTeam.Side);
		}

		private void HandleServerEventMissionStateChange(GameNetworkMessage baseMessage)
		{
			if (((MissionStateChange)baseMessage).CurrentState == MissionLobbyComponent.MultiplayerGameState.Ending && !this._recordedHistory)
			{
				MissionScoreboardComponent missionBehavior = base.Mission.GetMissionBehavior<MissionScoreboardComponent>();
				if (missionBehavior != null && !missionBehavior.IsOneSided)
				{
					int roundScore = missionBehavior.GetRoundScore(BattleSideEnum.Attacker);
					int roundScore2 = missionBehavior.GetRoundScore(BattleSideEnum.Defender);
					BattleSideEnum matchWinnerSide = missionBehavior.GetMatchWinnerSide();
					this._matchInfo.WinnerTeam = (int)matchWinnerSide;
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
					int roundScore = missionBehavior.GetRoundScore(BattleSideEnum.Attacker);
					int roundScore2 = missionBehavior.GetRoundScore(BattleSideEnum.Defender);
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
			MissionPeer.OnTeamChanged -= this.TeamChange;
			base.OnRemoveBehavior();
		}

		private bool _recordedHistory;

		private MatchInfo _matchInfo;
	}
}
