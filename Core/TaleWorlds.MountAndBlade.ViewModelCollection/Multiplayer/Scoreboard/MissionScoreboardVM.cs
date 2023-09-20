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
	public class MissionScoreboardVM : ViewModel
	{
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

		public void SetMouseState(bool isMouseVisible)
		{
			this.IsMouseEnabled = isMouseVisible;
		}

		private void ExecuteReport(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			MultiplayerReportPlayerManager.RequestReportPlayer(NetworkMain.GameClient.CurrentMatchId, missionScoreboardPlayerVM.Peer.Peer.Id, missionScoreboardPlayerVM.Peer.DisplayedName, true);
		}

		private void ExecuteMute(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			bool flag = this._chatBox.IsPlayerMutedFromGame(missionScoreboardPlayerVM.Peer.Peer.Id);
			this._chatBox.SetPlayerMuted(missionScoreboardPlayerVM.Peer.Peer.Id, !flag);
			GameTexts.SetVariable("PLAYER_NAME", missionScoreboardPlayerVM.Peer.DisplayedName);
			InformationManager.DisplayMessage(new InformationMessage((!flag) ? GameTexts.FindText("str_mute_notification", null).ToString() : GameTexts.FindText("str_unmute_notification", null).ToString()));
		}

		private void ExecuteMuteVoice(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			missionScoreboardPlayerVM.Peer.SetMuted(!missionScoreboardPlayerVM.Peer.IsMuted);
			missionScoreboardPlayerVM.UpdateIsMuted();
		}

		private void ExecutePermanentlyMute(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			PermaMuteList.MutePlayer(missionScoreboardPlayerVM.Peer.Peer.Id, missionScoreboardPlayerVM.Peer.Name);
			missionScoreboardPlayerVM.Peer.SetMuted(true);
			missionScoreboardPlayerVM.UpdateIsMuted();
			GameTexts.SetVariable("PLAYER_NAME", missionScoreboardPlayerVM.Peer.DisplayedName);
			InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_permanent_mute_notification", null).ToString()));
		}

		private void ExecuteLiftPermanentMute(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			PermaMuteList.RemoveMutedPlayer(missionScoreboardPlayerVM.Peer.Peer.Id);
			missionScoreboardPlayerVM.Peer.SetMuted(false);
			missionScoreboardPlayerVM.UpdateIsMuted();
			GameTexts.SetVariable("PLAYER_NAME", missionScoreboardPlayerVM.Peer.DisplayedName);
			InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_unmute_notification", null).ToString()));
		}

		private void ExecuteKick(object playerObj)
		{
			MissionScoreboardPlayerVM missionScoreboardPlayerVM = playerObj as MissionScoreboardPlayerVM;
			this._missionPollComponent.RequestKickPlayerPoll(missionScoreboardPlayerVM.Peer.GetNetworkPeer(), false);
		}

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

		private void OnRoundPropertiesChanged()
		{
			foreach (MissionScoreboardSideVM missionScoreboardSideVM in this._missionSides.Values)
			{
				missionScoreboardSideVM.UpdateRoundAttributes();
			}
		}

		private void OnPlayerPropertiesChanged(BattleSideEnum side, MissionPeer client)
		{
			if (this.IsSideValid(side))
			{
				this._missionSides[this._missionScoreboardComponent.GetSideSafe(side).Side].UpdatePlayerAttributes(client);
			}
		}

		private void OnBotPropertiesChanged(BattleSideEnum side)
		{
			BattleSideEnum side2 = this._missionScoreboardComponent.GetSideSafe(side).Side;
			if (this.IsSideValid(side2))
			{
				this._missionSides[side2].UpdateBotAttributes();
			}
		}

		private void OnScoreboardInitialized()
		{
			this.InitSides();
		}

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

		private bool IsSideValid(BattleSideEnum side)
		{
			if (this.IsSingleSide)
			{
				return this._missionScoreboardComponent != null && side != BattleSideEnum.None && side != BattleSideEnum.NumSides;
			}
			return this._missionScoreboardComponent != null && side != BattleSideEnum.None && side != BattleSideEnum.NumSides && this._missionScoreboardComponent.Sides.Any((MissionScoreboardComponent.MissionScoreboardSide s) => s != null && s.Side == side);
		}

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

		public void DecreaseSpectatorCount(MissionPeer spectatedPeer)
		{
		}

		public void IncreaseSpectatorCount(MissionPeer spectatedPeer)
		{
		}

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

		private void UpdateToggleMuteText()
		{
			if (this._hasMutedAll)
			{
				this.ToggleMuteText = this._unmuteAllText.ToString();
				return;
			}
			this.ToggleMuteText = this._muteAllText.ToString();
		}

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

		private const float AttributeRefreshDuration = 1f;

		private ChatBox _chatBox;

		private const float PermissionCheckDuration = 45f;

		private readonly Dictionary<BattleSideEnum, MissionScoreboardSideVM> _missionSides;

		private readonly MissionScoreboardComponent _missionScoreboardComponent;

		private readonly MultiplayerPollComponent _missionPollComponent;

		private VoiceChatHandler _voiceChatHandler;

		private MultiplayerPermissionHandler _permissionHandler;

		private readonly Mission _mission;

		private float _attributeRefreshTimeElapsed;

		private bool _hasMutedAll;

		private bool _canStartKickPolls;

		private TextObject _muteAllText = new TextObject("{=AZSbwcG5}Mute All", null);

		private TextObject _unmuteAllText = new TextObject("{=SzRVIPeZ}Unmute All", null);

		private bool _isActive;

		private InputKeyItemVM _showMouseKey;

		private MPEndOfBattleVM _endOfBattle;

		private MBBindingList<MissionScoreboardSideVM> _sides;

		private MBBindingList<StringPairItemWithActionVM> _playerActionList;

		private string _spectators;

		private string _missionName;

		private string _gameModeText;

		private string _mapName;

		private string _serverName;

		private bool _isBotsEnabled;

		private bool _isSingleSide;

		private bool _isInitalizationOver;

		private bool _isUpdateOver;

		private bool _isMouseEnabled;

		private bool _isPlayerActionsActive;

		private string _toggleMuteText;
	}
}
