using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Scoreboard
{
	// Token: 0x02000053 RID: 83
	public class MissionScoreboardVM : ViewModel
	{
		// Token: 0x060006D6 RID: 1750 RVA: 0x0001B56C File Offset: 0x0001976C
		public MissionScoreboardVM(bool isSingleTeam, Mission mission)
		{
			this._chatBox = Game.Current.GetGameHandler<ChatBox>();
			this._chatBox.OnPlayerMuteChanged += this.OnPlayerMuteChanged;
			this._mission = mission;
			MissionLobbyComponent missionBehavior = mission.GetMissionBehavior<MissionLobbyComponent>();
			this._missionScoreboardComponent = mission.GetMissionBehavior<MissionScoreboardComponent>();
			this._voiceChatHandler = this._mission.GetMissionBehavior<VoiceChatHandler>();
			this._permissionHandler = GameNetwork.GetNetworkComponent<MultiplayerPermissionHandler>();
			if (this._voiceChatHandler != null)
			{
				this._voiceChatHandler.OnPeerMuteStatusUpdated += this.OnPeerMuteStatusUpdated;
			}
			if (this._permissionHandler != null)
			{
				this._permissionHandler.OnPlayerPlatformMuteChanged += this.OnPlayerPlatformMuteChanged;
			}
			this._canStartKickPolls = MultiplayerOptions.OptionType.AllowPollsToKickPlayers.GetBoolValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			if (this._canStartKickPolls)
			{
				this._missionPollComponent = mission.GetMissionBehavior<MultiplayerPollComponent>();
			}
			this.EndOfBattle = new MPEndOfBattleVM(mission, this._missionScoreboardComponent, isSingleTeam);
			this.PlayerActionList = new MBBindingList<StringPairItemWithActionVM>();
			this.Sides = new MBBindingList<MissionScoreboardSideVM>();
			this._missionSides = new Dictionary<BattleSideEnum, MissionScoreboardSideVM>();
			this.IsSingleSide = isSingleTeam;
			this.InitSides();
			GameKey gameKey = HotKeyManager.GetCategory("ScoreboardHotKeyCategory").GetGameKey(35);
			this.ShowMouseKey = InputKeyItemVM.CreateFromGameKey(gameKey, false);
			this._missionScoreboardComponent.OnPlayerSideChanged += this.OnPlayerSideChanged;
			this._missionScoreboardComponent.OnPlayerPropertiesChanged += this.OnPlayerPropertiesChanged;
			this._missionScoreboardComponent.OnBotPropertiesChanged += this.OnBotPropertiesChanged;
			this._missionScoreboardComponent.OnRoundPropertiesChanged += this.OnRoundPropertiesChanged;
			this._missionScoreboardComponent.OnScoreboardInitialized += this.OnScoreboardInitialized;
			this._missionScoreboardComponent.OnMVPSelected += this.OnMVPSelected;
			this.MissionName = "";
			this.IsBotsEnabled = missionBehavior.MissionType == MissionLobbyComponent.MultiplayerGameType.Captain || missionBehavior.MissionType == MissionLobbyComponent.MultiplayerGameType.Battle;
			this.RefreshValues();
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0001B774 File Offset: 0x00019974
		private void OnPlayerPlatformMuteChanged(PlayerId playerId, bool isPlayerMuted)
		{
			foreach (MissionScoreboardSideVM missionScoreboardSideVM in this.Sides)
			{
				foreach (MissionScoreboardPlayerVM missionScoreboardPlayerVM in missionScoreboardSideVM.Players)
				{
					if (missionScoreboardPlayerVM.Peer.Peer.Id.Equals(playerId))
					{
						missionScoreboardPlayerVM.UpdateIsMuted();
						return;
					}
				}
			}
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x0001B81C File Offset: 0x00019A1C
		private void OnPlayerMuteChanged(PlayerId playerId, bool isMuted)
		{
			foreach (MissionScoreboardSideVM missionScoreboardSideVM in this.Sides)
			{
				foreach (MissionScoreboardPlayerVM missionScoreboardPlayerVM in missionScoreboardSideVM.Players)
				{
					if (missionScoreboardPlayerVM.Peer.Peer.Id.Equals(playerId))
					{
						missionScoreboardPlayerVM.UpdateIsMuted();
						return;
					}
				}
			}
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x0001B8C4 File Offset: 0x00019AC4
		public override void RefreshValues()
		{
			base.RefreshValues();
			MissionLobbyComponent missionBehavior = this._mission.GetMissionBehavior<MissionLobbyComponent>();
			this.UpdateToggleMuteText();
			this.GameModeText = GameTexts.FindText("str_multiplayer_game_type", missionBehavior.MissionType.ToString()).ToString().ToLower();
			this.EndOfBattle.RefreshValues();
			this.Sides.ApplyActionOnAllItems(delegate(MissionScoreboardSideVM x)
			{
				x.RefreshValues();
			});
			this.MapName = GameTexts.FindText("str_multiplayer_scene_name", missionBehavior.Mission.SceneName).ToString();
			this.ServerName = MultiplayerOptions.OptionType.ServerName.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			InputKeyItemVM showMouseKey = this.ShowMouseKey;
			if (showMouseKey == null)
			{
				return;
			}
			showMouseKey.RefreshValues();
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x0001B98C File Offset: 0x00019B8C
		private void ExecutePopulateActionList(MissionScoreboardPlayerVM player)
		{
			this.PlayerActionList.Clear();
			if (player.Peer != null && !player.IsMine && !player.IsBot)
			{
				PlayerId id = player.Peer.Peer.Id;
				bool flag = this._chatBox.IsPlayerMutedFromGame(id);
				bool flag2 = PermaMuteList.IsPlayerMuted(id);
				bool flag3 = this._chatBox.IsPlayerMutedFromPlatform(id);
				bool isMutedFromPlatform = player.Peer.IsMutedFromPlatform;
				if (!flag3)
				{
					if (!flag2)
					{
						if (PlatformServices.Instance.IsPermanentMuteAvailable)
						{
							this.PlayerActionList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecutePermanentlyMute), new TextObject("{=77jmd4QF}Mute Permanently", null).ToString(), "PermanentMute", player));
						}
						string text = (flag ? GameTexts.FindText("str_mp_scoreboard_context_unmute_text", null).ToString() : GameTexts.FindText("str_mp_scoreboard_context_mute_text", null).ToString());
						this.PlayerActionList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteMute), text, flag ? "UnmuteText" : "MuteText", player));
					}
					else
					{
						this.PlayerActionList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteLiftPermanentMute), new TextObject("{=CIVPNf2d}Remove Permanent Mute", null).ToString(), "UnmuteText", player));
					}
				}
				if (player.IsTeammate)
				{
					if (!isMutedFromPlatform && this._voiceChatHandler != null && !flag2)
					{
						string text2 = (player.Peer.IsMuted ? GameTexts.FindText("str_mp_scoreboard_context_unmute_voice", null).ToString() : GameTexts.FindText("str_mp_scoreboard_context_mute_voice", null).ToString());
						this.PlayerActionList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteMuteVoice), text2, player.Peer.IsMuted ? "UnmuteVoice" : "MuteVoice", player));
					}
					if (this._canStartKickPolls)
					{
						this.PlayerActionList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteKick), GameTexts.FindText("str_mp_scoreboard_context_kick", null).ToString(), "StartKickPoll", player));
					}
				}
				StringPairItemWithActionVM stringPairItemWithActionVM = new StringPairItemWithActionVM(new Action<object>(this.ExecuteReport), GameTexts.FindText("str_mp_scoreboard_context_report", null).ToString(), "Report", player);
				if (MultiplayerReportPlayerManager.IsPlayerReportedOverLimit(id))
				{
					stringPairItemWithActionVM.IsEnabled = false;
					stringPairItemWithActionVM.Hint.HintText = new TextObject("{=klkYFik9}You've already reported this player.", null);
				}
				this.PlayerActionList.Add(stringPairItemWithActionVM);
				MultiplayerPlayerContextMenuHelper.AddMissionViewProfileOptions(player, this.PlayerActionList);
			}
			if (this.PlayerActionList.Count > 0)
			{
				this.IsPlayerActionsActive = false;
				this.IsPlayerActionsActive = true;
			}
		}

		// Token: 0x060006DB RID: 1755 RVA: 0x0001BC15 File Offset: 0x00019E15
		public void SetMouseState(bool isMouseVisible)
		{
			this.IsMouseEnabled = isMouseVisible;
		}

		// Token: 0x060006DC RID: 1756 RVA: 0x0001BC20 File Offset: 0x00019E20
		private void ExecuteReport(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			MultiplayerReportPlayerManager.RequestReportPlayer(NetworkMain.GameClient.CurrentMatchId, missionScoreboardPlayerVM.Peer.Peer.Id, missionScoreboardPlayerVM.Peer.DisplayedName, true);
		}

		// Token: 0x060006DD RID: 1757 RVA: 0x0001BC60 File Offset: 0x00019E60
		private void ExecuteMute(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			bool flag = this._chatBox.IsPlayerMutedFromGame(missionScoreboardPlayerVM.Peer.Peer.Id);
			this._chatBox.SetPlayerMuted(missionScoreboardPlayerVM.Peer.Peer.Id, !flag);
			GameTexts.SetVariable("PLAYER_NAME", missionScoreboardPlayerVM.Peer.DisplayedName);
			InformationManager.DisplayMessage(new InformationMessage((!flag) ? GameTexts.FindText("str_mute_notification", null).ToString() : GameTexts.FindText("str_unmute_notification", null).ToString()));
		}

		// Token: 0x060006DE RID: 1758 RVA: 0x0001BCF4 File Offset: 0x00019EF4
		private void ExecuteMuteVoice(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			missionScoreboardPlayerVM.Peer.SetMuted(!missionScoreboardPlayerVM.Peer.IsMuted);
			missionScoreboardPlayerVM.UpdateIsMuted();
		}

		// Token: 0x060006DF RID: 1759 RVA: 0x0001BD28 File Offset: 0x00019F28
		private void ExecutePermanentlyMute(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			PermaMuteList.MutePlayer(missionScoreboardPlayerVM.Peer.Peer.Id, missionScoreboardPlayerVM.Peer.Name);
			missionScoreboardPlayerVM.Peer.SetMuted(true);
			missionScoreboardPlayerVM.UpdateIsMuted();
			GameTexts.SetVariable("PLAYER_NAME", missionScoreboardPlayerVM.Peer.DisplayedName);
			InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_permanent_mute_notification", null).ToString()));
		}

		// Token: 0x060006E0 RID: 1760 RVA: 0x0001BDA0 File Offset: 0x00019FA0
		private void ExecuteLiftPermanentMute(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			PermaMuteList.RemoveMutedPlayer(missionScoreboardPlayerVM.Peer.Peer.Id);
			missionScoreboardPlayerVM.Peer.SetMuted(false);
			missionScoreboardPlayerVM.UpdateIsMuted();
			GameTexts.SetVariable("PLAYER_NAME", missionScoreboardPlayerVM.Peer.DisplayedName);
			InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_unmute_notification", null).ToString()));
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x0001BE0C File Offset: 0x0001A00C
		private void ExecuteKick(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			this._missionPollComponent.RequestKickPlayerPoll(missionScoreboardPlayerVM.Peer.GetNetworkPeer(), false);
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x0001BE38 File Offset: 0x0001A038
		public void Tick(float dt)
		{
			if (this.IsActive)
			{
				MPEndOfBattleVM endOfBattle = this.EndOfBattle;
				if (endOfBattle != null)
				{
					endOfBattle.Tick(dt);
				}
				this.CheckAttributeRefresh(dt);
				foreach (MissionScoreboardSideVM missionScoreboardSideVM in this.Sides)
				{
					missionScoreboardSideVM.Tick(dt);
				}
				foreach (MissionScoreboardSideVM missionScoreboardSideVM2 in this.Sides)
				{
					foreach (MissionScoreboardPlayerVM missionScoreboardPlayerVM in missionScoreboardSideVM2.Players)
					{
						missionScoreboardPlayerVM.RefreshDivision(this.IsSingleSide);
					}
				}
			}
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x0001BF1C File Offset: 0x0001A11C
		private void CheckAttributeRefresh(float dt)
		{
			this._attributeRefreshTimeElapsed += dt;
			if (this._attributeRefreshTimeElapsed >= 1f)
			{
				this.UpdateSideAllPlayersAttributes(BattleSideEnum.Attacker);
				this.UpdateSideAllPlayersAttributes(BattleSideEnum.Defender);
				this._attributeRefreshTimeElapsed = 0f;
			}
		}

		// Token: 0x060006E4 RID: 1764 RVA: 0x0001BF54 File Offset: 0x0001A154
		public override void OnFinalize()
		{
			base.OnFinalize();
			this._missionScoreboardComponent.OnPlayerSideChanged -= this.OnPlayerSideChanged;
			this._missionScoreboardComponent.OnPlayerPropertiesChanged -= this.OnPlayerPropertiesChanged;
			this._missionScoreboardComponent.OnBotPropertiesChanged -= this.OnBotPropertiesChanged;
			this._missionScoreboardComponent.OnRoundPropertiesChanged -= this.OnRoundPropertiesChanged;
			this._missionScoreboardComponent.OnScoreboardInitialized -= this.OnScoreboardInitialized;
			this._missionScoreboardComponent.OnMVPSelected -= this.OnMVPSelected;
			this._chatBox.OnPlayerMuteChanged -= this.OnPlayerMuteChanged;
			if (this._voiceChatHandler != null)
			{
				this._voiceChatHandler.OnPeerMuteStatusUpdated -= this.OnPeerMuteStatusUpdated;
			}
			foreach (MissionScoreboardSideVM missionScoreboardSideVM in this.Sides)
			{
				missionScoreboardSideVM.OnFinalize();
			}
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x0001C064 File Offset: 0x0001A264
		private void UpdateSideAllPlayersAttributes(BattleSideEnum battleSide)
		{
			MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide = this._missionScoreboardComponent.Sides.FirstOrDefault((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == battleSide);
			if (missionScoreboardSide != null)
			{
				foreach (MissionPeer missionPeer in missionScoreboardSide.Players)
				{
					this.OnPlayerPropertiesChanged(battleSide, missionPeer);
				}
			}
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x0001C0E4 File Offset: 0x0001A2E4
		public void OnPlayerSideChanged(Team curTeam, Team nextTeam, MissionPeer client)
		{
			if (client.IsMine && nextTeam != null && this.IsSideValid(nextTeam.Side))
			{
				this.InitSides();
				return;
			}
			if (curTeam != null && this.IsSideValid(curTeam.Side))
			{
				this._missionSides[this._missionScoreboardComponent.GetSideSafe(curTeam.Side).Side].RemovePlayer(client);
			}
			if (nextTeam != null && this.IsSideValid(nextTeam.Side))
			{
				this._missionSides[this._missionScoreboardComponent.GetSideSafe(nextTeam.Side).Side].AddPlayer(client);
			}
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x0001C184 File Offset: 0x0001A384
		private void OnRoundPropertiesChanged()
		{
			foreach (MissionScoreboardSideVM missionScoreboardSideVM in this._missionSides.Values)
			{
				missionScoreboardSideVM.UpdateRoundAttributes();
			}
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x0001C1DC File Offset: 0x0001A3DC
		private void OnPlayerPropertiesChanged(BattleSideEnum side, MissionPeer client)
		{
			if (this.IsSideValid(side))
			{
				this._missionSides[this._missionScoreboardComponent.GetSideSafe(side).Side].UpdatePlayerAttributes(client);
			}
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x0001C20C File Offset: 0x0001A40C
		private void OnBotPropertiesChanged(BattleSideEnum side)
		{
			BattleSideEnum side2 = this._missionScoreboardComponent.GetSideSafe(side).Side;
			if (this.IsSideValid(side2))
			{
				this._missionSides[side2].UpdateBotAttributes();
			}
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x0001C245 File Offset: 0x0001A445
		private void OnScoreboardInitialized()
		{
			this.InitSides();
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x0001C250 File Offset: 0x0001A450
		private void OnMVPSelected(MissionPeer mvpPeer, int mvpCount)
		{
			foreach (MissionScoreboardSideVM missionScoreboardSideVM in this.Sides)
			{
				foreach (MissionScoreboardPlayerVM missionScoreboardPlayerVM in missionScoreboardSideVM.Players)
				{
					if (missionScoreboardPlayerVM.Peer == mvpPeer)
					{
						missionScoreboardPlayerVM.SetMVPBadgeCount(mvpCount);
						break;
					}
				}
			}
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x0001C2DC File Offset: 0x0001A4DC
		private bool IsSideValid(BattleSideEnum side)
		{
			if (this.IsSingleSide)
			{
				return this._missionScoreboardComponent != null && side != BattleSideEnum.None && side != BattleSideEnum.NumSides;
			}
			return this._missionScoreboardComponent != null && side != BattleSideEnum.None && side != BattleSideEnum.NumSides && this._missionScoreboardComponent.Sides.Any((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == side);
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x0001C358 File Offset: 0x0001A558
		private void InitSides()
		{
			this.Sides.Clear();
			this._missionSides.Clear();
			if (this.IsSingleSide)
			{
				MissionScoreboardComponent.MissionScoreboardSide sideSafe = this._missionScoreboardComponent.GetSideSafe(BattleSideEnum.Defender);
				MissionScoreboardSideVM missionScoreboardSideVM = new MissionScoreboardSideVM(sideSafe, new Action<MissionScoreboardPlayerVM>(this.ExecutePopulateActionList), this.IsSingleSide, false);
				this.Sides.Add(missionScoreboardSideVM);
				this._missionSides.Add(sideSafe.Side, missionScoreboardSideVM);
				return;
			}
			NetworkCommunicator myPeer = GameNetwork.MyPeer;
			MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
			BattleSideEnum firstSideToAdd = BattleSideEnum.Attacker;
			BattleSideEnum secondSideToAdd = BattleSideEnum.Defender;
			if (missionPeer != null)
			{
				Team team = missionPeer.Team;
				if (team != null && team.Side == BattleSideEnum.Defender)
				{
					firstSideToAdd = BattleSideEnum.Defender;
					secondSideToAdd = BattleSideEnum.Attacker;
				}
			}
			MissionScoreboardComponent.MissionScoreboardSide missionScoreboardSide = this._missionScoreboardComponent.Sides.FirstOrDefault((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == firstSideToAdd);
			if (missionScoreboardSide != null)
			{
				MissionScoreboardSideVM missionScoreboardSideVM2 = new MissionScoreboardSideVM(missionScoreboardSide, new Action<MissionScoreboardPlayerVM>(this.ExecutePopulateActionList), this.IsSingleSide, false);
				this.Sides.Add(missionScoreboardSideVM2);
				this._missionSides.Add(missionScoreboardSide.Side, missionScoreboardSideVM2);
			}
			missionScoreboardSide = this._missionScoreboardComponent.Sides.FirstOrDefault((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == secondSideToAdd);
			if (missionScoreboardSide != null)
			{
				MissionScoreboardSideVM missionScoreboardSideVM3 = new MissionScoreboardSideVM(missionScoreboardSide, new Action<MissionScoreboardPlayerVM>(this.ExecutePopulateActionList), this.IsSingleSide, true);
				this.Sides.Add(missionScoreboardSideVM3);
				this._missionSides.Add(missionScoreboardSide.Side, missionScoreboardSideVM3);
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x060006EE RID: 1774 RVA: 0x0001C4D4 File Offset: 0x0001A6D4
		private BattleSideEnum AllySide
		{
			get
			{
				BattleSideEnum battleSideEnum = BattleSideEnum.None;
				if (GameNetwork.IsMyPeerReady)
				{
					MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
					if (component != null && component.Team != null)
					{
						battleSideEnum = component.Team.Side;
					}
				}
				return battleSideEnum;
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x060006EF RID: 1775 RVA: 0x0001C510 File Offset: 0x0001A710
		private BattleSideEnum EnemySide
		{
			get
			{
				BattleSideEnum allySide = this.AllySide;
				if (allySide == BattleSideEnum.Defender)
				{
					return BattleSideEnum.Attacker;
				}
				if (allySide == BattleSideEnum.Attacker)
				{
					return BattleSideEnum.Defender;
				}
				Debug.FailedAssert("Ally side must be either Attacker or Defender", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\Scoreboard\\MissionScoreboardVM.cs", "EnemySide", 517);
				return BattleSideEnum.None;
			}
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x0001C549 File Offset: 0x0001A749
		public void DecreaseSpectatorCount(MissionPeer spectatedPeer)
		{
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x0001C54B File Offset: 0x0001A74B
		public void IncreaseSpectatorCount(MissionPeer spectatedPeer)
		{
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x0001C550 File Offset: 0x0001A750
		public void ExecuteToggleMute()
		{
			foreach (MissionScoreboardSideVM missionScoreboardSideVM in this.Sides)
			{
				foreach (MissionScoreboardPlayerVM missionScoreboardPlayerVM in missionScoreboardSideVM.Players)
				{
					if (!missionScoreboardPlayerVM.IsMine && missionScoreboardPlayerVM.Peer != null)
					{
						this._chatBox.SetPlayerMuted(missionScoreboardPlayerVM.Peer.Peer.Id, !this._hasMutedAll);
						missionScoreboardPlayerVM.Peer.SetMuted(!this._hasMutedAll);
						missionScoreboardPlayerVM.UpdateIsMuted();
					}
				}
			}
			this._hasMutedAll = !this._hasMutedAll;
			this.UpdateToggleMuteText();
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x0001C630 File Offset: 0x0001A830
		private void UpdateToggleMuteText()
		{
			if (this._hasMutedAll)
			{
				this.ToggleMuteText = this._unmuteAllText.ToString();
				return;
			}
			this.ToggleMuteText = this._muteAllText.ToString();
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x0001C660 File Offset: 0x0001A860
		private void OnPeerMuteStatusUpdated(MissionPeer peer)
		{
			foreach (MissionScoreboardSideVM missionScoreboardSideVM in this.Sides)
			{
				foreach (MissionScoreboardPlayerVM missionScoreboardPlayerVM in missionScoreboardSideVM.Players)
				{
					if (missionScoreboardPlayerVM.Peer == peer)
					{
						missionScoreboardPlayerVM.UpdateIsMuted();
						break;
					}
				}
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x060006F5 RID: 1781 RVA: 0x0001C6EC File Offset: 0x0001A8EC
		// (set) Token: 0x060006F6 RID: 1782 RVA: 0x0001C6F4 File Offset: 0x0001A8F4
		[DataSourceProperty]
		public MPEndOfBattleVM EndOfBattle
		{
			get
			{
				return this._endOfBattle;
			}
			set
			{
				if (value != this._endOfBattle)
				{
					this._endOfBattle = value;
					base.OnPropertyChangedWithValue<MPEndOfBattleVM>(value, "EndOfBattle");
				}
			}
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x060006F7 RID: 1783 RVA: 0x0001C712 File Offset: 0x0001A912
		// (set) Token: 0x060006F8 RID: 1784 RVA: 0x0001C71A File Offset: 0x0001A91A
		[DataSourceProperty]
		public MBBindingList<StringPairItemWithActionVM> PlayerActionList
		{
			get
			{
				return this._playerActionList;
			}
			set
			{
				if (value != this._playerActionList)
				{
					this._playerActionList = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemWithActionVM>>(value, "PlayerActionList");
				}
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x060006F9 RID: 1785 RVA: 0x0001C738 File Offset: 0x0001A938
		// (set) Token: 0x060006FA RID: 1786 RVA: 0x0001C740 File Offset: 0x0001A940
		[DataSourceProperty]
		public MBBindingList<MissionScoreboardSideVM> Sides
		{
			get
			{
				return this._sides;
			}
			set
			{
				if (value != this._sides)
				{
					this._sides = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionScoreboardSideVM>>(value, "Sides");
				}
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x060006FB RID: 1787 RVA: 0x0001C75E File Offset: 0x0001A95E
		// (set) Token: 0x060006FC RID: 1788 RVA: 0x0001C766 File Offset: 0x0001A966
		[DataSourceProperty]
		public bool IsUpdateOver
		{
			get
			{
				return this._isUpdateOver;
			}
			set
			{
				this._isUpdateOver = value;
				base.OnPropertyChangedWithValue(value, "IsUpdateOver");
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x060006FD RID: 1789 RVA: 0x0001C77B File Offset: 0x0001A97B
		// (set) Token: 0x060006FE RID: 1790 RVA: 0x0001C783 File Offset: 0x0001A983
		[DataSourceProperty]
		public bool IsInitalizationOver
		{
			get
			{
				return this._isInitalizationOver;
			}
			set
			{
				if (value != this._isInitalizationOver)
				{
					this._isInitalizationOver = value;
					base.OnPropertyChangedWithValue(value, "IsInitalizationOver");
				}
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x060006FF RID: 1791 RVA: 0x0001C7A1 File Offset: 0x0001A9A1
		// (set) Token: 0x06000700 RID: 1792 RVA: 0x0001C7A9 File Offset: 0x0001A9A9
		[DataSourceProperty]
		public bool IsMouseEnabled
		{
			get
			{
				return this._isMouseEnabled;
			}
			set
			{
				if (value != this._isMouseEnabled)
				{
					this._isMouseEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsMouseEnabled");
				}
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06000701 RID: 1793 RVA: 0x0001C7C7 File Offset: 0x0001A9C7
		// (set) Token: 0x06000702 RID: 1794 RVA: 0x0001C7CF File Offset: 0x0001A9CF
		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
				}
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000703 RID: 1795 RVA: 0x0001C7ED File Offset: 0x0001A9ED
		// (set) Token: 0x06000704 RID: 1796 RVA: 0x0001C7F5 File Offset: 0x0001A9F5
		[DataSourceProperty]
		public bool IsPlayerActionsActive
		{
			get
			{
				return this._isPlayerActionsActive;
			}
			set
			{
				if (value != this._isPlayerActionsActive)
				{
					this._isPlayerActionsActive = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerActionsActive");
				}
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000705 RID: 1797 RVA: 0x0001C813 File Offset: 0x0001AA13
		// (set) Token: 0x06000706 RID: 1798 RVA: 0x0001C81B File Offset: 0x0001AA1B
		[DataSourceProperty]
		public string Spectators
		{
			get
			{
				return this._spectators;
			}
			set
			{
				if (value != this._spectators)
				{
					this._spectators = value;
					base.OnPropertyChangedWithValue<string>(value, "Spectators");
				}
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000707 RID: 1799 RVA: 0x0001C83E File Offset: 0x0001AA3E
		// (set) Token: 0x06000708 RID: 1800 RVA: 0x0001C846 File Offset: 0x0001AA46
		[DataSourceProperty]
		public InputKeyItemVM ShowMouseKey
		{
			get
			{
				return this._showMouseKey;
			}
			set
			{
				if (value != this._showMouseKey)
				{
					this._showMouseKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ShowMouseKey");
				}
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000709 RID: 1801 RVA: 0x0001C864 File Offset: 0x0001AA64
		// (set) Token: 0x0600070A RID: 1802 RVA: 0x0001C86C File Offset: 0x0001AA6C
		[DataSourceProperty]
		public string MissionName
		{
			get
			{
				return this._missionName;
			}
			set
			{
				if (value != this._missionName)
				{
					this._missionName = value;
					base.OnPropertyChangedWithValue<string>(value, "MissionName");
				}
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x0600070B RID: 1803 RVA: 0x0001C88F File Offset: 0x0001AA8F
		// (set) Token: 0x0600070C RID: 1804 RVA: 0x0001C897 File Offset: 0x0001AA97
		[DataSourceProperty]
		public string GameModeText
		{
			get
			{
				return this._gameModeText;
			}
			set
			{
				if (value != this._gameModeText)
				{
					this._gameModeText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameModeText");
				}
			}
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x0600070D RID: 1805 RVA: 0x0001C8BA File Offset: 0x0001AABA
		// (set) Token: 0x0600070E RID: 1806 RVA: 0x0001C8C2 File Offset: 0x0001AAC2
		[DataSourceProperty]
		public string MapName
		{
			get
			{
				return this._mapName;
			}
			set
			{
				if (value != this._mapName)
				{
					this._mapName = value;
					base.OnPropertyChangedWithValue<string>(value, "MapName");
				}
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x0600070F RID: 1807 RVA: 0x0001C8E5 File Offset: 0x0001AAE5
		// (set) Token: 0x06000710 RID: 1808 RVA: 0x0001C8ED File Offset: 0x0001AAED
		[DataSourceProperty]
		public string ServerName
		{
			get
			{
				return this._serverName;
			}
			set
			{
				if (value != this._serverName)
				{
					this._serverName = value;
					base.OnPropertyChangedWithValue<string>(value, "ServerName");
				}
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000711 RID: 1809 RVA: 0x0001C910 File Offset: 0x0001AB10
		// (set) Token: 0x06000712 RID: 1810 RVA: 0x0001C918 File Offset: 0x0001AB18
		[DataSourceProperty]
		public bool IsBotsEnabled
		{
			get
			{
				return this._isBotsEnabled;
			}
			set
			{
				if (value != this._isBotsEnabled)
				{
					this._isBotsEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsBotsEnabled");
				}
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000713 RID: 1811 RVA: 0x0001C936 File Offset: 0x0001AB36
		// (set) Token: 0x06000714 RID: 1812 RVA: 0x0001C93E File Offset: 0x0001AB3E
		[DataSourceProperty]
		public bool IsSingleSide
		{
			get
			{
				return this._isSingleSide;
			}
			set
			{
				if (value != this._isSingleSide)
				{
					this._isSingleSide = value;
					base.OnPropertyChangedWithValue(value, "IsSingleSide");
				}
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000715 RID: 1813 RVA: 0x0001C95C File Offset: 0x0001AB5C
		// (set) Token: 0x06000716 RID: 1814 RVA: 0x0001C964 File Offset: 0x0001AB64
		[DataSourceProperty]
		public string ToggleMuteText
		{
			get
			{
				return this._toggleMuteText;
			}
			set
			{
				if (value != this._toggleMuteText)
				{
					this._toggleMuteText = value;
					base.OnPropertyChangedWithValue<string>(value, "ToggleMuteText");
				}
			}
		}

		// Token: 0x0400037B RID: 891
		private const float AttributeRefreshDuration = 1f;

		// Token: 0x0400037C RID: 892
		private ChatBox _chatBox;

		// Token: 0x0400037D RID: 893
		private const float PermissionCheckDuration = 45f;

		// Token: 0x0400037E RID: 894
		private readonly Dictionary<BattleSideEnum, MissionScoreboardSideVM> _missionSides;

		// Token: 0x0400037F RID: 895
		private readonly MissionScoreboardComponent _missionScoreboardComponent;

		// Token: 0x04000380 RID: 896
		private readonly MultiplayerPollComponent _missionPollComponent;

		// Token: 0x04000381 RID: 897
		private VoiceChatHandler _voiceChatHandler;

		// Token: 0x04000382 RID: 898
		private MultiplayerPermissionHandler _permissionHandler;

		// Token: 0x04000383 RID: 899
		private readonly Mission _mission;

		// Token: 0x04000384 RID: 900
		private float _attributeRefreshTimeElapsed;

		// Token: 0x04000385 RID: 901
		private bool _hasMutedAll;

		// Token: 0x04000386 RID: 902
		private bool _canStartKickPolls;

		// Token: 0x04000387 RID: 903
		private TextObject _muteAllText = new TextObject("{=AZSbwcG5}Mute All", null);

		// Token: 0x04000388 RID: 904
		private TextObject _unmuteAllText = new TextObject("{=SzRVIPeZ}Unmute All", null);

		// Token: 0x04000389 RID: 905
		private bool _isActive;

		// Token: 0x0400038A RID: 906
		private InputKeyItemVM _showMouseKey;

		// Token: 0x0400038B RID: 907
		private MPEndOfBattleVM _endOfBattle;

		// Token: 0x0400038C RID: 908
		private MBBindingList<MissionScoreboardSideVM> _sides;

		// Token: 0x0400038D RID: 909
		private MBBindingList<StringPairItemWithActionVM> _playerActionList;

		// Token: 0x0400038E RID: 910
		private string _spectators;

		// Token: 0x0400038F RID: 911
		private string _missionName;

		// Token: 0x04000390 RID: 912
		private string _gameModeText;

		// Token: 0x04000391 RID: 913
		private string _mapName;

		// Token: 0x04000392 RID: 914
		private string _serverName;

		// Token: 0x04000393 RID: 915
		private bool _isBotsEnabled;

		// Token: 0x04000394 RID: 916
		private bool _isSingleSide;

		// Token: 0x04000395 RID: 917
		private bool _isInitalizationOver;

		// Token: 0x04000396 RID: 918
		private bool _isUpdateOver;

		// Token: 0x04000397 RID: 919
		private bool _isMouseEnabled;

		// Token: 0x04000398 RID: 920
		private bool _isPlayerActionsActive;

		// Token: 0x04000399 RID: 921
		private string _toggleMuteText;
	}
}
