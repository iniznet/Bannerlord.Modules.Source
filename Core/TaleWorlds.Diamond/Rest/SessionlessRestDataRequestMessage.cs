using System;
using System.Runtime.Serialization;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000039 RID: 57
	[DataContract]
	[Serializable]
	public class SessionlessRestDataRequestMessage : SessionlessRestRequestMessage
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00003EDA File Offset: 0x000020DA
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00003EE2 File Offset: 0x000020E2
		[DataMember]
		public MessageType MessageType { get; private set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000123 RID: 291 RVA: 0x00003EEB File Offset: 0x000020EB
		// (set) Token: 0x06000124 RID: 292 RVA: 0x00003EF3 File Offset: 0x000020F3
		[DataMember]
		public byte[] MessageData { get; private set; }

		// Token: 0x06000125 RID: 293 RVA: 0x00003EFC File Offset: 0x000020FC
		public Message GetMessage()
		{
			return (Message)Common.DeserializeObject(this.MessageData);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00003F0E File Offset: 0x0000210E
		public SessionlessRestDataRequestMessage()
		{
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00003F16 File Offset: 0x00002116
		public SessionlessRestDataRequestMessage(Message message, MessageType messageType)
		{
			this.MessageData = Common.SerializeObject(message);
			this.MessageType = messageType;
		}
	}
}
