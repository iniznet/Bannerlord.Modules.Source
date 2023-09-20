using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CultureVoteServer : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public BasicCultureObject VotedCulture { get; private set; }

		public CultureVoteTypes VotedType { get; private set; }

		public CultureVoteServer()
		{
		}

		public CultureVoteServer(NetworkCommunicator peer, CultureVoteTypes type, BasicCultureObject culture)
		{
			this.Peer = peer;
			this.VotedType = type;
			this.VotedCulture = culture;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteIntToPacket((int)this.VotedType, CompressionMission.TeamSideCompressionInfo);
			MBReadOnlyList<BasicCultureObject> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>();
			GameNetworkMessage.WriteIntToPacket((this.VotedCulture == null) ? (-1) : objectTypeList.IndexOf(this.VotedCulture), CompressionBasic.CultureIndexCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.VotedType = (CultureVoteTypes)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref flag);
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.CultureIndexCompressionInfo, ref flag);
			if (flag)
			{
				MBReadOnlyList<BasicCultureObject> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>();
				this.VotedCulture = ((num < 0) ? null : objectTypeList[num]);
			}
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Culture ",
				this.VotedCulture.Name,
				" has been ",
				this.VotedType.ToString().ToLower(),
				(this.VotedType == CultureVoteTypes.Ban) ? "ned." : "ed."
			});
		}
	}
}
