using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	// Token: 0x020000CB RID: 203
	public class EncyclopediaTroopTreeNodeVM : ViewModel
	{
		// Token: 0x06001333 RID: 4915 RVA: 0x00049B88 File Offset: 0x00047D88
		public EncyclopediaTroopTreeNodeVM(CharacterObject rootCharacter, CharacterObject activeCharacter, bool isAlternativeUpgrade, PerkObject alternativeUpgradePerk = null)
		{
			this.Branch = new MBBindingList<EncyclopediaTroopTreeNodeVM>();
			this.IsActiveUnit = rootCharacter == activeCharacter;
			this.IsAlternativeUpgrade = isAlternativeUpgrade;
			if (alternativeUpgradePerk != null && this.IsAlternativeUpgrade)
			{
				this.AlternativeUpgradeTooltip = new BasicTooltipViewModel(delegate
				{
					TextObject textObject = new TextObject("{=LVJKy6a8}This troop requires {PERK_NAME} ({PERK_SKILL}) perk to upgrade.", null);
					textObject.SetTextVariable("PERK_NAME", alternativeUpgradePerk.Name);
					textObject.SetTextVariable("PERK_SKILL", alternativeUpgradePerk.Skill.Name);
					return textObject.ToString();
				});
			}
			this.Unit = new EncyclopediaUnitVM(rootCharacter, this.IsActiveUnit);
			foreach (CharacterObject characterObject in rootCharacter.UpgradeTargets)
			{
				if (characterObject == rootCharacter)
				{
					Debug.FailedAssert("A character cannot be it's own upgrade target!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Encyclopedia\\Items\\EncyclopediaTroopTreeNodeVM.cs", ".ctor", 36);
				}
				else if (Campaign.Current.EncyclopediaManager.GetPageOf(typeof(CharacterObject)).IsValidEncyclopediaItem(characterObject))
				{
					bool flag = rootCharacter.Culture.IsBandit && !characterObject.Culture.IsBandit;
					PerkObject perkObject;
					Campaign.Current.Models.PartyTroopUpgradeModel.DoesPartyHaveRequiredPerksForUpgrade(PartyBase.MainParty, rootCharacter, characterObject, out perkObject);
					this.Branch.Add(new EncyclopediaTroopTreeNodeVM(characterObject, activeCharacter, flag, perkObject));
				}
			}
		}

		// Token: 0x06001334 RID: 4916 RVA: 0x00049CAE File Offset: 0x00047EAE
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Branch.ApplyActionOnAllItems(delegate(EncyclopediaTroopTreeNodeVM x)
			{
				x.RefreshValues();
			});
			this.Unit.RefreshValues();
		}

		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06001335 RID: 4917 RVA: 0x00049CEB File Offset: 0x00047EEB
		// (set) Token: 0x06001336 RID: 4918 RVA: 0x00049CF3 File Offset: 0x00047EF3
		[DataSourceProperty]
		public bool IsActiveUnit
		{
			get
			{
				return this._isActiveUnit;
			}
			set
			{
				if (value != this._isActiveUnit)
				{
					this._isActiveUnit = value;
					base.OnPropertyChangedWithValue(value, "IsActiveUnit");
				}
			}
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06001337 RID: 4919 RVA: 0x00049D11 File Offset: 0x00047F11
		// (set) Token: 0x06001338 RID: 4920 RVA: 0x00049D19 File Offset: 0x00047F19
		[DataSourceProperty]
		public bool IsAlternativeUpgrade
		{
			get
			{
				return this._isAlternativeUpgrade;
			}
			set
			{
				if (value != this._isAlternativeUpgrade)
				{
					this._isAlternativeUpgrade = value;
					base.OnPropertyChangedWithValue(value, "IsAlternativeUpgrade");
				}
			}
		}

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06001339 RID: 4921 RVA: 0x00049D37 File Offset: 0x00047F37
		// (set) Token: 0x0600133A RID: 4922 RVA: 0x00049D3F File Offset: 0x00047F3F
		[DataSourceProperty]
		public MBBindingList<EncyclopediaTroopTreeNodeVM> Branch
		{
			get
			{
				return this._branch;
			}
			set
			{
				if (value != this._branch)
				{
					this._branch = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaTroopTreeNodeVM>>(value, "Branch");
				}
			}
		}

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x0600133B RID: 4923 RVA: 0x00049D5D File Offset: 0x00047F5D
		// (set) Token: 0x0600133C RID: 4924 RVA: 0x00049D65 File Offset: 0x00047F65
		[DataSourceProperty]
		public EncyclopediaUnitVM Unit
		{
			get
			{
				return this._unit;
			}
			set
			{
				if (value != this._unit)
				{
					this._unit = value;
					base.OnPropertyChangedWithValue<EncyclopediaUnitVM>(value, "Unit");
				}
			}
		}

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x0600133D RID: 4925 RVA: 0x00049D83 File Offset: 0x00047F83
		// (set) Token: 0x0600133E RID: 4926 RVA: 0x00049D8B File Offset: 0x00047F8B
		[DataSourceProperty]
		public BasicTooltipViewModel AlternativeUpgradeTooltip
		{
			get
			{
				return this._alternativeUpgradeTooltip;
			}
			set
			{
				if (value != this._alternativeUpgradeTooltip)
				{
					this._alternativeUpgradeTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "AlternativeUpgradeTooltip");
				}
			}
		}

		// Token: 0x040008E5 RID: 2277
		private MBBindingList<EncyclopediaTroopTreeNodeVM> _branch;

		// Token: 0x040008E6 RID: 2278
		private EncyclopediaUnitVM _unit;

		// Token: 0x040008E7 RID: 2279
		private bool _isActiveUnit;

		// Token: 0x040008E8 RID: 2280
		private bool _isAlternativeUpgrade;

		// Token: 0x040008E9 RID: 2281
		private BasicTooltipViewModel _alternativeUpgradeTooltip;
	}
}
