using System;

namespace TaleWorlds.MountAndBlade
{
	[AttributeUsage(AttributeTargets.Class)]
	internal sealed class DefineGameNetworkMessageType : Attribute
	{
		public DefineGameNetworkMessageType(GameNetworkMessageSendType sendType)
		{
			this.SendType = sendType;
		}

		public readonly GameNetworkMessageSendType SendType;
	}
}
