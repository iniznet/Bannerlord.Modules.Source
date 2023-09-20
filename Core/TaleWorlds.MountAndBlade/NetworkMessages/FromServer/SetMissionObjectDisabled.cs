using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SetMissionObjectDisabled : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public SetMissionObjectDisabled(MissionObject missionObject)
		{
			this.MissionObject = missionObject;
		}

		public SetMissionObjectDisabled()
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
			return MultiplayerMessageFilter.MissionObjects;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Mission Object with ID: ",
				this.MissionObject.Id.Id,
				" and with name: ",
				this.MissionObject.GameEntity.Name,
				" has been disabled."
			});
		}
	}
}
