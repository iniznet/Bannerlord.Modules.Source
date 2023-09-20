using System;
using TaleWorlds.Library;
using TaleWorlds.Network;

namespace TaleWorlds.Diamond.Socket
{
	// Token: 0x0200002E RID: 46
	[MessageId(1)]
	public class SocketMessage : MessageContract
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00003696 File Offset: 0x00001896
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x0000369E File Offset: 0x0000189E
		public Message Message { get; private set; }

		// Token: 0x060000E6 RID: 230 RVA: 0x000036A7 File Offset: 0x000018A7
		public SocketMessage()
		{
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000036AF File Offset: 0x000018AF
		public SocketMessage(Message message)
		{
			this.Message = message;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x000036C0 File Offset: 0x000018C0
		public override void SerializeToNetworkMessage(INetworkMessageWriter networkMessage)
		{
			byte[] array = Common.SerializeObject(this.Message);
			networkMessage.Write(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				networkMessage.Write(array[i]);
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x000036FC File Offset: 0x000018FC
		public override void DeserializeFromNetworkMessage(INetworkMessageReader networkMessage)
		{
			byte[] array = new byte[networkMessage.ReadInt32()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = networkMessage.ReadByte();
			}
			this.Message = (Message)Common.DeserializeObject(array);
		}
	}
}
