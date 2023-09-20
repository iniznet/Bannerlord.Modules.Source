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
	public class GameOverVM : ViewModel
	{
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

		public void ExecuteClose()
		{
			Action onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			Common.DynamicInvokeWithLog(onClose, Array.Empty<object>());
		}

		public void SetCloseInputKey(HotKey hotKey)
		{
			this.CloseInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

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

		private readonly Action _onClose;

		private readonly GameOverStatsProvider _statsProvider;

		private readonly GameOverState.GameOverReason _reason;

		private GameOverStatCategoryVM _currentCategory;

		private string _closeText;

		private string _titleText;

		private string _reasonAsString;

		private string _statisticsTitle;

		private bool _isPositiveGameOver;

		private InputKeyItemVM _closeInputKey;

		private ImageIdentifierVM _clanBanner;

		private MBBindingList<GameOverStatCategoryVM> _categories;
	}
}
