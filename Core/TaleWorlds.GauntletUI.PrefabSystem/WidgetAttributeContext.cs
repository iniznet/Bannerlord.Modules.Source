using System;
using System.Collections.Generic;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class WidgetAttributeContext
	{
		public IEnumerable<WidgetAttributeKeyType> RegisteredKeyTypes
		{
			get
			{
				return this._registeredKeyTypes;
			}
		}

		public IEnumerable<WidgetAttributeValueType> RegisteredValueTypes
		{
			get
			{
				return this._registeredValueTypes;
			}
		}

		public WidgetAttributeContext()
		{
			this._registeredKeyTypes = new List<WidgetAttributeKeyType>();
			this._registeredValueTypes = new List<WidgetAttributeValueType>();
			WidgetAttributeKeyTypeId widgetAttributeKeyTypeId = new WidgetAttributeKeyTypeId();
			WidgetAttributeKeyTypeParameter widgetAttributeKeyTypeParameter = new WidgetAttributeKeyTypeParameter();
			this._widgetAttributeKeyTypeAttribute = new WidgetAttributeKeyTypeAttribute();
			this.RegisterKeyType(widgetAttributeKeyTypeId);
			this.RegisterKeyType(widgetAttributeKeyTypeParameter);
			this.RegisterKeyType(this._widgetAttributeKeyTypeAttribute);
			WidgetAttributeValueTypeConstant widgetAttributeValueTypeConstant = new WidgetAttributeValueTypeConstant();
			WidgetAttributeValueTypeParameter widgetAttributeValueTypeParameter = new WidgetAttributeValueTypeParameter();
			this._widgetAttributeValueTypeDefault = new WidgetAttributeValueTypeDefault();
			this.RegisterValueType(widgetAttributeValueTypeConstant);
			this.RegisterValueType(widgetAttributeValueTypeParameter);
			this.RegisterValueType(this._widgetAttributeValueTypeDefault);
		}

		public void RegisterKeyType(WidgetAttributeKeyType keyType)
		{
			this._registeredKeyTypes.Add(keyType);
		}

		public void RegisterValueType(WidgetAttributeValueType valueType)
		{
			this._registeredValueTypes.Add(valueType);
		}

		public WidgetAttributeKeyType GetKeyType(string key)
		{
			WidgetAttributeKeyType widgetAttributeKeyType = null;
			foreach (WidgetAttributeKeyType widgetAttributeKeyType2 in this._registeredKeyTypes)
			{
				if (!(widgetAttributeKeyType2 is WidgetAttributeKeyTypeAttribute) && widgetAttributeKeyType2.CheckKeyType(key))
				{
					widgetAttributeKeyType = widgetAttributeKeyType2;
				}
			}
			if (widgetAttributeKeyType == null)
			{
				widgetAttributeKeyType = this._widgetAttributeKeyTypeAttribute;
			}
			return widgetAttributeKeyType;
		}

		public WidgetAttributeValueType GetValueType(string value)
		{
			WidgetAttributeValueType widgetAttributeValueType = null;
			foreach (WidgetAttributeValueType widgetAttributeValueType2 in this._registeredValueTypes)
			{
				if (!(widgetAttributeValueType2 is WidgetAttributeValueTypeDefault) && widgetAttributeValueType2.CheckValueType(value))
				{
					widgetAttributeValueType = widgetAttributeValueType2;
				}
			}
			if (widgetAttributeValueType == null)
			{
				widgetAttributeValueType = this._widgetAttributeValueTypeDefault;
			}
			return widgetAttributeValueType;
		}

		private List<WidgetAttributeKeyType> _registeredKeyTypes;

		private List<WidgetAttributeValueType> _registeredValueTypes;

		private WidgetAttributeKeyTypeAttribute _widgetAttributeKeyTypeAttribute;

		private WidgetAttributeValueTypeDefault _widgetAttributeValueTypeDefault;
	}
}
