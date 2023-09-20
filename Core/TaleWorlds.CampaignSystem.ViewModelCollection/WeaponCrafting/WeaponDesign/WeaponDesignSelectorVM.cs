using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	public class WeaponDesignSelectorVM : ViewModel
	{
		public WeaponDesign Design { get; }

		public WeaponDesignSelectorVM(WeaponDesign design, Action<WeaponDesignSelectorVM> onSelection)
		{
			this._onSelection = onSelection;
			this.Design = design;
			TextObject textObject = new TextObject("{=uZhHh7pm}Crafted {CURR_TEMPLATE_NAME}", null);
			textObject.SetTextVariable("CURR_TEMPLATE_NAME", design.Template.TemplateName);
			TextObject textObject2 = design.WeaponName ?? textObject;
			this.Name = textObject2.ToString();
			this._generatedVisualItem = new ItemObject();
			Crafting.GenerateItem(design, textObject2, Hero.MainHero.Culture, design.Template.ItemModifierGroup, ref this._generatedVisualItem);
			MBObjectManager.Instance.RegisterObject<ItemObject>(this._generatedVisualItem);
			this.Visual = new ImageIdentifierVM(this._generatedVisualItem, "");
			this.WeaponTypeCode = design.Template.StringId;
			this.Hint = new BasicTooltipViewModel(() => this.GetHint());
		}

		private List<TooltipProperty> GetHint()
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			list.Add(new TooltipProperty("", this._generatedVisualItem.Name.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.Title));
			foreach (CraftingStatData craftingStatData in Crafting.GetStatDatasFromTemplate(0, this._generatedVisualItem, this.Design.Template))
			{
				if (craftingStatData.IsValid && craftingStatData.CurValue > 0f && craftingStatData.MaxValue > 0f)
				{
					list.Add(new TooltipProperty(craftingStatData.DescriptionText.ToString(), craftingStatData.CurValue.ToString(), 0, false, TooltipProperty.TooltipPropertyFlags.None));
				}
			}
			return list;
		}

		public void ExecuteSelect()
		{
			Action<WeaponDesignSelectorVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			MBObjectManager.Instance.UnregisterObject(this._generatedVisualItem);
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
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
		public string WeaponTypeCode
		{
			get
			{
				return this._weaponTypeCode;
			}
			set
			{
				if (value != this._weaponTypeCode)
				{
					this._weaponTypeCode = value;
					base.OnPropertyChangedWithValue<string>(value, "WeaponTypeCode");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Hint");
				}
			}
		}

		private Action<WeaponDesignSelectorVM> _onSelection;

		private ItemObject _generatedVisualItem;

		private bool _isSelected;

		private string _name;

		private string _weaponTypeCode;

		private ImageIdentifierVM _visual;

		private BasicTooltipViewModel _hint;
	}
}
