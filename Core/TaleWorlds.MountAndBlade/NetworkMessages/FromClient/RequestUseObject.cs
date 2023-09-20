using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestUseObject : GameNetworkMessage
	{
		public UsableMissionObject UsableGameObject { get; private set; }

		public int UsedObjectPreferenceIndex { get; private set; }

		public RequestUseObject(UsableMissionObject usableGameObject, int usedObjectPreferenceIndex)
		{
			this.UsableGameObject = usableGameObject;
			this.UsedObjectPreferenceIndex = usedObjectPreferenceIndex;
		}

		public RequestUseObject()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableGameObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as UsableMissionObject;
			this.UsedObjectPreferenceIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableGameObject);
			GameNetworkMessage.WriteIntToPacket(this.UsedObjectPreferenceIndex, CompressionMission.WieldSlotCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Request to use UsableMissionObject with ID: ",
				this.UsableGameObject.Id,
				" and with name: ",
				this.UsableGameObject.GameEntity.Name
			});
		}
	}
}
