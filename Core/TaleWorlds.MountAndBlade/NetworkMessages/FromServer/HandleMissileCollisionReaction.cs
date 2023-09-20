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

		public int AttackerAgentIndex { get; private set; }

		public int AttachedAgentIndex { get; private set; }

		public bool AttachedToShield { get; private set; }

		public sbyte AttachedBoneIndex { get; private set; }

		public MissionObjectId AttachedMissionObjectId { get; private set; }

		public Vec3 BounceBackVelocity { get; private set; }

		public Vec3 BounceBackAngularVelocity { get; private set; }

		public int ForcedSpawnIndex { get; private set; }

		public HandleMissileCollisionReaction(int missileIndex, Mission.MissileCollisionReaction collisionReaction, MatrixFrame attachLocalFrame, bool isAttachedFrameLocal, int attackerAgentIndex, int attachedAgentIndex, bool attachedToShield, sbyte attachedBoneIndex, MissionObjectId attachedMissionObjectId, Vec3 bounceBackVelocity, Vec3 bounceBackAngularVelocity, int forcedSpawnIndex)
		{
			this.MissileIndex = missileIndex;
			this.CollisionReaction = collisionReaction;
			this.AttachLocalFrame = attachLocalFrame;
			this.IsAttachedFrameLocal = isAttachedFrameLocal;
			this.AttackerAgentIndex = attackerAgentIndex;
			this.AttachedAgentIndex = attachedAgentIndex;
			this.AttachedToShield = attachedToShield;
			this.AttachedBoneIndex = attachedBoneIndex;
			this.AttachedMissionObjectId = attachedMissionObjectId;
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
			this.AttackerAgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
			this.AttachedAgentIndex = -1;
			this.AttachedToShield = false;
			this.AttachedBoneIndex = -1;
			this.AttachedMissionObjectId = MissionObjectId.Invalid;
			if (this.CollisionReaction == Mission.MissileCollisionReaction.Stick || this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
			{
				if (GameNetworkMessage.ReadBoolFromPacket(ref flag))
				{
					this.AttachedAgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(ref flag);
					this.AttachedToShield = GameNetworkMessage.ReadBoolFromPacket(ref flag);
					if (!this.AttachedToShield)
					{
						this.AttachedBoneIndex = (sbyte)GameNetworkMessage.ReadIntFromPacket(CompressionMission.BoneIndexCompressionInfo, ref flag);
					}
				}
				else
				{
					this.AttachedMissionObjectId = GameNetworkMessage.ReadMissionObjectIdFromPacket(ref flag);
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
			GameNetworkMessage.WriteAgentIndexToPacket(this.AttackerAgentIndex);
			if (this.CollisionReaction == Mission.MissileCollisionReaction.Stick || this.CollisionReaction == Mission.MissileCollisionReaction.BounceBack)
			{
				bool flag = this.AttachedAgentIndex >= 0;
				GameNetworkMessage.WriteBoolToPacket(flag);
				if (flag)
				{
					GameNetworkMessage.WriteAgentIndexToPacket(this.AttachedAgentIndex);
					GameNetworkMessage.WriteBoolToPacket(this.AttachedToShield);
					if (!this.AttachedToShield)
					{
						GameNetworkMessage.WriteIntToPacket((int)this.AttachedBoneIndex, CompressionMission.BoneIndexCompressionInfo);
					}
				}
				else
				{
					GameNetworkMessage.WriteMissionObjectIdToPacket((this.AttachedMissionObjectId.Id >= 0) ? this.AttachedMissionObjectId : MissionObjectId.Invalid);
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
			return string.Concat(new object[]
			{
				"Handle Missile Collision with index: ",
				this.MissileIndex,
				" collision reaction: ",
				this.CollisionReaction,
				" AttackerAgent index: ",
				this.AttackerAgentIndex,
				" AttachedAgent index: ",
				this.AttachedAgentIndex,
				" AttachedToShield: ",
				this.AttachedToShield.ToString(),
				" AttachedBoneIndex: ",
				this.AttachedBoneIndex,
				" AttachedMissionObject id: ",
				(this.AttachedMissionObjectId != MissionObjectId.Invalid) ? this.AttachedMissionObjectId.Id.ToString() : "-1",
				" ForcedSpawnIndex: ",
				this.ForcedSpawnIndex
			});
		}
	}
}
