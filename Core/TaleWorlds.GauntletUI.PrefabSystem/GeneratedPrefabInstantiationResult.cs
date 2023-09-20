using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	public class GeneratedPrefabInstantiationResult
	{
		public Widget Root { get; private set; }

		public GeneratedPrefabInstantiationResult(Widget root)
		{
			this.Root = root;
			this._data = new Dictionary<string, object>();
		}

		public void AddData(string tag, object data)
		{
			this._data.Add(tag, data);
		}

		public object GetExtensionData(string tag)
		{
			object obj;
			this._data.TryGetValue(tag, out obj);
			return obj;
		}

		private Dictionary<string, object> _data;
	}
}
