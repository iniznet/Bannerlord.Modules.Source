using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	// Token: 0x0200007A RID: 122
	public class ItemMenuTooltipPropertyVM : TooltipProperty
	{
		// Token: 0x06000AF6 RID: 2806 RVA: 0x0002ADDA File Offset: 0x00028FDA
		public ItemMenuTooltipPropertyVM()
		{
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x0002ADE2 File Offset: 0x00028FE2
		public ItemMenuTooltipPropertyVM(string definition, string value, int textHeight, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(definition, value, textHeight, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x0002ADF8 File Offset: 0x00028FF8
		public ItemMenuTooltipPropertyVM(string definition, Func<string> _valueFunc, int textHeight, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(definition, _valueFunc, textHeight, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x0002AE0E File Offset: 0x0002900E
		public ItemMenuTooltipPropertyVM(Func<string> _definitionFunc, Func<string> _valueFunc, int textHeight, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(_definitionFunc, _valueFunc, textHeight, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x0002AE24 File Offset: 0x00029024
		public ItemMenuTooltipPropertyVM(Func<string> _definitionFunc, Func<string> _valueFunc, object[] valueArgs, int textHeight, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(_definitionFunc, _valueFunc, valueArgs, textHeight, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x0002AE3C File Offset: 0x0002903C
		public ItemMenuTooltipPropertyVM(string definition, string value, int textHeight, Color color, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
			: base(definition, value, textHeight, color, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x0002AE54 File Offset: 0x00029054
		public ItemMenuTooltipPropertyVM(string definition, Func<string> _valueFunc, int textHeight, Color color, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(definition, _valueFunc, textHeight, color, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x0002AE6C File Offset: 0x0002906C
		public ItemMenuTooltipPropertyVM(Func<string> _definitionFunc, Func<string> _valueFunc, int textHeight, Color color, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(_definitionFunc, _valueFunc, textHeight, color, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		// Token: 0x06000AFE RID: 2814 RVA: 0x0002AE84 File Offset: 0x00029084
		public ItemMenuTooltipPropertyVM(TooltipProperty property, HintViewModel propertyHint = null)
			: base(property)
		{
			this.PropertyHint = propertyHint;
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06000AFF RID: 2815 RVA: 0x0002AE94 File Offset: 0x00029094
		// (set) Token: 0x06000B00 RID: 2816 RVA: 0x0002AE9C File Offset: 0x0002909C
		[DataSourceProperty]
		public HintViewModel PropertyHint
		{
			get
			{
				return this._propertyHint;
			}
			set
			{
				if (value != this._propertyHint)
				{
					this._propertyHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PropertyHint");
				}
			}
		}

		// Token: 0x040004F5 RID: 1269
		private HintViewModel _propertyHint;
	}
}
