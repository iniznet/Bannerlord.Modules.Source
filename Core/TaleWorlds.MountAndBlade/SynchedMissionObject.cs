using System;
using System.Collections.Generic;
using NetworkMessages.FromServer;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200036B RID: 875
	public class SynchedMissionObject : MissionObject
	{
		// Token: 0x17000897 RID: 2199
		// (get) Token: 0x06002FBA RID: 12218 RVA: 0x000C3D55 File Offset: 0x000C1F55
		// (set) Token: 0x06002FBB RID: 12219 RVA: 0x000C3D5D File Offset: 0x000C1F5D
		public uint Color { get; private set; }

		// Token: 0x17000898 RID: 2200
		// (get) Token: 0x06002FBC RID: 12220 RVA: 0x000C3D66 File Offset: 0x000C1F66
		// (set) Token: 0x06002FBD RID: 12221 RVA: 0x000C3D6E File Offset: 0x000C1F6E
		public uint Color2 { get; private set; }

		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x06002FBE RID: 12222 RVA: 0x000C3D77 File Offset: 0x000C1F77
		public bool SynchronizeCompleted
		{
			get
			{
				return this._synchState == SynchedMissionObject.SynchState.SynchronizeCompleted;
			}
		}

		// Token: 0x06002FBF RID: 12223 RVA: 0x000C3D82 File Offset: 0x000C1F82
		protected internal override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06002FC0 RID: 12224 RVA: 0x000C3D96 File Offset: 0x000C1F96
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (!this.SynchronizeCompleted)
			{
				return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002FC1 RID: 12225 RVA: 0x000C3DB0 File Offset: 0x000C1FB0
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

		// Token: 0x06002FC2 RID: 12226 RVA: 0x000C402E File Offset: 0x000C222E
		private void SetSynchState(SynchedMissionObject.SynchState newState)
		{
			if (newState != this._synchState)
			{
				this._synchState = newState;
				base.SetScriptComponentToTick(this.GetTickRequirement());
			}
		}

		// Token: 0x06002FC3 RID: 12227 RVA: 0x000C404C File Offset: 0x000C224C
		public void SetLocalPositionSmoothStep(ref Vec3 targetPosition)
		{
			this._lastSynchedFrame.origin = targetPosition;
			this.SetSynchState(SynchedMissionObject.SynchState.SynchronizePosition);
		}

		// Token: 0x06002FC4 RID: 12228 RVA: 0x000C4068 File Offset: 0x000C2268
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

		// Token: 0x06002FC5 RID: 12229 RVA: 0x000C417C File Offset: 0x000C237C
		public virtual void SetPhysicsStateSynched(bool value, bool setChildren = true)
		{
		}

		// Token: 0x06002FC6 RID: 12230 RVA: 0x000C417E File Offset: 0x000C237E
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

		// Token: 0x06002FC7 RID: 12231 RVA: 0x000C41A8 File Offset: 0x000C23A8
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

		// Token: 0x06002FC8 RID: 12232 RVA: 0x000C4248 File Offset: 0x000C2448
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

		// Token: 0x06002FC9 RID: 12233 RVA: 0x000C4310 File Offset: 0x000C2510
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

		// Token: 0x06002FCA RID: 12234 RVA: 0x000C43C8 File Offset: 0x000C25C8
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

		// Token: 0x06002FCB RID: 12235 RVA: 0x000C44AE File Offset: 0x000C26AE
		public void SetAnimationAtChannelSynched(string animationName, int channelNo, float animationSpeed = 1f)
		{
			this.SetAnimationAtChannelSynched(MBAnimation.GetAnimationIndexWithName(animationName), channelNo, animationSpeed);
		}

		// Token: 0x06002FCC RID: 12236 RVA: 0x000C44C0 File Offset: 0x000C26C0
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

		// Token: 0x06002FCD RID: 12237 RVA: 0x000C456C File Offset: 0x000C276C
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

		// Token: 0x06002FCE RID: 12238 RVA: 0x000C45D8 File Offset: 0x000C27D8
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

		// Token: 0x06002FCF RID: 12239 RVA: 0x000C462C File Offset: 0x000C282C
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

		// Token: 0x06002FD0 RID: 12240 RVA: 0x000C467F File Offset: 0x000C287F
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

		// Token: 0x06002FD1 RID: 12241 RVA: 0x000C46AD File Offset: 0x000C28AD
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

		// Token: 0x06002FD2 RID: 12242 RVA: 0x000C46EC File Offset: 0x000C28EC
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

		// Token: 0x06002FD3 RID: 12243 RVA: 0x000C4748 File Offset: 0x000C2948
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

		// Token: 0x06002FD4 RID: 12244 RVA: 0x000C47A0 File Offset: 0x000C29A0
		public void SetTeamColors(uint color, uint color2)
		{
			this.Color = color;
			this.Color2 = color2;
			base.GameEntity.SetColor(color, color2, "use_team_color");
		}

		// Token: 0x06002FD5 RID: 12245 RVA: 0x000C47C4 File Offset: 0x000C29C4
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

		// Token: 0x06002FD6 RID: 12246 RVA: 0x000C4818 File Offset: 0x000C2A18
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

		// Token: 0x06002FD7 RID: 12247 RVA: 0x000C49D0 File Offset: 0x000C2BD0
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

		// Token: 0x040013CE RID: 5070
		private SynchedMissionObject.SynchFlags _initialSynchFlags;

		// Token: 0x040013CF RID: 5071
		private SynchedMissionObject.SynchState _synchState;

		// Token: 0x040013D0 RID: 5072
		private MatrixFrame _lastSynchedFrame;

		// Token: 0x040013D1 RID: 5073
		private MatrixFrame _firstFrame;

		// Token: 0x040013D2 RID: 5074
		private float _timer;

		// Token: 0x040013D3 RID: 5075
		private float _duration;

		// Token: 0x0200067C RID: 1660
		private enum SynchState
		{
			// Token: 0x04002116 RID: 8470
			SynchronizeCompleted,
			// Token: 0x04002117 RID: 8471
			SynchronizePosition,
			// Token: 0x04002118 RID: 8472
			SynchronizeFrame,
			// Token: 0x04002119 RID: 8473
			SynchronizeFrameOverTime
		}

		// Token: 0x0200067D RID: 1661
		[Flags]
		public enum SynchFlags : uint
		{
			// Token: 0x0400211B RID: 8475
			SynchNone = 0U,
			// Token: 0x0400211C RID: 8476
			SynchTransform = 1U,
			// Token: 0x0400211D RID: 8477
			SynchAnimation = 2U,
			// Token: 0x0400211E RID: 8478
			SynchBodyFlags = 4U,
			// Token: 0x0400211F RID: 8479
			SyncColors = 8U,
			// Token: 0x04002120 RID: 8480
			SynchAll = 4294967295U
		}
	}
}
