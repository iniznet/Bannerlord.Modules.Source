using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	// Token: 0x02000065 RID: 101
	public class MPLobbyAchievementBadgeGroupVM : ViewModel
	{
		// Token: 0x06000953 RID: 2387 RVA: 0x00022F30 File Offset: 0x00021130
		public MPLobbyAchievementBadgeGroupVM(string groupID, Action<MPLobbyAchievementBadgeGroupVM> onBadgeProgressInfoRequested)
		{
			this._onBadgeProgressInfoRequested = onBadgeProgressInfoRequested;
			this.GroupID = groupID;
			this.Badges = new MBBindingList<MPLobbyBadgeItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x00022F57 File Offset: 0x00021157
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ProgressCompletedText = new TextObject("{=vlACTion}You've unlocked all badges!", null).ToString();
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x00022F78 File Offset: 0x00021178
		public void RefreshKeyBindings(HotKey inspectProgressKey)
		{
			this._inspectProgressKey = inspectProgressKey;
			foreach (MPLobbyBadgeItemVM mplobbyBadgeItemVM in this.Badges)
			{
				mplobbyBadgeItemVM.RefreshKeyBindings(inspectProgressKey);
			}
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x00022FCC File Offset: 0x000211CC
		public void OnGroupBadgeAdded(MPLobbyBadgeItemVM badgeItem)
		{
			if (this.ShownBadgeItem == null)
			{
				this.ShownBadgeItem = badgeItem;
			}
			else if (badgeItem.IsEarned && badgeItem.Badge.Index > this.ShownBadgeItem.Badge.Index)
			{
				this.ShownBadgeItem = badgeItem;
			}
			badgeItem.SetGroup(this, this._onBadgeProgressInfoRequested);
			this._totalBadgeCount++;
			if (badgeItem.IsEarned)
			{
				this._unlockedBadgeCount++;
			}
			this.IsProgressComplete = this._totalBadgeCount == this._unlockedBadgeCount;
			ConditionalBadge conditionalBadge;
			if ((conditionalBadge = badgeItem.Badge as ConditionalBadge) != null && conditionalBadge.BadgeConditions.Count > 0 && !conditionalBadge.IsTimed)
			{
				BadgeCondition badgeCondition = conditionalBadge.BadgeConditions[0];
				string text;
				int num;
				if (badgeCondition.Parameters.TryGetValue("min_value", out text) && int.TryParse(text, out num))
				{
					int num2 = NetworkMain.GameClient.PlayerData.GetBadgeConditionNumericValue(badgeCondition);
					if (badgeCondition.StringId.Equals("Playtime"))
					{
						num /= 3600;
						num2 /= 3600;
					}
					this.TotalProgress = Math.Max(this.TotalProgress, num);
					this.CurrentProgress = num2;
				}
				else
				{
					this.SetProgressAsCompleted();
				}
			}
			else
			{
				this.SetProgressAsCompleted();
			}
			badgeItem.RefreshKeyBindings(this._inspectProgressKey);
			this.Badges.Add(badgeItem);
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x0002312C File Offset: 0x0002132C
		private void SetProgressAsCompleted()
		{
			this.TotalProgress = 1;
			this.CurrentProgress = 1;
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x0002313C File Offset: 0x0002133C
		public void UpdateBadgeSelection()
		{
			foreach (MPLobbyBadgeItemVM mplobbyBadgeItemVM in this.Badges)
			{
				mplobbyBadgeItemVM.UpdateIsSelected();
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06000959 RID: 2393 RVA: 0x00023188 File Offset: 0x00021388
		// (set) Token: 0x0600095A RID: 2394 RVA: 0x00023190 File Offset: 0x00021390
		[DataSourceProperty]
		public bool IsProgressComplete
		{
			get
			{
				return this._isProgressComplete;
			}
			set
			{
				if (value != this._isProgressComplete)
				{
					this._isProgressComplete = value;
					base.OnPropertyChangedWithValue(value, "IsProgressComplete");
					if (value)
					{
						this.SetProgressAsCompleted();
					}
				}
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x0600095B RID: 2395 RVA: 0x000231B7 File Offset: 0x000213B7
		// (set) Token: 0x0600095C RID: 2396 RVA: 0x000231BF File Offset: 0x000213BF
		[DataSourceProperty]
		public string ProgressCompletedText
		{
			get
			{
				return this._progressCompletedText;
			}
			set
			{
				if (value != this._progressCompletedText)
				{
					this._progressCompletedText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProgressCompletedText");
				}
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x0600095D RID: 2397 RVA: 0x000231E2 File Offset: 0x000213E2
		// (set) Token: 0x0600095E RID: 2398 RVA: 0x000231EA File Offset: 0x000213EA
		[DataSourceProperty]
		public int CurrentProgress
		{
			get
			{
				return this._currentProgress;
			}
			set
			{
				if (value != this._currentProgress)
				{
					this._currentProgress = value;
					base.OnPropertyChangedWithValue(value, "CurrentProgress");
				}
			}
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x0600095F RID: 2399 RVA: 0x00023208 File Offset: 0x00021408
		// (set) Token: 0x06000960 RID: 2400 RVA: 0x00023210 File Offset: 0x00021410
		[DataSourceProperty]
		public int TotalProgress
		{
			get
			{
				return this._totalProgress;
			}
			set
			{
				if (value != this._totalProgress)
				{
					this._totalProgress = value;
					base.OnPropertyChangedWithValue(value, "TotalProgress");
				}
			}
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06000961 RID: 2401 RVA: 0x0002322E File Offset: 0x0002142E
		// (set) Token: 0x06000962 RID: 2402 RVA: 0x00023236 File Offset: 0x00021436
		[DataSourceProperty]
		public MPLobbyBadgeItemVM ShownBadgeItem
		{
			get
			{
				return this._shownBadgeItem;
			}
			set
			{
				if (value != this._shownBadgeItem)
				{
					this._shownBadgeItem = value;
					base.OnPropertyChangedWithValue<MPLobbyBadgeItemVM>(value, "ShownBadgeItem");
				}
			}
		}

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06000963 RID: 2403 RVA: 0x00023254 File Offset: 0x00021454
		// (set) Token: 0x06000964 RID: 2404 RVA: 0x0002325C File Offset: 0x0002145C
		[DataSourceProperty]
		public MBBindingList<MPLobbyBadgeItemVM> Badges
		{
			get
			{
				return this._badges;
			}
			set
			{
				if (value != this._badges)
				{
					this._badges = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyBadgeItemVM>>(value, "Badges");
				}
			}
		}

		// Token: 0x04000491 RID: 1169
		public readonly string GroupID;

		// Token: 0x04000492 RID: 1170
		private readonly Action<MPLobbyAchievementBadgeGroupVM> _onBadgeProgressInfoRequested;

		// Token: 0x04000493 RID: 1171
		private int _unlockedBadgeCount;

		// Token: 0x04000494 RID: 1172
		private int _totalBadgeCount;

		// Token: 0x04000495 RID: 1173
		private const string PlaytimeConditionID = "Playtime";

		// Token: 0x04000496 RID: 1174
		private HotKey _inspectProgressKey;

		// Token: 0x04000497 RID: 1175
		private bool _isProgressComplete;

		// Token: 0x04000498 RID: 1176
		private string _progressCompletedText;

		// Token: 0x04000499 RID: 1177
		private int _currentProgress;

		// Token: 0x0400049A RID: 1178
		private int _totalProgress;

		// Token: 0x0400049B RID: 1179
		private MPLobbyBadgeItemVM _shownBadgeItem;

		// Token: 0x0400049C RID: 1180
		private MBBindingList<MPLobbyBadgeItemVM> _badges;
	}
}
