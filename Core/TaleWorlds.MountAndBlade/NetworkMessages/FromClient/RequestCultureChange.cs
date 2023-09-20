using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000014 RID: 20
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestCultureChange : GameNetworkMessage
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00002BA2 File Offset: 0x00000DA2
		// (set) Token: 0x06000095 RID: 149 RVA: 0x00002BAA File Offset: 0x00000DAA
		public BasicCultureObject Culture { get; private set; }

		// Token: 0x06000096 RID: 150 RVA: 0x00002BB3 File Offset: 0x00000DB3
		public RequestCultureChange()
		{
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00002BBB File Offset: 0x00000DBB
		public RequestCultureChange(BasicCultureObject culture)
		{
			this.Culture = culture;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00002BCA File Offset: 0x00000DCA
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteObjectReferenceToPacket(this.Culture, CompressionBasic.GUIDCompressionInfo);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00002BDC File Offset: 0x00000DDC
		protected override bool OnRead()
		{
			bool flag = true;
			this.Culture = (BasicCultureObject)GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00002C08 File Offset: 0x00000E08
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00002C10 File Offset: 0x00000E10
		protected override string OnGetLogFormat()
		{
			return "Requested culture: " + this.Culture.Name;
		}
	}
}
