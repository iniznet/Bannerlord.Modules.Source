using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	public class ItemMenuTooltipPropertyVM : TooltipProperty
	{
		public ItemMenuTooltipPropertyVM()
		{
		}

		public ItemMenuTooltipPropertyVM(string definition, string value, int textHeight, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(definition, value, textHeight, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		public ItemMenuTooltipPropertyVM(string definition, Func<string> _valueFunc, int textHeight, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(definition, _valueFunc, textHeight, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		public ItemMenuTooltipPropertyVM(Func<string> _definitionFunc, Func<string> _valueFunc, int textHeight, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(_definitionFunc, _valueFunc, textHeight, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		public ItemMenuTooltipPropertyVM(Func<string> _definitionFunc, Func<string> _valueFunc, object[] valueArgs, int textHeight, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(_definitionFunc, _valueFunc, valueArgs, textHeight, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		public ItemMenuTooltipPropertyVM(string definition, string value, int textHeight, Color color, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
			: base(definition, value, textHeight, color, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		public ItemMenuTooltipPropertyVM(string definition, Func<string> _valueFunc, int textHeight, Color color, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(definition, _valueFunc, textHeight, color, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		public ItemMenuTooltipPropertyVM(Func<string> _definitionFunc, Func<string> _valueFunc, int textHeight, Color color, bool onlyShowWhenExtended = false, HintViewModel propertyHint = null)
			: base(_definitionFunc, _valueFunc, textHeight, color, onlyShowWhenExtended, TooltipProperty.TooltipPropertyFlags.None)
		{
			this.PropertyHint = propertyHint;
		}

		public ItemMenuTooltipPropertyVM(TooltipProperty property, HintViewModel propertyHint = null)
			: base(property)
		{
			this.PropertyHint = propertyHint;
		}

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

		private HintViewModel _propertyHint;
	}
}
