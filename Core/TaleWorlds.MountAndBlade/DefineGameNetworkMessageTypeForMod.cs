using System;

namespace TaleWorlds.MountAndBlade
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefineGameNetworkMessageTypeForMod : Attribute
	{
		public DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType sendType)
		{
			this.SendType = sendType;
		}

		public readonly GameNetworkMessageSendType SendType;
	}
}
