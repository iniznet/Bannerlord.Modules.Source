using System;

namespace TaleWorlds.Diamond.HelloWorld
{
	// Token: 0x02000053 RID: 83
	[Serializable]
	public class HelloWorldTestFunctionResult : FunctionResult
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x000057B9 File Offset: 0x000039B9
		// (set) Token: 0x060001E2 RID: 482 RVA: 0x000057C1 File Offset: 0x000039C1
		public string Message { get; private set; }

		// Token: 0x060001E3 RID: 483 RVA: 0x000057CA File Offset: 0x000039CA
		public HelloWorldTestFunctionResult()
		{
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x000057D2 File Offset: 0x000039D2
		public HelloWorldTestFunctionResult(string message)
		{
			this.Message = message;
		}
	}
}
