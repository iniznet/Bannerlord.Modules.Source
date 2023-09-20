using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	public class EncyclopediaTroopTreeNodeVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Branch.ApplyActionOnAllItems(delegate(EncyclopediaTroopTreeNodeVM x)
			{
				x.RefreshValues();
			});
			this.Unit.RefreshValues();
		}

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

		private MBBindingList<EncyclopediaTroopTreeNodeVM> _branch;

		private EncyclopediaUnitVM _unit;

		private bool _isActiveUnit;

		private bool _isAlternativeUpgrade;

		private BasicTooltipViewModel _alternativeUpgradeTooltip;
	}
}
