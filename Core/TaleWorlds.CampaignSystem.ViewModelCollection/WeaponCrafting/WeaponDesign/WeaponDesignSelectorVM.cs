using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000E8 RID: 232
	public class WeaponDesignSelectorVM : ViewModel
	{
		// Token: 0x1700072E RID: 1838
		// (get) Token: 0x0600153F RID: 5439 RVA: 0x0004F343 File Offset: 0x0004D543
		public WeaponDesign Design { get; }

		// Token: 0x06001540 RID: 5440 RVA: 0x0004F34C File Offset: 0x0004D54C
		public WeaponDesignSelectorVM(WeaponDesign design, Action<WeaponDesignSelectorVM> onSelection)
		{
			this._onSelection = onSelection;
			this.Design = design;
			TextObject textObject = new TextObject("{=uZhHh7pm}Crafted {CURR_TEMPLATE_NAME}", null);
			textObject.SetTextVariable("CURR_TEMPLATE_NAME", design.Template.TemplateName);
			TextObject textObject2 = design.WeaponName ?? textObject;
			this.Name = textObject2.ToString();
			this._generatedVisualItem = new ItemObject();
			Crafting.GenerateItem(design, textObject2, Hero.MainHero.Culture, design.Template.ItemModifierGroup, ref this._generatedVisualItem, new Crafting.OverrideData(0f, 0, 0, 0, 0));
			MBObjectManager.Instance.RegisterObject<ItemObject>(this._generatedVisualItem);
			this.Visual = new ImageIdentifierVM(this._generatedVisualItem, "");
			this.WeaponTypeCode = design.Template.StringId;
			this.Hint = new BasicTooltipViewModel(() => this.GetHint());
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x0004F434 File Offset: 0x0004D634
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

		// Token: 0x06001542 RID: 5442 RVA: 0x0004F504 File Offset: 0x0004D704
		public void ExecuteSelect()
		{
			Action<WeaponDesignSelectorVM> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this);
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x0004F517 File Offset: 0x0004D717
		public override void OnFinalize()
		{
			base.OnFinalize();
			MBObjectManager.Instance.UnregisterObject(this._generatedVisualItem);
		}

		// Token: 0x1700072F RID: 1839
		// (get) Token: 0x06001544 RID: 5444 RVA: 0x0004F52F File Offset: 0x0004D72F
		// (set) Token: 0x06001545 RID: 5445 RVA: 0x0004F537 File Offset: 0x0004D737
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

		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x06001546 RID: 5446 RVA: 0x0004F555 File Offset: 0x0004D755
		// (set) Token: 0x06001547 RID: 5447 RVA: 0x0004F55D File Offset: 0x0004D75D
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

		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x06001548 RID: 5448 RVA: 0x0004F580 File Offset: 0x0004D780
		// (set) Token: 0x06001549 RID: 5449 RVA: 0x0004F588 File Offset: 0x0004D788
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

		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x0600154A RID: 5450 RVA: 0x0004F5AB File Offset: 0x0004D7AB
		// (set) Token: 0x0600154B RID: 5451 RVA: 0x0004F5B3 File Offset: 0x0004D7B3
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

		// Token: 0x17000733 RID: 1843
		// (get) Token: 0x0600154C RID: 5452 RVA: 0x0004F5D1 File Offset: 0x0004D7D1
		// (set) Token: 0x0600154D RID: 5453 RVA: 0x0004F5D9 File Offset: 0x0004D7D9
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

		// Token: 0x040009F0 RID: 2544
		private Action<WeaponDesignSelectorVM> _onSelection;

		// Token: 0x040009F1 RID: 2545
		private ItemObject _generatedVisualItem;

		// Token: 0x040009F2 RID: 2546
		private bool _isSelected;

		// Token: 0x040009F3 RID: 2547
		private string _name;

		// Token: 0x040009F4 RID: 2548
		private string _weaponTypeCode;

		// Token: 0x040009F5 RID: 2549
		private ImageIdentifierVM _visual;

		// Token: 0x040009F6 RID: 2550
		private BasicTooltipViewModel _hint;
	}
}
