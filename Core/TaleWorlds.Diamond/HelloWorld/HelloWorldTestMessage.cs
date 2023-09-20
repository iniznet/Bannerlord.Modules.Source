using System;

namespace TaleWorlds.Diamond.HelloWorld
{
	// Token: 0x02000051 RID: 81
	[Serializable]
	public class HelloWorldTestMessage : Message
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x00005769 File Offset: 0x00003969
		// (set) Token: 0x060001DA RID: 474 RVA: 0x00005771 File Offset: 0x00003971
		public string Message { get; private set; }

		// Token: 0x060001DB RID: 475 RVA: 0x0000577A File Offset: 0x0000397A
		public HelloWorldTestMessage()
		{
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00005782 File Offset: 0x00003982
		public HelloWorldTestMessage(string message)
		{
			this.Message = message;
		}
	}
}
