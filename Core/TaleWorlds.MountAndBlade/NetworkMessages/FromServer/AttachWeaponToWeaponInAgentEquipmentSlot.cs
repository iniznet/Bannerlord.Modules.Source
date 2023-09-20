using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AttachWeaponToWeaponInAgentEquipmentSlot : GameNetworkMessage
	{
		public MissionWeapon Weapon { get; private set; }

		public EquipmentIndex SlotIndex { get; private set; }

		public Agent Agent { get; private set; }

		public MatrixFrame AttachLocalFrame { get; private set; }

		public AttachWeaponToWeaponInAgentEquipmentSlot(MissionWeapon weapon, Agent agent, EquipmentIndex slot, MatrixFrame attachLocalFrame)
		{
			this.Weapon = weapon;
			this.Agent = agent;
			this.SlotIndex = slot;
			this.AttachLocalFrame = attachLocalFrame;
		}

		public AttachWeaponToWeaponInAgentEquipmentSlot()
		{
		}

		protected override void OnWrite()
		{
			ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.AttachLocalFrame.origin, CompressionBasic.LocalPositionCompressionInfo);
			GameNetworkMessage.WriteRotationMatrixToPacket(this.AttachLocalFrame.rotation);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.SlotIndex = (EquipmentIndex)GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref flag);
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
				"AttachWeaponToWeaponInAgentEquipmentSlot with name: ",
				(!this.Weapon.IsEmpty) ? this.Weapon.Item.Name : TextObject.Empty,
				" to SlotIndex: ",
				this.SlotIndex,
				" on agent: ",
				this.Agent.Name,
				" with index: ",
				this.Agent.Index
			});
		}
	}
}
