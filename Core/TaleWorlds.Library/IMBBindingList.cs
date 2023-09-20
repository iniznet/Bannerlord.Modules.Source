using System;
using System.Collections;

namespace TaleWorlds.Library
{
	// Token: 0x02000034 RID: 52
	public interface IMBBindingList : IList, ICollection, IEnumerable
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000199 RID: 409
		// (remove) Token: 0x0600019A RID: 410
		event ListChangedEventHandler ListChanged;
	}
}
