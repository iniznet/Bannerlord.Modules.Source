using System;
using System.Runtime.Serialization;
using TaleWorlds.Localization;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000015 RID: 21
	[DataContract]
	[Serializable]
	public sealed class LoginResult : FunctionResult
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000057 RID: 87 RVA: 0x000026F0 File Offset: 0x000008F0
		// (set) Token: 0x06000058 RID: 88 RVA: 0x000026F8 File Offset: 0x000008F8
		[DataMember]
		public PeerId PeerId { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00002701 File Offset: 0x00000901
		// (set) Token: 0x0600005A RID: 90 RVA: 0x00002709 File Offset: 0x00000909
		[DataMember]
		public SessionKey SessionKey { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00002712 File Offset: 0x00000912
		// (set) Token: 0x0600005C RID: 92 RVA: 0x0000271A File Offset: 0x0000091A
		[DataMember]
		public bool Successful { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00002723 File Offset: 0x00000923
		// (set) Token: 0x0600005E RID: 94 RVA: 0x0000272B File Offset: 0x0000092B
		[DataMember]
		public TextObject ErrorCode { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00002734 File Offset: 0x00000934
		// (set) Token: 0x06000060 RID: 96 RVA: 0x0000273C File Offset: 0x0000093C
		[DataMember]
		public LoginResultObject LoginResultObject { get; private set; }

		// Token: 0x06000061 RID: 97 RVA: 0x00002745 File Offset: 0x00000945
		public LoginResult(PeerId peerId, SessionKey sessionKey, LoginResultObject loginResultObject)
		{
			this.PeerId = peerId;
			this.SessionKey = sessionKey;
			this.Successful = true;
			this.ErrorCode = new TextObject("", null);
			this.LoginResultObject = loginResultObject;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000277A File Offset: 0x0000097A
		public LoginResult(PeerId peerId, SessionKey sessionKey)
			: this(peerId, sessionKey, null)
		{
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00002785 File Offset: 0x00000985
		public LoginResult(TextObject errorCode)
		{
			this.ErrorCode = errorCode;
			this.Successful = false;
		}
	}
}
