using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer
{
	// Token: 0x02000040 RID: 64
	[OverrideView(typeof(MissionMultiplayerEscapeMenu))]
	public class MissionGauntletMultiplayerEscapeMenu : MissionGauntletEscapeMenuBase
	{
		// Token: 0x060002FE RID: 766 RVA: 0x00010BB5 File Offset: 0x0000EDB5
		public MissionGauntletMultiplayerEscapeMenu(string gameType)
			: base("MultiplayerEscapeMenu")
		{
			this._gameType = gameType;
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00010BCC File Offset: 0x0000EDCC
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._missionOptionsComponent = base.Mission.GetMissionBehavior<MissionOptionsComponent>();
			this._missionLobbyEquipmentNetworkComponent = base.Mission.GetMissionBehavior<MissionLobbyEquipmentNetworkComponent>();
			this._missionLobbyComponent = base.Mission.GetMissionBehavior<MissionLobbyComponent>();
			this._missionAdminComponent = base.Mission.GetMissionBehavior<MultiplayerAdminComponent>();
			this._missionTeamSelectComponent = base.Mission.GetMissionBehavior<MultiplayerTeamSelectComponent>();
			this._gameModeClient = base.Mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			TextObject textObject = GameTexts.FindText("str_multiplayer_game_type", this._gameType);
			this.DataSource = new MPEscapeMenuVM(null, textObject);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00010C63 File Offset: 0x0000EE63
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this.DataSource.Tick(dt);
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00010C78 File Offset: 0x0000EE78
		public override bool OnEscape()
		{
			bool flag = base.OnEscape();
			if (base.IsActive)
			{
				if (this._gameModeClient.IsGameModeUsingAllowTroopChange)
				{
					this._changeTroopItem.IsDisabled = !this._gameModeClient.CanRequestTroopChange();
				}
				if (this._gameModeClient.IsGameModeUsingAllowCultureChange)
				{
					this._changeCultureItem.IsDisabled = !this._gameModeClient.CanRequestCultureChange();
				}
			}
			return flag;
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00010CE0 File Offset: 0x0000EEE0
		protected override List<EscapeMenuItemVM> GetEscapeMenuItems()
		{
			List<EscapeMenuItemVM> list = new List<EscapeMenuItemVM>();
			list.Add(new EscapeMenuItemVM(new TextObject("{=e139gKZc}Return to the Game", null), delegate(object o)
			{
				base.OnEscapeMenuToggled(false);
			}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), false));
			list.Add(new EscapeMenuItemVM(new TextObject("{=NqarFr4P}Options", null), delegate(object o)
			{
				base.OnEscapeMenuToggled(false);
				MissionOptionsComponent missionOptionsComponent = this._missionOptionsComponent;
				if (missionOptionsComponent == null)
				{
					return;
				}
				missionOptionsComponent.OnAddOptionsUIHandler();
			}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), false));
			MultiplayerTeamSelectComponent missionTeamSelectComponent = this._missionTeamSelectComponent;
			if (missionTeamSelectComponent != null && missionTeamSelectComponent.TeamSelectionEnabled)
			{
				list.Add(new EscapeMenuItemVM(new TextObject("{=2SEofGth}Change Team", null), delegate(object o)
				{
					base.OnEscapeMenuToggled(false);
					if (this._missionTeamSelectComponent != null)
					{
						this._missionTeamSelectComponent.SelectTeam();
					}
				}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), false));
			}
			if (this._gameModeClient.IsGameModeUsingAllowCultureChange)
			{
				this._changeCultureItem = new EscapeMenuItemVM(new TextObject("{=aGGq9lJT}Change Culture", null), delegate(object o)
				{
					base.OnEscapeMenuToggled(false);
					this._missionLobbyComponent.RequestCultureSelection();
				}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), false);
				list.Add(this._changeCultureItem);
			}
			if (this._gameModeClient.IsGameModeUsingAllowTroopChange)
			{
				this._changeTroopItem = new EscapeMenuItemVM(new TextObject("{=Yza0JYJt}Change Troop", null), delegate(object o)
				{
					base.OnEscapeMenuToggled(false);
					this._missionLobbyComponent.RequestTroopSelection();
				}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), false);
				list.Add(this._changeTroopItem);
			}
			list.Add(new EscapeMenuItemVM(new TextObject("{=InGwtrWt}Quit", null), delegate(object o)
			{
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=InGwtrWt}Quit", null).ToString(), new TextObject("{=lxq6SaQn}Are you sure want to quit?", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
				{
					LobbyClient gameClient = NetworkMain.GameClient;
					if (gameClient.CurrentState == 16)
					{
						gameClient.QuitFromCustomGame();
						return;
					}
					if (gameClient.CurrentState == 14)
					{
						gameClient.EndCustomGame();
						return;
					}
					gameClient.QuitFromMatchmakerGame();
				}, null, "", 0f, null, null, null), false, false);
			}, null, () => new Tuple<bool, TextObject>(false, TextObject.Empty), false));
			return list;
		}

		// Token: 0x04000193 RID: 403
		private MissionOptionsComponent _missionOptionsComponent;

		// Token: 0x04000194 RID: 404
		private MissionLobbyEquipmentNetworkComponent _missionLobbyEquipmentNetworkComponent;

		// Token: 0x04000195 RID: 405
		private MissionLobbyComponent _missionLobbyComponent;

		// Token: 0x04000196 RID: 406
		private MultiplayerAdminComponent _missionAdminComponent;

		// Token: 0x04000197 RID: 407
		private MultiplayerTeamSelectComponent _missionTeamSelectComponent;

		// Token: 0x04000198 RID: 408
		private MissionMultiplayerGameModeBaseClient _gameModeClient;

		// Token: 0x04000199 RID: 409
		private readonly string _gameType;

		// Token: 0x0400019A RID: 410
		private EscapeMenuItemVM _changeTroopItem;

		// Token: 0x0400019B RID: 411
		private EscapeMenuItemVM _changeCultureItem;
	}
}
