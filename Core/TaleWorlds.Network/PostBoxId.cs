using System;

namespace TaleWorlds.Network
{
	// Token: 0x0200000C RID: 12
	public class PostBoxId : Attribute
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000042 RID: 66 RVA: 0x0000289E File Offset: 0x00000A9E
		// (set) Token: 0x06000043 RID: 67 RVA: 0x000028A6 File Offset: 0x00000AA6
		public string Id { get; private set; }

		// Token: 0x06000044 RID: 68 RVA: 0x000028AF File Offset: 0x00000AAF
		public PostBoxId(string id)
		{
			this.Id = id;
		}
	}
}
