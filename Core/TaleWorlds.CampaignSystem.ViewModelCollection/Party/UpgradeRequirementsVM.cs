using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	public class UpgradeRequirementsVM : ViewModel
	{
		public UpgradeRequirementsVM()
		{
			this.IsItemRequirementMet = true;
			this.IsPerkRequirementMet = true;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdateItemRequirementHint();
			this.UpdatePerkRequirementHint();
		}

		public void SetItemRequirement(ItemCategory category)
		{
			if (category != null)
			{
				this.HasItemRequirement = true;
				this._category = category;
				this.ItemRequirement = category.StringId.ToLower();
				this.UpdateItemRequirementHint();
			}
		}

		public void SetPerkRequirement(PerkObject perk)
		{
			if (perk != null)
			{
				this.HasPerkRequirement = true;
				this._perk = perk;
				this.PerkRequirement = perk.Skill.StringId.ToLower();
				this.UpdatePerkRequirementHint();
			}
		}

		public void SetRequirementsMet(bool isItemRequirementMet, bool isPerkRequirementMet)
		{
			this.IsItemRequirementMet = !this.HasItemRequirement || isItemRequirementMet;
			this.IsPerkRequirementMet = !this.HasPerkRequirement || isPerkRequirementMet;
		}

		private void UpdateItemRequirementHint()
		{
			if (this._category == null)
			{
				return;
			}
			TextObject textObject = new TextObject("{=Q0j1umAt}Requirement: {REQUIREMENT_NAME}", null);
			textObject.SetTextVariable("REQUIREMENT_NAME", this._category.GetName().ToString());
			this.ItemRequirementHint = new HintViewModel(textObject, null);
		}

		private void UpdatePerkRequirementHint()
		{
			if (this._perk == null)
			{
				return;
			}
			TextObject textObject = new TextObject("{=Q0j1umAt}Requirement: {REQUIREMENT_NAME}", null);
			textObject.SetTextVariable("REQUIREMENT_NAME", this._perk.Name.ToString());
			this.PerkRequirementHint = new HintViewModel(textObject, null);
		}

		[DataSourceProperty]
		public bool IsItemRequirementMet
		{
			get
			{
				return this._isItemRequirementMet;
			}
			set
			{
				if (value != this._isItemRequirementMet)
				{
					this._isItemRequirementMet = value;
					base.OnPropertyChangedWithValue(value, "IsItemRequirementMet");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPerkRequirementMet
		{
			get
			{
				return this._isPerkRequirementMet;
			}
			set
			{
				if (value != this._isPerkRequirementMet)
				{
					this._isPerkRequirementMet = value;
					base.OnPropertyChangedWithValue(value, "IsPerkRequirementMet");
				}
			}
		}

		[DataSourceProperty]
		public bool HasItemRequirement
		{
			get
			{
				return this._hasItemRequirement;
			}
			set
			{
				if (value != this._hasItemRequirement)
				{
					this._hasItemRequirement = value;
					base.OnPropertyChangedWithValue(value, "HasItemRequirement");
				}
			}
		}

		[DataSourceProperty]
		public bool HasPerkRequirement
		{
			get
			{
				return this._hasPerkRequirement;
			}
			set
			{
				if (value != this._hasPerkRequirement)
				{
					this._hasPerkRequirement = value;
					base.OnPropertyChangedWithValue(value, "HasPerkRequirement");
				}
			}
		}

		[DataSourceProperty]
		public string PerkRequirement
		{
			get
			{
				return this._perkRequirement;
			}
			set
			{
				if (value != this._perkRequirement)
				{
					this._perkRequirement = value;
					base.OnPropertyChangedWithValue<string>(value, "PerkRequirement");
				}
			}
		}

		[DataSourceProperty]
		public string ItemRequirement
		{
			get
			{
				return this._itemRequirement;
			}
			set
			{
				if (value != this._itemRequirement)
				{
					this._itemRequirement = value;
					base.OnPropertyChangedWithValue<string>(value, "ItemRequirement");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ItemRequirementHint
		{
			get
			{
				return this._itemRequirementHint;
			}
			set
			{
				if (value != this._itemRequirementHint)
				{
					this._itemRequirementHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ItemRequirementHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel PerkRequirementHint
		{
			get
			{
				return this._perkRequirementHint;
			}
			set
			{
				if (value != this._perkRequirementHint)
				{
					this._perkRequirementHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PerkRequirementHint");
				}
			}
		}

		private ItemCategory _category;

		private PerkObject _perk;

		private bool _isItemRequirementMet;

		private bool _isPerkRequirementMet;

		private bool _hasItemRequirement;

		private bool _hasPerkRequirement;

		private string _perkRequirement = "";

		private string _itemRequirement = "";

		private HintViewModel _itemRequirementHint;

		private HintViewModel _perkRequirementHint;
	}
}
