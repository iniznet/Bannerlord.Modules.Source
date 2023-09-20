using System;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000004 RID: 4
	// (Invoke) Token: 0x0600001C RID: 28
	public delegate void ClientMessageHandler<TMessage>(TMessage message) where TMessage : Message;
}
