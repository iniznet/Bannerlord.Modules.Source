using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001C1 RID: 449
	public struct MBMusicTrack
	{
		// Token: 0x060019DC RID: 6620 RVA: 0x0005C16C File Offset: 0x0005A36C
		public MBMusicTrack(MBMusicTrack obj)
		{
			this.index = obj.index;
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x0005C17A File Offset: 0x0005A37A
		internal MBMusicTrack(int i)
		{
			this.index = i;
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x060019DE RID: 6622 RVA: 0x0005C183 File Offset: 0x0005A383
		private bool IsValid
		{
			get
			{
				return this.index >= 0;
			}
		}

		// Token: 0x060019DF RID: 6623 RVA: 0x0005C191 File Offset: 0x0005A391
		public bool Equals(MBMusicTrack obj)
		{
			return this.index == obj.index;
		}

		// Token: 0x060019E0 RID: 6624 RVA: 0x0005C1A1 File Offset: 0x0005A3A1
		public override int GetHashCode()
		{
			return this.index;
		}

		// Token: 0x04000815 RID: 2069
		private int index;
	}
}
