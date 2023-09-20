using System;

namespace TaleWorlds.Network
{
	internal class MessageContractCreator<T> : MessageContractCreator where T : MessageContract, new()
	{
		public override MessageContract Invoke()
		{
			return new T();
		}
	}
}
