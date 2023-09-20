using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	// Token: 0x02000066 RID: 102
	public class MPLobbyBadgeProgressInformationVM : ViewModel
	{
		// Token: 0x06000965 RID: 2405 RVA: 0x0002327C File Offset: 0x0002147C
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

		// Token: 0x06000966 RID: 2406 RVA: 0x000232D4 File Offset: 0x000214D4
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

		// Token: 0x06000967 RID: 2407 RVA: 0x00023344 File Offset: 0x00021544
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

		// Token: 0x06000968 RID: 2408 RVA: 0x00023466 File Offset: 0x00021666
		public void ExecuteClosePopup()
		{
			this.BadgeGroup = null;
			this.IsEnabled = false;
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x00023476 File Offset: 0x00021676
		private void ExecuteIncreaseActiveBadgeIndices()
		{
			this._shownBadgeIndexOffset++;
			this.RefreshShownBadges();
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x0002348C File Offset: 0x0002168C
		private void ExecuteDecreaseActiveBadgeIndices()
		{
			this._shownBadgeIndexOffset--;
			this.RefreshShownBadges();
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x0600096B RID: 2411 RVA: 0x000234A2 File Offset: 0x000216A2
		// (set) Token: 0x0600096C RID: 2412 RVA: 0x000234AA File Offset: 0x000216AA
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

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x0600096D RID: 2413 RVA: 0x000234C8 File Offset: 0x000216C8
		// (set) Token: 0x0600096E RID: 2414 RVA: 0x000234D0 File Offset: 0x000216D0
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

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x0600096F RID: 2415 RVA: 0x000234EE File Offset: 0x000216EE
		// (set) Token: 0x06000970 RID: 2416 RVA: 0x000234F6 File Offset: 0x000216F6
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

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06000971 RID: 2417 RVA: 0x00023514 File Offset: 0x00021714
		// (set) Token: 0x06000972 RID: 2418 RVA: 0x0002351C File Offset: 0x0002171C
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

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06000973 RID: 2419 RVA: 0x0002353A File Offset: 0x0002173A
		// (set) Token: 0x06000974 RID: 2420 RVA: 0x00023542 File Offset: 0x00021742
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

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06000975 RID: 2421 RVA: 0x00023565 File Offset: 0x00021765
		// (set) Token: 0x06000976 RID: 2422 RVA: 0x0002356D File Offset: 0x0002176D
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

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06000977 RID: 2423 RVA: 0x00023590 File Offset: 0x00021790
		// (set) Token: 0x06000978 RID: 2424 RVA: 0x00023598 File Offset: 0x00021798
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

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06000979 RID: 2425 RVA: 0x000235B6 File Offset: 0x000217B6
		// (set) Token: 0x0600097A RID: 2426 RVA: 0x000235BE File Offset: 0x000217BE
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

		// Token: 0x0400049D RID: 1181
		private int _shownBadgeIndexOffset;

		// Token: 0x0400049E RID: 1182
		private const int MaxShownBadgeCount = 5;

		// Token: 0x0400049F RID: 1183
		private readonly Func<string> _getContinueKeyText;

		// Token: 0x040004A0 RID: 1184
		private int _shownBadgeCount;

		// Token: 0x040004A1 RID: 1185
		private bool _isEnabled;

		// Token: 0x040004A2 RID: 1186
		private bool _canIncreaseBadgeIndices;

		// Token: 0x040004A3 RID: 1187
		private bool _canDecreaseBadgeIndices;

		// Token: 0x040004A4 RID: 1188
		private string _clickToCloseText;

		// Token: 0x040004A5 RID: 1189
		private string _titleText;

		// Token: 0x040004A6 RID: 1190
		private MPLobbyAchievementBadgeGroupVM _badgeGroup;

		// Token: 0x040004A7 RID: 1191
		private MBBindingList<StringPairItemVM> _availableBadgeIDs;
	}
}
