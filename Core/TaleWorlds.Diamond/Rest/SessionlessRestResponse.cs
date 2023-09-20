using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x0200003C RID: 60
	[DataContract]
	[Serializable]
	public sealed class SessionlessRestResponse : RestData
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600012F RID: 303 RVA: 0x00003F79 File Offset: 0x00002179
		// (set) Token: 0x06000130 RID: 304 RVA: 0x00003F81 File Offset: 0x00002181
		[DataMember]
		public bool Successful { get; private set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00003F8A File Offset: 0x0000218A
		// (set) Token: 0x06000132 RID: 306 RVA: 0x00003F92 File Offset: 0x00002192
		[DataMember]
		public string SuccessfulReason { get; private set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00003F9B File Offset: 0x0000219B
		// (set) Token: 0x06000134 RID: 308 RVA: 0x00003FA3 File Offset: 0x000021A3
		[DataMember]
		public RestFunctionResult FunctionResult { get; set; }

		// Token: 0x06000136 RID: 310 RVA: 0x00003FB4 File Offset: 0x000021B4
		public void SetSuccessful(bool successful, string succressfulReason)
		{
			this.Successful = successful;
			this.SuccessfulReason = succressfulReason;
		}
	}
}
