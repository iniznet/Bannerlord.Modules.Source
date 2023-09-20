using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	public class PropertyBasedTooltipVM : TooltipVM
	{
		public PropertyBasedTooltipVM()
		{
			this.TooltipPropertyList = new MBBindingList<TooltipProperty>();
		}

		public override void OnShowTooltip(Type type, object[] args)
		{
			if (PropertyBasedTooltipVM._tooltipActions.ContainsKey(type) && (!(this._shownType == type) || this._typeArgs == null || !args.SequenceEqual(this._typeArgs) || !base.IsActive))
			{
				if (base.IsActive)
				{
					this.OnHideTooltip();
				}
				this._typeArgs = args;
				this._shownType = type;
				this._showTimer = 0.1f;
			}
		}

		public override void OnHideTooltip()
		{
			base.IsActive = false;
			this._typeArgs = null;
			this._shownType = null;
			this._showTimer = -1f;
			this.TooltipPropertyList.Clear();
		}

		public static void AddTooltipType(Type type, Action<PropertyBasedTooltipVM, object[]> action)
		{
			PropertyBasedTooltipVM._tooltipActions.Add(type, action);
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

		public void UpdateTooltip(List<TooltipProperty> list)
		{
			this.Mode = 0;
			foreach (TooltipProperty tooltipProperty in list.Where((TooltipProperty p) => (p.OnlyShowWhenExtended && base.IsExtended) || (!p.OnlyShowWhenExtended && p.OnlyShowWhenNotExtended && !base.IsExtended) || (!p.OnlyShowWhenExtended && !p.OnlyShowWhenNotExtended)))
			{
				this.AddPropertyDuplicate(tooltipProperty);
			}
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			if (this._showTimer >= 0f)
			{
				this._showTimer -= dt;
				if (this._showTimer < 0f)
				{
					this._showTimer = -1f;
					this.Refresh();
				}
			}
			if (base.IsActive && this._refreshTimer >= 0f)
			{
				this._refreshTimer -= dt;
				if (this._refreshTimer < 0f)
				{
					this._refreshTimer = 2f;
					foreach (TooltipProperty tooltipProperty in this.TooltipPropertyList)
					{
						tooltipProperty.RefreshDefinition();
						tooltipProperty.RefreshValue();
					}
				}
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
			Action<PropertyBasedTooltipVM, object[]> action;
			if (this._shownType != null && PropertyBasedTooltipVM._tooltipActions.TryGetValue(this._shownType, out action))
			{
				action(this, this._typeArgs);
				if (this.TooltipPropertyList.Count > 0)
				{
					base.IsActive = true;
					return;
				}
			}
			else
			{
				string text = "Unsupported tooltip type: ";
				Type shownType = this._shownType;
				Debug.FailedAssert(text + ((shownType != null) ? shownType.Name : null), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core.ViewModelCollection\\Information\\PropertyBasedTooltipVM.cs", "Refresh", 144);
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

		private float _showTimer = -1f;

		private const float _showDelay = 0.1f;

		private float _refreshTimer;

		private const float _refreshDelay = 2f;

		private Type _shownType;

		private object[] _typeArgs;

		private static Dictionary<Type, Action<PropertyBasedTooltipVM, object[]>> _tooltipActions = new Dictionary<Type, Action<PropertyBasedTooltipVM, object[]>>();

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
