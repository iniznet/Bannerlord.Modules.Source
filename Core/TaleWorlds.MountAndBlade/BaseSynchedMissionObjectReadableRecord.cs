using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	[DefineSynchedMissionObjectType(typeof(SynchedMissionObject))]
	public struct BaseSynchedMissionObjectReadableRecord
	{
		public bool SetVisibilityExcludeParents { get; private set; }

		public bool SynchTransform { get; private set; }

		public MatrixFrame GameObjectFrame { get; private set; }

		public bool SynchronizeFrameOverTime { get; private set; }

		public MatrixFrame LastSynchedFrame { get; private set; }

		public float Duration { get; private set; }

		public bool HasSkeleton { get; private set; }

		public bool SynchAnimation { get; private set; }

		public int AnimationIndex { get; private set; }

		public float AnimationSpeed { get; private set; }

		public float AnimationParameter { get; private set; }

		public bool IsSkeletonAnimationPaused { get; private set; }

		public bool SynchColors { get; private set; }

		public uint Color { get; private set; }

		public uint Color2 { get; private set; }

		public bool IsDisabled { get; private set; }

		public bool ReadFromNetwork(ref bool bufferReadValid)
		{
			this.SetVisibilityExcludeParents = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
			this.SynchTransform = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
			if (this.SynchTransform)
			{
				this.GameObjectFrame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref bufferReadValid);
				this.SynchronizeFrameOverTime = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
				if (this.SynchronizeFrameOverTime)
				{
					this.LastSynchedFrame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref bufferReadValid);
					this.Duration = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagCapturePointDurationCompressionInfo, ref bufferReadValid);
				}
			}
			this.HasSkeleton = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
			if (this.HasSkeleton)
			{
				this.SynchAnimation = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
				if (this.SynchAnimation)
				{
					this.AnimationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationIndexCompressionInfo, ref bufferReadValid);
					this.AnimationSpeed = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationSpeedCompressionInfo, ref bufferReadValid);
					this.AnimationParameter = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationProgressCompressionInfo, ref bufferReadValid);
					this.IsSkeletonAnimationPaused = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
				}
			}
			this.SynchColors = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
			if (this.SynchColors)
			{
				this.Color = GameNetworkMessage.ReadUintFromPacket(CompressionBasic.ColorCompressionInfo, ref bufferReadValid);
				this.Color2 = GameNetworkMessage.ReadUintFromPacket(CompressionBasic.ColorCompressionInfo, ref bufferReadValid);
			}
			this.IsDisabled = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
			return bufferReadValid;
		}

		public static ValueTuple<BaseSynchedMissionObjectReadableRecord, ISynchedMissionObjectReadableRecord> CreateFromNetworkWithTypeIndex(int typeIndex)
		{
			bool flag = true;
			BaseSynchedMissionObjectReadableRecord baseSynchedMissionObjectReadableRecord = default(BaseSynchedMissionObjectReadableRecord);
			baseSynchedMissionObjectReadableRecord.ReadFromNetwork(ref flag);
			ISynchedMissionObjectReadableRecord synchedMissionObjectReadableRecord = null;
			if (typeIndex >= 0)
			{
				synchedMissionObjectReadableRecord = Activator.CreateInstance(GameNetwork.GetSynchedMissionObjectReadableRecordTypeFromIndex(typeIndex)) as ISynchedMissionObjectReadableRecord;
				synchedMissionObjectReadableRecord.ReadFromNetwork(ref flag);
			}
			return new ValueTuple<BaseSynchedMissionObjectReadableRecord, ISynchedMissionObjectReadableRecord>(baseSynchedMissionObjectReadableRecord, synchedMissionObjectReadableRecord);
		}
	}
}
