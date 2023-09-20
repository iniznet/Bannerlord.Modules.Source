using System;

namespace TaleWorlds.Network
{
	// Token: 0x0200001D RID: 29
	internal class MessageContractHandler<T> : MessageContractHandler where T : MessageContract
	{
		// Token: 0x0600009E RID: 158 RVA: 0x00003357 File Offset: 0x00001557
		public MessageContractHandler(MessageContractHandlerDelegate<T> method)
		{
			this._method = method;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00003366 File Offset: 0x00001566
		public override void Invoke(MessageContract messageContract)
		{
			this._method(messageContract as T);
		}

		// Token: 0x0400003A RID: 58
		private MessageContractHandlerDelegate<T> _method;
	}
}
