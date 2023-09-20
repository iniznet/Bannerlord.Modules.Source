using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectAnimationAtChannel : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public int ChannelNo { get; private set; }

		public int AnimationIndex { get; private set; }

		public float AnimationSpeed { get; private set; }

		public SetMissionObjectAnimationAtChannel(MissionObject missionObject, int channelNo, int animationIndex, float animationSpeed)
		{
			this.MissionObject = missionObject;
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
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.ChannelNo = (GameNetworkMessage.ReadBoolFromPacket(ref flag) ? 1 : 0);
			this.AnimationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationIndexCompressionInfo, ref flag);
			this.AnimationSpeed = (GameNetworkMessage.ReadBoolFromPacket(ref flag) ? GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationSpeedCompressionInfo, ref flag) : 1f);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
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
			return string.Concat(new object[]
			{
				"Set animation: ",
				this.AnimationIndex,
				" on channel: ",
				this.ChannelNo,
				" of MissionObject with ID: ",
				this.MissionObject.Id.Id,
				this.MissionObject.Id.CreatedAtRuntime ? " (Created at runtime)" : "",
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
