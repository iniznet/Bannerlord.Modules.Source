using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SynchronizeMissionObject : GameNetworkMessage
	{
		public SynchedMissionObject MissionObject { get; private set; }

		public SynchronizeMissionObject(SynchedMissionObject missionObject)
		{
			this.MissionObject = missionObject;
		}

		public SynchronizeMissionObject()
		{
		}

		protected override void OnWrite()
		{
			base.WriteSynchedMissionObjectToPacket(this.MissionObject);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadSynchedMissionObjectFromPacket(ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Synchronize MissionObject with Id: ",
				this.MissionObject.Id.Id,
				" and name: ",
				(this.MissionObject.GameEntity != null) ? this.MissionObject.GameEntity.Name : "null entity"
			});
		}
	}
}
