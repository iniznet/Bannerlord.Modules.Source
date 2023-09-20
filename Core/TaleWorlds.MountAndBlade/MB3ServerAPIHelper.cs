using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000323 RID: 803
	public static class MB3ServerAPIHelper
	{
		// Token: 0x06002B83 RID: 11139 RVA: 0x000A9B24 File Offset: 0x000A7D24
		public static ItemObject GetItemObject(this ItemData item)
		{
			return MBObjectManager.Instance.GetObject<ItemObject>(item.TypeId);
		}
	}
}
