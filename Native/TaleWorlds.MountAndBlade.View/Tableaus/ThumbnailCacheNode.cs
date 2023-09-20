using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	// Token: 0x02000025 RID: 37
	public class ThumbnailCacheNode
	{
		// Token: 0x0600019E RID: 414 RVA: 0x0000EAE6 File Offset: 0x0000CCE6
		public ThumbnailCacheNode()
		{
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000EAEE File Offset: 0x0000CCEE
		public ThumbnailCacheNode(string key, Texture value, int frameNo)
		{
			this.Key = key;
			this.Value = value;
			this.FrameNo = frameNo;
			this.ReferenceCount = 0;
		}

		// Token: 0x0400010A RID: 266
		public string Key;

		// Token: 0x0400010B RID: 267
		public Texture Value;

		// Token: 0x0400010C RID: 268
		public int FrameNo;

		// Token: 0x0400010D RID: 269
		public int ReferenceCount;
	}
}
