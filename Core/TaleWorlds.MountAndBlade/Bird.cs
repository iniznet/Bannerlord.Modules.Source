using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000331 RID: 817
	public class Bird : MissionObject
	{
		// Token: 0x06002C1A RID: 11290 RVA: 0x000AAC72 File Offset: 0x000A8E72
		private Bird.State GetState()
		{
			if (this.CanFly && !this._canLand)
			{
				return Bird.State.Airborne;
			}
			return Bird.State.Perched;
		}

		// Token: 0x06002C1B RID: 11291 RVA: 0x000AAC88 File Offset: 0x000A8E88
		protected internal override void OnInit()
		{
			base.OnInit();
			base.GameEntity.SetAnimationSoundActivation(true);
			base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_idle", 0, 1f, -1f, 0f);
			base.GameEntity.Skeleton.SetAnimationParameterAtChannel(0, MBRandom.RandomFloat * 0.5f);
			this._kmPerHour = 4f;
			this._state = this.GetState();
			if (this._timer == null)
			{
				this._timer = new BasicMissionTimer();
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06002C1C RID: 11292 RVA: 0x000AAD20 File Offset: 0x000A8F20
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_idle", 0, 1f, -1f, 0f);
			base.GameEntity.Skeleton.SetAnimationParameterAtChannel(0, 0f);
		}

		// Token: 0x06002C1D RID: 11293 RVA: 0x000AAD6E File Offset: 0x000A8F6E
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
		}

		// Token: 0x06002C1E RID: 11294 RVA: 0x000AAD78 File Offset: 0x000A8F78
		protected internal override void OnTick(float dt)
		{
			switch (this._state)
			{
			case Bird.State.TakingOff:
				this.ApplyDisplacement(dt);
				if (base.GameEntity.Skeleton.GetAnimationParameterAtChannel(0) > 0.99f)
				{
					base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_cycle", 0, 1f, -1f, 0f);
					this._timer.Reset();
					this.SetDisplacement();
					this._state = Bird.State.Airborne;
					return;
				}
				break;
			case Bird.State.Airborne:
				if (this._timer.ElapsedTime <= 5f)
				{
					this.ApplyDisplacement(dt);
					return;
				}
				if (this._canLand)
				{
					base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_landing", 0, 1f, -1f, 0f);
					this._timer.Reset();
					this._state = Bird.State.Landing;
					this.SetDisplacement();
					return;
				}
				base.GameEntity.SetVisibilityExcludeParents(false);
				return;
			case Bird.State.Landing:
				this.ApplyDisplacement(dt);
				if (base.GameEntity.Skeleton.GetAnimationParameterAtChannel(0) > 0.99f)
				{
					base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_idle", 0, 1f, -1f, 0f);
					this._timer.Reset();
					this._state = Bird.State.Perched;
					return;
				}
				break;
			case Bird.State.Perched:
				if (this.CanFly && this._timer.ElapsedTime > 5f + MBRandom.RandomFloat * 13f && base.GameEntity.Skeleton.GetAnimationParameterAtChannel(0) > 0.99f)
				{
					base.GameEntity.Skeleton.SetAnimationAtChannel("anim_bird_flying", 0, 1f, -1f, 0f);
					this._timer.Reset();
					this._state = Bird.State.TakingOff;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06002C1F RID: 11295 RVA: 0x000AAF40 File Offset: 0x000A9140
		private void ApplyDisplacement(float dt)
		{
			float num = this._kmPerHour * dt;
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			MatrixFrame matrixFrame = globalFrame;
			Vec3 f = globalFrame.rotation.f;
			Vec3 u = globalFrame.rotation.u;
			f.Normalize();
			u.Normalize();
			if (this._state == Bird.State.TakingOff)
			{
				matrixFrame.origin = matrixFrame.origin - f * 0.30769232f + u * 0.1f;
			}
			else if (this._state == Bird.State.Airborne)
			{
				globalFrame.origin -= f * num;
				matrixFrame.origin -= f * num;
			}
			else if (this._state == Bird.State.Landing)
			{
				matrixFrame.origin = matrixFrame.origin - f * 0.30769232f - u * 0.1f;
			}
			base.GameEntity.SetGlobalFrame(globalFrame);
		}

		// Token: 0x06002C20 RID: 11296 RVA: 0x000AB04C File Offset: 0x000A924C
		private void SetDisplacement()
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			Vec3 f = globalFrame.rotation.f;
			Vec3 u = globalFrame.rotation.u;
			f.Normalize();
			u.Normalize();
			if (this._state == Bird.State.TakingOff)
			{
				globalFrame.origin -= f * 20f - u * 6.5f;
			}
			else if (this._state == Bird.State.Landing)
			{
				globalFrame.origin -= f * 20f + u * 6.5f;
			}
			base.GameEntity.SetGlobalFrame(globalFrame);
		}

		// Token: 0x040010AE RID: 4270
		private const float Speed = 14400f;

		// Token: 0x040010AF RID: 4271
		private const string IdleAnimation = "anim_bird_idle";

		// Token: 0x040010B0 RID: 4272
		private const string LandingAnimation = "anim_bird_landing";

		// Token: 0x040010B1 RID: 4273
		private const string TakingOffAnimation = "anim_bird_flying";

		// Token: 0x040010B2 RID: 4274
		private const string FlyCycleAnimation = "anim_bird_cycle";

		// Token: 0x040010B3 RID: 4275
		public bool CanFly;

		// Token: 0x040010B4 RID: 4276
		private float _kmPerHour;

		// Token: 0x040010B5 RID: 4277
		private Bird.State _state;

		// Token: 0x040010B6 RID: 4278
		private BasicMissionTimer _timer;

		// Token: 0x040010B7 RID: 4279
		private bool _canLand = true;

		// Token: 0x02000646 RID: 1606
		private enum State
		{
			// Token: 0x04002055 RID: 8277
			TakingOff,
			// Token: 0x04002056 RID: 8278
			Airborne,
			// Token: 0x04002057 RID: 8279
			Landing,
			// Token: 0x04002058 RID: 8280
			Perched
		}
	}
}
