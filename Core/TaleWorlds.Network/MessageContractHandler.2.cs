using System;

namespace TaleWorlds.Network
{
	internal class MessageContractHandler<T> : MessageContractHandler where T : MessageContract
	{
		public MessageContractHandler(MessageContractHandlerDelegate<T> method)
		{
			this._method = method;
		}

		public override void Invoke(MessageContract messageContract)
		{
			this._method(messageContract as T);
		}

		private MessageContractHandlerDelegate<T> _method;
	}
}
