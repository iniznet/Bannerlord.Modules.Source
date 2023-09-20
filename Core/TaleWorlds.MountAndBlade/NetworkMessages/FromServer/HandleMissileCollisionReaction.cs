using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200007E RID: 126
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class HandleMissileCollisionReaction : GameNetworkMessage
	{
		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060004F1 RID: 1265 RVA: 0x000096B2 File Offset: 0x000078B2
		// (set) Token: 0x060004F2 RID: 1266 RVA: 0x000096BA File Offset: 0x000078BA
		public int MissileIndex { get; private set; }

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060004F3 RID: 1267 RVA: 0x000096C3 File Offset: 0x000078C3
		// (set) Token: 0x060004F4 RID: 1268 RVA: 0x000096CB File Offset: 0x000078CB
		public Mission.MissileCollisionReaction CollisionReaction { get; private set; }

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060004F5 RID: 1269 RVA: 0x000096D4 File Offset: 0x000078D4
		// (set) Token: 0x060004F6 RID: 1270 RVA: 0x000096DC File Offset: 0x000078DC
		public MatrixFrame AttachLocalFrame { get; private set; }

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060004F7 RID: 1271 RVA: 0x000096E5 File Offset: 0x000078E5
		// (set) Token: 0x060004F8 RID: 1272 RVA: 0x000096ED File Offset: 0x000078ED
		public bool IsAttachedFrameLocal { get; private set; }

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x000096F6 File Offset: 0x000078F6
		// (set) Token: 0x060004FA RID: 1274 RVA: 0x000096FE File Offset: 0x000078FE
		public Agent AttackerAgent { get; private set; }

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060004FB RID: 1275 RVA: 0x00009707 File Offset: 0x00007907
		// (set) Token: 0x060004FC RID: 1276 RVA: 0x0000970F File Offset: 0x0000790F
		public Agent AttachedAgent { get; private set; }

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x00009718 File Offset: 0x00007918
		// (set) Token: 0x060004FE RID: 1278 RVA: 0x00009720 File Offset: 0x00007920
		public bool AttachedToShield { get; private set; }

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060004FF RID: 1279 RVA: 0x00009729 File Offset: 0x00007929
		// (set) Token: 0x06000500 RID: 1280 RVA: 0x00009731 File Offset: 0x00007931
		public sbyte AttachedBoneIndex { get; private set; }

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000501 RID: 1281 RVA: 0x0000973A File Offset: 0x0000793A
		// (set) Token: 0x06000502 RID: 1282 RVA: 0x00009742 File Offset: 0x00007942
		public MissionObject AttachedMissionObject { get; private set; }

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000503 RID: 1283 RVA: 0x0000974B File Offset: 0x0000794B
		// (set) Token: 0x06000504 RID: 1284 RVA: 0x00009753 File Offset: 0x00007953
		public Vec3 BounceBackVelocity { get; private set; }

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000505 RID: 1285 RVA: 0x0000975C File Offset: 0x0000795C
		// (set) Token: 0x06000506 RID: 1286 RVA: 0x00009764 File Offset: 0x00007964
		public Vec3 BounceBackAngularVelocity { get; private set; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000507 RID: 1287 RVA: 0x0000976D File Offset: 0x0000796D
		// (set) Token: 0x06000508 RID: 1288 RVA: 0x00009775 File Offset: 0x00007975
		public int ForcedSpawnIndex { get; private set; }

		// Token: 0x06000509 RID: 1289 RVA: 0x00009780 File Offset: 0x00007980
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

		// Token: 0x0600050A RID: 1290 RVA: 0x000097F0 File Offset: 0x000079F0
		public HandleMissileCollisionReaction()
		{
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x000097F8 File Offset: 0x000079F8
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

		// Token: 0x0600050C RID: 1292 RVA: 0x00009984 File Offset: 0x00007B84
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

		// Token: 0x0600050D RID: 1293 RVA: 0x00009AB4 File Offset: 0x00007CB4
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Items;
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x00009AB8 File Offset: 0x00007CB8
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
