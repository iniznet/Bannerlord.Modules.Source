using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting
{
	// Token: 0x020000D8 RID: 216
	public class CraftingResourceItemVM : ViewModel
	{
		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x060013FD RID: 5117 RVA: 0x0004C034 File Offset: 0x0004A234
		// (set) Token: 0x060013FE RID: 5118 RVA: 0x0004C03C File Offset: 0x0004A23C
		public ItemObject ResourceItem { get; private set; }

		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x060013FF RID: 5119 RVA: 0x0004C045 File Offset: 0x0004A245
		// (set) Token: 0x06001400 RID: 5120 RVA: 0x0004C04D File Offset: 0x0004A24D
		public CraftingMaterials ResourceMaterial { get; private set; }

		// Token: 0x06001401 RID: 5121 RVA: 0x0004C058 File Offset: 0x0004A258
		public CraftingResourceItemVM(CraftingMaterials material, int amount, int changeAmount = 0)
		{
			this.ResourceMaterial = material;
			Campaign campaign = Campaign.Current;
			ItemObject itemObject;
			if (campaign == null)
			{
				itemObject = null;
			}
			else
			{
				GameModels models = campaign.Models;
				if (models == null)
				{
					itemObject = null;
				}
				else
				{
					SmithingModel smithingModel = models.SmithingModel;
					itemObject = ((smithingModel != null) ? smithingModel.GetCraftingMaterialItem(material) : null);
				}
			}
			this.ResourceItem = itemObject;
			ItemObject resourceItem = this.ResourceItem;
			string text;
			if (resourceItem == null)
			{
				text = null;
			}
			else
			{
				TextObject name = resourceItem.Name;
				text = ((name != null) ? name.ToString() : null);
			}
			this.ResourceName = text ?? "none";
			this.ResourceHint = new HintViewModel(new TextObject("{=!}" + this.ResourceName, null), null);
			this.ResourceAmount = amount;
			ItemObject resourceItem2 = this.ResourceItem;
			this.ResourceItemStringId = ((resourceItem2 != null) ? resourceItem2.StringId : null) ?? "none";
			this.ResourceMaterialTypeAsStr = this.ResourceMaterial.ToString();
			this.ResourceChangeAmount = changeAmount;
		}

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x06001402 RID: 5122 RVA: 0x0004C13A File Offset: 0x0004A33A
		// (set) Token: 0x06001403 RID: 5123 RVA: 0x0004C142 File Offset: 0x0004A342
		[DataSourceProperty]
		public string ResourceName
		{
			get
			{
				return this._resourceName;
			}
			set
			{
				if (value != this._resourceName)
				{
					this._resourceName = value;
					base.OnPropertyChangedWithValue<string>(value, "ResourceName");
				}
			}
		}

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x06001404 RID: 5124 RVA: 0x0004C165 File Offset: 0x0004A365
		// (set) Token: 0x06001405 RID: 5125 RVA: 0x0004C16D File Offset: 0x0004A36D
		[DataSourceProperty]
		public HintViewModel ResourceHint
		{
			get
			{
				return this._resourceHint;
			}
			set
			{
				if (value != this._resourceHint)
				{
					this._resourceHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ResourceHint");
				}
			}
		}

		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x06001406 RID: 5126 RVA: 0x0004C18B File Offset: 0x0004A38B
		// (set) Token: 0x06001407 RID: 5127 RVA: 0x0004C193 File Offset: 0x0004A393
		[DataSourceProperty]
		public string ResourceMaterialTypeAsStr
		{
			get
			{
				return this._resourceMaterialTypeAsStr;
			}
			set
			{
				if (value != this._resourceMaterialTypeAsStr)
				{
					this._resourceMaterialTypeAsStr = value;
					base.OnPropertyChangedWithValue<string>(value, "ResourceMaterialTypeAsStr");
				}
			}
		}

		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x06001408 RID: 5128 RVA: 0x0004C1B6 File Offset: 0x0004A3B6
		// (set) Token: 0x06001409 RID: 5129 RVA: 0x0004C1BE File Offset: 0x0004A3BE
		[DataSourceProperty]
		public int ResourceAmount
		{
			get
			{
				return this._resourceUsageAmount;
			}
			set
			{
				if (value != this._resourceUsageAmount)
				{
					this._resourceUsageAmount = value;
					base.OnPropertyChangedWithValue(value, "ResourceAmount");
				}
			}
		}

		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x0600140A RID: 5130 RVA: 0x0004C1DC File Offset: 0x0004A3DC
		// (set) Token: 0x0600140B RID: 5131 RVA: 0x0004C1E4 File Offset: 0x0004A3E4
		[DataSourceProperty]
		public int ResourceChangeAmount
		{
			get
			{
				return this._resourceChangeAmount;
			}
			set
			{
				if (value != this._resourceChangeAmount)
				{
					this._resourceChangeAmount = value;
					base.OnPropertyChangedWithValue(value, "ResourceChangeAmount");
				}
			}
		}

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x0600140C RID: 5132 RVA: 0x0004C202 File Offset: 0x0004A402
		// (set) Token: 0x0600140D RID: 5133 RVA: 0x0004C20A File Offset: 0x0004A40A
		[DataSourceProperty]
		public string ResourceItemStringId
		{
			get
			{
				return this._resourceItemStringId;
			}
			set
			{
				if (value != this._resourceItemStringId)
				{
					this._resourceItemStringId = value;
					base.OnPropertyChangedWithValue<string>(value, "ResourceItemStringId");
				}
			}
		}

		// Token: 0x04000953 RID: 2387
		private string _resourceName;

		// Token: 0x04000954 RID: 2388
		private string _resourceItemStringId;

		// Token: 0x04000955 RID: 2389
		private int _resourceUsageAmount;

		// Token: 0x04000956 RID: 2390
		private int _resourceChangeAmount;

		// Token: 0x04000957 RID: 2391
		private string _resourceMaterialTypeAsStr;

		// Token: 0x04000958 RID: 2392
		private HintViewModel _resourceHint;
	}
}
