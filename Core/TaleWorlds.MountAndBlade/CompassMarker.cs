using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000245 RID: 581
	public class CompassMarker
	{
		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x06001F7B RID: 8059 RVA: 0x0006FA8D File Offset: 0x0006DC8D
		// (set) Token: 0x06001F7C RID: 8060 RVA: 0x0006FA95 File Offset: 0x0006DC95
		public string Id { get; private set; }

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06001F7D RID: 8061 RVA: 0x0006FA9E File Offset: 0x0006DC9E
		// (set) Token: 0x06001F7E RID: 8062 RVA: 0x0006FAA6 File Offset: 0x0006DCA6
		public float Angle { get; private set; }

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06001F7F RID: 8063 RVA: 0x0006FAAF File Offset: 0x0006DCAF
		// (set) Token: 0x06001F80 RID: 8064 RVA: 0x0006FAB7 File Offset: 0x0006DCB7
		public bool IsPrimary { get; private set; }

		// Token: 0x06001F81 RID: 8065 RVA: 0x0006FAC0 File Offset: 0x0006DCC0
		public CompassMarker(string id, float angle, bool isPrimary)
		{
			this.Id = id;
			this.Angle = angle % 360f;
			this.IsPrimary = isPrimary;
		}
	}
}
