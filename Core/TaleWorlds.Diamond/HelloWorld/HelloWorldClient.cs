using System;
using System.Threading.Tasks;
using TaleWorlds.Diamond.ClientApplication;

namespace TaleWorlds.Diamond.HelloWorld
{
	// Token: 0x02000050 RID: 80
	public class HelloWorldClient : SessionlessClient<HelloWorldClient>
	{
		// Token: 0x060001D6 RID: 470 RVA: 0x00005708 File Offset: 0x00003908
		public HelloWorldClient(DiamondClientApplication diamondClientApplication, ISessionlessClientDriverProvider<HelloWorldClient> driverProvider)
			: base(diamondClientApplication, driverProvider)
		{
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x00005712 File Offset: 0x00003912
		public void SendTestMessage(HelloWorldTestMessage message)
		{
			base.SendMessage(message);
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000571C File Offset: 0x0000391C
		public async Task<HelloWorldTestFunctionResult> CallTestFunction(HelloWorldTestFunctionMessage message)
		{
			return await base.CallFunction<HelloWorldTestFunctionResult>(message);
		}
	}
}
