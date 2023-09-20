using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectVertexAnimationProgress : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public float Progress { get; private set; }

		public SetMissionObjectVertexAnimationProgress(MissionObjectId missionObjectId, float progress)
		{
			this.MissionObjectId = missionObjectId;
			this.Progress = progress;
		}

		public SetMissionObjectVertexAnimationProgress()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.Progress = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationProgressCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
			GameNetworkMessage.WriteFloatToPacket(this.Progress, CompressionBasic.AnimationProgressCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Set progress of Vertex Animation on MissionObject with ID: ", this.MissionObjectId, " to: ", this.Progress });
		}
	}
}
