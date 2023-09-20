using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class WidgetCreationData
	{
		public Widget Parent { get; private set; }

		public UIContext Context { get; private set; }

		public WidgetFactory WidgetFactory { get; private set; }

		public BrushFactory BrushFactory
		{
			get
			{
				return this.Context.BrushFactory;
			}
		}

		public SpriteData SpriteData
		{
			get
			{
				return this.Context.SpriteData;
			}
		}

		public PrefabExtensionContext PrefabExtensionContext
		{
			get
			{
				return this.WidgetFactory.PrefabExtensionContext;
			}
		}

		public WidgetCreationData(UIContext context, WidgetFactory widgetFactory, Widget parent)
		{
			this.Context = context;
			this.WidgetFactory = widgetFactory;
			this.Parent = parent;
			this._extensionData = new Dictionary<string, object>();
		}

		public WidgetCreationData(UIContext context, WidgetFactory widgetFactory)
		{
			this.Context = context;
			this.WidgetFactory = widgetFactory;
			this.Parent = null;
			this._extensionData = new Dictionary<string, object>();
		}

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

		public void AddExtensionData(string name, object data)
		{
			if (this._extensionData.ContainsKey(name))
			{
				this._extensionData[name] = data;
				return;
			}
			this._extensionData.Add(name, data);
		}

		public T GetExtensionData<T>(string name) where T : class
		{
			if (this._extensionData.ContainsKey(name))
			{
				return this._extensionData[name] as T;
			}
			return default(T);
		}

		public void AddExtensionData(object data)
		{
			this.AddExtensionData(data.GetType().Name, data);
		}

		public T GetExtensionData<T>() where T : class
		{
			return this.GetExtensionData<T>(typeof(T).Name);
		}

		private Dictionary<string, object> _extensionData;
	}
}
