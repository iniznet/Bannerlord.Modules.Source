using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.OfficialGame
{
	public class MPMatchmakingSelectionInfoVM : ViewModel
	{
		public MPMatchmakingSelectionInfoVM()
		{
			this.Name = "";
			this.Description = "";
			this.ExtraInfos = new MBBindingList<StringPairItemVM>();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this._playersDescription = new TextObject("{=RfXJdNye}Players", null).ToString();
			this._averagePlaytimeDescription = new TextObject("{=YAaAlbkX}Avg. Playtime", null).ToString();
			this._roundsDescription = new TextObject("{=iKtIhlbo}Rounds", null).ToString();
			this._roundTimeDescription = new TextObject("{=r5WzivPb}Round Time", null).ToString();
			this._objectivesDescription = new TextObject("{=gqNxq11A}Objectives", null).ToString();
			this._troopsDescription = new TextObject("{=5k4dxUEJ}Troops", null).ToString();
		}

		public void UpdateForGameType(string gameTypeStr)
		{
			this.Name = GameTexts.FindText("str_multiplayer_official_game_type_name", gameTypeStr).ToString();
			MBTextManager.SetTextVariable("newline", "\n", false);
			this.Description = GameTexts.FindText("str_multiplayer_official_game_type_description", gameTypeStr).ToString();
			this.ExtraInfos.Clear();
			int num = MultiplayerOptions.Instance.GetNumberOfPlayersForGameMode(gameTypeStr) / 2;
			int roundCountForGameMode = MultiplayerOptions.Instance.GetRoundCountForGameMode(gameTypeStr);
			int roundTimeLimitInMinutesForGameMode = MultiplayerOptions.Instance.GetRoundTimeLimitInMinutesForGameMode(gameTypeStr);
			int num2 = ((roundCountForGameMode == 1) ? 1 : (roundCountForGameMode / 2 + 1));
			int num3 = num2 * roundTimeLimitInMinutesForGameMode;
			MBTextManager.SetTextVariable("PLAYER_COUNT", num.ToString(), false);
			string text = GameTexts.FindText("str_multiplayer_official_game_type_player_info_for_versus", null).ToString();
			MBTextManager.SetTextVariable("PLAY_TIME", num3.ToString(), false);
			string text2 = GameTexts.FindText("str_multiplayer_official_game_type_playtime_info_in_minutes", null).ToString();
			MBTextManager.SetTextVariable("ROUND_COUNT", num2.ToString(), false);
			string text3 = GameTexts.FindText("str_multiplayer_official_game_type_rounds_info_for_best_of", null).ToString();
			MBTextManager.SetTextVariable("PLAY_TIME", roundTimeLimitInMinutesForGameMode.ToString(), false);
			string text4 = GameTexts.FindText("str_multiplayer_official_game_type_playtime_info_in_minutes", null).ToString();
			string text5 = GameTexts.FindText("str_multiplayer_official_game_type_objective_info", gameTypeStr).ToString();
			string text6 = GameTexts.FindText("str_multiplayer_official_game_type_troops_info", gameTypeStr).ToString();
			this.ExtraInfos.Add(new StringPairItemVM(this._playersDescription, text, null));
			this.ExtraInfos.Add(new StringPairItemVM(this._averagePlaytimeDescription, text2, null));
			this.ExtraInfos.Add(new StringPairItemVM(this._roundsDescription, text3, null));
			this.ExtraInfos.Add(new StringPairItemVM(this._roundTimeDescription, text4, null));
			this.ExtraInfos.Add(new StringPairItemVM(this._objectivesDescription, text5, null));
			this.ExtraInfos.Add(new StringPairItemVM(this._troopsDescription, text6, null));
		}

		public void SetEnabled(bool isEnabled)
		{
			this.IsEnabled = isEnabled;
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<StringPairItemVM> ExtraInfos
		{
			get
			{
				return this._extraInfos;
			}
			set
			{
				if (value != this._extraInfos)
				{
					this._extraInfos = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemVM>>(value, "ExtraInfos");
				}
			}
		}

		private string _playersDescription;

		private string _averagePlaytimeDescription;

		private string _roundsDescription;

		private string _roundTimeDescription;

		private string _objectivesDescription;

		private string _troopsDescription;

		private string _name;

		private string _description;

		private bool _isEnabled;

		private MBBindingList<StringPairItemVM> _extraInfos;
	}
}
