using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AddPrefabComponentToAgentBone : GameNetworkMessage
	{
		public Agent Agent { get; private set; }

		public string PrefabName { get; private set; }

		public sbyte BoneIndex { get; private set; }

		public AddPrefabComponentToAgentBone(Agent agent, string prefabName, sbyte boneIndex)
		{
			this.Agent = agent;
			this.PrefabName = prefabName;
			this.BoneIndex = boneIndex;
		}

		public AddPrefabComponentToAgentBone()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.PrefabName = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.BoneIndex = (sbyte)GameNetworkMessage.ReadIntFromPacket(CompressionMission.BoneIndexCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteStringToPacket(this.PrefabName);
			GameNetworkMessage.WriteIntToPacket((int)this.BoneIndex, CompressionMission.BoneIndexCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Add prefab component: ",
				this.PrefabName,
				" on bone with index: ",
				this.BoneIndex,
				" on agent with name: ",
				this.Agent.Name,
				" and agent-index: ",
				this.Agent.Index
			});
		}
	}
}
