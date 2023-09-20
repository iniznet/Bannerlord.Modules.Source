using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Profile
{
	public class MPLobbyAchievementBadgeGroupVM : ViewModel
	{
		public MPLobbyAchievementBadgeGroupVM(string groupID, Action<MPLobbyAchievementBadgeGroupVM> onBadgeProgressInfoRequested)
		{
			this._onBadgeProgressInfoRequested = onBadgeProgressInfoRequested;
			this.GroupID = groupID;
			this.Badges = new MBBindingList<MPLobbyBadgeItemVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ProgressCompletedText = new TextObject("{=vlACTion}You've unlocked all badges!", null).ToString();
		}

		public void RefreshKeyBindings(HotKey inspectProgressKey)
		{
			this._inspectProgressKey = inspectProgressKey;
			foreach (MPLobbyBadgeItemVM mplobbyBadgeItemVM in this.Badges)
			{
				mplobbyBadgeItemVM.RefreshKeyBindings(inspectProgressKey);
			}
		}

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
					int num2 = BadgeManager.GetBadgeConditionNumericValue(NetworkMain.GameClient.PlayerData, badgeCondition);
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

		private void SetProgressAsCompleted()
		{
			this.TotalProgress = 1;
			this.CurrentProgress = 1;
		}

		public void UpdateBadgeSelection()
		{
			foreach (MPLobbyBadgeItemVM mplobbyBadgeItemVM in this.Badges)
			{
				mplobbyBadgeItemVM.UpdateIsSelected();
			}
		}

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

		public readonly string GroupID;

		private readonly Action<MPLobbyAchievementBadgeGroupVM> _onBadgeProgressInfoRequested;

		private int _unlockedBadgeCount;

		private int _totalBadgeCount;

		private const string PlaytimeConditionID = "Playtime";

		private HotKey _inspectProgressKey;

		private bool _isProgressComplete;

		private string _progressCompletedText;

		private int _currentProgress;

		private int _totalProgress;

		private MPLobbyBadgeItemVM _shownBadgeItem;

		private MBBindingList<MPLobbyBadgeItemVM> _badges;
	}
}
