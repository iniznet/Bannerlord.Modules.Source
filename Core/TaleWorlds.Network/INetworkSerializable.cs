using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000014 RID: 20
	public interface INetworkSerializable
	{
		// Token: 0x06000071 RID: 113
		void SerializeToNetworkMessage(INetworkMessageWriter networkMessage);

		// Token: 0x06000072 RID: 114
		void DeserializeFromNetworkMessage(INetworkMessageReader networkMessage);
	}
}
