using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public static class MB3ServerAPIHelper
	{
		public static ItemObject GetItemObject(this ItemData item)
		{
			return MBObjectManager.Instance.GetObject<ItemObject>(item.TypeId);
		}
	}
}
