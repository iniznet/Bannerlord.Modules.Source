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
	[OverrideView(typeof(MissionMultiplayerEscapeMenu))]
	public class MissionGauntletMultiplayerEscapeMenu : MissionGauntletEscapeMenuBase
	{
		public MissionGauntletMultiplayerEscapeMenu(string gameType)
			: base("MultiplayerEscapeMenu")
		{
			this._gameType = gameType;
		}

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

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this.DataSource.Tick(dt);
		}

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

		private MissionOptionsComponent _missionOptionsComponent;

		private MissionLobbyEquipmentNetworkComponent _missionLobbyEquipmentNetworkComponent;

		private MissionLobbyComponent _missionLobbyComponent;

		private MultiplayerAdminComponent _missionAdminComponent;

		private MultiplayerTeamSelectComponent _missionTeamSelectComponent;

		private MissionMultiplayerGameModeBaseClient _gameModeClient;

		private readonly string _gameType;

		private EscapeMenuItemVM _changeTroopItem;

		private EscapeMenuItemVM _changeCultureItem;
	}
}
