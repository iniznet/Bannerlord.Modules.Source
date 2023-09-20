using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	// Token: 0x0200002F RID: 47
	public class GenericComparer<T> : Comparer<T> where T : IComparable<T>
	{
		// Token: 0x0600016E RID: 366 RVA: 0x00005F89 File Offset: 0x00004189
		public override int Compare(T x, T y)
		{
			if (x != null)
			{
				if (y != null)
				{
					return x.CompareTo(y);
				}
				return 1;
			}
			else
			{
				if (y != null)
				{
					return -1;
				}
				return 0;
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00005FB7 File Offset: 0x000041B7
		public override bool Equals(object obj)
		{
			return obj is GenericComparer<T>;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00005FC2 File Offset: 0x000041C2
		public override int GetHashCode()
		{
			return base.GetType().Name.GetHashCode();
		}
	}
}
