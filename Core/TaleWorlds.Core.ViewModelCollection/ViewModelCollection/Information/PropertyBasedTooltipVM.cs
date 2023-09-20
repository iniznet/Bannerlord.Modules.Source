using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class PropertyBasedTooltipVM : TooltipBaseVM
	{
		public PropertyBasedTooltipVM(Type invokedType, object[] invokedArgs)
			: base(invokedType, invokedArgs)
		{
			this.TooltipPropertyList = new MBBindingList<TooltipProperty>();
			this._isPeriodicRefreshEnabled = true;
			this._periodicRefreshDelay = 2f;
			this.Refresh();
		}

		protected override void OnFinalizeInternal()
		{
			base.IsActive = false;
			this.TooltipPropertyList.Clear();
		}

		public static void AddKeyType(string keyID, Func<string> getKeyText)
		{
			PropertyBasedTooltipVM._keyTextGetters.Add(keyID, getKeyText);
		}

		public string GetKeyText(string keyID)
		{
			if (PropertyBasedTooltipVM._keyTextGetters.ContainsKey(keyID))
			{
				return PropertyBasedTooltipVM._keyTextGetters[keyID]();
			}
			return "";
		}

		protected override void OnPeriodicRefresh()
		{
			base.OnPeriodicRefresh();
			foreach (TooltipProperty tooltipProperty in this.TooltipPropertyList)
			{
				tooltipProperty.RefreshDefinition();
				tooltipProperty.RefreshValue();
			}
		}

		protected override void OnIsExtendedChanged()
		{
			if (base.IsActive)
			{
				base.IsActive = false;
				this.TooltipPropertyList.Clear();
				this.Refresh();
			}
		}

		private void Refresh()
		{
			base.InvokeRefreshData<PropertyBasedTooltipVM>(this);
			if (this.TooltipPropertyList.Count > 0)
			{
				base.IsActive = true;
			}
		}

		public static void RefreshGenericPropertyBasedTooltip(PropertyBasedTooltipVM propertyBasedTooltip, object[] args)
		{
			IEnumerable<TooltipProperty> enumerable = args[0] as List<TooltipProperty>;
			propertyBasedTooltip.Mode = 0;
			Func<TooltipProperty, bool> <>9__0;
			Func<TooltipProperty, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (TooltipProperty p) => (p.OnlyShowWhenExtended && propertyBasedTooltip.IsExtended) || (!p.OnlyShowWhenExtended && p.OnlyShowWhenNotExtended && !propertyBasedTooltip.IsExtended) || (!p.OnlyShowWhenExtended && !p.OnlyShowWhenNotExtended));
			}
			foreach (TooltipProperty tooltipProperty in enumerable.Where(func))
			{
				propertyBasedTooltip.AddPropertyDuplicate(tooltipProperty);
			}
		}

		public void AddProperty(string definition, string value, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		public void AddModifierProperty(string definition, int modifierValue, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			string text = ((modifierValue > 0) ? ("+" + modifierValue.ToString()) : modifierValue.ToString());
			TooltipProperty tooltipProperty = new TooltipProperty(definition, text, textHeight, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		public void AddProperty(string definition, Func<string> value, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		public void AddProperty(Func<string> definition, Func<string> value, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		public void AddColoredProperty(string definition, string value, Color color, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			if (color == Colors.Black)
			{
				this.AddProperty(definition, value, textHeight, TooltipProperty.TooltipPropertyFlags.None);
				return;
			}
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, color, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		public void AddColoredProperty(string definition, Func<string> value, Color color, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			if (color == Colors.Black)
			{
				this.AddProperty(definition, value, textHeight, TooltipProperty.TooltipPropertyFlags.None);
				return;
			}
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, color, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		public void AddColoredProperty(Func<string> definition, Func<string> value, Color color, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			if (color == Colors.Black)
			{
				this.AddProperty(definition, value, textHeight, TooltipProperty.TooltipPropertyFlags.None);
				return;
			}
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, color, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		private void AddPropertyDuplicate(TooltipProperty property)
		{
			TooltipProperty tooltipProperty = new TooltipProperty(property);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		[DataSourceProperty]
		public MBBindingList<TooltipProperty> TooltipPropertyList
		{
			get
			{
				return this._tooltipPropertyList;
			}
			set
			{
				if (value != this._tooltipPropertyList)
				{
					this._tooltipPropertyList = value;
					base.OnPropertyChangedWithValue<MBBindingList<TooltipProperty>>(value, "TooltipPropertyList");
				}
			}
		}

		[DataSourceProperty]
		public int Mode
		{
			get
			{
				return this._mode;
			}
			set
			{
				if (value != this._mode)
				{
					this._mode = value;
					base.OnPropertyChangedWithValue(value, "Mode");
				}
			}
		}

		private static Dictionary<string, Func<string>> _keyTextGetters = new Dictionary<string, Func<string>>();

		private MBBindingList<TooltipProperty> _tooltipPropertyList;

		private int _mode;

		public enum TooltipMode
		{
			DefaultGame,
			DefaultCampaign,
			Ally,
			Enemy,
			War
		}
	}
}
