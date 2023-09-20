using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond
{
	// Token: 0x0200001C RID: 28
	[DataContract]
	[Serializable]
	public sealed class SessionCredentials
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00002B43 File Offset: 0x00000D43
		// (set) Token: 0x0600007A RID: 122 RVA: 0x00002B4B File Offset: 0x00000D4B
		[DataMember]
		public PeerId PeerId { get; private set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00002B54 File Offset: 0x00000D54
		// (set) Token: 0x0600007C RID: 124 RVA: 0x00002B5C File Offset: 0x00000D5C
		[DataMember]
		public SessionKey SessionKey { get; private set; }

		// Token: 0x0600007D RID: 125 RVA: 0x00002B65 File Offset: 0x00000D65
		public SessionCredentials(PeerId peerId, SessionKey sessionKey)
		{
			this.PeerId = peerId;
			this.SessionKey = sessionKey;
		}
	}
}
