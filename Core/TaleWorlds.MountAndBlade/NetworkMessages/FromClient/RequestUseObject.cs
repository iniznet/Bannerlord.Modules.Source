using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x0200002A RID: 42
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class RequestUseObject : GameNetworkMessage
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000149 RID: 329 RVA: 0x0000387B File Offset: 0x00001A7B
		// (set) Token: 0x0600014A RID: 330 RVA: 0x00003883 File Offset: 0x00001A83
		public UsableMissionObject UsableGameObject { get; private set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600014B RID: 331 RVA: 0x0000388C File Offset: 0x00001A8C
		// (set) Token: 0x0600014C RID: 332 RVA: 0x00003894 File Offset: 0x00001A94
		public int UsedObjectPreferenceIndex { get; private set; }

		// Token: 0x0600014D RID: 333 RVA: 0x0000389D File Offset: 0x00001A9D
		public RequestUseObject(UsableMissionObject usableGameObject, int usedObjectPreferenceIndex)
		{
			this.UsableGameObject = usableGameObject;
			this.UsedObjectPreferenceIndex = usedObjectPreferenceIndex;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x000038B3 File Offset: 0x00001AB3
		public RequestUseObject()
		{
		}

		// Token: 0x0600014F RID: 335 RVA: 0x000038BC File Offset: 0x00001ABC
		protected override bool OnRead()
		{
			bool flag = true;
			this.UsableGameObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag) as UsableMissionObject;
			this.UsedObjectPreferenceIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x000038F0 File Offset: 0x00001AF0
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.UsableGameObject);
			GameNetworkMessage.WriteIntToPacket(this.UsedObjectPreferenceIndex, CompressionMission.WieldSlotCompressionInfo);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000390D File Offset: 0x00001B0D
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00003918 File Offset: 0x00001B18
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
