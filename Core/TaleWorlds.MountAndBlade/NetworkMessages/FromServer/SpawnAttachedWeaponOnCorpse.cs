using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SpawnAttachedWeaponOnCorpse : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public int AttachedIndex { get; private set; }

		public int ForcedIndex { get; private set; }

		public SpawnAttachedWeaponOnCorpse(int agentIndex, int attachedIndex, int forcedIndex)
		{
			this.AgentIndex = agentIndex;
			this.AttachedIndex = attachedIndex;
			this.ForcedIndex = forcedIndex;
		}

		public SpawnAttachedWeaponOnCorpse()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.AttachedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponAttachmentIndexCompressionInfo, ref flag);
			this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteIntToPacket(this.AttachedIndex, CompressionMission.WeaponAttachmentIndexCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "SpawnAttachedWeaponOnCorpse with agent-index: ", this.AgentIndex, ", and with ID: ", this.ForcedIndex });
		}
	}
}
