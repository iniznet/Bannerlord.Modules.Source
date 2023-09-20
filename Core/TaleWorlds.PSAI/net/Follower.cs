using System;

namespace psai.net
{
	// Token: 0x0200001E RID: 30
	public struct Follower
	{
		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600020C RID: 524 RVA: 0x000090B1 File Offset: 0x000072B1
		// (set) Token: 0x0600020D RID: 525 RVA: 0x000090B9 File Offset: 0x000072B9
		public float compatibility { get; private set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600020E RID: 526 RVA: 0x000090C2 File Offset: 0x000072C2
		// (set) Token: 0x0600020F RID: 527 RVA: 0x000090CA File Offset: 0x000072CA
		public int snippetId { get; private set; }

		// Token: 0x06000210 RID: 528 RVA: 0x000090D3 File Offset: 0x000072D3
		public Follower(int id, float compatibility)
		{
			this.snippetId = id;
			this.compatibility = compatibility;
		}
	}
}
