using System;
using SandBox.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.ViewModelCollection.GameOver
{
	// Token: 0x0200003A RID: 58
	public class GameOverVM : ViewModel
	{
		// Token: 0x0600041D RID: 1053 RVA: 0x00012854 File Offset: 0x00010A54
		public GameOverVM(GameOverState.GameOverReason reason, Action onClose)
		{
			this._onClose = onClose;
			this._reason = reason;
			this._statsProvider = new GameOverStatsProvider();
			this.Categories = new MBBindingList<GameOverStatCategoryVM>();
			this.IsPositiveGameOver = this._reason == 2;
			this.ClanBanner = new ImageIdentifierVM(BannerCode.CreateFrom(Hero.MainHero.ClanBanner), true);
			this.ReasonAsString = Enum.GetName(typeof(GameOverState.GameOverReason), this._reason);
			this.RefreshValues();
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x000128DC File Offset: 0x00010ADC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CloseText = (this.IsPositiveGameOver ? new TextObject("{=DM6luo3c}Continue", null).ToString() : GameTexts.FindText("str_main_menu", null).ToString());
			this.TitleText = GameTexts.FindText("str_game_over_title", this.ReasonAsString).ToString();
			this.StatisticsTitle = GameTexts.FindText("str_statistics", null).ToString();
			this.Categories.Clear();
			foreach (StatCategory statCategory in this._statsProvider.GetGameOverStats())
			{
				this.Categories.Add(new GameOverStatCategoryVM(statCategory, new Action<GameOverStatCategoryVM>(this.OnCategorySelection)));
			}
			this.OnCategorySelection(this.Categories[0]);
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x000129C8 File Offset: 0x00010BC8
		private void OnCategorySelection(GameOverStatCategoryVM newCategory)
		{
			if (this._currentCategory != null)
			{
				this._currentCategory.IsSelected = false;
			}
			this._currentCategory = newCategory;
			if (this._currentCategory != null)
			{
				this._currentCategory.IsSelected = true;
			}
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x000129F9 File Offset: 0x00010BF9
		public void ExecuteClose()
		{
			Action onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			Common.DynamicInvokeWithLog(onClose, Array.Empty<object>());
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00012A11 File Offset: 0x00010C11
		public void SetCloseInputKey(HotKey hotKey)
		{
			this.CloseInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00012A20 File Offset: 0x00010C20
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM closeInputKey = this.CloseInputKey;
			if (closeInputKey == null)
			{
				return;
			}
			closeInputKey.OnFinalize();
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000423 RID: 1059 RVA: 0x00012A38 File Offset: 0x00010C38
		// (set) Token: 0x06000424 RID: 1060 RVA: 0x00012A40 File Offset: 0x00010C40
		[DataSourceProperty]
		public string CloseText
		{
			get
			{
				return this._closeText;
			}
			set
			{
				if (value != this._closeText)
				{
					this._closeText = value;
					base.OnPropertyChangedWithValue<string>(value, "CloseText");
				}
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000425 RID: 1061 RVA: 0x00012A63 File Offset: 0x00010C63
		// (set) Token: 0x06000426 RID: 1062 RVA: 0x00012A6B File Offset: 0x00010C6B
		[DataSourceProperty]
		public string StatisticsTitle
		{
			get
			{
				return this._statisticsTitle;
			}
			set
			{
				if (value != this._statisticsTitle)
				{
					this._statisticsTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "StatisticsTitle");
				}
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x00012A8E File Offset: 0x00010C8E
		// (set) Token: 0x06000428 RID: 1064 RVA: 0x00012A96 File Offset: 0x00010C96
		[DataSourceProperty]
		public string ReasonAsString
		{
			get
			{
				return this._reasonAsString;
			}
			set
			{
				if (value != this._reasonAsString)
				{
					this._reasonAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "ReasonAsString");
				}
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x00012AB9 File Offset: 0x00010CB9
		// (set) Token: 0x0600042A RID: 1066 RVA: 0x00012AC1 File Offset: 0x00010CC1
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

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x0600042B RID: 1067 RVA: 0x00012AE4 File Offset: 0x00010CE4
		// (set) Token: 0x0600042C RID: 1068 RVA: 0x00012AEC File Offset: 0x00010CEC
		[DataSourceProperty]
		public ImageIdentifierVM ClanBanner
		{
			get
			{
				return this._clanBanner;
			}
			set
			{
				if (value != this._clanBanner)
				{
					this._clanBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ClanBanner");
				}
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x0600042D RID: 1069 RVA: 0x00012B0A File Offset: 0x00010D0A
		// (set) Token: 0x0600042E RID: 1070 RVA: 0x00012B12 File Offset: 0x00010D12
		[DataSourceProperty]
		public bool IsPositiveGameOver
		{
			get
			{
				return this._isPositiveGameOver;
			}
			set
			{
				if (value != this._isPositiveGameOver)
				{
					this._isPositiveGameOver = value;
					base.OnPropertyChangedWithValue(value, "IsPositiveGameOver");
				}
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x0600042F RID: 1071 RVA: 0x00012B30 File Offset: 0x00010D30
		// (set) Token: 0x06000430 RID: 1072 RVA: 0x00012B38 File Offset: 0x00010D38
		[DataSourceProperty]
		public InputKeyItemVM CloseInputKey
		{
			get
			{
				return this._closeInputKey;
			}
			set
			{
				if (value != this._closeInputKey)
				{
					this._closeInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CloseInputKey");
				}
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x00012B56 File Offset: 0x00010D56
		// (set) Token: 0x06000432 RID: 1074 RVA: 0x00012B5E File Offset: 0x00010D5E
		[DataSourceProperty]
		public MBBindingList<GameOverStatCategoryVM> Categories
		{
			get
			{
				return this._categories;
			}
			set
			{
				if (value != this._categories)
				{
					this._categories = value;
					base.OnPropertyChangedWithValue<MBBindingList<GameOverStatCategoryVM>>(value, "Categories");
				}
			}
		}

		// Token: 0x04000223 RID: 547
		private readonly Action _onClose;

		// Token: 0x04000224 RID: 548
		private readonly GameOverStatsProvider _statsProvider;

		// Token: 0x04000225 RID: 549
		private readonly GameOverState.GameOverReason _reason;

		// Token: 0x04000226 RID: 550
		private GameOverStatCategoryVM _currentCategory;

		// Token: 0x04000227 RID: 551
		private string _closeText;

		// Token: 0x04000228 RID: 552
		private string _titleText;

		// Token: 0x04000229 RID: 553
		private string _reasonAsString;

		// Token: 0x0400022A RID: 554
		private string _statisticsTitle;

		// Token: 0x0400022B RID: 555
		private bool _isPositiveGameOver;

		// Token: 0x0400022C RID: 556
		private InputKeyItemVM _closeInputKey;

		// Token: 0x0400022D RID: 557
		private ImageIdentifierVM _clanBanner;

		// Token: 0x0400022E RID: 558
		private MBBindingList<GameOverStatCategoryVM> _categories;
	}
}
