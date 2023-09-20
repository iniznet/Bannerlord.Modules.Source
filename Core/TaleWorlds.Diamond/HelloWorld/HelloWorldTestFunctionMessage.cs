using System;

namespace TaleWorlds.Diamond.HelloWorld
{
	// Token: 0x02000052 RID: 82
	[Serializable]
	public class HelloWorldTestFunctionMessage : Message
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001DD RID: 477 RVA: 0x00005791 File Offset: 0x00003991
		// (set) Token: 0x060001DE RID: 478 RVA: 0x00005799 File Offset: 0x00003999
		public string Message { get; private set; }

		// Token: 0x060001DF RID: 479 RVA: 0x000057A2 File Offset: 0x000039A2
		public HelloWorldTestFunctionMessage()
		{
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x000057AA File Offset: 0x000039AA
		public HelloWorldTestFunctionMessage(string message)
		{
			this.Message = message;
		}
	}
}
