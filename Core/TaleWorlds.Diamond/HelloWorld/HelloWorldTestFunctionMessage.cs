using System;

namespace TaleWorlds.Diamond.HelloWorld
{
	[Serializable]
	public class HelloWorldTestFunctionMessage : Message
	{
		public string Message { get; private set; }

		public HelloWorldTestFunctionMessage()
		{
		}

		public HelloWorldTestFunctionMessage(string message)
		{
			this.Message = message;
		}
	}
}
