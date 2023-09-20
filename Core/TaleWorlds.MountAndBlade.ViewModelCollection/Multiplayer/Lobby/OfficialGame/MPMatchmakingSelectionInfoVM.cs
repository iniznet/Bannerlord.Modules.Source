using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.OfficialGame
{
	// Token: 0x02000074 RID: 116
	public class MPMatchmakingSelectionInfoVM : ViewModel
	{
		// Token: 0x06000AA4 RID: 2724 RVA: 0x000260AE File Offset: 0x000242AE
		public MPMatchmakingSelectionInfoVM()
		{
			this.Name = "";
			this.Description = "";
			this.ExtraInfos = new MBBindingList<StringPairItemVM>();
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x000260D8 File Offset: 0x000242D8
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

		// Token: 0x06000AA6 RID: 2726 RVA: 0x00026170 File Offset: 0x00024370
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

		// Token: 0x06000AA7 RID: 2727 RVA: 0x00026346 File Offset: 0x00024546
		public void SetEnabled(bool isEnabled)
		{
			this.IsEnabled = isEnabled;
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06000AA8 RID: 2728 RVA: 0x0002634F File Offset: 0x0002454F
		// (set) Token: 0x06000AA9 RID: 2729 RVA: 0x00026357 File Offset: 0x00024557
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

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06000AAA RID: 2730 RVA: 0x0002637A File Offset: 0x0002457A
		// (set) Token: 0x06000AAB RID: 2731 RVA: 0x00026382 File Offset: 0x00024582
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

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06000AAC RID: 2732 RVA: 0x000263A0 File Offset: 0x000245A0
		// (set) Token: 0x06000AAD RID: 2733 RVA: 0x000263A8 File Offset: 0x000245A8
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

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06000AAE RID: 2734 RVA: 0x000263CB File Offset: 0x000245CB
		// (set) Token: 0x06000AAF RID: 2735 RVA: 0x000263D3 File Offset: 0x000245D3
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

		// Token: 0x04000526 RID: 1318
		private string _playersDescription;

		// Token: 0x04000527 RID: 1319
		private string _averagePlaytimeDescription;

		// Token: 0x04000528 RID: 1320
		private string _roundsDescription;

		// Token: 0x04000529 RID: 1321
		private string _roundTimeDescription;

		// Token: 0x0400052A RID: 1322
		private string _objectivesDescription;

		// Token: 0x0400052B RID: 1323
		private string _troopsDescription;

		// Token: 0x0400052C RID: 1324
		private string _name;

		// Token: 0x0400052D RID: 1325
		private string _description;

		// Token: 0x0400052E RID: 1326
		private bool _isEnabled;

		// Token: 0x0400052F RID: 1327
		private MBBindingList<StringPairItemVM> _extraInfos;
	}
}
