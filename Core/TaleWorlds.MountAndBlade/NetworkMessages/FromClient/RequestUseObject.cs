using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestUseObject : GameNetworkMessage
	{
		public MissionObjectId UsableMissionObjectId { get; private set; }

		public int UsedObjectPreferenceIndex { get; private set; }

		public RequestUseObject(MissionObjectId usableMissionObjectId, int usedObjectPreferenceIndex)
		{
			this.UsableMissionObjectId = usableMissionObjectId;
			this.UsedObjectPreferenceIndex = usedObjectPreferenceIndex;
		}

		public RequestUseObject()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableMissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.UsedObjectPreferenceIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.UsableMissionObjectId);
			GameNetworkMessage.WriteIntToPacket(this.UsedObjectPreferenceIndex, CompressionMission.WieldSlotCompressionInfo);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return "Request to use UsableMissionObject with ID: " + this.UsableMissionObjectId;
		}
	}
}
