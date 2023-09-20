using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SynchronizeMissionObject : GameNetworkMessage
	{
		public MissionObjectId MissionObjectId { get; private set; }

		public int RecordTypeIndex { get; private set; }

		public ValueTuple<BaseSynchedMissionObjectReadableRecord, ISynchedMissionObjectReadableRecord> RecordPair { get; private set; }

		public SynchronizeMissionObject(SynchedMissionObject synchedMissionObject)
		{
			this._synchedMissionObject = synchedMissionObject;
			this.MissionObjectId = synchedMissionObject.Id;
			this.RecordTypeIndex = GameNetwork.GetSynchedMissionObjectReadableRecordIndexFromType(synchedMissionObject.GetType());
		}

		public SynchronizeMissionObject()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectIdToPacket(this.MissionObjectId);
			GameNetworkMessage.WriteIntToPacket(this.RecordTypeIndex, CompressionMission.SynchedMissionObjectReadableRecordTypeIndex);
			this._synchedMissionObject.WriteToNetwork();
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
			this.RecordTypeIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SynchedMissionObjectReadableRecordTypeIndex, ref flag);
			this.RecordPair = BaseSynchedMissionObjectReadableRecord.CreateFromNetworkWithTypeIndex(this.RecordTypeIndex);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return "Synchronize MissionObject with Id: " + this.MissionObjectId;
		}

		private SynchedMissionObject _synchedMissionObject;
	}
}
