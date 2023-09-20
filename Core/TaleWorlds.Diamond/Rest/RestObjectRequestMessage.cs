using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000043 RID: 67
	[DataContract]
	[Serializable]
	public class RestObjectRequestMessage : RestRequestMessage
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000178 RID: 376 RVA: 0x00004F84 File Offset: 0x00003184
		// (set) Token: 0x06000179 RID: 377 RVA: 0x00004F8C File Offset: 0x0000318C
		[DataMember]
		public MessageType MessageType { get; private set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600017A RID: 378 RVA: 0x00004F95 File Offset: 0x00003195
		// (set) Token: 0x0600017B RID: 379 RVA: 0x00004F9D File Offset: 0x0000319D
		[DataMember]
		public SessionCredentials SessionCredentials { get; private set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00004FA6 File Offset: 0x000031A6
		// (set) Token: 0x0600017D RID: 381 RVA: 0x00004FAE File Offset: 0x000031AE
		[DataMember]
		public Message Message { get; private set; }

		// Token: 0x0600017E RID: 382 RVA: 0x00004FB7 File Offset: 0x000031B7
		public RestObjectRequestMessage()
		{
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00004FBF File Offset: 0x000031BF
		public RestObjectRequestMessage(SessionCredentials sessionCredentials, Message message, MessageType messageType)
		{
			this.Message = message;
			this.MessageType = messageType;
			this.SessionCredentials = sessionCredentials;
		}
	}
}
