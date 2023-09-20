using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection
{
	public class PerkSelectionItemVM : ViewModel
	{
		public PerkSelectionItemVM(PerkObject perk, Action<PerkSelectionItemVM> onSelection)
		{
			this.Perk = perk;
			this._onSelection = onSelection;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PickText = new TextObject("{=1CXlqb2U}Pick:", null).ToString();
			this.PerkName = this.Perk.Name.ToString();
			this.PerkDescription = this.Perk.Description.ToString();
			TextObject combinedPerkRoleText = CampaignUIHelper.GetCombinedPerkRoleText(this.Perk);
			this.PerkRole = ((combinedPerkRoleText != null) ? combinedPerkRoleText.ToString() : null) ?? "";
		}

		public void ExecuteSelection()
		{
			this._onSelection(this);
		}

		[DataSourceProperty]
		public string PickText
		{
			get
			{
				return this._pickText;
			}
			set
			{
				if (value != this._pickText)
				{
					this._pickText = value;
					base.OnPropertyChangedWithValue<string>(value, "PickText");
				}
			}
		}

		[DataSourceProperty]
		public string PerkName
		{
			get
			{
				return this._perkName;
			}
			set
			{
				if (value != this._perkName)
				{
					this._perkName = value;
					base.OnPropertyChangedWithValue<string>(value, "PerkName");
				}
			}
		}

		[DataSourceProperty]
		public string PerkDescription
		{
			get
			{
				return this._perkDescription;
			}
			set
			{
				if (value != this._perkDescription)
				{
					this._perkDescription = value;
					base.OnPropertyChangedWithValue<string>(value, "PerkDescription");
				}
			}
		}

		[DataSourceProperty]
		public string PerkRole
		{
			get
			{
				return this._perkRole;
			}
			set
			{
				if (value != this._perkRole)
				{
					this._perkRole = value;
					base.OnPropertyChangedWithValue<string>(value, "PerkRole");
				}
			}
		}

		private readonly Action<PerkSelectionItemVM> _onSelection;

		public readonly PerkObject Perk;

		private string _pickText;

		private string _perkName;

		private string _perkDescription;

		private string _perkRole;
	}
}
