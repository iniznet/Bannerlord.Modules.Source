using System;
using System.IO;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class CustomWidgetType
	{
		public WidgetTemplate WidgetTemplate
		{
			get
			{
				return this.WidgetPrefab.RootTemplate;
			}
		}

		public WidgetPrefab WidgetPrefab { get; private set; }

		public WidgetFactory WidgetFactory { get; private set; }

		public string Name { get; private set; }

		public string FullPath
		{
			get
			{
				return this._resourcesPath + this.Name + ".xml";
			}
		}

		public CustomWidgetType(WidgetFactory widgetFactory, string resourcesPath, string name)
		{
			this._resourcesPath = resourcesPath;
			this.Name = name;
			this.WidgetFactory = widgetFactory;
			this.Load();
			this._lastWriteTime = File.GetLastWriteTime(this.FullPath);
		}

		public bool CheckForUpdate()
		{
			DateTime lastWriteTime = File.GetLastWriteTime(this.FullPath);
			if (this._lastWriteTime != lastWriteTime)
			{
				try
				{
					this.Load();
					this._lastWriteTime = lastWriteTime;
					return true;
				}
				catch
				{
				}
				return false;
			}
			return false;
		}

		private void Load()
		{
			this.WidgetPrefab = WidgetPrefab.LoadFrom(this.WidgetFactory.PrefabExtensionContext, this.WidgetFactory.WidgetAttributeContext, this.FullPath);
		}

		private DateTime _lastWriteTime = DateTime.MinValue;

		private string _resourcesPath;
	}
}
