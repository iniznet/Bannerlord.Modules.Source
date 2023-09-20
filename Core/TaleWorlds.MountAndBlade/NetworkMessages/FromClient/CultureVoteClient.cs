using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class CultureVoteClient : GameNetworkMessage
	{
		public BasicCultureObject VotedCulture { get; private set; }

		public CultureVoteTypes VotedType { get; private set; }

		public CultureVoteClient()
		{
		}

		public CultureVoteClient(CultureVoteTypes type, BasicCultureObject culture)
		{
			this.VotedType = type;
			this.VotedCulture = culture;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket((int)this.VotedType, CompressionMission.TeamSideCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>().IndexOf(this.VotedCulture), CompressionBasic.CultureIndexCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.VotedType = (CultureVoteTypes)GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref flag);
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.CultureIndexCompressionInfo, ref flag);
			if (flag)
			{
				this.VotedCulture = MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>()[num];
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
