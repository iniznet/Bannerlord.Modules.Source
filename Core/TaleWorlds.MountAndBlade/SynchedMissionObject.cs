using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	public class SynchedMissionObject : MissionObject
	{
		public uint Color { get; private set; }

		public uint Color2 { get; private set; }

		public bool SynchronizeCompleted
		{
			get
			{
				return this._synchState == SynchedMissionObject.SynchState.SynchronizeCompleted;
			}
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (!this.SynchronizeCompleted)
			{
				return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		protected internal override void OnTick(float dt)
		{
			if (!this.SynchronizeCompleted)
			{
				MatrixFrame frame = base.GameEntity.GetFrame();
				if ((this._synchState == SynchedMissionObject.SynchState.SynchronizePosition && this._lastSynchedFrame.origin.NearlyEquals(frame.origin, 1E-05f)) || this._lastSynchedFrame.NearlyEquals(frame, 1E-05f))
				{
					this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeCompleted);
					return;
				}
				MatrixFrame matrixFrame;
				matrixFrame.origin = ((this._synchState == SynchedMissionObject.SynchState.SynchronizeFrameOverTime) ? MBMath.Lerp(this._firstFrame.origin, this._lastSynchedFrame.origin, this._timer / this._duration, 0.2f * dt) : MBMath.Lerp(frame.origin, this._lastSynchedFrame.origin, 8f * dt, 0.2f * dt));
				if (this._synchState == SynchedMissionObject.SynchState.SynchronizeFrame || this._synchState == SynchedMissionObject.SynchState.SynchronizeFrameOverTime)
				{
					matrixFrame.rotation.s = MBMath.Lerp(frame.rotation.s, this._lastSynchedFrame.rotation.s, 8f * dt, 0.2f * dt);
					matrixFrame.rotation.f = MBMath.Lerp(frame.rotation.f, this._lastSynchedFrame.rotation.f, 8f * dt, 0.2f * dt);
					matrixFrame.rotation.u = MBMath.Lerp(frame.rotation.u, this._lastSynchedFrame.rotation.u, 8f * dt, 0.2f * dt);
					if (matrixFrame.origin != this._lastSynchedFrame.origin || matrixFrame.rotation.s != this._lastSynchedFrame.rotation.s || matrixFrame.rotation.f != this._lastSynchedFrame.rotation.f || matrixFrame.rotation.u != this._lastSynchedFrame.rotation.u)
					{
						matrixFrame.rotation.Orthonormalize();
						if (this._lastSynchedFrame.rotation.HasScale())
						{
							matrixFrame.rotation.ApplyScaleLocal(this._lastSynchedFrame.rotation.GetScaleVector());
						}
					}
					base.GameEntity.SetFrame(ref matrixFrame);
				}
				else
				{
					base.GameEntity.SetLocalPosition(matrixFrame.origin);
				}
				this._timer = MathF.Min(this._timer + dt, this._duration);
			}
		}

		private void SetSynchState(SynchedMissionObject.SynchState newState)
		{
			if (newState != this._synchState)
			{
				this._synchState = newState;
				base.SetScriptComponentToTick(this.GetTickRequirement());
			}
		}

		public void SetLocalPositionSmoothStep(ref Vec3 targetPosition)
		{
			this._lastSynchedFrame.origin = targetPosition;
			this.SetSynchState(SynchedMissionObject.SynchState.SynchronizePosition);
		}

		public virtual void SetVisibleSynched(bool value, bool forceChildrenVisible = false)
		{
			bool flag = base.GameEntity.IsVisibleIncludeParents() != value;
			List<GameEntity> list = null;
			if (!flag && forceChildrenVisible)
			{
				list = new List<GameEntity>();
				base.GameEntity.GetChildrenRecursive(ref list);
				using (List<GameEntity>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.GetPhysicsState() != value)
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (base.GameEntity != null && flag)
			{
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectVisibility(this, value));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				base.GameEntity.SetVisibilityExcludeParents(value);
				if (forceChildrenVisible)
				{
					if (list == null)
					{
						list = new List<GameEntity>();
						base.GameEntity.GetChildrenRecursive(ref list);
					}
					foreach (GameEntity gameEntity in list)
					{
						gameEntity.SetVisibilityExcludeParents(value);
					}
				}
			}
		}

		public virtual void SetPhysicsStateSynched(bool value, bool setChildren = true)
		{
		}

		public virtual void SetDisabledSynched()
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetMissionObjectDisabled(this));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			base.SetDisabledAndMakeInvisible(false);
		}

		public void SetFrameSynched(ref MatrixFrame frame, bool isClient = false)
		{
			if (base.GameEntity.GetFrame() != frame || this._synchState != SynchedMissionObject.SynchState.SynchronizeCompleted)
			{
				this._duration = 0f;
				this._timer = 0f;
				if (GameNetwork.IsClientOrReplay)
				{
					this._lastSynchedFrame = frame;
					this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeFrame);
					return;
				}
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectFrame(this, ref frame));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeCompleted);
				base.GameEntity.SetFrame(ref frame);
				this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchTransform;
			}
		}

		public void SetGlobalFrameSynched(ref MatrixFrame frame, bool isClient = false)
		{
			this._duration = 0f;
			this._timer = 0f;
			if (base.GameEntity.GetGlobalFrame() != frame)
			{
				if (GameNetwork.IsClientOrReplay)
				{
					this._lastSynchedFrame = ((base.GameEntity.Parent != null) ? base.GameEntity.Parent.GetGlobalFrame().TransformToLocalNonOrthogonal(ref frame) : frame);
					this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeFrame);
					return;
				}
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectGlobalFrame(this, ref frame));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeCompleted);
				base.GameEntity.SetGlobalFrame(frame);
				this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchTransform;
			}
		}

		public void SetFrameSynchedOverTime(ref MatrixFrame frame, float duration, bool isClient = false)
		{
			if (base.GameEntity.GetFrame() != frame || duration.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				this._firstFrame = base.GameEntity.GetFrame();
				this._lastSynchedFrame = frame;
				this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeFrameOverTime);
				this._duration = (duration.ApproximatelyEqualsTo(0f, 1E-05f) ? 0.1f : duration);
				this._timer = 0f;
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectFrameOverTime(this, ref frame, duration));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchTransform;
			}
		}

		public void SetGlobalFrameSynchedOverTime(ref MatrixFrame frame, float duration, bool isClient = false)
		{
			if (base.GameEntity.GetGlobalFrame() != frame || duration.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				this._firstFrame = base.GameEntity.GetFrame();
				this._lastSynchedFrame = ((base.GameEntity.Parent != null) ? base.GameEntity.Parent.GetGlobalFrame().TransformToLocalNonOrthogonal(ref frame) : frame);
				this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeFrameOverTime);
				this._duration = (duration.ApproximatelyEqualsTo(0f, 1E-05f) ? 0.1f : duration);
				this._timer = 0f;
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectGlobalFrameOverTime(this, ref frame, duration));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchTransform;
			}
		}

		public void SetAnimationAtChannelSynched(string animationName, int channelNo, float animationSpeed = 1f)
		{
			this.SetAnimationAtChannelSynched(MBAnimation.GetAnimationIndexWithName(animationName), channelNo, animationSpeed);
		}

		public void SetAnimationAtChannelSynched(int animationIndex, int channelNo, float animationSpeed = 1f)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				int animationIndexAtChannel = base.GameEntity.Skeleton.GetAnimationIndexAtChannel(channelNo);
				bool flag = true;
				if (animationIndexAtChannel == animationIndex && base.GameEntity.Skeleton.GetAnimationSpeedAtChannel(channelNo).ApproximatelyEqualsTo(animationSpeed, 1E-05f) && base.GameEntity.Skeleton.GetAnimationParameterAtChannel(channelNo) < 0.02f)
				{
					flag = false;
				}
				if (flag)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectAnimationAtChannel(this, channelNo, animationIndex, animationSpeed));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
					this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchAnimation;
				}
			}
			base.GameEntity.Skeleton.SetAnimationAtChannel(animationIndex, channelNo, animationSpeed, -1f, 0f);
		}

		public void SetAnimationChannelParameterSynched(int channelNo, float parameter)
		{
			if (!base.GameEntity.Skeleton.GetAnimationParameterAtChannel(channelNo).ApproximatelyEqualsTo(parameter, 1E-05f))
			{
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectAnimationChannelParameter(this, channelNo, parameter));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				base.GameEntity.Skeleton.SetAnimationParameterAtChannel(channelNo, parameter);
				this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchAnimation;
			}
		}

		public void PauseSkeletonAnimationSynched()
		{
			if (!base.GameEntity.IsSkeletonAnimationPaused())
			{
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectAnimationPaused(this, true));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				base.GameEntity.PauseSkeletonAnimation();
				this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchAnimation;
			}
		}

		public void ResumeSkeletonAnimationSynched()
		{
			if (base.GameEntity.IsSkeletonAnimationPaused())
			{
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectAnimationPaused(this, false));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				base.GameEntity.ResumeSkeletonAnimation();
				this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchAnimation;
			}
		}

		public void BurstParticlesSynched(bool doChildren = true)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new BurstMissionObjectParticles(this, false));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			base.GameEntity.BurstEntityParticle(doChildren);
		}

		public void ApplyImpulseSynched(Vec3 localPosition, Vec3 impulse)
		{
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetMissionObjectImpulse(this, localPosition, impulse));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			base.GameEntity.ApplyLocalImpulseToDynamicBody(localPosition, impulse);
			this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchTransform;
		}

		public void AddBodyFlagsSynched(BodyFlags flags, bool applyToChildren = true)
		{
			if ((base.GameEntity.BodyFlag & flags) != flags)
			{
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new AddMissionObjectBodyFlags(this, flags, applyToChildren));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				base.GameEntity.AddBodyFlags(flags, applyToChildren);
				this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchBodyFlags;
			}
		}

		public void RemoveBodyFlagsSynched(BodyFlags flags, bool applyToChildren = true)
		{
			if ((base.GameEntity.BodyFlag & flags) != BodyFlags.None)
			{
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new RemoveMissionObjectBodyFlags(this, flags, applyToChildren));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				base.GameEntity.RemoveBodyFlags(flags, applyToChildren);
				this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchBodyFlags;
			}
		}

		public void SetTeamColors(uint color, uint color2)
		{
			this.Color = color;
			this.Color2 = color2;
			base.GameEntity.SetColor(color, color2, "use_team_color");
		}

		public virtual void SetTeamColorsSynched(uint color, uint color2)
		{
			if (base.GameEntity != null)
			{
				if (GameNetwork.IsServerOrRecorder)
				{
					GameNetwork.BeginBroadcastModuleEvent();
					GameNetwork.WriteMessage(new SetMissionObjectColors(this, color, color2));
					GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
				}
				this.SetTeamColors(color, color2);
				this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SyncColors;
			}
		}

		public virtual bool ReadFromNetwork()
		{
			bool flag = true;
			base.GameEntity.SetVisibilityExcludeParents(GameNetworkMessage.ReadBoolFromPacket(ref flag));
			if (GameNetworkMessage.ReadBoolFromPacket(ref flag))
			{
				MatrixFrame matrixFrame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
				base.GameEntity.SetFrame(ref matrixFrame);
				if (GameNetworkMessage.ReadBoolFromPacket(ref flag))
				{
					this._firstFrame = base.GameEntity.GetFrame();
					this._lastSynchedFrame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
					this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeFrameOverTime);
					this._duration = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagCapturePointDurationCompressionInfo, ref flag);
					this._timer = 0f;
					if (this._duration.ApproximatelyEqualsTo(0f, 1E-05f))
					{
						this._duration = 0.1f;
					}
				}
			}
			if (base.GameEntity.Skeleton != null && GameNetworkMessage.ReadBoolFromPacket(ref flag))
			{
				int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationIndexCompressionInfo, ref flag);
				float num2 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationSpeedCompressionInfo, ref flag);
				float num3 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationProgressCompressionInfo, ref flag);
				base.GameEntity.Skeleton.SetAnimationAtChannel(num, 0, num2, 0f, 0f);
				base.GameEntity.Skeleton.SetAnimationParameterAtChannel(0, num3);
				if (GameNetworkMessage.ReadBoolFromPacket(ref flag))
				{
					base.GameEntity.Skeleton.TickAnimationsAndForceUpdate(0.001f, base.GameEntity.GetGlobalFrame(), true);
					base.GameEntity.PauseSkeletonAnimation();
				}
				else
				{
					base.GameEntity.ResumeSkeletonAnimation();
				}
			}
			if (GameNetworkMessage.ReadBoolFromPacket(ref flag))
			{
				uint num4 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
				uint num5 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref flag);
				if (flag)
				{
					base.GameEntity.SetColor(num4, num5, "use_team_color");
				}
			}
			if (GameNetworkMessage.ReadBoolFromPacket(ref flag))
			{
				base.SetDisabledAndMakeInvisible(false);
			}
			return flag;
		}

		public virtual void WriteToNetwork()
		{
			GameEntity gameEntity = base.GameEntity;
			GameNetworkMessage.WriteBoolToPacket(gameEntity.GetVisibilityExcludeParents());
			GameNetworkMessage.WriteBoolToPacket(this._initialSynchFlags.HasAnyFlag(SynchedMissionObject.SynchFlags.SynchTransform));
			if (this._initialSynchFlags.HasAnyFlag(SynchedMissionObject.SynchFlags.SynchTransform))
			{
				GameNetworkMessage.WriteMatrixFrameToPacket(gameEntity.GetFrame());
				GameNetworkMessage.WriteBoolToPacket(this._synchState == SynchedMissionObject.SynchState.SynchronizeFrameOverTime);
				if (this._synchState == SynchedMissionObject.SynchState.SynchronizeFrameOverTime)
				{
					GameNetworkMessage.WriteMatrixFrameToPacket(this._lastSynchedFrame);
					GameNetworkMessage.WriteFloatToPacket(this._duration - this._timer, CompressionMission.FlagCapturePointDurationCompressionInfo);
				}
			}
			Skeleton skeleton = gameEntity.Skeleton;
			if (skeleton != null)
			{
				int animationIndexAtChannel = skeleton.GetAnimationIndexAtChannel(0);
				bool flag = animationIndexAtChannel >= 0;
				GameNetworkMessage.WriteBoolToPacket(flag && this._initialSynchFlags.HasAnyFlag(SynchedMissionObject.SynchFlags.SynchAnimation));
				if (flag && this._initialSynchFlags.HasAnyFlag(SynchedMissionObject.SynchFlags.SynchAnimation))
				{
					float animationSpeedAtChannel = skeleton.GetAnimationSpeedAtChannel(0);
					float animationParameterAtChannel = skeleton.GetAnimationParameterAtChannel(0);
					GameNetworkMessage.WriteIntToPacket(animationIndexAtChannel, CompressionBasic.AnimationIndexCompressionInfo);
					GameNetworkMessage.WriteFloatToPacket(animationSpeedAtChannel, CompressionBasic.AnimationSpeedCompressionInfo);
					GameNetworkMessage.WriteFloatToPacket(animationParameterAtChannel, CompressionBasic.AnimationProgressCompressionInfo);
					GameNetworkMessage.WriteBoolToPacket(gameEntity.IsSkeletonAnimationPaused());
				}
			}
			GameNetworkMessage.WriteBoolToPacket(this._initialSynchFlags.HasAnyFlag(SynchedMissionObject.SynchFlags.SyncColors));
			if (this._initialSynchFlags.HasAnyFlag(SynchedMissionObject.SynchFlags.SyncColors))
			{
				GameNetworkMessage.WriteUintToPacket(this.Color, CompressionGeneric.ColorCompressionInfo);
				GameNetworkMessage.WriteUintToPacket(this.Color2, CompressionGeneric.ColorCompressionInfo);
			}
			GameNetworkMessage.WriteBoolToPacket(base.IsDisabled);
		}

		private SynchedMissionObject.SynchFlags _initialSynchFlags;

		private SynchedMissionObject.SynchState _synchState;

		private MatrixFrame _lastSynchedFrame;

		private MatrixFrame _firstFrame;

		private float _timer;

		private float _duration;

		private enum SynchState
		{
			SynchronizeCompleted,
			SynchronizePosition,
			SynchronizeFrame,
			SynchronizeFrameOverTime
		}

		[Flags]
		public enum SynchFlags : uint
		{
			SynchNone = 0U,
			SynchTransform = 1U,
			SynchAnimation = 2U,
			SynchBodyFlags = 4U,
			SyncColors = 8U,
			SynchAll = 4294967295U
		}
	}
}
