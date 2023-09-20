using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	// Token: 0x0200002D RID: 45
	public class UpgradeRequirementsVM : ViewModel
	{
		// Token: 0x06000469 RID: 1129 RVA: 0x000181E6 File Offset: 0x000163E6
		public UpgradeRequirementsVM()
		{
			this.IsItemRequirementMet = true;
			this.IsPerkRequirementMet = true;
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00018212 File Offset: 0x00016412
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdateItemRequirementHint();
			this.UpdatePerkRequirementHint();
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00018226 File Offset: 0x00016426
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

		// Token: 0x0600046C RID: 1132 RVA: 0x00018250 File Offset: 0x00016450
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

		// Token: 0x0600046D RID: 1133 RVA: 0x0001827F File Offset: 0x0001647F
		public void SetRequirementsMet(bool isItemRequirementMet, bool isPerkRequirementMet)
		{
			this.IsItemRequirementMet = !this.HasItemRequirement || isItemRequirementMet;
			this.IsPerkRequirementMet = !this.HasPerkRequirement || isPerkRequirementMet;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x000182A4 File Offset: 0x000164A4
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

		// Token: 0x0600046F RID: 1135 RVA: 0x000182F0 File Offset: 0x000164F0
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

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x0001833B File Offset: 0x0001653B
		// (set) Token: 0x06000471 RID: 1137 RVA: 0x00018343 File Offset: 0x00016543
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

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000472 RID: 1138 RVA: 0x00018361 File Offset: 0x00016561
		// (set) Token: 0x06000473 RID: 1139 RVA: 0x00018369 File Offset: 0x00016569
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

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000474 RID: 1140 RVA: 0x00018387 File Offset: 0x00016587
		// (set) Token: 0x06000475 RID: 1141 RVA: 0x0001838F File Offset: 0x0001658F
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

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000476 RID: 1142 RVA: 0x000183AD File Offset: 0x000165AD
		// (set) Token: 0x06000477 RID: 1143 RVA: 0x000183B5 File Offset: 0x000165B5
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

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000478 RID: 1144 RVA: 0x000183D3 File Offset: 0x000165D3
		// (set) Token: 0x06000479 RID: 1145 RVA: 0x000183DB File Offset: 0x000165DB
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

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x000183FE File Offset: 0x000165FE
		// (set) Token: 0x0600047B RID: 1147 RVA: 0x00018406 File Offset: 0x00016606
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

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x0600047C RID: 1148 RVA: 0x00018429 File Offset: 0x00016629
		// (set) Token: 0x0600047D RID: 1149 RVA: 0x00018431 File Offset: 0x00016631
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

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x0600047E RID: 1150 RVA: 0x0001844F File Offset: 0x0001664F
		// (set) Token: 0x0600047F RID: 1151 RVA: 0x00018457 File Offset: 0x00016657
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

		// Token: 0x040001E6 RID: 486
		private ItemCategory _category;

		// Token: 0x040001E7 RID: 487
		private PerkObject _perk;

		// Token: 0x040001E8 RID: 488
		private bool _isItemRequirementMet;

		// Token: 0x040001E9 RID: 489
		private bool _isPerkRequirementMet;

		// Token: 0x040001EA RID: 490
		private bool _hasItemRequirement;

		// Token: 0x040001EB RID: 491
		private bool _hasPerkRequirement;

		// Token: 0x040001EC RID: 492
		private string _perkRequirement = "";

		// Token: 0x040001ED RID: 493
		private string _itemRequirement = "";

		// Token: 0x040001EE RID: 494
		private HintViewModel _itemRequirementHint;

		// Token: 0x040001EF RID: 495
		private HintViewModel _perkRequirementHint;
	}
}
