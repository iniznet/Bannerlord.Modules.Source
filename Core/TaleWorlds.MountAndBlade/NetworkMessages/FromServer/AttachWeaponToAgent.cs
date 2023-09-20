using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AttachWeaponToAgent : GameNetworkMessage
	{
		public MissionWeapon Weapon { get; private set; }

		public Agent Agent { get; private set; }

		public sbyte BoneIndex { get; private set; }

		public MatrixFrame AttachLocalFrame { get; private set; }

		public AttachWeaponToAgent(MissionWeapon weapon, Agent agent, sbyte boneIndex, MatrixFrame attachLocalFrame)
		{
			this.Weapon = weapon;
			this.Agent = agent;
			this.BoneIndex = boneIndex;
			this.AttachLocalFrame = attachLocalFrame;
		}

		public AttachWeaponToAgent()
		{
		}

		protected override void OnWrite()
		{
			ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
			GameNetworkMessage.WriteIntToPacket((int)this.BoneIndex, CompressionMission.BoneIndexCompressionInfo);
			GameNetworkMessage.WriteVec3ToPacket(this.AttachLocalFrame.origin, CompressionBasic.LocalPositionCompressionInfo);
			GameNetworkMessage.WriteRotationMatrixToPacket(this.AttachLocalFrame.rotation);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref flag);
			this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, false);
			this.BoneIndex = (sbyte)GameNetworkMessage.ReadIntFromPacket(CompressionMission.BoneIndexCompressionInfo, ref flag);
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
				"AttachWeaponToAgent with name: ",
				(!this.Weapon.IsEmpty) ? this.Weapon.Item.Name : TextObject.Empty,
				" to bone index: ",
				this.BoneIndex,
				" on agent: ",
				this.Agent.Name,
				" with index: ",
				this.Agent.Index
			});
		}
	}
}
