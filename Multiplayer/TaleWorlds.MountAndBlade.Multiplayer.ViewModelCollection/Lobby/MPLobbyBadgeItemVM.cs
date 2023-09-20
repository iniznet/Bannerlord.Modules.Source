using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Profile;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby
{
	public class MPLobbyBadgeItemVM : ViewModel
	{
		public Badge Badge { get; private set; }

		public MPLobbyBadgeItemVM(Badge badge, Action onSelectedBadgeChange, Func<Badge, bool> hasPlayerEarnedBadge, Action<MPLobbyBadgeItemVM> onInspected)
		{
			this._hasPlayerEarnedBadge = hasPlayerEarnedBadge;
			this._onSelectedBadgeChange = onSelectedBadgeChange;
			this._onInspected = onInspected;
			this.Badge = badge;
			this.Conditions = new MBBindingList<StringPairItemVM>();
			this.UpdateWith(this.Badge);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.BadgeConditionsText = GameTexts.FindText("str_multiplayer_badge_conditions", null).ToString();
		}

		public void RefreshKeyBindings(HotKey inspectProgressKey)
		{
			this.InspectProgressKey = InputKeyItemVM.CreateFromHotKey(inspectProgressKey, false);
		}

		public override void OnFinalize()
		{
			InputKeyItemVM inspectProgressKey = this.InspectProgressKey;
			if (inspectProgressKey == null)
			{
				return;
			}
			inspectProgressKey.OnFinalize();
		}

		public void UpdateWith(Badge badge)
		{
			this.Badge = badge;
			this.BadgeId = ((this.Badge == null) ? "none" : this.Badge.StringId);
			this.UpdateIsSelected();
			this.IsEarned = this._hasPlayerEarnedBadge(badge);
			this.RefreshProperties();
		}

		private void RefreshProperties()
		{
			this.Conditions.Clear();
			if (this.Badge != null)
			{
				this.Name = this.Badge.Name.ToString();
				this.Description = this.Badge.Description.ToString();
				ConditionalBadge conditionalBadge;
				if ((conditionalBadge = this.Badge as ConditionalBadge) == null || conditionalBadge.BadgeConditions.Count <= 0 || this.Badge.IsTimed)
				{
					return;
				}
				using (IEnumerator<BadgeCondition> enumerator = conditionalBadge.BadgeConditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BadgeCondition badgeCondition = enumerator.Current;
						if (badgeCondition.Type == 2)
						{
							int num = BadgeManager.GetBadgeConditionNumericValue(NetworkMain.GameClient.PlayerData, badgeCondition);
							if (badgeCondition.StringId.Equals("Playtime"))
							{
								num /= 3600;
							}
							this.Conditions.Add(new StringPairItemVM(badgeCondition.Description.ToString(), num.ToString(), null));
						}
					}
					return;
				}
			}
			this.Name = new TextObject("{=koX9okuG}None", null).ToString();
			this.Description = new TextObject("{=gcl2duJH}Reset your badge", null).ToString();
		}

		private async void ExecuteSetAsActive()
		{
			this.IsBeingChanged = true;
			if (this.Badge != null)
			{
				if (this.IsEarned)
				{
					await NetworkMain.GameClient.UpdateShownBadgeId(this.Badge.StringId);
				}
				else
				{
					InformationManager.ShowInquiry(new InquiryData(string.Empty, new TextObject("{=B1KQ4i9q}Badge is not earned yet. Please check conditions.", null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), string.Empty, null, null, "", 0f, null, null, null), false, false);
				}
			}
			else
			{
				await NetworkMain.GameClient.UpdateShownBadgeId("");
			}
			this.IsBeingChanged = false;
			this._onSelectedBadgeChange();
		}

		private void ExecuteShowProgression()
		{
			if (this.Badge is ConditionalBadge)
			{
				Action<MPLobbyAchievementBadgeGroupVM> onBadgeProgressInfoRequested = this._onBadgeProgressInfoRequested;
				if (onBadgeProgressInfoRequested == null)
				{
					return;
				}
				onBadgeProgressInfoRequested(this._group);
			}
		}

		public void UpdateIsSelected()
		{
			if (this.Badge == null)
			{
				PlayerData playerData = NetworkMain.GameClient.PlayerData;
				this.IsSelected = string.IsNullOrEmpty((playerData != null) ? playerData.ShownBadgeId : null);
				return;
			}
			string stringId = this.Badge.StringId;
			PlayerData playerData2 = NetworkMain.GameClient.PlayerData;
			this.IsSelected = stringId == ((playerData2 != null) ? playerData2.ShownBadgeId : null);
		}

		public void SetGroup(MPLobbyAchievementBadgeGroupVM group, Action<MPLobbyAchievementBadgeGroupVM> onBadgeProgressInfoRequested)
		{
			this._group = group;
			this._onBadgeProgressInfoRequested = onBadgeProgressInfoRequested;
		}

		private void ExecuteGainFocus()
		{
			this.IsFocused = true;
			if (this.HasNotification)
			{
				this.HasNotification = false;
			}
			Action<MPLobbyBadgeItemVM> onInspected = this._onInspected;
			if (onInspected == null)
			{
				return;
			}
			onInspected(this);
		}

		private void ExecuteLoseFocus()
		{
			this.IsFocused = false;
			Action<MPLobbyBadgeItemVM> onInspected = this._onInspected;
			if (onInspected == null)
			{
				return;
			}
			onInspected(null);
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
		public string BadgeConditionsText
		{
			get
			{
				return this._badgeConditionsText;
			}
			set
			{
				if (value != this._badgeConditionsText)
				{
					this._badgeConditionsText = value;
					base.OnPropertyChangedWithValue<string>(value, "BadgeConditionsText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<StringPairItemVM> Conditions
		{
			get
			{
				return this._conditions;
			}
			set
			{
				if (value != this._conditions)
				{
					this._conditions = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemVM>>(value, "Conditions");
				}
			}
		}

		[DataSourceProperty]
		public string BadgeId
		{
			get
			{
				return this._badgeId;
			}
			set
			{
				if (value != this._badgeId)
				{
					this._badgeId = value;
					base.OnPropertyChangedWithValue<string>(value, "BadgeId");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEarned
		{
			get
			{
				return this._isEarned;
			}
			set
			{
				if (value != this._isEarned)
				{
					this._isEarned = value;
					base.OnPropertyChangedWithValue(value, "IsEarned");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool HasNotification
		{
			get
			{
				return this._hasNotification;
			}
			set
			{
				if (value != this._hasNotification)
				{
					this._hasNotification = value;
					base.OnPropertyChangedWithValue(value, "HasNotification");
				}
			}
		}

		[DataSourceProperty]
		public bool IsBeingChanged
		{
			get
			{
				return this._isBeingChanged;
			}
			set
			{
				if (value != this._isBeingChanged)
				{
					this._isBeingChanged = value;
					base.OnPropertyChangedWithValue(value, "IsBeingChanged");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			set
			{
				if (value != this._isFocused)
				{
					this._isFocused = value;
					base.OnPropertyChangedWithValue(value, "IsFocused");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM InspectProgressKey
		{
			get
			{
				return this._inspectProgressKey;
			}
			set
			{
				if (value != this._inspectProgressKey)
				{
					this._inspectProgressKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "InspectProgressKey");
				}
			}
		}

		private readonly Func<Badge, bool> _hasPlayerEarnedBadge;

		private readonly Action _onSelectedBadgeChange;

		private readonly Action<MPLobbyBadgeItemVM> _onInspected;

		private MPLobbyAchievementBadgeGroupVM _group;

		private Action<MPLobbyAchievementBadgeGroupVM> _onBadgeProgressInfoRequested;

		private const string PlaytimeConditionID = "Playtime";

		private string _name;

		private string _description;

		private string _badgeConditionsText;

		private string _badgeId;

		private bool _isEarned;

		private bool _isSelected;

		private bool _hasNotification;

		private bool _isBeingChanged;

		private bool _isFocused;

		private MBBindingList<StringPairItemVM> _conditions;

		private InputKeyItemVM _inspectProgressKey;
	}
}
