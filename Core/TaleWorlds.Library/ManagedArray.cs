using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200005A RID: 90
	[Serializable]
	public struct ManagedArray
	{
		// Token: 0x06000288 RID: 648 RVA: 0x0000739C File Offset: 0x0000559C
		public ManagedArray(IntPtr array, int length)
		{
			this.Array = array;
			this.Length = length;
		}

		// Token: 0x040000F1 RID: 241
		internal IntPtr Array;

		// Token: 0x040000F2 RID: 242
		internal int Length;
	}
}
