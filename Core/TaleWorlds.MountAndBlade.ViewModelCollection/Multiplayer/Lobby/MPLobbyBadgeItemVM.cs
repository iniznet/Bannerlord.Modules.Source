using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x02000058 RID: 88
	public class MPLobbyBadgeItemVM : ViewModel
	{
		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000775 RID: 1909 RVA: 0x0001DA40 File Offset: 0x0001BC40
		// (set) Token: 0x06000776 RID: 1910 RVA: 0x0001DA48 File Offset: 0x0001BC48
		public Badge Badge { get; private set; }

		// Token: 0x06000777 RID: 1911 RVA: 0x0001DA54 File Offset: 0x0001BC54
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

		// Token: 0x06000778 RID: 1912 RVA: 0x0001DAA1 File Offset: 0x0001BCA1
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.BadgeConditionsText = GameTexts.FindText("str_multiplayer_badge_conditions", null).ToString();
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x0001DABF File Offset: 0x0001BCBF
		public void RefreshKeyBindings(HotKey inspectProgressKey)
		{
			this.InspectProgressKey = InputKeyItemVM.CreateFromHotKey(inspectProgressKey, false);
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x0001DACE File Offset: 0x0001BCCE
		public override void OnFinalize()
		{
			InputKeyItemVM inspectProgressKey = this.InspectProgressKey;
			if (inspectProgressKey == null)
			{
				return;
			}
			inspectProgressKey.OnFinalize();
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x0001DAE0 File Offset: 0x0001BCE0
		public void UpdateWith(Badge badge)
		{
			this.Badge = badge;
			this.BadgeId = ((this.Badge == null) ? "none" : this.Badge.StringId);
			this.UpdateIsSelected();
			this.IsEarned = this._hasPlayerEarnedBadge(badge);
			this.RefreshProperties();
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x0001DB34 File Offset: 0x0001BD34
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
						if (badgeCondition.Type == ConditionType.PlayerDataNumeric)
						{
							int num = NetworkMain.GameClient.PlayerData.GetBadgeConditionNumericValue(badgeCondition);
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

		// Token: 0x0600077D RID: 1917 RVA: 0x0001DC74 File Offset: 0x0001BE74
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

		// Token: 0x0600077E RID: 1918 RVA: 0x0001DCAD File Offset: 0x0001BEAD
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

		// Token: 0x0600077F RID: 1919 RVA: 0x0001DCD4 File Offset: 0x0001BED4
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

		// Token: 0x06000780 RID: 1920 RVA: 0x0001DD37 File Offset: 0x0001BF37
		public void SetGroup(MPLobbyAchievementBadgeGroupVM group, Action<MPLobbyAchievementBadgeGroupVM> onBadgeProgressInfoRequested)
		{
			this._group = group;
			this._onBadgeProgressInfoRequested = onBadgeProgressInfoRequested;
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x0001DD47 File Offset: 0x0001BF47
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

		// Token: 0x06000782 RID: 1922 RVA: 0x0001DD70 File Offset: 0x0001BF70
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

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000783 RID: 1923 RVA: 0x0001DD8A File Offset: 0x0001BF8A
		// (set) Token: 0x06000784 RID: 1924 RVA: 0x0001DD92 File Offset: 0x0001BF92
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

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000785 RID: 1925 RVA: 0x0001DDB5 File Offset: 0x0001BFB5
		// (set) Token: 0x06000786 RID: 1926 RVA: 0x0001DDBD File Offset: 0x0001BFBD
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

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06000787 RID: 1927 RVA: 0x0001DDE0 File Offset: 0x0001BFE0
		// (set) Token: 0x06000788 RID: 1928 RVA: 0x0001DDE8 File Offset: 0x0001BFE8
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

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06000789 RID: 1929 RVA: 0x0001DE0B File Offset: 0x0001C00B
		// (set) Token: 0x0600078A RID: 1930 RVA: 0x0001DE13 File Offset: 0x0001C013
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

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x0600078B RID: 1931 RVA: 0x0001DE31 File Offset: 0x0001C031
		// (set) Token: 0x0600078C RID: 1932 RVA: 0x0001DE39 File Offset: 0x0001C039
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

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x0600078D RID: 1933 RVA: 0x0001DE5C File Offset: 0x0001C05C
		// (set) Token: 0x0600078E RID: 1934 RVA: 0x0001DE64 File Offset: 0x0001C064
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

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x0600078F RID: 1935 RVA: 0x0001DE82 File Offset: 0x0001C082
		// (set) Token: 0x06000790 RID: 1936 RVA: 0x0001DE8A File Offset: 0x0001C08A
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

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000791 RID: 1937 RVA: 0x0001DEA8 File Offset: 0x0001C0A8
		// (set) Token: 0x06000792 RID: 1938 RVA: 0x0001DEB0 File Offset: 0x0001C0B0
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

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000793 RID: 1939 RVA: 0x0001DECE File Offset: 0x0001C0CE
		// (set) Token: 0x06000794 RID: 1940 RVA: 0x0001DED6 File Offset: 0x0001C0D6
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

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06000795 RID: 1941 RVA: 0x0001DEF4 File Offset: 0x0001C0F4
		// (set) Token: 0x06000796 RID: 1942 RVA: 0x0001DEFC File Offset: 0x0001C0FC
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

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06000797 RID: 1943 RVA: 0x0001DF1A File Offset: 0x0001C11A
		// (set) Token: 0x06000798 RID: 1944 RVA: 0x0001DF22 File Offset: 0x0001C122
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

		// Token: 0x040003CC RID: 972
		private readonly Func<Badge, bool> _hasPlayerEarnedBadge;

		// Token: 0x040003CD RID: 973
		private readonly Action _onSelectedBadgeChange;

		// Token: 0x040003CE RID: 974
		private readonly Action<MPLobbyBadgeItemVM> _onInspected;

		// Token: 0x040003CF RID: 975
		private MPLobbyAchievementBadgeGroupVM _group;

		// Token: 0x040003D0 RID: 976
		private Action<MPLobbyAchievementBadgeGroupVM> _onBadgeProgressInfoRequested;

		// Token: 0x040003D1 RID: 977
		private const string PlaytimeConditionID = "Playtime";

		// Token: 0x040003D2 RID: 978
		private string _name;

		// Token: 0x040003D3 RID: 979
		private string _description;

		// Token: 0x040003D4 RID: 980
		private string _badgeConditionsText;

		// Token: 0x040003D5 RID: 981
		private string _badgeId;

		// Token: 0x040003D6 RID: 982
		private bool _isEarned;

		// Token: 0x040003D7 RID: 983
		private bool _isSelected;

		// Token: 0x040003D8 RID: 984
		private bool _hasNotification;

		// Token: 0x040003D9 RID: 985
		private bool _isBeingChanged;

		// Token: 0x040003DA RID: 986
		private bool _isFocused;

		// Token: 0x040003DB RID: 987
		private MBBindingList<StringPairItemVM> _conditions;

		// Token: 0x040003DC RID: 988
		private InputKeyItemVM _inspectProgressKey;
	}
}
