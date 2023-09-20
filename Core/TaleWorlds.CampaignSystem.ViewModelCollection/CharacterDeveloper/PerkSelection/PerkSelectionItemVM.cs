using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection
{
	// Token: 0x0200011D RID: 285
	public class PerkSelectionItemVM : ViewModel
	{
		// Token: 0x06001BDC RID: 7132 RVA: 0x000645A5 File Offset: 0x000627A5
		public PerkSelectionItemVM(PerkObject perk, Action<PerkSelectionItemVM> onSelection)
		{
			this.Perk = perk;
			this._onSelection = onSelection;
			this.RefreshValues();
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x000645C4 File Offset: 0x000627C4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PickText = new TextObject("{=1CXlqb2U}Pick:", null).ToString();
			this.PerkName = this.Perk.Name.ToString();
			this.PerkDescription = this.Perk.Description.ToString();
			TextObject combinedPerkRoleText = CampaignUIHelper.GetCombinedPerkRoleText(this.Perk);
			this.PerkRole = ((combinedPerkRoleText != null) ? combinedPerkRoleText.ToString() : null) ?? "";
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x0006463F File Offset: 0x0006283F
		public void ExecuteSelection()
		{
			this._onSelection(this);
		}

		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x06001BDF RID: 7135 RVA: 0x0006464D File Offset: 0x0006284D
		// (set) Token: 0x06001BE0 RID: 7136 RVA: 0x00064655 File Offset: 0x00062855
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

		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x06001BE1 RID: 7137 RVA: 0x00064678 File Offset: 0x00062878
		// (set) Token: 0x06001BE2 RID: 7138 RVA: 0x00064680 File Offset: 0x00062880
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

		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x06001BE3 RID: 7139 RVA: 0x000646A3 File Offset: 0x000628A3
		// (set) Token: 0x06001BE4 RID: 7140 RVA: 0x000646AB File Offset: 0x000628AB
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

		// Token: 0x17000991 RID: 2449
		// (get) Token: 0x06001BE5 RID: 7141 RVA: 0x000646CE File Offset: 0x000628CE
		// (set) Token: 0x06001BE6 RID: 7142 RVA: 0x000646D6 File Offset: 0x000628D6
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

		// Token: 0x04000D28 RID: 3368
		private readonly Action<PerkSelectionItemVM> _onSelection;

		// Token: 0x04000D29 RID: 3369
		public readonly PerkObject Perk;

		// Token: 0x04000D2A RID: 3370
		private string _pickText;

		// Token: 0x04000D2B RID: 3371
		private string _perkName;

		// Token: 0x04000D2C RID: 3372
		private string _perkDescription;

		// Token: 0x04000D2D RID: 3373
		private string _perkRole;
	}
}
