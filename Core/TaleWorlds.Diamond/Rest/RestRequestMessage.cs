using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000045 RID: 69
	[DataContract]
	[Serializable]
	public abstract class RestRequestMessage : RestData
	{
		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000180 RID: 384 RVA: 0x00004FDC File Offset: 0x000031DC
		// (set) Token: 0x06000181 RID: 385 RVA: 0x00004FE4 File Offset: 0x000031E4
		[DataMember]
		public byte[] UserCertificate { get; set; }
	}
}
