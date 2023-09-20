using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	// Token: 0x02000067 RID: 103
	public class MBReadOnlyList<T> : List<T>
	{
		// Token: 0x06000392 RID: 914 RVA: 0x0000B418 File Offset: 0x00009618
		public MBReadOnlyList()
		{
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0000B420 File Offset: 0x00009620
		public MBReadOnlyList(int capacity)
			: base(capacity)
		{
		}

		// Token: 0x06000394 RID: 916 RVA: 0x0000B429 File Offset: 0x00009629
		public MBReadOnlyList(IEnumerable<T> collection)
			: base(collection)
		{
		}
	}
}
