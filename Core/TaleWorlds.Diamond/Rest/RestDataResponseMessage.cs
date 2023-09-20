using System;
using System.Runtime.Serialization;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000032 RID: 50
	[DataContract]
	[Serializable]
	public class RestDataResponseMessage : RestResponseMessage
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00003890 File Offset: 0x00001A90
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00003898 File Offset: 0x00001A98
		[DataMember]
		public byte[] MessageData { get; private set; }

		// Token: 0x060000FC RID: 252 RVA: 0x000038A1 File Offset: 0x00001AA1
		public override Message GetMessage()
		{
			return (Message)Common.DeserializeObject(this.MessageData);
		}

		// Token: 0x060000FD RID: 253 RVA: 0x000038B3 File Offset: 0x00001AB3
		public RestDataResponseMessage()
		{
		}

		// Token: 0x060000FE RID: 254 RVA: 0x000038BB File Offset: 0x00001ABB
		public RestDataResponseMessage(Message message)
		{
			this.MessageData = Common.SerializeObject(message);
		}
	}
}
