using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class SyncObjectDestructionLevel : GameNetworkMessage
	{
		public MissionObject MissionObject { get; private set; }

		public int DestructionLevel { get; private set; }

		public int ForcedIndex { get; private set; }

		public float BlowMagnitude { get; private set; }

		public Vec3 BlowPosition { get; private set; }

		public Vec3 BlowDirection { get; private set; }

		public SyncObjectDestructionLevel(MissionObject missionObject, int destructionLevel, int forcedIndex, float blowMagnitude, Vec3 blowPosition, Vec3 blowDirection)
		{
			this.MissionObject = missionObject;
			this.DestructionLevel = destructionLevel;
			this.ForcedIndex = forcedIndex;
			this.BlowMagnitude = blowMagnitude;
			this.BlowPosition = blowPosition;
			this.BlowDirection = blowDirection;
		}

		public SyncObjectDestructionLevel()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			this.DestructionLevel = GameNetworkMessage.ReadIntFromPacket(CompressionMission.UsableGameObjectDestructionStateCompressionInfo, ref flag);
			this.ForcedIndex = (GameNetworkMessage.ReadBoolFromPacket(ref flag) ? GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag) : (-1));
			this.BlowMagnitude = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.UsableGameObjectBlowMagnitude, ref flag);
			this.BlowPosition = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref flag);
			this.BlowDirection = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.UsableGameObjectBlowDirection, ref flag);
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteIntToPacket(this.DestructionLevel, CompressionMission.UsableGameObjectDestructionStateCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.ForcedIndex != -1);
			if (this.ForcedIndex != -1)
			{
				GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
			}
			GameNetworkMessage.WriteFloatToPacket(this.BlowMagnitude, CompressionMission.UsableGameObjectBlowMagnitude);
			GameNetworkMessage.WriteVec3ToPacket(this.BlowPosition, CompressionBasic.PositionCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.BlowDirection, CompressionMission.UsableGameObjectBlowDirection);
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.SiegeWeaponsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Synchronize DestructionLevel: ",
				this.DestructionLevel,
				" of MissionObject with Id: ",
				this.MissionObject.Id,
				" and name: ",
				this.MissionObject.GameEntity.Name,
				(this.ForcedIndex != -1) ? (" (New object will have ID: " + this.ForcedIndex + ")") : ""
			});
		}
	}
}
