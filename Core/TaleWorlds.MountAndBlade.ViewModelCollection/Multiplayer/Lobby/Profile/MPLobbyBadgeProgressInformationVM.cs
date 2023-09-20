using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	public class MPLobbyBadgeProgressInformationVM : ViewModel
	{
		public MPLobbyBadgeProgressInformationVM(Func<string> getContinueKeyText)
		{
			this._getContinueKeyText = getContinueKeyText;
			this.AvailableBadgeIDs = new MBBindingList<StringPairItemVM>();
			this.ShownBadgeCount = 5;
			for (int i = 0; i < this.ShownBadgeCount; i++)
			{
				this.AvailableBadgeIDs.Add(new StringPairItemVM(string.Empty, string.Empty, null));
			}
		}

		public void OpenWith(MPLobbyAchievementBadgeGroupVM badgeGroup)
		{
			this.BadgeGroup = badgeGroup;
			this.TitleText = (this.BadgeGroup.ShownBadgeItem.Badge as ConditionalBadge).BadgeConditions[0].Description.ToString();
			this._shownBadgeIndexOffset = 0;
			this.RefreshShownBadges();
			Func<string> getContinueKeyText = this._getContinueKeyText;
			this.ClickToCloseText = ((getContinueKeyText != null) ? getContinueKeyText() : null);
			this.IsEnabled = true;
		}

		private void RefreshShownBadges()
		{
			int num = this.BadgeGroup.Badges.IndexOf(this.BadgeGroup.ShownBadgeItem) + this._shownBadgeIndexOffset;
			int num2 = 0;
			int num3 = this.ShownBadgeCount / 2;
			for (int i = num - num3; i <= num + num3; i++)
			{
				if (i >= 0 && i < this.BadgeGroup.Badges.Count)
				{
					MPLobbyBadgeItemVM mplobbyBadgeItemVM = this.BadgeGroup.Badges[i];
					this.AvailableBadgeIDs[num2].Value = mplobbyBadgeItemVM.BadgeId;
					this.AvailableBadgeIDs[num2].Definition = mplobbyBadgeItemVM.Name;
				}
				else
				{
					this.AvailableBadgeIDs[num2].Value = string.Empty;
					this.AvailableBadgeIDs[num2].Definition = string.Empty;
				}
				num2++;
			}
			this.CanIncreaseBadgeIndices = this.BadgeGroup.Badges.IndexOf(this.BadgeGroup.Badges[num]) < this.BadgeGroup.Badges.Count - 1;
			this.CanDecreaseBadgeIndices = num > 0;
		}

		public void ExecuteClosePopup()
		{
			this.BadgeGroup = null;
			this.IsEnabled = false;
		}

		private void ExecuteIncreaseActiveBadgeIndices()
		{
			this._shownBadgeIndexOffset++;
			this.RefreshShownBadges();
		}

		private void ExecuteDecreaseActiveBadgeIndices()
		{
			this._shownBadgeIndexOffset--;
			this.RefreshShownBadges();
		}

		[DataSourceProperty]
		public int ShownBadgeCount
		{
			get
			{
				return this._shownBadgeCount;
			}
			set
			{
				if (value != this._shownBadgeCount)
				{
					this._shownBadgeCount = value;
					base.OnPropertyChangedWithValue(value, "ShownBadgeCount");
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
		public bool CanIncreaseBadgeIndices
		{
			get
			{
				return this._canIncreaseBadgeIndices;
			}
			set
			{
				if (value != this._canIncreaseBadgeIndices)
				{
					this._canIncreaseBadgeIndices = value;
					base.OnPropertyChangedWithValue(value, "CanIncreaseBadgeIndices");
				}
			}
		}

		[DataSourceProperty]
		public bool CanDecreaseBadgeIndices
		{
			get
			{
				return this._canDecreaseBadgeIndices;
			}
			set
			{
				if (value != this._canDecreaseBadgeIndices)
				{
					this._canDecreaseBadgeIndices = value;
					base.OnPropertyChangedWithValue(value, "CanDecreaseBadgeIndices");
				}
			}
		}

		[DataSourceProperty]
		public string ClickToCloseText
		{
			get
			{
				return this._clickToCloseText;
			}
			set
			{
				if (value != this._clickToCloseText)
				{
					this._clickToCloseText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClickToCloseText");
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
		public MPLobbyAchievementBadgeGroupVM BadgeGroup
		{
			get
			{
				return this._badgeGroup;
			}
			set
			{
				if (value != this._badgeGroup)
				{
					this._badgeGroup = value;
					base.OnPropertyChangedWithValue<MPLobbyAchievementBadgeGroupVM>(value, "BadgeGroup");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<StringPairItemVM> AvailableBadgeIDs
		{
			get
			{
				return this._availableBadgeIDs;
			}
			set
			{
				if (value != this._availableBadgeIDs)
				{
					this._availableBadgeIDs = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemVM>>(value, "AvailableBadgeIDs");
				}
			}
		}

		private int _shownBadgeIndexOffset;

		private const int MaxShownBadgeCount = 5;

		private readonly Func<string> _getContinueKeyText;

		private int _shownBadgeCount;

		private bool _isEnabled;

		private bool _canIncreaseBadgeIndices;

		private bool _canDecreaseBadgeIndices;

		private string _clickToCloseText;

		private string _titleText;

		private MPLobbyAchievementBadgeGroupVM _badgeGroup;

		private MBBindingList<StringPairItemVM> _availableBadgeIDs;
	}
}
