using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x0200001A RID: 26
	public class WidgetInstantiationResult
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00003D4D File Offset: 0x00001F4D
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x00003D55 File Offset: 0x00001F55
		public Widget Widget { get; private set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00003D5E File Offset: 0x00001F5E
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x00003D66 File Offset: 0x00001F66
		public WidgetTemplate Template { get; private set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x00003D6F File Offset: 0x00001F6F
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x00003D77 File Offset: 0x00001F77
		public WidgetInstantiationResult CustomWidgetInstantiationData { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00003D80 File Offset: 0x00001F80
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00003D88 File Offset: 0x00001F88
		public List<WidgetInstantiationResult> Children { get; private set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00003D91 File Offset: 0x00001F91
		internal IEnumerable<WidgetInstantiationResultExtensionData> ExtensionDatas
		{
			get
			{
				return this._entensionData.Values;
			}
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00003D9E File Offset: 0x00001F9E
		public WidgetInstantiationResult(Widget widget, WidgetTemplate widgetTemplate, WidgetInstantiationResult customWidgetInstantiationData)
		{
			this.CustomWidgetInstantiationData = customWidgetInstantiationData;
			this.Widget = widget;
			this.Template = widgetTemplate;
			this.Children = new List<WidgetInstantiationResult>();
			this._entensionData = new Dictionary<string, WidgetInstantiationResultExtensionData>();
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00003DD4 File Offset: 0x00001FD4
		public void AddExtensionData(string name, object data, bool passToChildWidgetCreation = false)
		{
			WidgetInstantiationResultExtensionData widgetInstantiationResultExtensionData = default(WidgetInstantiationResultExtensionData);
			widgetInstantiationResultExtensionData.Name = name;
			widgetInstantiationResultExtensionData.Data = data;
			widgetInstantiationResultExtensionData.PassToChildWidgetCreation = passToChildWidgetCreation;
			this._entensionData.Add(name, widgetInstantiationResultExtensionData);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00003E10 File Offset: 0x00002010
		public T GetExtensionData<T>(string name)
		{
			return (T)((object)this._entensionData[name].Data);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00003E36 File Offset: 0x00002036
		internal WidgetInstantiationResultExtensionData GetExtensionData(string name)
		{
			return this._entensionData[name];
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00003E44 File Offset: 0x00002044
		public void AddExtensionData(object data, bool passToChildWidgetCreation = false)
		{
			this.AddExtensionData(data.GetType().Name, data, passToChildWidgetCreation);
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00003E59 File Offset: 0x00002059
		public T GetExtensionData<T>() where T : class
		{
			return this.GetExtensionData<T>(typeof(T).Name);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00003E70 File Offset: 0x00002070
		public WidgetInstantiationResult(Widget widget, WidgetTemplate widgetTemplate)
			: this(widget, widgetTemplate, null)
		{
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00003E7B File Offset: 0x0000207B
		public WidgetInstantiationResult GetLogicalOrDefaultChildrenLocation()
		{
			return WidgetInstantiationResult.GetLogicalOrDefaultChildrenLocation(this, true);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00003E84 File Offset: 0x00002084
		private static WidgetInstantiationResult GetLogicalOrDefaultChildrenLocation(WidgetInstantiationResult data, bool isRoot)
		{
			if (isRoot)
			{
				foreach (WidgetInstantiationResult widgetInstantiationResult in data.CustomWidgetInstantiationData.Children)
				{
					if (widgetInstantiationResult.Template.LogicalChildrenLocation)
					{
						return widgetInstantiationResult;
					}
				}
				foreach (WidgetInstantiationResult widgetInstantiationResult2 in data.CustomWidgetInstantiationData.Children)
				{
					WidgetInstantiationResult logicalOrDefaultChildrenLocation = WidgetInstantiationResult.GetLogicalOrDefaultChildrenLocation(widgetInstantiationResult2, false);
					if (logicalOrDefaultChildrenLocation != null)
					{
						return logicalOrDefaultChildrenLocation;
					}
				}
				return data;
			}
			foreach (WidgetInstantiationResult widgetInstantiationResult3 in data.Children)
			{
				if (widgetInstantiationResult3.Template.LogicalChildrenLocation)
				{
					return widgetInstantiationResult3;
				}
			}
			foreach (WidgetInstantiationResult widgetInstantiationResult4 in data.Children)
			{
				WidgetInstantiationResult logicalOrDefaultChildrenLocation2 = WidgetInstantiationResult.GetLogicalOrDefaultChildrenLocation(widgetInstantiationResult4, false);
				if (logicalOrDefaultChildrenLocation2 != null)
				{
					return logicalOrDefaultChildrenLocation2;
				}
			}
			return null;
		}

		// Token: 0x04000040 RID: 64
		private Dictionary<string, WidgetInstantiationResultExtensionData> _entensionData;
	}
}
