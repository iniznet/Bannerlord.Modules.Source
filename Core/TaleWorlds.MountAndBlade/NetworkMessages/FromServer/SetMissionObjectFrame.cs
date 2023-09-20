using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectFrame : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public MatrixFrame Frame { get; private set; }

		public SetMissionObjectFrame(MissionObject missionObject, ref MatrixFrame frame)
		{
			this.MissionObject = missionObject;
			this.Frame = frame;
		}

		public SetMissionObjectFrame()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Set Frame on MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
