using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000016 RID: 22
	public class MessageId : Attribute
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600007A RID: 122 RVA: 0x0000307A File Offset: 0x0000127A
		// (set) Token: 0x0600007B RID: 123 RVA: 0x00003082 File Offset: 0x00001282
		public byte Id { get; private set; }

		// Token: 0x0600007C RID: 124 RVA: 0x0000308B File Offset: 0x0000128B
		public MessageId(byte id)
		{
			this.Id = id;
		}
	}
}
