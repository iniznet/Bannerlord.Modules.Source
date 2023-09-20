using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestCultureChange : GameNetworkMessage
	{
		public BasicCultureObject Culture { get; private set; }

		public RequestCultureChange()
		{
		}

		public RequestCultureChange(BasicCultureObject culture)
		{
			this.Culture = culture;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteObjectReferenceToPacket(this.Culture, CompressionBasic.GUIDCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Culture = (BasicCultureObject)GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return "Requested culture: " + this.Culture.Name;
		}
	}
}
