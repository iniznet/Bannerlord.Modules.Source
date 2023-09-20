using System;
using System.Runtime.Serialization;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000042 RID: 66
	[DataContract]
	[Serializable]
	public class RestDataRequestMessage : RestRequestMessage
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00004EBF File Offset: 0x000030BF
		// (set) Token: 0x0600016D RID: 365 RVA: 0x00004EC7 File Offset: 0x000030C7
		[DataMember]
		public MessageType MessageType { get; private set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600016E RID: 366 RVA: 0x00004ED0 File Offset: 0x000030D0
		// (set) Token: 0x0600016F RID: 367 RVA: 0x00004ED8 File Offset: 0x000030D8
		[DataMember]
		public SessionCredentials SessionCredentials { get; private set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000170 RID: 368 RVA: 0x00004EE1 File Offset: 0x000030E1
		// (set) Token: 0x06000171 RID: 369 RVA: 0x00004EE9 File Offset: 0x000030E9
		[DataMember]
		public byte[] MessageData { get; private set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000172 RID: 370 RVA: 0x00004EF2 File Offset: 0x000030F2
		// (set) Token: 0x06000173 RID: 371 RVA: 0x00004EFA File Offset: 0x000030FA
		[DataMember]
		public string MessageName { get; private set; }

		// Token: 0x06000174 RID: 372 RVA: 0x00004F03 File Offset: 0x00003103
		public Message GetMessage()
		{
			return (Message)Common.DeserializeObject(this.MessageData);
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00004F15 File Offset: 0x00003115
		public RestDataRequestMessage()
		{
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00004F1D File Offset: 0x0000311D
		public RestDataRequestMessage(SessionCredentials sessionCredentials, Message message, MessageType messageType)
		{
			this.MessageData = Common.SerializeObject(message);
			this.MessageType = messageType;
			this.SessionCredentials = sessionCredentials;
			this.MessageName = message.GetType().Name;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00004F50 File Offset: 0x00003150
		public override string ToString()
		{
			return string.Concat(new object[] { "Rest Data Request Message: ", this.MessageName, "-", this.MessageType });
		}
	}
}
