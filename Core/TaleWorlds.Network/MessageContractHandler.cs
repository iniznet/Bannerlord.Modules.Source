using System;

namespace TaleWorlds.Network
{
	internal abstract class MessageContractHandler
	{
		public abstract void Invoke(MessageContract messageContract);
	}
}
