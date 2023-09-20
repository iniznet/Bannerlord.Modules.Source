using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class HandleMissileCollisionReaction : GameNetworkMessage
	{
		public int MissileIndex { get; private set; }

		public Mission.MissileCollisionReaction CollisionReaction { get; private set; }

		public MatrixFrame AttachLocalFrame { get; private set; }

		public bool IsAttachedFrameLocal { get; private set; }

		public Agent AttackerAgent { get; private set; }

		public Agent AttachedAgent { get; private set; }

		public bool AttachedToShield { get; private set; }

		public sbyte AttachedBoneIndex { get; private set; }

		public MissionObject AttachedMissionObject { get; private set; }

		public Vec3 BounceBackVelocity { get; private set; }

		public Vec3 BounceBackAngularVelocity { get; private set; }

		public int ForcedSpawnIndex { get; private set; }

		public HandleMissileCollisionReaction(int missileIndex, Mission.MissileCollisionReaction collisionReaction, MatrixFrame attachLocalFrame, bool isAttachedFrameLocal, Agent attackerAgent, Agent attachedAgent, bool attachedToShield, sbyte attachedBoneIndex, MissionObject attachedMissionObject, Vec3 bounceBackVelocity, Vec3 bounceBackAngularVelocity, int forcedSpawnIndex)
		{
			this.MissileIndex = missileIndex;
			this.CollisionReaction = collisionReaction;
			this.AttachLocalFrame = attachLocalFrame;
			this.IsAttachedFrameLocal = isAttachedFrameLocal;
			this.AttackerAgent = attackerAgent;
			this.AttachedAgent = attachedAgent;
			this.AttachedToShield = attachedToShield;
			this.AttachedBoneIndex = attachedBoneIndex;
			this.AttachedMissionObject = attachedMissionObject;
			this.BounceBackVelocity = bounceBackVelocity;
			this.BounceBackAngularVelocity = bounceBackAngularVelocity;
			this.ForcedSpawnIndex = forcedSpawnIndex;
		}

		public HandleMissileCollisionReaction()
		{
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.MissileIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissileCompressionInfo, ref flag);
			this.CollisionReaction = (Mission.MissileCollisionReaction)GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissileCollisionReactionCompressionInfo, ref flag);
			this.AttackerAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
			this.AttachedAgent = null;
			this.AttachedToShield = false;
			this.AttachedBoneIndex = -1;
			this.AttachedMissionObject = null;
			if (this.CollisionReaction == Mission.MissileCollisionReaction.Stick || this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
			{
				if (GameNetworkMessage.ReadBoolFromPacket(ref flag))
				{
					this.AttachedAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref flag, true);
					this.AttachedToShield = GameNetworkMessage.ReadBoolFromPacket(ref flag);
					if (!this.AttachedToShield)
					{
						this.AttachedBoneIndex = (sbyte)GameNetworkMessage.ReadIntFromPacket(CompressionMission.BoneIndexCompressionInfo, ref flag);
					}
				}
				else
				{
					this.AttachedMissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref flag);
				}
			}
			if (this.CollisionReaction != Mission.MissileCollisionReaction.BecomeInvisible && this.CollisionReaction != Mission.MissileCollisionReaction.PassThrough)
			{
				this.IsAttachedFrameLocal = GameNetworkMessage.ReadBoolFromPacket(ref flag);
				if (this.IsAttachedFrameLocal)
				{
					this.AttachLocalFrame = GameNetworkMessage.ReadNonUniformTransformFromPacket(CompressionBasic.BigRangeLowResLocalPositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo, ref flag);
				}
				else
				{
					this.AttachLocalFrame = GameNetworkMessage.ReadNonUniformTransformFromPacket(CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo, ref flag);
				}
			}
			else
			{
				this.AttachLocalFrame = MatrixFrame.Identity;
			}
			if (this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
			{
				this.BounceBackVelocity = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.SpawnedItemVelocityCompressionInfo, ref flag);
				this.BounceBackAngularVelocity = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.SpawnedItemAngularVelocityCompressionInfo, ref flag);
			}
			else
			{
				this.BounceBackVelocity = Vec3.Zero;
				this.BounceBackAngularVelocity = Vec3.Zero;
			}
			if (this.CollisionReaction == Mission.MissileCollisionReaction.Stick || this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
			{
				this.ForcedSpawnIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref flag);
			}
			return flag;
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.MissileIndex, CompressionMission.MissileCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this.CollisionReaction, CompressionMission.MissileCollisionReactionCompressionInfo);
			GameNetworkMessage.WriteAgentReferenceToPacket(this.AttackerAgent);
			if (this.CollisionReaction == Mission.MissileCollisionReaction.Stick || this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
			{
				bool flag = this.AttachedAgent != null;
				GameNetworkMessage.WriteBoolToPacket(flag);
				if (flag)
				{
					GameNetworkMessage.WriteAgentReferenceToPacket(this.AttachedAgent);
					GameNetworkMessage.WriteBoolToPacket(this.AttachedToShield);
					if (!this.AttachedToShield)
					{
						GameNetworkMessage.WriteIntToPacket((int)this.AttachedBoneIndex, CompressionMission.BoneIndexCompressionInfo);
					}
				}
				else
				{
					GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.AttachedMissionObject);
				}
			}
			if (this.CollisionReaction != Mission.MissileCollisionReaction.BecomeInvisible && this.CollisionReaction != Mission.MissileCollisionReaction.PassThrough)
			{
				GameNetworkMessage.WriteBoolToPacket(this.IsAttachedFrameLocal);
				if (this.IsAttachedFrameLocal)
				{
					GameNetworkMessage.WriteNonUniformTransformToPacket(this.AttachLocalFrame, CompressionBasic.BigRangeLowResLocalPositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo);
				}
				else
				{
					GameNetworkMessage.WriteNonUniformTransformToPacket(this.AttachLocalFrame, CompressionBasic.PositionCompressionInfo, CompressionBasic.LowResQuaternionCompressionInfo);
				}
			}
			if (this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
			{
				GameNetworkMessage.WriteVec3ToPacket(this.BounceBackVelocity, CompressionMission.SpawnedItemVelocityCompressionInfo);
				GameNetworkMessage.WriteVec3ToPacket(this.BounceBackAngularVelocity, CompressionMission.SpawnedItemAngularVelocityCompressionInfo);
			}
			if (this.CollisionReaction == Mission.MissileCollisionReaction.Stick || this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
			{
				GameNetworkMessage.WriteIntToPacket(this.ForcedSpawnIndex, CompressionBasic.MissionObjectIDCompressionInfo);
			}
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		protected override string OnGetLogFormat()
		{
			object[] array = new object[16];
			array[0] = "Handle Missile Collision with index: ";
			array[1] = this.MissileIndex;
			array[2] = " collision reaction: ";
			array[3] = this.CollisionReaction;
			array[4] = " AttackerAgent index: ";
			int num = 5;
			Agent attackerAgent = this.AttackerAgent;
			array[num] = ((attackerAgent != null) ? attackerAgent.Index : (-1));
			array[6] = " AttachedAgent index: ";
			int num2 = 7;
			Agent attachedAgent = this.AttachedAgent;
			array[num2] = ((attachedAgent != null) ? attachedAgent.Index : (-1));
			array[8] = " AttachedToShield: ";
			array[9] = this.AttachedToShield.ToString();
			array[10] = " AttachedBoneIndex: ";
			array[11] = this.AttachedBoneIndex;
			array[12] = " AttachedMissionObject id: ";
			array[13] = ((this.AttachedMissionObject != null) ? this.AttachedMissionObject.Id.ToString() : "-1");
			array[14] = " ForcedSpawnIndex: ";
			array[15] = this.ForcedSpawnIndex;
			return string.Concat(array);
		}
	}
}
