using System;
using System.Threading.Tasks;
using TaleWorlds.Diamond.ClientApplication;

namespace TaleWorlds.Diamond.HelloWorld
{
	public class HelloWorldClient : SessionlessClient<HelloWorldClient>
	{
		public HelloWorldClient(DiamondClientApplication diamondClientApplication, ISessionlessClientDriverProvider<HelloWorldClient> driverProvider)
			: base(diamondClientApplication, driverProvider)
		{
		}

		public void SendTestMessage(HelloWorldTestMessage message)
		{
			base.SendMessage(message);
		}

		public async Task<HelloWorldTestFunctionResult> CallTestFunction(HelloWorldTestFunctionMessage message)
		{
			return await base.CallFunction<HelloWorldTestFunctionResult>(message);
		}
	}
}
