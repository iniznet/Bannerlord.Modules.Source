using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TournamentLeaderboard
{
	// Token: 0x02000099 RID: 153
	public class TournamentLeaderboardVM : ViewModel
	{
		// Token: 0x06000ED7 RID: 3799 RVA: 0x0003A790 File Offset: 0x00038990
		public TournamentLeaderboardVM()
		{
			this.Entries = new MBBindingList<TournamentLeaderboardEntryItemVM>();
			List<KeyValuePair<Hero, int>> leaderboard = Campaign.Current.TournamentManager.GetLeaderboard();
			for (int i = 0; i < leaderboard.Count; i++)
			{
				this.Entries.Add(new TournamentLeaderboardEntryItemVM(leaderboard[i].Key, leaderboard[i].Value, i + 1));
			}
			this.SortController = new TournamentLeaderboardSortControllerVM(ref this._entries);
			this.RefreshValues();
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x0003A818 File Offset: 0x00038A18
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.Entries.ApplyActionOnAllItems(delegate(TournamentLeaderboardEntryItemVM x)
			{
				x.RefreshValues();
			});
			this.HeroText = GameTexts.FindText("str_hero", null).ToString();
			this.VictoriesText = GameTexts.FindText("str_leaderboard_victories", null).ToString();
			this.RankText = GameTexts.FindText("str_rank_sign", null).ToString();
			this.TitleText = GameTexts.FindText("str_leaderboard_title", null).ToString();
		}

		// Token: 0x06000ED9 RID: 3801 RVA: 0x0003A8C3 File Offset: 0x00038AC3
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey == null)
			{
				return;
			}
			doneInputKey.OnFinalize();
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x0003A8DB File Offset: 0x00038ADB
		public void ExecuteDone()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x0003A8E4 File Offset: 0x00038AE4
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06000EDC RID: 3804 RVA: 0x0003A8F3 File Offset: 0x00038AF3
		// (set) Token: 0x06000EDD RID: 3805 RVA: 0x0003A8FB File Offset: 0x00038AFB
		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06000EDE RID: 3806 RVA: 0x0003A919 File Offset: 0x00038B19
		// (set) Token: 0x06000EDF RID: 3807 RVA: 0x0003A921 File Offset: 0x00038B21
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

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06000EE0 RID: 3808 RVA: 0x0003A93F File Offset: 0x00038B3F
		// (set) Token: 0x06000EE1 RID: 3809 RVA: 0x0003A947 File Offset: 0x00038B47
		[DataSourceProperty]
		public TournamentLeaderboardSortControllerVM SortController
		{
			get
			{
				return this._sortController;
			}
			set
			{
				if (value != this._sortController)
				{
					this._sortController = value;
					base.OnPropertyChangedWithValue<TournamentLeaderboardSortControllerVM>(value, "SortController");
				}
			}
		}

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06000EE2 RID: 3810 RVA: 0x0003A965 File Offset: 0x00038B65
		// (set) Token: 0x06000EE3 RID: 3811 RVA: 0x0003A96D File Offset: 0x00038B6D
		[DataSourceProperty]
		public MBBindingList<TournamentLeaderboardEntryItemVM> Entries
		{
			get
			{
				return this._entries;
			}
			set
			{
				if (value != this._entries)
				{
					this._entries = value;
					base.OnPropertyChangedWithValue<MBBindingList<TournamentLeaderboardEntryItemVM>>(value, "Entries");
				}
			}
		}

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06000EE4 RID: 3812 RVA: 0x0003A98B File Offset: 0x00038B8B
		// (set) Token: 0x06000EE5 RID: 3813 RVA: 0x0003A993 File Offset: 0x00038B93
		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (value != this._doneText)
				{
					this._doneText = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneText");
				}
			}
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06000EE6 RID: 3814 RVA: 0x0003A9B6 File Offset: 0x00038BB6
		// (set) Token: 0x06000EE7 RID: 3815 RVA: 0x0003A9BE File Offset: 0x00038BBE
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06000EE8 RID: 3816 RVA: 0x0003A9E1 File Offset: 0x00038BE1
		// (set) Token: 0x06000EE9 RID: 3817 RVA: 0x0003A9E9 File Offset: 0x00038BE9
		[DataSourceProperty]
		public string HeroText
		{
			get
			{
				return this._heroText;
			}
			set
			{
				if (value != this._heroText)
				{
					this._heroText = value;
					base.OnPropertyChangedWithValue<string>(value, "HeroText");
				}
			}
		}

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x06000EEA RID: 3818 RVA: 0x0003AA0C File Offset: 0x00038C0C
		// (set) Token: 0x06000EEB RID: 3819 RVA: 0x0003AA14 File Offset: 0x00038C14
		[DataSourceProperty]
		public string VictoriesText
		{
			get
			{
				return this._victoriesText;
			}
			set
			{
				if (value != this._victoriesText)
				{
					this._victoriesText = value;
					base.OnPropertyChangedWithValue<string>(value, "VictoriesText");
				}
			}
		}

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06000EEC RID: 3820 RVA: 0x0003AA37 File Offset: 0x00038C37
		// (set) Token: 0x06000EED RID: 3821 RVA: 0x0003AA3F File Offset: 0x00038C3F
		[DataSourceProperty]
		public string RankText
		{
			get
			{
				return this._rankText;
			}
			set
			{
				if (value != this._rankText)
				{
					this._rankText = value;
					base.OnPropertyChangedWithValue<string>(value, "RankText");
				}
			}
		}

		// Token: 0x040006E5 RID: 1765
		private InputKeyItemVM _doneInputKey;

		// Token: 0x040006E6 RID: 1766
		private bool _isEnabled;

		// Token: 0x040006E7 RID: 1767
		private string _doneText;

		// Token: 0x040006E8 RID: 1768
		private string _heroText;

		// Token: 0x040006E9 RID: 1769
		private string _victoriesText;

		// Token: 0x040006EA RID: 1770
		private string _rankText;

		// Token: 0x040006EB RID: 1771
		private string _titleText;

		// Token: 0x040006EC RID: 1772
		private MBBindingList<TournamentLeaderboardEntryItemVM> _entries;

		// Token: 0x040006ED RID: 1773
		private TournamentLeaderboardSortControllerVM _sortController;
	}
}
