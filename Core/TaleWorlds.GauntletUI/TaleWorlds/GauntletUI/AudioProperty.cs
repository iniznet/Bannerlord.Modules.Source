using System;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200002A RID: 42
	public class AudioProperty
	{
		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x0000DF64 File Offset: 0x0000C164
		// (set) Token: 0x060002E9 RID: 745 RVA: 0x0000DF6C File Offset: 0x0000C16C
		[Editor(false)]
		public string AudioName { get; set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x060002EA RID: 746 RVA: 0x0000DF75 File Offset: 0x0000C175
		// (set) Token: 0x060002EB RID: 747 RVA: 0x0000DF7D File Offset: 0x0000C17D
		[Editor(false)]
		public bool Delay { get; set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x060002EC RID: 748 RVA: 0x0000DF86 File Offset: 0x0000C186
		// (set) Token: 0x060002ED RID: 749 RVA: 0x0000DF8E File Offset: 0x0000C18E
		[Editor(false)]
		public float DelaySeconds { get; set; }

		// Token: 0x060002EE RID: 750 RVA: 0x0000DF97 File Offset: 0x0000C197
		public void FillFrom(AudioProperty audioProperty)
		{
			this.AudioName = audioProperty.AudioName;
			this.Delay = audioProperty.Delay;
			this.DelaySeconds = audioProperty.DelaySeconds;
		}
	}
}
