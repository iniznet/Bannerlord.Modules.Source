using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000035 RID: 53
	[DataContract]
	[Serializable]
	public class RestObjectResponseMessage : RestResponseMessage
	{
		// Token: 0x06000104 RID: 260 RVA: 0x000038F6 File Offset: 0x00001AF6
		public override Message GetMessage()
		{
			return this._message;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000038FE File Offset: 0x00001AFE
		public RestObjectResponseMessage()
		{
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00003906 File Offset: 0x00001B06
		public RestObjectResponseMessage(Message message)
		{
			this._message = message;
		}

		// Token: 0x04000045 RID: 69
		[DataMember]
		private Message _message;
	}
}
