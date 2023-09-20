using System;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000292 RID: 658
	public class MissionMatchHistoryComponent : MissionNetwork
	{
		// Token: 0x06002302 RID: 8962 RVA: 0x0007FB10 File Offset: 0x0007DD10
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

		// Token: 0x06002303 RID: 8963 RVA: 0x0007FB7C File Offset: 0x0007DD7C
		private async void LoadMatchhHistory()
		{
			await MatchHistory.LoadMatchHistory();
		}

		// Token: 0x06002304 RID: 8964 RVA: 0x0007FBB0 File Offset: 0x0007DDB0
		public override void OnBehaviorInitialize()
		{
			MissionMultiplayerGameModeBaseClient missionBehavior = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			MissionLobbyComponent.MultiplayerGameType multiplayerGameType = ((missionBehavior != null) ? missionBehavior.GameType : MissionLobbyComponent.MultiplayerGameType.FreeForAll);
			this._matchInfo.GameType = multiplayerGameType.ToString();
		}

		// Token: 0x06002305 RID: 8965 RVA: 0x0007FBEC File Offset: 0x0007DDEC
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

		// Token: 0x06002306 RID: 8966 RVA: 0x0007FC79 File Offset: 0x0007DE79
		protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			registerer.Register<MissionStateChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<MissionStateChange>(this.HandleServerEventMissionStateChange));
		}

		// Token: 0x06002307 RID: 8967 RVA: 0x0007FC90 File Offset: 0x0007DE90
		private void TeamChange(NetworkCommunicator player, Team oldTeam, Team nextTeam)
		{
			this._matchInfo.AddOrUpdatePlayer(player.VirtualPlayer.Id.ToString(), player.VirtualPlayer.UserName, player.ForcedAvatarIndex, (int)nextTeam.Side);
		}

		// Token: 0x06002308 RID: 8968 RVA: 0x0007FCD8 File Offset: 0x0007DED8
		private void HandleServerEventMissionStateChange(MissionStateChange message)
		{
			if (message.CurrentState == MissionLobbyComponent.MultiplayerGameState.Ending && !this._recordedHistory)
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

		// Token: 0x06002309 RID: 8969 RVA: 0x0007FE08 File Offset: 0x0007E008
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

		// Token: 0x04000D02 RID: 3330
		private bool _recordedHistory;

		// Token: 0x04000D03 RID: 3331
		private MatchInfo _matchInfo;
	}
}
