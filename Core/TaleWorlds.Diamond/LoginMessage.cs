using System;
using System.Runtime.Serialization;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000014 RID: 20
	[DataContract]
	[Serializable]
	public abstract class LoginMessage : Message
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000054 RID: 84 RVA: 0x000026D0 File Offset: 0x000008D0
		// (set) Token: 0x06000055 RID: 85 RVA: 0x000026D8 File Offset: 0x000008D8
		[DataMember]
		public PeerId PeerId { get; private set; }

		// Token: 0x06000056 RID: 86 RVA: 0x000026E1 File Offset: 0x000008E1
		protected LoginMessage(PeerId peerId)
		{
			this.PeerId = peerId;
		}
	}
}
