using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x0200003D RID: 61
	[DataContract]
	[Serializable]
	public class AliveMessage : RestRequestMessage
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000137 RID: 311 RVA: 0x00003FC4 File Offset: 0x000021C4
		// (set) Token: 0x06000138 RID: 312 RVA: 0x00003FCC File Offset: 0x000021CC
		[DataMember]
		public SessionCredentials SessionCredentials { get; private set; }

		// Token: 0x06000139 RID: 313 RVA: 0x00003FD5 File Offset: 0x000021D5
		public AliveMessage()
		{
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00003FDD File Offset: 0x000021DD
		public AliveMessage(SessionCredentials sessionCredentials)
		{
			this.SessionCredentials = sessionCredentials;
		}
	}
}
