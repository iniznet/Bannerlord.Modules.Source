using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.PrefabSystem
{
	// Token: 0x02000007 RID: 7
	public class GeneratedPrefabInstantiationResult
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002837 File Offset: 0x00000A37
		// (set) Token: 0x0600002F RID: 47 RVA: 0x0000283F File Offset: 0x00000A3F
		public Widget Root { get; private set; }

		// Token: 0x06000030 RID: 48 RVA: 0x00002848 File Offset: 0x00000A48
		public GeneratedPrefabInstantiationResult(Widget root)
		{
			this.Root = root;
			this._data = new Dictionary<string, object>();
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002862 File Offset: 0x00000A62
		public void AddData(string tag, object data)
		{
			this._data.Add(tag, data);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002874 File Offset: 0x00000A74
		public object GetExtensionData(string tag)
		{
			object obj;
			this._data.TryGetValue(tag, out obj);
			return obj;
		}

		// Token: 0x0400001D RID: 29
		private Dictionary<string, object> _data;
	}
}
