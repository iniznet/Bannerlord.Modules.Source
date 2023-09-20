using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting
{
	public class CraftingResourceItemVM : ViewModel
	{
		public ItemObject ResourceItem { get; private set; }

		public CraftingMaterials ResourceMaterial { get; private set; }

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

		private string _resourceName;

		private string _resourceItemStringId;

		private int _resourceUsageAmount;

		private int _resourceChangeAmount;

		private string _resourceMaterialTypeAsStr;

		private HintViewModel _resourceHint;
	}
}
