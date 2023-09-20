using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002EA RID: 746
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefineGameNetworkMessageTypeForMod : Attribute
	{
		// Token: 0x0600287C RID: 10364 RVA: 0x0009D32D File Offset: 0x0009B52D
		public DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType sendType)
		{
			this.SendType = sendType;
		}

		// Token: 0x04000F46 RID: 3910
		public readonly GameNetworkMessageSendType SendType;
	}
}
