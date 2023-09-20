using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000302 RID: 770
	public class IntermissionVoteItem
	{
		// Token: 0x17000765 RID: 1893
		// (get) Token: 0x06002998 RID: 10648 RVA: 0x000A15D3 File Offset: 0x0009F7D3
		// (set) Token: 0x06002999 RID: 10649 RVA: 0x000A15DB File Offset: 0x0009F7DB
		public int VoteCount { get; private set; }

		// Token: 0x0600299A RID: 10650 RVA: 0x000A15E4 File Offset: 0x0009F7E4
		public IntermissionVoteItem(string id, int index)
		{
			this.Id = id;
			this.Index = index;
			this.VoteCount = 0;
		}

		// Token: 0x0600299B RID: 10651 RVA: 0x000A1601 File Offset: 0x0009F801
		public void SetVoteCount(int voteCount)
		{
			this.VoteCount = voteCount;
		}

		// Token: 0x0600299C RID: 10652 RVA: 0x000A160A File Offset: 0x0009F80A
		public void IncreaseVoteCount(int incrementAmount)
		{
			this.VoteCount += incrementAmount;
		}

		// Token: 0x04000FD5 RID: 4053
		public readonly string Id;

		// Token: 0x04000FD6 RID: 4054
		public readonly int Index;
	}
}
