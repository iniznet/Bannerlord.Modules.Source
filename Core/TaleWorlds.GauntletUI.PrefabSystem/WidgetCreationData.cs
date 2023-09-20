using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x02000017 RID: 23
	public class WidgetCreationData
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00002F81 File Offset: 0x00001181
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00002F89 File Offset: 0x00001189
		public Widget Parent { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00002F92 File Offset: 0x00001192
		// (set) Token: 0x0600008A RID: 138 RVA: 0x00002F9A File Offset: 0x0000119A
		public UIContext Context { get; private set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00002FA3 File Offset: 0x000011A3
		// (set) Token: 0x0600008C RID: 140 RVA: 0x00002FAB File Offset: 0x000011AB
		public WidgetFactory WidgetFactory { get; private set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00002FB4 File Offset: 0x000011B4
		public BrushFactory BrushFactory
		{
			get
			{
				return this.Context.BrushFactory;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00002FC1 File Offset: 0x000011C1
		public SpriteData SpriteData
		{
			get
			{
				return this.Context.SpriteData;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00002FCE File Offset: 0x000011CE
		public PrefabExtensionContext PrefabExtensionContext
		{
			get
			{
				return this.WidgetFactory.PrefabExtensionContext;
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00002FDB File Offset: 0x000011DB
		public WidgetCreationData(UIContext context, WidgetFactory widgetFactory, Widget parent)
		{
			this.Context = context;
			this.WidgetFactory = widgetFactory;
			this.Parent = parent;
			this._extensionData = new Dictionary<string, object>();
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00003003 File Offset: 0x00001203
		public WidgetCreationData(UIContext context, WidgetFactory widgetFactory)
		{
			this.Context = context;
			this.WidgetFactory = widgetFactory;
			this.Parent = null;
			this._extensionData = new Dictionary<string, object>();
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000302C File Offset: 0x0000122C
		public WidgetCreationData(WidgetCreationData widgetCreationData, WidgetInstantiationResult parentResult)
		{
			this.Context = widgetCreationData.Context;
			this.WidgetFactory = widgetCreationData.WidgetFactory;
			this.Parent = parentResult.Widget;
			this._extensionData = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> keyValuePair in widgetCreationData._extensionData)
			{
				this._extensionData.Add(keyValuePair.Key, keyValuePair.Value);
			}
			foreach (WidgetInstantiationResultExtensionData widgetInstantiationResultExtensionData in parentResult.ExtensionDatas)
			{
				if (widgetInstantiationResultExtensionData.PassToChildWidgetCreation)
				{
					this.AddExtensionData(widgetInstantiationResultExtensionData.Name, widgetInstantiationResultExtensionData.Data);
				}
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00003118 File Offset: 0x00001318
		public void AddExtensionData(string name, object data)
		{
			if (this._extensionData.ContainsKey(name))
			{
				this._extensionData[name] = data;
				return;
			}
			this._extensionData.Add(name, data);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00003144 File Offset: 0x00001344
		public T GetExtensionData<T>(string name) where T : class
		{
			if (this._extensionData.ContainsKey(name))
			{
				return this._extensionData[name] as T;
			}
			return default(T);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000317F File Offset: 0x0000137F
		public void AddExtensionData(object data)
		{
			this.AddExtensionData(data.GetType().Name, data);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003193 File Offset: 0x00001393
		public T GetExtensionData<T>() where T : class
		{
			return this.GetExtensionData<T>(typeof(T).Name);
		}

		// Token: 0x04000031 RID: 49
		private Dictionary<string, object> _extensionData;
	}
}
