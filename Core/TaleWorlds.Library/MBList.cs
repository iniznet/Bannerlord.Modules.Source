using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	// Token: 0x02000062 RID: 98
	public class MBList<T> : MBReadOnlyList<T>
	{
		// Token: 0x0600030E RID: 782 RVA: 0x00009FD0 File Offset: 0x000081D0
		public MBList()
		{
		}

		// Token: 0x0600030F RID: 783 RVA: 0x00009FD8 File Offset: 0x000081D8
		public MBList(int capacity)
			: base(capacity)
		{
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00009FE1 File Offset: 0x000081E1
		public MBList(IEnumerable<T> collection)
			: base(collection)
		{
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00009FEA File Offset: 0x000081EA
		public MBList(List<T> collection)
			: base(collection)
		{
		}
	}
}
