using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001C8 RID: 456
	public struct MBSoundTrack
	{
		// Token: 0x060019F8 RID: 6648 RVA: 0x0005C3DF File Offset: 0x0005A5DF
		internal MBSoundTrack(int i)
		{
			this.index = i;
		}

		// Token: 0x060019F9 RID: 6649 RVA: 0x0005C3E8 File Offset: 0x0005A5E8
		public bool Equals(MBSoundTrack a)
		{
			return this.index == a.index;
		}

		// Token: 0x060019FA RID: 6650 RVA: 0x0005C3F8 File Offset: 0x0005A5F8
		public override int GetHashCode()
		{
			return this.index;
		}

		// Token: 0x0400082D RID: 2093
		private int index;
	}
}
