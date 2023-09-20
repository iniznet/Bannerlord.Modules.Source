using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	// Token: 0x02000026 RID: 38
	public class NodeComparer : IComparer<ThumbnailCacheNode>
	{
		// Token: 0x060001A0 RID: 416 RVA: 0x0000EB12 File Offset: 0x0000CD12
		public int Compare(ThumbnailCacheNode x, ThumbnailCacheNode y)
		{
			return x.FrameNo.CompareTo(y.FrameNo);
		}
	}
}
