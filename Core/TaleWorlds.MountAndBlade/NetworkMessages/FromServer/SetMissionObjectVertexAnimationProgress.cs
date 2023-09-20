using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectVertexAnimationProgress : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public float Progress { get; private set; }

		public SetMissionObjectVertexAnimationProgress(MissionObject missionObject, float progress)
		{
			this.MissionObject = missionObject;
			this.Progress = progress;
		}

		public SetMissionObjectVertexAnimationProgress()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Progress = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationProgressCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteFloatToPacket(this.Progress, CompressionBasic.AnimationProgressCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set progress of Vertex Animation on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name,
				" to: ",
				this.Progress
			});
		}
	}
}
