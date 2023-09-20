using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectAnimationAtChannel : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public int ChannelNo { get; private set; }

		public int AnimationIndex { get; private set; }

		public float AnimationSpeed { get; private set; }

		public SetMissionObjectAnimationAtChannel(MissionObjectId missionObjectId, int channelNo, int animationIndex, float animationSpeed)
		{
			this.MissionObjectId = missionObjectId;
			this.ChannelNo = channelNo;
			this.AnimationIndex = animationIndex;
			this.AnimationSpeed = animationSpeed;
		}

		public SetMissionObjectAnimationAtChannel()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.ChannelNo = (GameNetworkMessage.ReadBoolFromPacket(ref flag) ? 1 : 0);
			this.AnimationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationIndexCompressionInfo, ref flag);
			this.AnimationSpeed = (GameNetworkMessage.ReadBoolFromPacket(ref flag) ? GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationSpeedCompressionInfo, ref flag) : 1f);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
			GameNetworkMessage.WriteBoolToPacket(this.ChannelNo == 1);
			GameNetworkMessage.WriteIntToPacket(this.AnimationIndex, CompressionBasic.AnimationIndexCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.AnimationSpeed != 1f);
			if (this.AnimationSpeed != 1f)
			{
				GameNetworkMessage.WriteFloatToPacket(this.AnimationSpeed, CompressionBasic.AnimationSpeedCompressionInfo);
			}
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set animation: ", this.AnimationIndex, " on channel: ", this.ChannelNo, " of MissionObject with ID: ", this.MissionObjectId });
		}
	}
}
