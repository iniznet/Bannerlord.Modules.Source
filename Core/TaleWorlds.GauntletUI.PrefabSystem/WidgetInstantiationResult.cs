using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class WidgetInstantiationResult
	{
		public Widget Widget { get; private set; }

		public WidgetTemplate Template { get; private set; }

		public WidgetInstantiationResult CustomWidgetInstantiationData { get; private set; }

		public List<WidgetInstantiationResult> Children { get; private set; }

		internal IEnumerable<WidgetInstantiationResultExtensionData> ExtensionDatas
		{
			get
			{
				return this._entensionData.Values;
			}
		}

		public WidgetInstantiationResult(Widget widget, WidgetTemplate widgetTemplate, WidgetInstantiationResult customWidgetInstantiationData)
		{
			this.CustomWidgetInstantiationData = customWidgetInstantiationData;
			this.Widget = widget;
			this.Template = widgetTemplate;
			this.Children = new List<WidgetInstantiationResult>();
			this._entensionData = new Dictionary<string, WidgetInstantiationResultExtensionData>();
		}

		public void AddExtensionData(string name, object data, bool passToChildWidgetCreation = false)
		{
			WidgetInstantiationResultExtensionData widgetInstantiationResultExtensionData = default(WidgetInstantiationResultExtensionData);
			widgetInstantiationResultExtensionData.Name = name;
			widgetInstantiationResultExtensionData.Data = data;
			widgetInstantiationResultExtensionData.PassToChildWidgetCreation = passToChildWidgetCreation;
			this._entensionData.Add(name, widgetInstantiationResultExtensionData);
		}

		public T GetExtensionData<T>(string name)
		{
			return (T)((object)this._entensionData[name].Data);
		}

		internal WidgetInstantiationResultExtensionData GetExtensionData(string name)
		{
			return this._entensionData[name];
		}

		public void AddExtensionData(object data, bool passToChildWidgetCreation = false)
		{
			this.AddExtensionData(data.GetType().Name, data, passToChildWidgetCreation);
		}

		public T GetExtensionData<T>() where T : class
		{
			return this.GetExtensionData<T>(typeof(T).Name);
		}

		public WidgetInstantiationResult(Widget widget, WidgetTemplate widgetTemplate)
			: this(widget, widgetTemplate, null)
		{
		}

		public WidgetInstantiationResult GetLogicalOrDefaultChildrenLocation()
		{
			return WidgetInstantiationResult.GetLogicalOrDefaultChildrenLocation(this, true);
		}

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

		private Dictionary<string, WidgetInstantiationResultExtensionData> _entensionData;
	}
}
