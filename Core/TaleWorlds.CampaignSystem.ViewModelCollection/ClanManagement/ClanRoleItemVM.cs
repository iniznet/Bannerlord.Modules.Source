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
	public class ClanRoleItemVM : ViewModel
	{
		public SkillEffect.PerkRole Role { get; private set; }

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

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Members.ApplyActionOnAllItems(delegate(ClanRoleMemberItemVM x)
			{
				x.OnFinalize();
			});
		}

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

		public void ExecuteToggleRoleSelection()
		{
			Action<ClanRoleItemVM> onRoleSelectionToggled = this._onRoleSelectionToggled;
			if (onRoleSelectionToggled == null)
			{
				return;
			}
			onRoleSelectionToggled(this);
		}

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

		public void SetEnabled(bool enabled, TextObject disabledHint)
		{
			this.IsEnabled = enabled;
			this.DisabledHint.HintText = disabledHint;
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

		private Action<ClanRoleItemVM> _onRoleSelectionToggled;

		private Action _onRoleAssigned;

		private MBBindingList<ClanPartyMemberItemVM> _heroMembers;

		private MobileParty _party;

		private ClanRoleItemVM.ClanRoleMemberComparer _comparer;

		private bool _isEnabled;

		private MBBindingList<ClanRoleMemberItemVM> _members;

		private ClanRoleMemberItemVM _effectiveOwner;

		private HintViewModel _notAssignedHint;

		private HintViewModel _disabledHint;

		private bool _isNotAssigned;

		private bool _hasEffects;

		private string _name;

		private string _assignedMemberEffects;

		private string _noEffectText;

		private class ClanRoleMemberComparer : IComparer<ClanRoleMemberItemVM>
		{
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
