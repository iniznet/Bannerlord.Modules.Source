using System;

namespace TaleWorlds.Diamond
{
	internal sealed class ThreadedClientHandleMessageTask : ThreadedClientTask
	{
		public Message Message { get; private set; }

		public ThreadedClientHandleMessageTask(IClient client, Message message)
			: base(client)
		{
			this.Message = message;
		}

		public override void DoJob()
		{
			base.Client.HandleMessage(this.Message);
		}
	}
}
