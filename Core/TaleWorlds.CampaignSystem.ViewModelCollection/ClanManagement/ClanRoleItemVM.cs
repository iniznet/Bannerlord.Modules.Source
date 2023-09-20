using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x02000107 RID: 263
	public class ClanRoleItemVM : ViewModel
	{
		// Token: 0x170008A2 RID: 2210
		// (get) Token: 0x06001929 RID: 6441 RVA: 0x0005AEA6 File Offset: 0x000590A6
		// (set) Token: 0x0600192A RID: 6442 RVA: 0x0005AEAE File Offset: 0x000590AE
		public SkillEffect.PerkRole Role { get; private set; }

		// Token: 0x0600192B RID: 6443 RVA: 0x0005AEB8 File Offset: 0x000590B8
		public ClanRoleItemVM(MobileParty party, SkillEffect.PerkRole role, MBBindingList<ClanPartyMemberItemVM> heroMembers, Action<ClanRoleItemVM> onRoleSelectionToggled, Action onRoleAssigned)
		{
			this.Role = role;
			this._comparer = new ClanRoleItemVM.ClanRoleMemberComparer();
			this._party = party;
			this._onRoleSelectionToggled = onRoleSelectionToggled;
			this._onRoleAssigned = onRoleAssigned;
			this._heroMembers = heroMembers;
			this.Members = new MBBindingList<ClanRoleMemberItemVM>();
			this.NotAssignedHint = new HintViewModel(new TextObject("{=S1iS3OYj}Party leader is default for unassigned roles", null), null);
			this.DisabledHint = new HintViewModel();
			this.IsEnabled = true;
			this.Refresh();
			this.RefreshValues();
		}

		// Token: 0x0600192C RID: 6444 RVA: 0x0005AF3C File Offset: 0x0005913C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = GameTexts.FindText("role", this.Role.ToString()).ToString();
			this.NoEffectText = GameTexts.FindText("str_clan_role_no_effect", null).ToString();
			ClanRoleMemberItemVM effectiveOwner = this.EffectiveOwner;
			this.AssignedMemberEffects = ((effectiveOwner != null) ? effectiveOwner.GetEffectsList(this.Role) : null) ?? "";
			this.HasEffects = !string.IsNullOrEmpty(this.AssignedMemberEffects);
			this.Members.ApplyActionOnAllItems(delegate(ClanRoleMemberItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x0005AFF3 File Offset: 0x000591F3
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Members.ApplyActionOnAllItems(delegate(ClanRoleMemberItemVM x)
			{
				x.OnFinalize();
			});
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x0005B028 File Offset: 0x00059228
		public void Refresh()
		{
			this.Members.ApplyActionOnAllItems(delegate(ClanRoleMemberItemVM x)
			{
				x.OnFinalize();
			});
			this.Members.Clear();
			foreach (ClanPartyMemberItemVM clanPartyMemberItemVM in this._heroMembers)
			{
				if (ClanRoleMemberItemVM.IsHeroAssignableForRole(clanPartyMemberItemVM.HeroObject, this.Role, this._party))
				{
					this.Members.Add(new ClanRoleMemberItemVM(this._party, this.Role, clanPartyMemberItemVM, new Action(this.OnRoleAssigned)));
				}
			}
			this.Members.Add(new ClanRoleMemberItemVM(this._party, this.Role, null, new Action(this.OnRoleAssigned)));
			this.Members.Sort(this._comparer);
			Hero effectiveRoleOwner;
			Hero hero;
			this.GetMemberAssignedToRole(this._party, this.Role, out hero, out effectiveRoleOwner);
			this.EffectiveOwner = this.Members.FirstOrDefault(delegate(ClanRoleMemberItemVM x)
			{
				ClanPartyMemberItemVM member = x.Member;
				return ((member != null) ? member.HeroObject : null) == effectiveRoleOwner;
			});
			this.IsNotAssigned = hero == null;
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x0005B168 File Offset: 0x00059368
		public void ExecuteToggleRoleSelection()
		{
			Action<ClanRoleItemVM> onRoleSelectionToggled = this._onRoleSelectionToggled;
			if (onRoleSelectionToggled == null)
			{
				return;
			}
			onRoleSelectionToggled(this);
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x0005B17C File Offset: 0x0005937C
		private void GetMemberAssignedToRole(MobileParty party, SkillEffect.PerkRole role, out Hero roleOwner, out Hero effectiveRoleOwner)
		{
			roleOwner = party.GetRoleHolder(role);
			switch (role)
			{
			case SkillEffect.PerkRole.Surgeon:
				effectiveRoleOwner = party.EffectiveSurgeon;
				return;
			case SkillEffect.PerkRole.Engineer:
				effectiveRoleOwner = party.EffectiveEngineer;
				return;
			case SkillEffect.PerkRole.Scout:
				effectiveRoleOwner = party.EffectiveScout;
				return;
			case SkillEffect.PerkRole.Quartermaster:
				effectiveRoleOwner = party.EffectiveQuartermaster;
				return;
			default:
				effectiveRoleOwner = party.LeaderHero;
				roleOwner = party.LeaderHero;
				Debug.FailedAssert("Given party role is not valid.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\ClanManagement\\ClanRoleItemVM.cs", "GetMemberAssignedToRole", 107);
				return;
			}
		}

		// Token: 0x06001931 RID: 6449 RVA: 0x0005B1FB File Offset: 0x000593FB
		private void OnRoleAssigned()
		{
			MBInformationManager.HideInformations();
			Action onRoleAssigned = this._onRoleAssigned;
			if (onRoleAssigned == null)
			{
				return;
			}
			onRoleAssigned();
		}

		// Token: 0x06001932 RID: 6450 RVA: 0x0005B212 File Offset: 0x00059412
		public void SetEnabled(bool enabled, TextObject disabledHint)
		{
			this.IsEnabled = enabled;
			this.DisabledHint.HintText = disabledHint;
		}

		// Token: 0x170008A3 RID: 2211
		// (get) Token: 0x06001933 RID: 6451 RVA: 0x0005B227 File Offset: 0x00059427
		// (set) Token: 0x06001934 RID: 6452 RVA: 0x0005B22F File Offset: 0x0005942F
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

		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x06001935 RID: 6453 RVA: 0x0005B24D File Offset: 0x0005944D
		// (set) Token: 0x06001936 RID: 6454 RVA: 0x0005B255 File Offset: 0x00059455
		[DataSourceProperty]
		public MBBindingList<ClanRoleMemberItemVM> Members
		{
			get
			{
				return this._members;
			}
			set
			{
				if (value != this._members)
				{
					this._members = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanRoleMemberItemVM>>(value, "Members");
				}
			}
		}

		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x06001937 RID: 6455 RVA: 0x0005B273 File Offset: 0x00059473
		// (set) Token: 0x06001938 RID: 6456 RVA: 0x0005B27B File Offset: 0x0005947B
		[DataSourceProperty]
		public ClanRoleMemberItemVM EffectiveOwner
		{
			get
			{
				return this._effectiveOwner;
			}
			set
			{
				if (value != this._effectiveOwner)
				{
					this._effectiveOwner = value;
					base.OnPropertyChangedWithValue<ClanRoleMemberItemVM>(value, "EffectiveOwner");
				}
			}
		}

		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x06001939 RID: 6457 RVA: 0x0005B299 File Offset: 0x00059499
		// (set) Token: 0x0600193A RID: 6458 RVA: 0x0005B2A1 File Offset: 0x000594A1
		[DataSourceProperty]
		public HintViewModel NotAssignedHint
		{
			get
			{
				return this._notAssignedHint;
			}
			set
			{
				if (value != this._notAssignedHint)
				{
					this._notAssignedHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "NotAssignedHint");
				}
			}
		}

		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x0600193B RID: 6459 RVA: 0x0005B2BF File Offset: 0x000594BF
		// (set) Token: 0x0600193C RID: 6460 RVA: 0x0005B2C7 File Offset: 0x000594C7
		[DataSourceProperty]
		public HintViewModel DisabledHint
		{
			get
			{
				return this._disabledHint;
			}
			set
			{
				if (value != this._disabledHint)
				{
					this._disabledHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DisabledHint");
				}
			}
		}

		// Token: 0x170008A8 RID: 2216
		// (get) Token: 0x0600193D RID: 6461 RVA: 0x0005B2E5 File Offset: 0x000594E5
		// (set) Token: 0x0600193E RID: 6462 RVA: 0x0005B2ED File Offset: 0x000594ED
		[DataSourceProperty]
		public bool IsNotAssigned
		{
			get
			{
				return this._isNotAssigned;
			}
			set
			{
				if (value != this._isNotAssigned)
				{
					this._isNotAssigned = value;
					base.OnPropertyChangedWithValue(value, "IsNotAssigned");
				}
			}
		}

		// Token: 0x170008A9 RID: 2217
		// (get) Token: 0x0600193F RID: 6463 RVA: 0x0005B30B File Offset: 0x0005950B
		// (set) Token: 0x06001940 RID: 6464 RVA: 0x0005B313 File Offset: 0x00059513
		[DataSourceProperty]
		public bool HasEffects
		{
			get
			{
				return this._hasEffects;
			}
			set
			{
				if (value != this._hasEffects)
				{
					this._hasEffects = value;
					base.OnPropertyChangedWithValue(value, "HasEffects");
				}
			}
		}

		// Token: 0x170008AA RID: 2218
		// (get) Token: 0x06001941 RID: 6465 RVA: 0x0005B331 File Offset: 0x00059531
		// (set) Token: 0x06001942 RID: 6466 RVA: 0x0005B339 File Offset: 0x00059539
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

		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x06001943 RID: 6467 RVA: 0x0005B35C File Offset: 0x0005955C
		// (set) Token: 0x06001944 RID: 6468 RVA: 0x0005B364 File Offset: 0x00059564
		[DataSourceProperty]
		public string AssignedMemberEffects
		{
			get
			{
				return this._assignedMemberEffects;
			}
			set
			{
				if (value != this._assignedMemberEffects)
				{
					this._assignedMemberEffects = value;
					base.OnPropertyChangedWithValue<string>(value, "AssignedMemberEffects");
				}
			}
		}

		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x06001945 RID: 6469 RVA: 0x0005B387 File Offset: 0x00059587
		// (set) Token: 0x06001946 RID: 6470 RVA: 0x0005B38F File Offset: 0x0005958F
		[DataSourceProperty]
		public string NoEffectText
		{
			get
			{
				return this._noEffectText;
			}
			set
			{
				if (value != this._noEffectText)
				{
					this._noEffectText = value;
					base.OnPropertyChangedWithValue<string>(value, "NoEffectText");
				}
			}
		}

		// Token: 0x04000BF4 RID: 3060
		private Action<ClanRoleItemVM> _onRoleSelectionToggled;

		// Token: 0x04000BF5 RID: 3061
		private Action _onRoleAssigned;

		// Token: 0x04000BF6 RID: 3062
		private MBBindingList<ClanPartyMemberItemVM> _heroMembers;

		// Token: 0x04000BF7 RID: 3063
		private MobileParty _party;

		// Token: 0x04000BF8 RID: 3064
		private ClanRoleItemVM.ClanRoleMemberComparer _comparer;

		// Token: 0x04000BF9 RID: 3065
		private bool _isEnabled;

		// Token: 0x04000BFA RID: 3066
		private MBBindingList<ClanRoleMemberItemVM> _members;

		// Token: 0x04000BFB RID: 3067
		private ClanRoleMemberItemVM _effectiveOwner;

		// Token: 0x04000BFC RID: 3068
		private HintViewModel _notAssignedHint;

		// Token: 0x04000BFD RID: 3069
		private HintViewModel _disabledHint;

		// Token: 0x04000BFE RID: 3070
		private bool _isNotAssigned;

		// Token: 0x04000BFF RID: 3071
		private bool _hasEffects;

		// Token: 0x04000C00 RID: 3072
		private string _name;

		// Token: 0x04000C01 RID: 3073
		private string _assignedMemberEffects;

		// Token: 0x04000C02 RID: 3074
		private string _noEffectText;

		// Token: 0x0200023B RID: 571
		private class ClanRoleMemberComparer : IComparer<ClanRoleMemberItemVM>
		{
			// Token: 0x06002163 RID: 8547 RVA: 0x00070AC8 File Offset: 0x0006ECC8
			public int Compare(ClanRoleMemberItemVM x, ClanRoleMemberItemVM y)
			{
				int num = y.RelevantSkillValue.CompareTo(x.RelevantSkillValue);
				if (num == 0)
				{
					return x.Member.HeroObject.Name.ToString().CompareTo(y.Member.HeroObject.Name.ToString());
				}
				return num;
			}
		}
	}
}
