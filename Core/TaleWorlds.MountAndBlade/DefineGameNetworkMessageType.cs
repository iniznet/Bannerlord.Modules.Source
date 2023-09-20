using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002EB RID: 747
	[AttributeUsage(AttributeTargets.Class)]
	internal sealed class DefineGameNetworkMessageType : Attribute
	{
		// Token: 0x0600287D RID: 10365 RVA: 0x0009D33C File Offset: 0x0009B53C
		public DefineGameNetworkMessageType(GameNetworkMessageSendType sendType)
		{
			this.SendType = sendType;
		}

		// Token: 0x04000F47 RID: 3911
		public readonly GameNetworkMessageSendType SendType;
	}
}
