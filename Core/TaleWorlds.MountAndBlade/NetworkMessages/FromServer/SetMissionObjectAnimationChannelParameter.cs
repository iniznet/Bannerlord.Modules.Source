using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectAnimationChannelParameter : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public int ChannelNo { get; private set; }

		public float Parameter { get; private set; }

		public SetMissionObjectAnimationChannelParameter(MissionObject missionObject, int channelNo, float parameter)
		{
			this.MissionObject = missionObject;
			this.ChannelNo = channelNo;
			this.Parameter = parameter;
		}

		public SetMissionObjectAnimationChannelParameter()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			bool flag2 = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			if (flag)
			{
				this.ChannelNo = (flag2 ? 1 : 0);
			}
			this.Parameter = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationProgressCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteBoolToPacket(this.ChannelNo == 1);
			GameNetworkMessage.WriteFloatToPacket(this.Parameter, CompressionBasic.AnimationProgressCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set animation parameter: ",
				this.Parameter,
				" on channel: ",
				this.ChannelNo,
				" of MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
