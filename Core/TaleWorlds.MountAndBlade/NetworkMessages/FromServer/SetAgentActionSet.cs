using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetAgentActionSet : GameNetworkMessage
	{
		public int AgentIndex { get; private set; }

		public MBActionSet ActionSet { get; private set; }

		public int NumPaces { get; private set; }

		public int MonsterUsageSetIndex { get; private set; }

		public float WalkingSpeedLimit { get; private set; }

		public float CrouchWalkingSpeedLimit { get; private set; }

		public float StepSize { get; private set; }

		public SetAgentActionSet(int agentIndex, AnimationSystemData animationSystemData)
		{
			this.AgentIndex = agentIndex;
			this.ActionSet = animationSystemData.ActionSet;
			this.NumPaces = animationSystemData.NumPaces;
			this.MonsterUsageSetIndex = animationSystemData.MonsterUsageSetIndex;
			this.WalkingSpeedLimit = animationSystemData.WalkingSpeedLimit;
			this.CrouchWalkingSpeedLimit = animationSystemData.CrouchWalkingSpeedLimit;
			this.StepSize = animationSystemData.StepSize;
		}

		public SetAgentActionSet()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.ActionSet = GameNetworkMessage.ReadActionSetReferenceFromPacket(CompressionMission.ActionSetCompressionInfo, ref flag);
			this.NumPaces = GameNetworkMessage.ReadIntFromPacket(CompressionMission.NumberOfPacesCompressionInfo, ref flag);
			this.MonsterUsageSetIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MonsterUsageSetCompressionInfo, ref flag);
			this.WalkingSpeedLimit = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.WalkingSpeedLimitCompressionInfo, ref flag);
			this.CrouchWalkingSpeedLimit = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.WalkingSpeedLimitCompressionInfo, ref flag);
			this.StepSize = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.StepSizeCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteAgentIndexToPacket(this.AgentIndex);
			GameNetworkMessage.WriteActionSetReferenceToPacket(this.ActionSet, CompressionMission.ActionSetCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.NumPaces, CompressionMission.NumberOfPacesCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.MonsterUsageSetIndex, CompressionMission.MonsterUsageSetCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.WalkingSpeedLimit, CompressionMission.WalkingSpeedLimitCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.CrouchWalkingSpeedLimit, CompressionMission.WalkingSpeedLimitCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.StepSize, CompressionMission.StepSizeCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.AgentAnimations;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set ActionSet: ", this.ActionSet, " on agent with agent-index: ", this.AgentIndex });
		}
	}
}
