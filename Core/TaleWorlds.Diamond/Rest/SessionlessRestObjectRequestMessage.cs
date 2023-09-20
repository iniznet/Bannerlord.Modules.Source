using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x0200003A RID: 58
	[DataContract]
	[Serializable]
	public class SessionlessRestObjectRequestMessage : SessionlessRestRequestMessage
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00003F31 File Offset: 0x00002131
		// (set) Token: 0x06000129 RID: 297 RVA: 0x00003F39 File Offset: 0x00002139
		[DataMember]
		public MessageType MessageType { get; private set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600012A RID: 298 RVA: 0x00003F42 File Offset: 0x00002142
		// (set) Token: 0x0600012B RID: 299 RVA: 0x00003F4A File Offset: 0x0000214A
		[DataMember]
		public Message Message { get; private set; }

		// Token: 0x0600012C RID: 300 RVA: 0x00003F53 File Offset: 0x00002153
		public SessionlessRestObjectRequestMessage()
		{
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00003F5B File Offset: 0x0000215B
		public SessionlessRestObjectRequestMessage(Message message, MessageType messageType)
		{
			this.Message = message;
			this.MessageType = messageType;
		}
	}
}
