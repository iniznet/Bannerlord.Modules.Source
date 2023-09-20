using System;
using System.Collections.Generic;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x0200000D RID: 13
	public class WidgetAttributeContext
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002CB4 File Offset: 0x00000EB4
		public IEnumerable<WidgetAttributeKeyType> RegisteredKeyTypes
		{
			get
			{
				return this._registeredKeyTypes;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00002CBC File Offset: 0x00000EBC
		public IEnumerable<WidgetAttributeValueType> RegisteredValueTypes
		{
			get
			{
				return this._registeredValueTypes;
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00002CC4 File Offset: 0x00000EC4
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

		// Token: 0x0600005A RID: 90 RVA: 0x00002D4F File Offset: 0x00000F4F
		public void RegisterKeyType(WidgetAttributeKeyType keyType)
		{
			this._registeredKeyTypes.Add(keyType);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00002D5D File Offset: 0x00000F5D
		public void RegisterValueType(WidgetAttributeValueType valueType)
		{
			this._registeredValueTypes.Add(valueType);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002D6C File Offset: 0x00000F6C
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

		// Token: 0x0600005D RID: 93 RVA: 0x00002DD8 File Offset: 0x00000FD8
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

		// Token: 0x04000026 RID: 38
		private List<WidgetAttributeKeyType> _registeredKeyTypes;

		// Token: 0x04000027 RID: 39
		private List<WidgetAttributeValueType> _registeredValueTypes;

		// Token: 0x04000028 RID: 40
		private WidgetAttributeKeyTypeAttribute _widgetAttributeKeyTypeAttribute;

		// Token: 0x04000029 RID: 41
		private WidgetAttributeValueTypeDefault _widgetAttributeValueTypeDefault;
	}
}
