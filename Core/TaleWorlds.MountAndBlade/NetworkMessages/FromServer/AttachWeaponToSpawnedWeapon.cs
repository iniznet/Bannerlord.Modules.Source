using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AttachWeaponToSpawnedWeapon : GameNetworkMessage
	{
		public MissionWeapon Weapon { get; private set; }

		public MissionObject MissionObject { get; private set; }

		public MatrixFrame AttachLocalFrame { get; private set; }

		public AttachWeaponToSpawnedWeapon(MissionWeapon weapon, MissionObject missionObject, MatrixFrame attachLocalFrame)
		{
			this.Weapon = weapon;
			this.MissionObject = missionObject;
			this.AttachLocalFrame = attachLocalFrame;
		}

		public AttachWeaponToSpawnedWeapon()
		{
		}

		protected override void OnWrite()
		{
			ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
			GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
			GameNetworkMessage.WriteVec3ToPacket(this.AttachLocalFrame.origin, CompressionBasic.LocalPositionCompressionInfo);
			GameNetworkMessage.WriteRotationMatrixToPacket(this.AttachLocalFrame.rotation);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
			Vec3 vec = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.LocalPositionCompressionInfo, ref flag);
			Mat3 mat = GameNetworkMessage.ReadRotationMatrixFromPacket(ref flag);
			if (flag)
			{
				this.AttachLocalFrame = new MatrixFrame(mat, vec);
			}
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"AttachWeaponToSpawnedWeapon with name: ",
				(!this.Weapon.IsEmpty) ? this.Weapon.Item.Name : TextObject.Empty,
				" to MissionObject: ",
				this.MissionObject.Id.Id
			});
		}
	}
}
