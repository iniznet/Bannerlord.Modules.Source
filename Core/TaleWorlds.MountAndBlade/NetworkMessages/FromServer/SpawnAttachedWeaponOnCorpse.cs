using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SpawnAttachedWeaponOnCorpse : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public int AttachedIndex { get; private set; }

		public int ForcedIndex { get; private set; }

		public SpawnAttachedWeaponOnCorpse(Agent agent, int attachedIndex, int forcedIndex)
		{
			this.Agent = agent;
			this.AttachedIndex = attachedIndex;
			this.ForcedIndex = forcedIndex;
		}

		public SpawnAttachedWeaponOnCorpse()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.AttachedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponAttachmentIndexCompressionInfo, ref flag);
			this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket(this.AttachedIndex, CompressionMission.WeaponAttachmentIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"SpawnAttachedWeaponOnCorpse with index: ",
				this.Agent.Index,
				", and with ID: ",
				this.ForcedIndex
			});
		}
	}
}
