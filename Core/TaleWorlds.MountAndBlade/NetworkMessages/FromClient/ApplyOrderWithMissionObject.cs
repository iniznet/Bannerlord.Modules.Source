using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class ApplyOrderWithMissionObject : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public ApplyOrderWithMissionObject(MissionObject missionObject)
		{
			this.MissionObject = missionObject;
		}

		public ApplyOrderWithMissionObject()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.Orders;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Apply order to MissionObject with ID: ",
				this.MissionObject.Id,
				" and with name ",
				this.MissionObject.GameEntity.Name
			});
		}
	}
}
