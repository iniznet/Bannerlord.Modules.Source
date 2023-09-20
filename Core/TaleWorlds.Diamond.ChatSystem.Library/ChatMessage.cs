using System;

namespace TaleWorlds.Diamond.ChatSystem.Library
{
	// Token: 0x02000005 RID: 5
	public class ChatMessage
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002349 File Offset: 0x00000549
		// (set) Token: 0x06000012 RID: 18 RVA: 0x00002351 File Offset: 0x00000551
		public string Name { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000235A File Offset: 0x0000055A
		// (set) Token: 0x06000014 RID: 20 RVA: 0x00002362 File Offset: 0x00000562
		public Guid RoomId { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000015 RID: 21 RVA: 0x0000236B File Offset: 0x0000056B
		// (set) Token: 0x06000016 RID: 22 RVA: 0x00002373 File Offset: 0x00000573
		public string Text { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000017 RID: 23 RVA: 0x0000237C File Offset: 0x0000057C
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002384 File Offset: 0x00000584
		public MessageType Type { get; set; }
	}
}
